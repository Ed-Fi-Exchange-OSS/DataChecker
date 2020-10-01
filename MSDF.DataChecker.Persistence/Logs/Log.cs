// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MSDF.DataChecker.Persistence.Logs
{
    [Table("Logs", Schema = "core")]
    public class Log
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Information { get; set; }
        public string Source { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
