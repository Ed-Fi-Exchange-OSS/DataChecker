// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;

namespace MSDF.DataChecker.Services.Models
{
    public class RuleExecutionLogDetailBO
    {
        public int RuleExecutionLogId { get; set; }
        public string RuleDiagnosticSql { get; set; }
        public string DestinationTable { get; set; }
        public string EnvironmentName { get; set; }
        public string RuleName { get; set; }
        public string Source { get; set; }
        public string ExecutionDateTime { get; set; }
        public List<string> Columns { get; set; }
        public List<Dictionary<string, string>> Rows { get; set; }
    }

    public class RuleExecutionLogDetailExportToTableBO
    {
        public string TableName { get; set; }
        public bool AlreadyExist { get; set; }
        public bool Created { get; set; }
        public string Message { get; set; }
        public int RuleExecutionLogId { get; set; }
    }
}
