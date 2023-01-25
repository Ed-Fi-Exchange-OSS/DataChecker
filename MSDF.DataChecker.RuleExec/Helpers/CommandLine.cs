using Microsoft.Extensions.DependencyInjection;
using MSDF.DataChecker.Persistence.ValidationsRun;
using MSDF.DataChecker.Services;
using MSDF.DataChecker.Services.Models;
using MSDF.DataChecker.Services.RuleExecution;
using Npgsql;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Parsing;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MSDF.DataChecker.RuleExec.Helpers
{
    static class CommandLine
    {
        public static readonly string _defaultValue = "";
        public static RootCommand GetCommandLineParameters()
        {
            //var sqlRules = "\nselect DisciplineIncident.IncidentIdentifier,\n\t DisciplineIncident.SchoolId as educationOrganizationId,\n\t   'Edfi.School' discrimentator,\n\t DisciplineIncident.IncidentDate\nFrom edfi.DisciplineIncident\nleft join edfi.DisciplineIncidentBehavior\n\ton DisciplineIncident.IncidentIdentifier = DisciplineIncidentBehavior.IncidentIdentifier\nWhere DisciplineIncidentBehavior.BehaviorDescriptorId is null\n";
            // Create some options:
            var connString = new Option<string>(
                 "--connString",
                 description: "Connection string to DataBase.", getDefaultValue: () => _defaultValue);
            connString.AddValidator(r =>
            {
                var value = r.GetValueOrDefault<string>();
                if (!string.IsNullOrEmpty(value))
                    if (value.ToLower().Contains("your_connection_string"))
                        return $"{r.Token.Value} is not valid \r";
                return "";
            });
            connString.Required = true;
            var sqlRules = new Option<string>(
              "--sqlRules",
              description: "Connection string to DataBase.", getDefaultValue: () => _defaultValue);
            sqlRules.AddValidator(r =>
            {
                var value = r.GetValueOrDefault<string>();
                if (!string.IsNullOrEmpty(value))
                    if (value.ToLower().Contains("your_collection_Rules") || value.Contains("<") || value.Contains(">"))
                        return $"{r.Token.Value} is not valid \r";
                return "";
            });
            sqlRules.Required = true;

            var ruleName = new Option<string>(
             "--ruleName",
             description: "Connection string to DataBase.", getDefaultValue: () => _defaultValue);
            ruleName.AddValidator(r =>
            {
                var value = r.GetValueOrDefault<string>();
                if (!string.IsNullOrEmpty(value))
                    if (value.ToLower().Contains("your_collection_Rules") || value.Contains("<") || value.Contains(">"))
                        return $"{r.Token.Value} is not valid \r";
                return "";
            });
            ruleName.Required = true;
            // Add the options to a root command:
            var rootCommand = new RootCommand
                {
                    connString,
                    sqlRules,
                    ruleName
                };
            rootCommand.Description = "MSDF.DataChecker.RuleExec (Example of parameters :  --connString Server=localhost;Port=5432;Database=DataCheckerPersistence1;User Id=postgres;Password=******)";
            return rootCommand;
        }

        public static CollectionJson BuildCollectionJsonRule(string collectionRules, string ruleName)
        {
            var collection = new CollectionJson();
            var containerJson = new ContainerJson();
            var rules = new RuleJson();
            collection.Name = $"{ruleName}  data validations";
            collection.Description = $"{ruleName}  data validations";
            collection.EnvironmentType = $"Ed-Fi v3.X";

            rules.Name = $" {ruleName}";
            rules.SeverityLevel = 1;
            rules.Sql = collectionRules;
            rules.Version = "1";
            rules.Description = $"Rules related to {ruleName}";

            containerJson.Description = $"Rules related to {ruleName}";
            containerJson.Name = $"{ruleName}";
            containerJson.Rules.Add(rules);
            collection.Containers.Add(containerJson);

            return collection;
        }

        public static DatabaseEnvironmentBO BuildDatabaseEnvironment(string connString, string Engine)
        {
            var dbEnvironment = new DatabaseEnvironmentBO();
            var builderNpgsql = new NpgsqlConnectionStringBuilder();
            var builderSql = new SqlConnectionStringBuilder();
            var isPostgres = true;
            if (Engine == "SqlServer")
            {
                isPostgres = false;
                builderSql = new SqlConnectionStringBuilder(connString);
            }
            else
                builderNpgsql = new NpgsqlConnectionStringBuilder(connString);

            dbEnvironment.Database = isPostgres ? builderNpgsql.Database : builderSql.InitialCatalog;
            dbEnvironment.DataSource = isPostgres ? builderNpgsql.Host : builderSql.DataSource;
            dbEnvironment.Name = isPostgres ? builderNpgsql.Database : builderSql.InitialCatalog;
            dbEnvironment.User = isPostgres ? builderNpgsql.Username : builderSql.UserID;
            dbEnvironment.Password = isPostgres ? builderNpgsql.Password : builderSql.Password;
            dbEnvironment.SecurityIntegrated = false;
            dbEnvironment.MaxNumberResults = 100;
            return dbEnvironment;
        }


        public static List<RuleBO> RulesToExecute(
      IContainerService _containerService,
      IRuleService _ruleService,
      CollectionJson collectionJsonRule)
        {
            var toRun = new List<RuleBO>();
            var collections = new List<Guid>();
            var containers = new List<Guid>();
            var tags = new List<int>();

            var listCollections = _containerService.GetAsync().GetAwaiter().GetResult();
            var listContainers = _containerService.GetChildContainersAsync().GetAwaiter().GetResult();
            var existContainer = listContainers.FirstOrDefault(rec => rec.Name == collectionJsonRule.Containers.FirstOrDefault().Name && rec.ParentContainerName == collectionJsonRule.Name);
            var existCollection = listCollections.FirstOrDefault(rec => rec.Name == collectionJsonRule.Name);
            containers.Add(existContainer.Id);
            collections.Add(existCollection.Id);

            var resultRules = _ruleService.SearchRulesAsync(collections, containers, tags, string.Empty, string.Empty, null, null).GetAwaiter().GetResult();
            toRun.AddRange(resultRules.Rules);
            toRun = toRun.Where(r => r.Id != Guid.Empty).ToList();
            return toRun;
        }

        public static async Task<RuleTestResult> ExecuteRuleByEnvironmentId(
            IContainerService _containerService,
            IValidationRunService _validationRunService,
            IRuleService _ruleService,
            IRuleExecService _executionService,
            IDatabaseEnvironmentService _databaseEnvironmentService,
            CollectionJson collectionJsonRule, DatabaseEnvironmentBO databaseEnvironment)
        {
            var result = new RuleTestResult();
            //var _containerService = serviceProvider.GetService<IContainerService>();
            //var tagService = serviceProvider.GetService<ITagService>();
            //var _ruleService = serviceProvider.GetService<IRuleService>();
            //var _executionService = serviceProvider.GetService<IRuleExecService>();
            //var _catalogService = serviceProvider.GetService<ICatalogService>();
            //var _databaseEnvironmentService = serviceProvider.GetService<IDatabaseEnvironmentService>();
            //var collectionJsonRuleResult = await _containerService.UploadCollectionAsJson(collectionJsonRule);
            var toRun = new List<RuleBO>();
            var collections = new List<Guid>();
            var containers = new List<Guid>();
            var tags = new List<int>();

            var listCollections = _containerService.GetAsync().GetAwaiter().GetResult();
            var listContainers = _containerService.GetChildContainersAsync().GetAwaiter().GetResult();
            var existContainer = listContainers.FirstOrDefault(rec => rec.Name == collectionJsonRule.Containers.FirstOrDefault().Name && rec.ParentContainerName == collectionJsonRule.Name);
            var existCollection = listCollections.FirstOrDefault(rec => rec.Name == collectionJsonRule.Name);
            containers.Add(existContainer.Id);
            collections.Add(existCollection.Id);

            var resultRules = _ruleService.SearchRulesAsync(collections, containers, tags, string.Empty, string.Empty, null, null).GetAwaiter().GetResult();
            toRun.AddRange(resultRules.Rules);
            toRun = toRun.Where(r => r.Id != Guid.Empty).ToList();
            databaseEnvironment.UserParams = new List<UserParamBO>();
            databaseEnvironment = await _databaseEnvironmentService.GetAsync(databaseEnvironment.Id);
            var validationRun = new ValidationRunBO
            {
                HostDatabase = databaseEnvironment.Database,
                HostServer = databaseEnvironment.DataSource,
                RunStatus = "Running",
                Source = "Manual",
                StartTime = DateTime.Now
            };

            var validationResult = await _validationRunService.AddAsync(validationRun);

            foreach (var r in toRun)
            {

                result = await _executionService.ExecuteRuleByEnvironmentIdAsync(validationResult.Id, r.Id, databaseEnvironment);
            }
            return result;
        }
        public static bool validateRule(this string sql)
        {
            var isValid = true;
            string[] sqlCheckList = { "--",";--",";","/*","*/","@@","@","char","nchar","varchar","nvarchar","alter","begin","cast","create","cursor","declare",

                                       "delete","drop","end","exec","execute","fetch","insert","kill","sys","sysobjects","syscolumns","update"};
            string CheckString = sql.Replace("'", "''");

            for (int i = 0; i <= sqlCheckList.Length - 1; i++)
            {

                if ((CheckString.IndexOf(sqlCheckList[i], StringComparison.OrdinalIgnoreCase) >= 0))
                { isValid = false; }
            }

            return isValid;
        }
    }
}
