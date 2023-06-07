// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.ComponentModel.DataAnnotations.Schema;
using MSDF.DataChecker.Persistence.ValidationsRun;

namespace MSDF.DataChecker.Persistence.RuleExecutionLogs
{
    [Table("RuleExecutionLogs", Schema = "destination")]
    public class RuleExecutionLog
    {
        public int Id { get; set; }
        public Guid RuleId { get; set; }
        public Guid DatabaseEnvironmentId { get; set; }
        public string Response { get; set; }
        public int Result { get; set; }
        public bool Evaluation { get; set; }
        public int  StatusId { get; set; }
        public DateTime ExecutionDate { get; set; }
        public long ExecutionTimeMs { get; set; }
        public string ExecutedSql { get; set; }
        public string DiagnosticSql { get; set; }
        public int? RuleDetailsDestinationId { get; set; }
        public string DetailsSchema { get; set; }
        public string DetailsTableName { get; set; }


        public int? ValidationRunId { get; set; }
        [ForeignKey("ValidationRunId")]
        public ValidationRun ValidationRun { get; set; }
    }
}
