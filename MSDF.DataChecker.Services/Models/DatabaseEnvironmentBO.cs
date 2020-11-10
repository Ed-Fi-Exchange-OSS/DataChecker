// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;

namespace MSDF.DataChecker.Services.Models
{
    public class DatabaseEnvironmentBO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Version { get; set; }
        public string Database { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public string DataSource { get; set; }
        public string ExtraData { get; set; }
        public string MapTables { get; set; }
        public DateTime CreatedDate { get; set; }
        public List<UserParamBO> UserParams { get; set; }
        public bool? SecurityIntegrated { get; set; }
        public int? MaxNumberResults { get; set; }
        public int? TimeoutInMinutes { get; set; }
        public string GetConnectionString()
        {
            if (SecurityIntegrated != null && SecurityIntegrated.Value)
                return string.Format("Data Source={0};Database={1};Integrated Security=true;{2}", DataSource, Database, ExtraData);

            return string.Format("Data Source={0};Database={1};User Id={2};Password={3};{4}", DataSource, Database, User, Password, ExtraData);
        }
    }
}
