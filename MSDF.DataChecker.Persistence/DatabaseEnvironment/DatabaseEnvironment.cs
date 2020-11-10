// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using MSDF.DataChecker.Persistence.UserParams;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MSDF.DataChecker.Persistence.DatabaseEnvironments
{
    [Table("DatabaseEnvironments", Schema = "datachecker")]
    public class DatabaseEnvironment
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
        public DateTime CreatedDate{ get; set; }
        public virtual List<UserParam> UserParams { get; set; }
        public bool? SecurityIntegrated { get; set; }
        public int? MaxNumberResults { get; set; }
        public int? TimeoutInMinutes { get; set; }
        public string DecryptPassword()
        {
            return Tools.CryptoTools.DecryptString(Password);
        }
    }  
}
