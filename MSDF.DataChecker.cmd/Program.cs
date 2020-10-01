// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MSDF.DataChecker.cmd.ExtensionMethods;
using MSDF.DataChecker.Services;
using MSDF.DataChecker.Services.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MSDF.DataChecker.cmd
{
    class Program
    {
        public static IConfiguration configuration;

        static void Main(string[] args)
        {
            string ruleId = string.Empty, containerId = string.Empty, environmentId = string.Empty, environmentName = string.Empty;

            if (args == null)
            {
                Console.WriteLine("Parameters are needed");
                return;
            }

            if (args.Length != 8)
            {
                Console.WriteLine("All parameters need to be provided");
                return;
            }

            for(int i=0; i< args.Length; i++)
            {
                string arg = args[i].ToLower();
                switch (arg)
                {
                    case "--ruleid":
                        ruleId = args[i + 1];
                        break;

                    case "--containerid":
                        containerId = args[i + 1];
                        break;

                    case "--environmentid":
                        environmentId = args[i + 1];
                        break;

                    case "--environmentname":
                        environmentName = args[i + 1];
                        break;
                }
            }

            Guid environmentGUID;
            if (!Guid.TryParse(environmentId, out environmentGUID))
            {
                Console.WriteLine("Please provide a valid environmentId");
                return;
            }

            // IoC
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var environmentService = serviceProvider.GetService<IDatabaseEnvironmentService>();
            var ruleService = serviceProvider.GetService<IRuleService>();
            var dataChecker = serviceProvider.GetService<IRuleExecutionService>();            

            var getEnvironment = environmentService.GetAsync(environmentGUID).GetAwaiter().GetResult();
            if (getEnvironment == null || string.IsNullOrEmpty(getEnvironment.GetConnectionString()))
            {
                Console.WriteLine("Please provide a valid environmentId");
                return;
            }

            if (getEnvironment.Name.ToLower() != environmentName)
            {
                Console.WriteLine("environmentName does not match, review it");
                return;
            }

            Console.WriteLine($"Running Data Checker V{getEnvironment.Version} Session ({Guid.NewGuid()})");
            Console.WriteLine("");

            List<RuleBO> rules = new List<RuleBO>();

            // Get all rules to execute
            Console.Write("Loading rules ...");
            var sw = System.Diagnostics.Stopwatch.StartNew();

            if (!string.IsNullOrEmpty(containerId))
            {
                string[] listContainers = containerId.Split(",");
                foreach (string currentContainer in listContainers)
                {
                    Guid containerGUID;
                    if (!Guid.TryParse(currentContainer, out containerGUID))
                    {
                        Console.WriteLine("Please provide a valid containerId");
                        return;
                    }
                    Console.WriteLine($"Getting rules from containerId:{currentContainer}");
                    rules.AddRange(ruleService.GetWithLogsByContainerIdAsync(containerGUID).GetAwaiter().GetResult());
                }
                rules = rules.Distinct().ToList();
            }

            if (!string.IsNullOrEmpty(ruleId))
            {
                if (rules.Count == 0)
                    rules = ruleService.GetAsync().GetAwaiter().GetResult();

                Guid ruleGUID;
                if (!Guid.TryParse(ruleId, out ruleGUID))
                {
                    Console.WriteLine("Please provide a valid ruleId");
                    return;
                }
                rules = rules.Where(p => p.Id == ruleGUID).ToList();

                if (rules.Count == 0)
                {
                    Console.WriteLine($"Rule with Id {ruleId} not found");
                    return;
                }
            }

            sw.Stop();
            Console.WriteLine($" Loaded {rules.Count()} in {sw.ElapsedMilliseconds}ms");
            Console.WriteLine("");

            // Execute the rules
            var testReuslts = dataChecker.ExecuteRulesByEnvironmentIdAsync(rules,getEnvironment).GetAwaiter().GetResult();

            foreach (var result in testReuslts)
            {
                if (result.Evaluation)
                    ConsoleX.WriteLineSuccess($"Test {result.Rule.Id} \"{result.Rule.Name}\" executed SUCCESSFULLY! Found {result.Result} records in {result.ExecutionTimeMs} ms.");
                else
                {
                    ConsoleX.WriteLineError($"Test {result.Rule.Id} \"{result.Rule.Name}\" FAILED! {result.Rule.ErrorMessage.Replace("{TestResult.Result}", result.Result.ToString())}");

                    if (!string.IsNullOrEmpty(result.Rule.DiagnosticSql))
                        ConsoleX.WriteLineInfo($"For diagnostics data run: {result.Rule.DiagnosticSql}");
                }
            }

            // TODO: Use a logging framework like the one in .net core or Log4net
            WriteErrorsToFile(testReuslts);

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();

            Console.SetOut(Console.Out);
        }

        private static void ConfigureServices(IServiceCollection serviceCollection)
        {
            // Add logging
            serviceCollection.AddLogging(c => c.AddConsole())
                .AddTransient<RuleService>()
                .AddTransient<RuleExecutionService>()
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
            serviceCollection.AddTransient<IRuleExecutionService, RuleExecutionService>();
            serviceCollection.AddTransient<IDatabaseEnvironmentService, DatabaseEnvironmentService>();

            Services.Infrastructure.IoC.IoCConfig.RegisterDependencies(serviceCollection, configuration);
        }

        private static void WriteErrorsToFile(List<RuleTestResult> allResults)
        {
            var errors = allResults.Where(x => x.Evaluation == false);

            using (var fileStream = new FileStream("./log.txt",FileMode.OpenOrCreate, FileAccess.Write))
                using (var streamWriter = new StreamWriter(fileStream))
                {
                    foreach (var error in errors)
                    {
                        var rule = error.Rule;
                        streamWriter.WriteLine($"[Error] - Test {rule.Id} FAILED! { rule.ErrorMessage.Replace("{TestResult.Result}", error.Result.ToString())}");

                        if (!string.IsNullOrEmpty(rule.DiagnosticSql))
                            streamWriter.WriteLine($"          For diagnostics data run: {rule.DiagnosticSql}");
                    }
                }
        }
    }
}
