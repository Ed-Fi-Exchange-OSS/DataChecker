// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MSDF.DataChecker.Persistence.Providers;
using MSDF.DataChecker.Services;
using MSDF.DataChecker.Services.Models;
using MSDF.DataChecker.Services.RuleExecution;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace MSDF.DataChecker.cmd
{
    class Program
    {
        public static IConfiguration configuration;
        public static IDbAccessProvider dataAccessProvider { get; set; }
        static void Main(string[] args)
        {
            try
            {
                string containerName = string.Empty, environmentName = string.Empty, collectionName = string.Empty, tagsName = string.Empty;

                if (args == null)
                {
                    Console.WriteLine("Parameters are needed");
                    return;
                }

                for (int i = 0; i < args.Length; i++)
                {
                    string arg = args[i];
                    switch (arg)
                    {
                        case "--Collection":
                            collectionName = args[i + 1];
                            break;

                        case "--Container":
                            containerName = args[i + 1];
                            break;

                        case "--Tag":
                            tagsName = args[i + 1];
                            break;

                        case "--Environment":
                            environmentName = args[i + 1];
                            break;
                    }
                }

                Process[] localByName = Process.GetProcesses();
                var totalOfProcess = localByName.Where(rec => rec.ProcessName.Contains("DataChecker")).ToList();

                if (totalOfProcess.Count > 5)
                {
                    Console.WriteLine("No more than 5 process should be running at the same time.");
                    return;
                }

                //Validations
                if (string.IsNullOrEmpty(environmentName))
                {
                    Console.WriteLine("Environment should be defined");
                    return;
                }

                if (string.IsNullOrEmpty(tagsName) && string.IsNullOrEmpty(collectionName) && string.IsNullOrEmpty(containerName))
                {
                    Console.WriteLine("Collection, Container or Tag should be defined");
                    return;
                }

                if (!string.IsNullOrEmpty(containerName) && string.IsNullOrEmpty(collectionName))
                {
                    Console.WriteLine("Collection should be defined");
                    return;
                }

                // IoC
                var serviceCollection = new ServiceCollection();
                ConfigureServices(serviceCollection);
                var serviceProvider = serviceCollection.BuildServiceProvider();
                var _databaseEnvironmentService = serviceProvider.GetService<IDatabaseEnvironmentService>();
                var containerService = serviceProvider.GetService<IContainerService>();
                var tagService = serviceProvider.GetService<ITagService>();
                var _ruleService = serviceProvider.GetService<IRuleService>();
                var _executionService = serviceProvider.GetService<IRuleExecService>();

                List<RuleBO> toRun = new List<RuleBO>();

                var listEnvironments = _databaseEnvironmentService.GetAsync().GetAwaiter().GetResult();
                var databaseEnvironment = listEnvironments.FirstOrDefault(rec => rec.Name == environmentName);

                if (databaseEnvironment == null)
                {
                    Console.WriteLine("Environment does not exist.");
                    return;
                }

                List<Guid> collections = new List<Guid>();
                List<Guid> containers = new List<Guid>();
                List<int> tags = new List<int>();

                if (!string.IsNullOrEmpty(tagsName))
                {
                    var listTags = tagService.GetAsync().GetAwaiter().GetResult();
                    var existTag = listTags.FirstOrDefault(rec => rec.Name == tagsName);
                    if (existTag == null)
                    {
                        Console.WriteLine("Tag does not exist.");
                        return;
                    }
                    else
                    {
                        tags.Add(existTag.Id);
                    }
                }
                else
                {
                    var listCollections = containerService.GetAsync().GetAwaiter().GetResult();
                    var listContainers = containerService.GetChildContainersAsync().GetAwaiter().GetResult();

                    if (!string.IsNullOrEmpty(collectionName) && !string.IsNullOrEmpty(containerName))
                    {
                        var existContainer = listContainers.FirstOrDefault(rec => rec.Name == containerName && rec.ParentContainerName == collectionName);
                        if (existContainer == null)
                        {
                            Console.WriteLine("Collection or Container does not exist.");
                            return;
                        }
                        else
                        {
                            containers.Add(existContainer.Id);
                        }
                    }
                    else if (!string.IsNullOrEmpty(collectionName))
                    {
                        var existCollection = listCollections.FirstOrDefault(rec => rec.Name == collectionName);
                        if (existCollection == null)
                        {
                            Console.WriteLine("Collection does not exist.");
                            return;
                        }
                        else
                        {
                            collections.Add(existCollection.Id);
                        }
                    }
                }

                var result = _ruleService.SearchRulesAsync(collections, containers, tags, string.Empty, string.Empty, null, null).GetAwaiter().GetResult();
                toRun.AddRange(result.Rules);
                toRun = toRun.Where(r => r.Id != Guid.Empty).ToList();

                foreach (var r in toRun)
                {
                    _executionService.ExecuteRuleByEnvironmentIdAsync(r.Id, databaseEnvironment).GetAwaiter().GetResult();
                }

                Console.WriteLine("Finished.");
                Console.SetOut(Console.Out);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception:");
                Console.WriteLine(ex.Message);
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
                .AddTransient<IRuleExecService>()
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
            serviceCollection.AddTransient<IRuleExecService, IRuleExecService>();
            serviceCollection.AddTransient<IDatabaseEnvironmentService, DatabaseEnvironmentService>();
            serviceCollection.AddTransient<IContainerService, ContainerService>();
            serviceCollection.AddTransient<ITagService, TagService>();

            dataAccessProvider = new DbAccessProvider();
            Services.Infrastructure.IoC.IoCConfig.RegisterDependencies(serviceCollection, configuration, dataAccessProvider); ;
        }
    }
}
