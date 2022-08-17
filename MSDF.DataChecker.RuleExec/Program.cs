// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MSDF.DataChecker.Persistence.Providers;
using MSDF.DataChecker.Persistence.Settings;
using MSDF.DataChecker.RuleExec.Helpers;
using MSDF.DataChecker.Services;
using MSDF.DataChecker.Services.RuleExecution;
using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.IO;
using System.Linq;
using Process = System.Diagnostics.Process;

namespace MSDF.DataChecker.RuleExec
{

    class Program
    {
        public static readonly string _defaultValue = "";
        public static IConfiguration configuration;
        public static IDbAccessProvider dataAccessProvider { get; set; }
        static int Main(string[] args)
        {
            var commandLineParameters = CommandLine.GetCommandLineParameters();
            commandLineParameters.Handler = CommandHandler.Create<string, string, string>(async (connString, sqlRules, ruleName) =>
            {
                // IoC
                var serviceCollection = new ServiceCollection();
                ConfigureServices(serviceCollection);
                var serviceProvider = serviceCollection.BuildServiceProvider();
                var _containerService = serviceProvider.GetService<IContainerService>();
                var tagService = serviceProvider.GetService<ITagService>();
                var _ruleService = serviceProvider.GetService<IRuleService>();
                var _executionService = serviceProvider.GetService<IRuleExecService>();
                var _catalogService = serviceProvider.GetService<ICatalogService>();
                var _databaseEnvironmentService = serviceProvider.GetService<IDatabaseEnvironmentService>();
                var listEnvironments = _databaseEnvironmentService.GetAsync().GetAwaiter().GetResult();
                var collection = CommandLine.BuildCollectionJsonRule(sqlRules, ruleName);
                var settings = configuration.GetSection("Settings").Get<DataBaseSettings>();
                var dbEnvironment = CommandLine.BuildDatabaseEnvironment(connString, settings.Engine);

                var databaseEnvironment = listEnvironments.FirstOrDefault(rec => rec.Name == dbEnvironment.Database && rec.DataSource == dbEnvironment.DataSource);
                if (databaseEnvironment == null)
                {
                    var _catalogResult = await _catalogService.GetByTypeAsync("EnvironmentType");
                    dbEnvironment.Version = _catalogResult.LastOrDefault().Id;
                    databaseEnvironment = await _databaseEnvironmentService.AddAsync(dbEnvironment);
                    databaseEnvironment = await _databaseEnvironmentService.GetAsync(databaseEnvironment.Id);
                }

                await _containerService.UploadCollectionAsJson(collection);
                var rulesToExecute = CommandLine.RulesToExecute(_containerService, _ruleService, collection);

                foreach (var r in rulesToExecute)
                {
                    var result = await _executionService.ExecuteRuleByEnvironmentIdAsync(r.Id, databaseEnvironment);
                }
                return;

            });

            try
            {
                return commandLineParameters.Invoke(args);
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.Write(e.Message);
                Console.ResetColor();
                return -1;
            }
        }

        private static void ConfigureServices(IServiceCollection serviceCollection)
        {
            // Add logging
            serviceCollection.AddLogging(c => c.AddConsole())
                .AddTransient<ContainerService>()
                .AddTransient<RuleService>()
                .AddTransient<TagService>()
                //.AddTransient<RuleExecutionService>()
                .AddTransient<RuleExecService>()
                .AddTransient<DatabaseEnvironmentService>();

            // Build configuration
            configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
                .AddJsonFile("appsettings.json", false)
                .Build();

            // Add access to generic IConfigurationRoot
            serviceCollection.AddSingleton<IConfiguration>(configuration);

            // Add services
            // TODO: Add generic way of registering services.
            serviceCollection.AddTransient<IRuleService, RuleService>();
            //serviceCollection.AddTransient<IRuleExecutionService, RuleExecutionService>();
            serviceCollection.AddTransient<IRuleExecService, RuleExecService>();
            serviceCollection.AddTransient<IDatabaseEnvironmentService, DatabaseEnvironmentService>();
            serviceCollection.AddTransient<IContainerService, ContainerService>();
            serviceCollection.AddTransient<ITagService, TagService>();
            dataAccessProvider = new DbAccessProvider();
            Services.Infrastructure.IoC.IoCConfig.RegisterDependencies(serviceCollection, configuration, dataAccessProvider);
        }
    }
}

