// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using MSDF.DataChecker.Persistence.Settings;
using MSDF.DataChecker.Persistence.Providers;

namespace MSDF.DataChecker.Persistence.Infrastructure.IoC
{
    public static class IoCConfig
    {
        public static void RegisterDependencies(IServiceCollection container, IConfiguration configuration, IDbAccessProvider dataAccessProvider)
        {
            container.Configure<DataBaseSettings>(configuration.GetSection("DatabaseSettings"));
            var settings = configuration.GetSection("DatabaseSettings").Get<DataBaseSettings>();
            if (settings.Engine == "SqlServer")
                dataAccessProvider.SQLServer(container, settings.ConnectionStrings.SqlServer);
            else
                dataAccessProvider.PostgresSQL(container, settings.ConnectionStrings.PostgresSql);

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

            var providers = (
             from interfaceType in types.Where(t => t.Name.StartsWith("I") && t.Name.EndsWith("Provider"))
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

            foreach (var pair in providers)
                container.AddScoped(pair.InterfaceType, pair.ServiceType);
        }
    }
}
