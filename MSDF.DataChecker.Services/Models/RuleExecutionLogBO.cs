// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;

namespace MSDF.DataChecker.Services.Models
{
    public class RuleExecutionLogBO
    {
        public int Id { get; set; }
        public Guid RuleId { get; set; }
        public Guid DatabaseEnvironmentId { get; set; }
        public string Response { get; set; }
        public int Result { get; set; }
        public bool Evaluation { get; set; }
        public int StatusId { get; set; }
        public DateTime ExecutionDate { get; set; }
        public long ExecutionTimeMs { get; set; }
        public string ExecutedSql { get; set; }
        public string DiagnosticSql { get; set; }
    }
}
