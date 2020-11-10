// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using MSDF.DataChecker.Persistence.Catalogs;
using MSDF.DataChecker.Persistence.DatabaseEnvironments;
using MSDF.DataChecker.Persistence.RuleExecutionLogDetails;
using MSDF.DataChecker.Persistence.RuleExecutionLogs;
using MSDF.DataChecker.Persistence.Rules;
using MSDF.DataChecker.Services.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MSDF.DataChecker.Services
{
    public interface IRuleExecutionLogDetailService
    { 
        Task<RuleExecutionLogDetailBO> GetByRuleExecutionLogIdAsync(int id);
        Task<int> GetLastRuleExecutionLogByEnvironmentAndRuleAsync(Guid environmentId, Guid ruleId);
        Task<RuleExecutionLogDetailExportToTableBO> ExportToTableByRuleExecutionLogIdAsync(int id);
        Task<RuleExecutionLogDetailBO> ExecutionDiagnosticSqlByLogIdAsync(int id);
    }

    public class RuleExecutionLogDetailService : IRuleExecutionLogDetailService
    {
        private IRuleExecutionLogDetailQueries _queries;
        private IRuleExecutionLogQueries _queriesRuleExecutionLog;
        private ICatalogQueries _queriesCatalog;
        private IRuleExecutionLogDetailCommands _commandRuleExecutionLogDetail;
        private IRuleExecutionLogCommands _commandRuleExecutionLog;
        private IRuleQueries _queriesRule;
        private IDatabaseEnvironmentQueries _queriesDatabaseEnvironments;

        public RuleExecutionLogDetailService(
            IRuleExecutionLogDetailQueries queries,
            IRuleExecutionLogQueries queriesRuleExecutionLog,
            ICatalogQueries queriesCatalog,
            IRuleExecutionLogDetailCommands commandRuleExecutionLogDetail,
            IRuleExecutionLogCommands commandRuleExecutionLog,
            IRuleQueries queriesRule,
            IDatabaseEnvironmentQueries queriesDatabaseEnvironments)
        {
            _queries = queries;
            _queriesRuleExecutionLog = queriesRuleExecutionLog;
            _queriesCatalog = queriesCatalog;
            _commandRuleExecutionLogDetail = commandRuleExecutionLogDetail;
            _commandRuleExecutionLog = commandRuleExecutionLog;
            _queriesRule = queriesRule;
            _queriesDatabaseEnvironments = queriesDatabaseEnvironments;
        }

        public async Task<RuleExecutionLogDetailExportToTableBO> ExportToTableByRuleExecutionLogIdAsync(int id)
        {
            RuleExecutionLogDetailExportToTableBO result = new RuleExecutionLogDetailExportToTableBO();

            var ruleExecutionLog = await _queriesRuleExecutionLog.GetAsync(id);
            if (ruleExecutionLog != null && !string.IsNullOrEmpty(ruleExecutionLog.DetailsTableName))
            {
                result.TableName = ruleExecutionLog.DetailsTableName;
                result.AlreadyExist = await _queries.ExistExportTableFromRuleExecutionLogAsync(ruleExecutionLog.DetailsTableName, "destination");
            }

            if (!result.AlreadyExist)
            {
                Persistence.Rules.Rule ruleFromLog = await _queriesRule.GetAsync(ruleExecutionLog.RuleId);
                string ruleName = ruleFromLog.Name;

                ruleName = Regex.Replace(ruleName, @"[^\w\.@-]", "_",RegexOptions.None, TimeSpan.FromSeconds(1.5));
                string tableName = $"T_{id}-{ruleName}";
                if(tableName.Length>128)
                    tableName = tableName.Substring(0, 128);

                Dictionary<string, string> columns = new Dictionary<string, string>();
                columns = JsonConvert.DeserializeObject<Dictionary<string, string>>(ruleExecutionLog.DetailsSchema);

                List<string> sqlColumns = new List<string>();
                foreach (var column in columns)
                {
                    if (column.Value == "string")
                        sqlColumns.Add(string.Format($"[{column.Key}] [nvarchar](max) NULL"));
                    else
                        sqlColumns.Add(string.Format($"[{column.Key}] [datetime2](7) NULL"));
                }

                string sqlCreate = $"CREATE TABLE [destination].[{tableName}]({string.Join(",", sqlColumns)}) ";
                await _commandRuleExecutionLogDetail.ExecuteSqlAsync(sqlCreate);

                result.TableName = tableName;
                result.Created = await _queries.ExistExportTableFromRuleExecutionLogAsync(tableName, "destination");

                if (result.Created)
                {
                    var ruleExecutionLogInfo = await GetByRuleExecutionLogIdAsync(id);
                    
                    DataTable infoToInsert = Utils.GetTableForSqlBulk(ruleExecutionLogInfo.Rows, columns);
                    await _commandRuleExecutionLogDetail.ExecuteSqlBulkCopy(infoToInsert, $"[destination].[{tableName}]");
                    
                    ruleExecutionLog.DetailsTableName = tableName;
                    await _commandRuleExecutionLog.UpdateAsync(ruleExecutionLog);
                }
            }

            return result;
        }

        public async Task<RuleExecutionLogDetailBO> GetByRuleExecutionLogIdAsync(int id)
        {
            RuleExecutionLogDetailBO result = new RuleExecutionLogDetailBO
            {
                Columns = new List<string>(),
                Rows = new List<Dictionary<string, string>>()
            };

            var ruleExecutionLog = await _queriesRuleExecutionLog.GetAsync(id);
            if (ruleExecutionLog != null && ruleExecutionLog.RuleDetailsDestinationId != null)
            {
                var existRule = await _queriesRule.GetAsync(ruleExecutionLog.RuleId);
                var existDatabaseEnvironment = await _queriesDatabaseEnvironments.GetAsync(ruleExecutionLog.DatabaseEnvironmentId);

                result.RuleName = existRule.Name;
                result.EnvironmentName = existDatabaseEnvironment.Name;
                result.ExecutionDateTime = ruleExecutionLog.ExecutionDate.ToLocalTime().ToString("MM/dd/yyyy HH:mm");

                var catalog = await _queriesCatalog.GetAsync(ruleExecutionLog.RuleDetailsDestinationId.Value);
                if (catalog != null)
                {
                    result.DestinationTable = catalog.Name;
                    result.RuleDiagnosticSql = ruleExecutionLog.DiagnosticSql;

                    Dictionary<string, string> columnsFromLog = new Dictionary<string, string>();
                    if (!string.IsNullOrEmpty(ruleExecutionLog.DetailsSchema))
                    {
                        columnsFromLog = JsonConvert.DeserializeObject<Dictionary<string, string>>(ruleExecutionLog.DetailsSchema);
                    }

                    var reader = await _queries.GetByRuleExecutionLogIdAsync(id, catalog.Name);
                    if (reader != null)
                    {
                        List<string> columnsToIgnore = new List<string>() { "id", "otherdetails", "ruleexecutionlogid" };
                        List<string> columnsToExport = new List<string>();

                        foreach (DataColumn column in reader.Columns)
                        {
                            string columnName = column.ColumnName.ToLower();
                            if (!columnsToIgnore.Contains(columnName) && (columnsFromLog.Count==0 || columnsFromLog.ContainsKey(columnName)))
                            {
                                columnsToExport.Add(columnName);
                            }
                        }

                        List<Dictionary<string,string>> rowsToExport = new List<Dictionary<string, string>>();

                        foreach (DataRow row in reader.Rows)
                        {
                            Dictionary<string, string> newRow = new Dictionary<string, string>();
                            foreach (var column in columnsToExport)
                            {
                                newRow.Add(column, row[column].ToString());
                            }

                            string otherDetails = row.Field<string>("otherdetails").ToString();
                            if (!string.IsNullOrEmpty(otherDetails))
                            {
                                Dictionary<string, string> jsonValues = JsonConvert.DeserializeObject<Dictionary<string, string>>(otherDetails);
                                foreach (var element in jsonValues)
                                {
                                    if(columnsToExport.Contains(element.Key) || columnsFromLog.ContainsKey(element.Key))
                                        newRow.Add(element.Key, element.Value);
                                }
                            }
                            rowsToExport.Add(newRow);
                        }

                        if (columnsFromLog.Count > 0)
                            columnsToExport = columnsFromLog.Select(rec => rec.Key).ToList();

                        result.RuleExecutionLogId = id;
                        result.Columns = columnsToExport;
                        result.Rows = rowsToExport;
                    }
                }
            }

            return result;
        }

        public async Task<int> GetLastRuleExecutionLogByEnvironmentAndRuleAsync(Guid environmentId, Guid ruleId)
        {
            int result = 0;
            var listLogs = await _queriesRuleExecutionLog.GetByRuleIdAsync(ruleId);
            if (listLogs != null && listLogs.Any())
            {
                var existLog = listLogs.OrderByDescending(rec => rec.ExecutionDate).FirstOrDefault();
                if (existLog != null)
                    result = existLog.Id;
            }
            return result;
        }

        public async Task<RuleExecutionLogDetailBO> ExecutionDiagnosticSqlByLogIdAsync(int id)
        {
            RuleExecutionLogDetailBO result = new RuleExecutionLogDetailBO
            {
                Columns = new List<string>(),
                Rows = new List<Dictionary<string, string>>()
            };

            var ruleExecutionLog = await _queriesRuleExecutionLog.GetAsync(id);
            if (ruleExecutionLog != null)
            {
                var existDatabaseEnvironment = await _queriesDatabaseEnvironments.GetAsync(ruleExecutionLog.DatabaseEnvironmentId);
                DatabaseEnvironmentBO envBO = new DatabaseEnvironmentBO
                {
                    Database = existDatabaseEnvironment.Database,
                    DataSource = existDatabaseEnvironment.DataSource,
                    ExtraData = existDatabaseEnvironment.ExtraData,
                    Password = existDatabaseEnvironment.Password,
                    SecurityIntegrated = existDatabaseEnvironment.SecurityIntegrated,
                    TimeoutInMinutes = existDatabaseEnvironment.TimeoutInMinutes,
                    User = existDatabaseEnvironment.User
                };

                string connectionString = envBO.GetConnectionString();
                if (!connectionString.ToLower().Contains("timeout") && envBO.TimeoutInMinutes == null)
                    connectionString += " Connection Timeout = 60";
                else if (envBO.TimeoutInMinutes != null)
                    connectionString += " Connection Timeout = " + (envBO.TimeoutInMinutes.Value * 60).ToString();

                try
                {
                    using (var conn = new SqlConnection(connectionString))
                    {
                        conn.Open();

                        using (var cmd = new SqlCommand(ruleExecutionLog.DiagnosticSql, conn))
                        {
                            if (envBO.TimeoutInMinutes != null)
                                cmd.CommandTimeout = (envBO.TimeoutInMinutes.Value * 60);

                            result.RuleDiagnosticSql = ruleExecutionLog.DiagnosticSql;

                            var reader = await cmd.ExecuteReaderAsync();
                            if (reader.HasRows)
                            {
                                DataTable dt = new DataTable();
                                dt.Load(reader);

                                if (dt != null)
                                {
                                    List<string> columnsToExport = new List<string>();

                                    foreach (DataColumn column in dt.Columns)
                                    {
                                        string columnName = column.ColumnName.ToLower();
                                        columnsToExport.Add(columnName);
                                    }

                                    List<Dictionary<string, string>> rowsToExport = new List<Dictionary<string, string>>();

                                    foreach (DataRow row in dt.Rows)
                                    {
                                        Dictionary<string, string> newRow = new Dictionary<string, string>();
                                        foreach (var column in columnsToExport)
                                        {
                                            newRow.Add(column, row[column].ToString());
                                        }
                                        rowsToExport.Add(newRow);
                                    }
                                    result.RuleExecutionLogId = id;
                                    result.Columns = columnsToExport;
                                    result.Rows = rowsToExport;
                                }
                            }
                        }
                    }
                }
                catch
                {
                }
            }
            return result;
        }
    }
}
