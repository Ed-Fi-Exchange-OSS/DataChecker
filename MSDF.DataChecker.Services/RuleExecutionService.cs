// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using MSDF.DataChecker.Persistence.Catalogs;
using MSDF.DataChecker.Persistence.Collections;
using MSDF.DataChecker.Persistence.RuleExecutionLogDetails;
using MSDF.DataChecker.Persistence.RuleExecutionLogs;
using MSDF.DataChecker.Services.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace MSDF.DataChecker.Services
{
    public interface IRuleExecutionService
    {
        Task<List<RuleTestResult>> ExecuteRulesByEnvironmentIdAsync(List<RuleBO> rules, DatabaseEnvironmentBO databaseEnvironment);
        Task<RuleTestResult> ExecuteRuleByEnvironmentIdAsync(Guid ruleId,DatabaseEnvironmentBO databaseEnvironment);
        Task<TableResult> ExecuteRuleDiagnosticByRuleLogIdAndEnvironmentIdAsync(int ruleLogId, DatabaseEnvironmentBO databaseEnvironment);
        Task<RuleTestResult> ExecuteRuleAsync(RuleBO rule, string connectionString, List<UserParamBO> userParams, int? timeout);
    }

    public class RuleExecutionService : IRuleExecutionService
    {
        private readonly IRuleService _ruleService;
        private readonly IRuleExecutionLogCommands _ruleExecutionLogCommands;
        private readonly IRuleExecutionLogQueries _ruleExecutionLogQueries;
        private readonly IRuleExecutionLogDetailCommands _edFiRuleExecutionLogDetailCommands;
        private readonly IRuleExecutionLogDetailQueries _edFiRuleExecutionLogDetailQueries;
        private readonly ICatalogQueries _catalogQueries;
        private readonly ICollectionQueries _collectionQueries;

        public RuleExecutionService(
            IRuleService ruleService
            ,IRuleExecutionLogCommands ruleExecutionLogCommands
            ,IRuleExecutionLogQueries ruleExecutionLogQueries
            ,IRuleExecutionLogDetailCommands edFiRuleExecutionLogDetailCommands
            ,ICatalogQueries catalogQueries
            ,IRuleExecutionLogDetailQueries edFiRuleExecutionLogDetailQueries,
            ICollectionQueries collectionQueries)
        {
            _ruleService = ruleService;
            _ruleExecutionLogCommands = ruleExecutionLogCommands;
            _ruleExecutionLogQueries = ruleExecutionLogQueries;
            _edFiRuleExecutionLogDetailCommands = edFiRuleExecutionLogDetailCommands;
            _catalogQueries = catalogQueries;
            _edFiRuleExecutionLogDetailQueries = edFiRuleExecutionLogDetailQueries;
            _collectionQueries = collectionQueries;
        }

        public async Task<List<RuleTestResult>> ExecuteRulesByEnvironmentIdAsync(List<RuleBO> rules, DatabaseEnvironmentBO databaseEnvironment)
        {
            var results = new List<RuleTestResult>();
            var connectionString = databaseEnvironment
                .GetConnectionString();

            foreach (var rule in rules)
            {
                results.Add(await ExecuteRuleAsync(rule, connectionString, databaseEnvironment.UserParams, databaseEnvironment.TimeoutInMinutes));
            }
            return results;
        }
        
        public async Task<RuleTestResult> ExecuteRuleByEnvironmentIdAsync(Guid ruleId,DatabaseEnvironmentBO databaseEnvironment)
        {
            int? ruleDetailsDestinationId = null;
            var rule = await _ruleService.GetAsync(ruleId);
            var executionLogs = await _ruleExecutionLogQueries.GetByRuleIdAsync(ruleId);

            var connectionString = databaseEnvironment.GetConnectionString();
                
            var stopWatch = System.Diagnostics.Stopwatch.StartNew();

            RuleTestResult testResult = await ExecuteRuleAsync(rule, connectionString, databaseEnvironment.UserParams, databaseEnvironment.TimeoutInMinutes);

            var containerParent = await _collectionQueries.GetAsync(rule.ContainerId);
            if (containerParent.ParentContainerId != null)
            {
                var collectionParent = await _collectionQueries.GetAsync(containerParent.ParentContainerId.Value);
                ruleDetailsDestinationId = collectionParent.RuleDetailsDestinationId;
            }            

            RuleExecutionLog ruleExecutionLog = new RuleExecutionLog()
            {
                Id = 0,
                Evaluation = testResult.Evaluation,
                RuleId = ruleId,
                StatusId = (int)testResult.Status,
                Result = testResult.Result,
                Response = testResult.Evaluation ? "Ok" : testResult.ErrorMessage,
                DatabaseEnvironmentId = databaseEnvironment.Id,
                ExecutedSql = testResult.ExecutedSql,
                DiagnosticSql = rule.DiagnosticSql,
                RuleDetailsDestinationId = ruleDetailsDestinationId
            };

            if (ruleExecutionLog.RuleDetailsDestinationId == null || ruleExecutionLog.RuleDetailsDestinationId.Value == 0)
                ruleExecutionLog.RuleDetailsDestinationId = null;

            testResult.LastExecuted = executionLogs.Any() ? executionLogs.FirstOrDefault().ExecutionDate : (DateTime?)null;
            ruleExecutionLog.ExecutionTimeMs = stopWatch.ElapsedMilliseconds;
            ruleExecutionLog.Result = testResult.Result;

            var newRuleExecutionLog = await _ruleExecutionLogCommands.AddAsync(ruleExecutionLog);
            testResult.TestResults = await _ruleService.GetTopResults(ruleId,databaseEnvironment.Id);

            if (!testResult.Evaluation)
            {
                testResult.DiagnosticSql = rule.DiagnosticSql;
            }

            try
            {
                //Validate if the rule is going to any queue table
                if (ruleDetailsDestinationId != null && ruleDetailsDestinationId.Value > 0)
                {
                    var existCatalog = await _catalogQueries.GetAsync(ruleDetailsDestinationId.Value);
                    if (existCatalog != null)
                    {
                        int maxNumberResults = databaseEnvironment.MaxNumberResults.Value;
                        if (rule.MaxNumberResults != null)
                            maxNumberResults = rule.MaxNumberResults.Value;

                        await InsertDiagnosticSqlIntoDetails(rule, newRuleExecutionLog, connectionString, databaseEnvironment.UserParams, existCatalog.Name, maxNumberResults);
                    }
                }
            }
            catch(Exception ex)
            {
                newRuleExecutionLog.Result = -1;
                newRuleExecutionLog.Response = ex.Message;
                await _ruleExecutionLogCommands.UpdateAsync(newRuleExecutionLog);
            }

            return testResult;
        }

        public async Task<RuleTestResult> ExecuteRuleAsync(RuleBO rule, string connectionString, List<UserParamBO> userParams, int? timeout)
        {
            var stopWatch = System.Diagnostics.Stopwatch.StartNew();
            RuleTestResult testResult;

            try
            {
                if (!connectionString.ToLower().Contains("timeout") && timeout == null)
                    connectionString += " Connection Timeout = 60";
                else if (timeout != null)
                    connectionString += " Connection Timeout = " + (timeout.Value * 60).ToString();

                using (var conn = new SqlConnection(connectionString))
                {
                    int execution = 0;
                    bool resultWithErrors = false;
                    await conn.OpenAsync();
                    string sqlToRun = Utils.GenerateSqlWithCount(rule.DiagnosticSql);

                    try
                    {
                        if (string.IsNullOrEmpty(sqlToRun))
                        {
                            sqlToRun = rule.DiagnosticSql;
                            using (var cmd = new SqlCommand(sqlToRun, conn))
                            {
                                if (timeout != null)
                                    cmd.CommandTimeout = (timeout.Value * 60);

                                AddParameters(sqlToRun, cmd, userParams);
                                var reader = await cmd.ExecuteReaderAsync();
                                if (reader.HasRows)
                                {
                                    while (reader.Read())
                                        execution++;
                                }
                            }
                        }
                        else
                        {
                            using (var cmd = new SqlCommand(sqlToRun, conn))
                            {
                                if (timeout != null)
                                    cmd.CommandTimeout = (timeout.Value * 60);

                                AddParameters(sqlToRun, cmd, userParams);
                                execution = Convert.ToInt32(cmd.ExecuteScalar());
                            }
                        }
                    }
                    catch 
                    {
                        sqlToRun = rule.DiagnosticSql;
                        using (var cmd = new SqlCommand(sqlToRun, conn))
                        {
                            if (timeout != null)
                                cmd.CommandTimeout = (timeout.Value * 60);

                            AddParameters(sqlToRun, cmd, userParams);
                            var reader = await cmd.ExecuteReaderAsync();
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                    execution++;
                            }
                        }
                    }

                    resultWithErrors = execution > 0;
                    testResult = new RuleTestResult
                    {
                        Id = 0,
                        Rule = rule,
                        Result = execution,
                        Evaluation = !resultWithErrors,
                        Status = !resultWithErrors ? Status.Succeded : Status.Failed,
                        ErrorMessage = !resultWithErrors ? "" : rule.ErrorMessage,
                        ExecutedSql = rule.DiagnosticSql
                    };
                }
            }
            catch (Exception e)
            {
                testResult = new RuleTestResult()
                {
                    Rule = rule,
                    Result = -1,
                    Evaluation = false,
                    Status = Status.Error,
                    ErrorMessage = e.Message,
                    ExecutedSql = rule.DiagnosticSql
                };              
            }
        
            stopWatch.Stop();
            testResult.ExecutionTimeMs = stopWatch.ElapsedMilliseconds;           
            return testResult;
        }

        public async Task<TableResult> ExecuteRuleDiagnosticByRuleLogIdAndEnvironmentIdAsync(int ruleLogId, DatabaseEnvironmentBO databaseEnvironment)
        {
            TableResult result = new TableResult();

            var existLog = await _ruleExecutionLogQueries.GetAsync(ruleLogId);
            var connectionString = databaseEnvironment.GetConnectionString();

            try
            {
                if (!connectionString.ToLower().Contains("timeout") && databaseEnvironment.TimeoutInMinutes == null)
                    connectionString += " Connection Timeout = 60";
                else if (databaseEnvironment.TimeoutInMinutes != null)
                    connectionString += " Connection Timeout = " + (databaseEnvironment.TimeoutInMinutes.Value * 60).ToString();

                using (var conn = new SqlConnection(connectionString))
                {
                    await conn.OpenAsync();

                    using (var cmd = new SqlCommand(existLog.DiagnosticSql, conn))
                    {
                        if (databaseEnvironment.TimeoutInMinutes != null)
                            cmd.CommandTimeout = (databaseEnvironment.TimeoutInMinutes.Value * 60);

                        AddParameters(existLog.DiagnosticSql, cmd, databaseEnvironment.UserParams);
                        var getReader = await cmd.ExecuteReaderAsync();
                        DataTable dt = new DataTable();
                        dt.Load(getReader);

                        result.Columns = dt.Columns.Count;
                        foreach (var dataColumn in dt.Columns)
                        {
                            result.ColumnsName.Add(dataColumn.ToString());
                        }

                        var informationAsList = (from x in dt.AsEnumerable() select x).ToList();
                        result.Information = Utils.Serialize(result.ColumnsName, informationAsList);
                        result.Rows = result.Information.Count;
                    }
                }
            }
            catch(Exception ex)
            {
                result.MessageError = ex.Message;
            }

            return result;
        }

        private async Task InsertDiagnosticSqlIntoDetails(RuleBO rule, RuleExecutionLog ruleExecutionLog, string connectionString, List<UserParamBO> sqlParams, string tableName, int maxNumberResults)
        {
            string sqlToRun = Utils.GenerateSqlWithTop(rule.DiagnosticSql, maxNumberResults.ToString());
            string columnsSchema = string.Empty;
            var listColumnsFromDestination = await _edFiRuleExecutionLogDetailQueries.GetColumnsByTableAsync(tableName, "destination");

            using (var conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new SqlCommand(sqlToRun, conn))
                {
                    AddParameters(sqlToRun,cmd,sqlParams);
                    var reader = await cmd.ExecuteReaderAsync();

                    Dictionary<string, string> listColumns = new Dictionary<string, string>();
                    listColumnsFromDestination.ForEach(rec => listColumns.Add(rec.Name, rec.Type));

                    DataTable tableToInsert = Utils.GetTableForSqlBulk(ruleExecutionLog.Id, reader, listColumns, out columnsSchema);
                    if (tableToInsert != null && tableToInsert.Rows.Count > 0)
                    {
                        if (ruleExecutionLog != null)
                        {
                            ruleExecutionLog.DetailsSchema = columnsSchema;
                            await _ruleExecutionLogCommands.UpdateAsync(ruleExecutionLog);
                        }
                        await _edFiRuleExecutionLogDetailCommands.ExecuteSqlBulkCopy(tableToInsert, $"[destination].[{tableName}]");
                    }
                }
            }
        }

        private void AddParameters(string sql, SqlCommand sqlCommand, List<UserParamBO> parameters)
        {
            if (parameters != null)
            {
                parameters.ForEach(m =>
                {
                    if (sql.Contains("@" + m.Name))
                        sqlCommand.Parameters.AddWithValue("@" + m.Name, m.Value);
                });
            }
        }
    }
}
