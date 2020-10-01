// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MSDF.DataChecker.Persistence.EntityFramework;
using System;
using System.Linq;
using Hangfire;
using Hangfire.SqlServer;

namespace MSDF.DataChecker.Persistence.Infrastructure.IoC
{
    public static class IoCConfig
    {
        public static void RegisterDependencies(IServiceCollection container, IConfiguration configuration)
        {
            container.AddDbContext<DatabaseContext>(options =>options.UseSqlServer(configuration.GetConnectionString("DataCheckerStore")),ServiceLifetime.Transient);

            container.AddHangfire(globalConfiguration => globalConfiguration
                  .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                  .UseSimpleAssemblyNameTypeSerializer()
                  .UseRecommendedSerializerSettings()
                  .UseSqlServerStorage(configuration.GetConnectionString("DataCheckerStore"), new SqlServerStorageOptions
                  {
                      CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                      SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                      QueuePollInterval = TimeSpan.Zero,
                      UseRecommendedIsolationLevel = true,
                      DisableGlobalLocks = true
                  }));

            // Add the processing server as IHostedService
            container.AddHangfireServer();

            RegisterCommandsAndQueriesByConvention<IPersistenceMarker>(container);
        }

        private static void RegisterCommandsAndQueriesByConvention<TMarker>(IServiceCollection container)
        {
            var types = typeof(TMarker).Assembly.ExportedTypes;

            var commandsToRegister = (
                from interfaceType in types.Where(t => t.Name.StartsWith("I") && t.Name.EndsWith("Commands"))
                from serviceType in types.Where(t => t.Name == interfaceType.Name.Substring(1))
                select new
                {
                    InterfaceType = interfaceType,
                    ServiceType = serviceType
                }
            );

            var queriesToRegister = (
                from interfaceType in types.Where(t => t.Name.StartsWith("I") && t.Name.EndsWith("Queries"))
                from serviceType in types.Where(t => t.Name == interfaceType.Name.Substring(1))
                select new
                {
                    InterfaceType = interfaceType,
                    ServiceType = serviceType
                }
            );

            foreach (var pair in commandsToRegister)
                container.AddScoped(pair.InterfaceType, pair.ServiceType);

            foreach (var pair in queriesToRegister)
                container.AddScoped(pair.InterfaceType, pair.ServiceType);
        }
    }
}
