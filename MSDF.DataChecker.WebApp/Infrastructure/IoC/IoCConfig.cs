// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MSDF.DataChecker.Persistence.Providers;

namespace MSDF.DataChecker.WebApp.Infrastructure.IoC
{
    public static class IoCConfig
    {
        public static void RegisterDependencies(IServiceCollection container, IConfiguration configuration, IDbAccessProvider dataAccessProvider)
        {
            container.AddSingleton<IConfiguration>(configuration);
            // Register resources/services dependencies
            Services.Infrastructure.IoC.IoCConfig.RegisterDependencies(container, configuration, dataAccessProvider);
        }
    }
}
