using Microsoft.Extensions.Options;
using MSDF.DataChecker.Persistence.Catalogs;
using MSDF.DataChecker.Persistence.Collections;
using MSDF.DataChecker.Persistence.Providers;
using MSDF.DataChecker.Persistence.RuleExecutionLogDetails;
using MSDF.DataChecker.Persistence.RuleExecutionLogs;
using MSDF.DataChecker.Persistence.Settings;
using MSDF.DataChecker.Services.Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MSDF.DataChecker.Services.RuleExecution
{
    public interface IRuleExecService
    {
        string ConnectionString { get; set; }
        IDataProvider DataProvider { get; }
        Task<List<RuleTestResult>> ExecuteRulesByEnvironmentIdAsync(List<RuleBO> rules, DatabaseEnvironmentBO databaseEnvironment);
        Task<RuleTestResult> ExecuteRuleByEnvironmentIdAsync(Guid ruleId, DatabaseEnvironmentBO databaseEnvironment);
        Task<TableResult> ExecuteRuleDiagnosticByRuleLogIdAndEnvironmentIdAsync(int ruleLogId, DatabaseEnvironmentBO databaseEnvironment);
        Task<RuleTestResult> ExecuteRuleAsync(RuleBO rule, string connectionString, List<UserParamBO> userParams, int? timeout);
    }
    public class RuleExecService : IRuleExecService
    {
        private readonly IRuleService _ruleService;
        private readonly IRuleExecutionLogCommands _ruleExecutionLogCommands;
        private readonly IRuleExecutionLogQueries _ruleExecutionLogQueries;
        private readonly IRuleExecutionLogDetailCommands _edFiRuleExecutionLogDetailCommands;
        private readonly IRuleExecutionLogDetailQueries _edFiRuleExecutionLogDetailQueries;
        private readonly ICatalogQueries _catalogQueries;
        private readonly ICollectionQueries _collectionQueries;
        public IDataProvider _dataProvider;
        private readonly DataBaseSettings _appSettings;
        public string ConnectionString
        {
            get => _dataProvider.ConnectionString; set => _dataProvider.ConnectionString = this.ConnectionString;
        }

        public IDataProvider DataProvider { get { return _dataProvider; } }

        public RuleExecService(
            IRuleService ruleService
            , IRuleExecutionLogCommands ruleExecutionLogCommands
            , IRuleExecutionLogQueries ruleExecutionLogQueries
            , IRuleExecutionLogDetailCommands edFiRuleExecutionLogDetailCommands
            , ICatalogQueries catalogQueries
            , IRuleExecutionLogDetailQueries edFiRuleExecutionLogDetailQueries
            , ICollectionQueries collectionQueries
            , IDataProvider dataProvider
            , IOptionsSnapshot<DataBaseSettings> appSettings)
        {
            _ruleService = ruleService;
            _ruleExecutionLogCommands = ruleExecutionLogCommands;
            _ruleExecutionLogQueries = ruleExecutionLogQueries;
            _edFiRuleExecutionLogDetailCommands = edFiRuleExecutionLogDetailCommands;
            _catalogQueries = catalogQueries;
            _edFiRuleExecutionLogDetailQueries = edFiRuleExecutionLogDetailQueries;
            _collectionQueries = collectionQueries;
            _dataProvider = dataProvider;
            _appSettings = appSettings.Value;
        }

        public async Task<List<RuleTestResult>> ExecuteRulesByEnvironmentIdAsync(List<RuleBO> rules, DatabaseEnvironmentBO databaseEnvironment)
        {
            var results = new List<RuleTestResult>();
            var connectionString = databaseEnvironment.GetConnectionString(_appSettings.Engine);

            foreach (var rule in rules)
            {
                results.Add(await ExecuteRuleAsync(rule, connectionString, databaseEnvironment.UserParams, databaseEnvironment.TimeoutInMinutes));
            }
            return results;
        }

        public async Task<RuleTestResult> ExecuteRuleByEnvironmentIdAsync(Guid ruleId, DatabaseEnvironmentBO databaseEnvironment)
        {
            int? ruleDetailsDestinationId = null;
            var rule = await _ruleService.GetAsync(ruleId);
            var executionLogs = await _ruleExecutionLogQueries.GetByRuleIdAsync(ruleId);

            var connectionString = databaseEnvironment.GetConnectionString(_appSettings.Engine);

            var stopWatch = System.Diagnostics.Stopwatch.StartNew();

            if (string.IsNullOrEmpty(_dataProvider.ConnectionString))
                _dataProvider.ConnectionString = connectionString;
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
            testResult.TestResults = await _ruleService.GetTopResults(ruleId, databaseEnvironment.Id);

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

                        await InsertDiagnosticSqlIntoDetails(rule, newRuleExecutionLog, connectionString, databaseEnvironment.UserParams, existCatalog.Name, maxNumberResults,_appSettings.Engine);
                    }
                }
            }
            catch (Exception ex)
            {
                newRuleExecutionLog.Result = -1;
                newRuleExecutionLog.Response = ex.Message;
                await _ruleExecutionLogCommands.UpdateAsync(newRuleExecutionLog);
            }

            return testResult;
        }

        public Task<RuleTestResult> ExecuteRuleAsync(RuleBO rule, string connectionString, List<UserParamBO> userParams, int? timeout)
        {
            var stopWatch = System.Diagnostics.Stopwatch.StartNew();
            RuleTestResult testResult;

            try
            {
                if (string.IsNullOrEmpty(_dataProvider.ConnectionString))
                    _dataProvider.ConnectionString = connectionString;

                int execution = 0;
                bool resultWithErrors = false;
                string sqlToRun = Utils.GenerateSqlWithCount(rule.DiagnosticSql);
                var parameters = new Dictionary<string, string>();
                userParams.ForEach(item => { parameters.Add(item.Name, item.Value); });

                // userParams.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).ToDictionary(prop => prop.Name, prop => (string)prop.GetValue(userParams, null));
                if (string.IsNullOrEmpty(_dataProvider.ConnectionString))
                    _dataProvider.ConnectionString = connectionString;

               

                if (string.IsNullOrEmpty(sqlToRun))
                {
                    sqlToRun = rule.DiagnosticSql;
                    var dataReader =  _dataProvider.ExecuteReader(connectionString, sqlToRun, parameters);
                    if (dataReader.Rows.Count > 0)
                        execution = dataReader.Rows.Count;
                }
                else
                    execution = Convert.ToInt32(_dataProvider.ExecuteScalar(connectionString, sqlToRun, parameters));

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
            return Task.FromResult(testResult);
        }

        public TableResult GetExecutedRuleDiagnosticByRuleLog(DataTable dt)
        {
            var diagnosticResult = new TableResult();
            diagnosticResult.Columns = dt.Columns.Count;
            foreach (var dataColumn in dt.Columns)
            {
                diagnosticResult.ColumnsName.Add(dataColumn.ToString());
            }
            var informationAsList = (from x in dt.AsEnumerable() select x).ToList();
            diagnosticResult.Information = Utils.Serialize(diagnosticResult.ColumnsName, informationAsList);
            diagnosticResult.Rows = diagnosticResult.Information.Count;
            return diagnosticResult;
        }

        public async Task<TableResult> ExecuteRuleDiagnosticByRuleLogIdAndEnvironmentIdAsync(int ruleLogId, DatabaseEnvironmentBO databaseEnvironment)
        {
            var diagnosticResult = new TableResult();
            var existLog = await _ruleExecutionLogQueries.GetAsync(ruleLogId);
            var connectionString = databaseEnvironment.GetConnectionString(_appSettings.Engine);
            if (string.IsNullOrEmpty(_dataProvider.ConnectionString))
                _dataProvider.ConnectionString = connectionString;
            try
            {
                DataTable dt = new DataTable();
                var parameters = new Dictionary<string, string>();
                databaseEnvironment.UserParams.ForEach(item => { parameters.Add(item.Name, item.Value); });

                if (!connectionString.ToLower().Contains("timeout") && databaseEnvironment.TimeoutInMinutes == null)
                    connectionString += " Connection Timeout = 60";
                else if (databaseEnvironment.TimeoutInMinutes != null)
                    connectionString += " Connection Timeout = " + (databaseEnvironment.TimeoutInMinutes.Value * 60).ToString();

                var dataReader = _dataProvider.ExecuteReader(connectionString, existLog.DiagnosticSql, parameters);
                diagnosticResult = GetExecutedRuleDiagnosticByRuleLog(dataReader);
            }
            catch (Exception ex)
            {
                diagnosticResult.MessageError = ex.Message;
            }

            return diagnosticResult;
        }

        private async Task InsertDiagnosticSqlIntoDetails(RuleBO rule, RuleExecutionLog ruleExecutionLog, string connectionString, List<UserParamBO> sqlParams, string tableName, int maxNumberResults,string engine)
        {
            var tableForSqlBulk = new DataTable();
            string sqlToRun = Utils.GenerateSqlWithTop(rule.DiagnosticSql, maxNumberResults.ToString(), engine);
            string columnsSchema = string.Empty;

            var listColumnsFromDestination = await _edFiRuleExecutionLogDetailQueries.GetColumnsByTableAsync(tableName, "destination");
            Dictionary<string, string> listColumns = new Dictionary<string, string>();
            listColumnsFromDestination.ForEach(rec => listColumns.Add(rec.Name, rec.Type));

            var parameters = new Dictionary<string, string>();
            sqlParams.ForEach(item => { parameters.Add(item.Name, item.Value); });

            if (string.IsNullOrEmpty(_dataProvider.ConnectionString))
                _dataProvider.ConnectionString = connectionString;

            var dataReader = _dataProvider.ExecuteReader(connectionString, sqlToRun, parameters);
            tableForSqlBulk = Utils.GetTableForSqlBulk(ruleExecutionLog.Id, dataReader, listColumns, out columnsSchema);

            if (tableForSqlBulk != null && tableForSqlBulk.Rows.Count > 0)
            {
                if (ruleExecutionLog != null)
                {
                    ruleExecutionLog.DetailsSchema = columnsSchema;
                    await _ruleExecutionLogCommands.UpdateAsync(ruleExecutionLog);
                }
                await _edFiRuleExecutionLogDetailCommands.ExecuteSqlBulkCopy(tableForSqlBulk, $"[destination].[{tableName}]",_appSettings.Engine);
            }
        }
    }
}

