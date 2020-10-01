// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using MSDF.DataChecker.Persistence.RuleExecutionLogs;
using System.ComponentModel.DataAnnotations.Schema;

namespace MSDF.DataChecker.Persistence.RuleExecutionLogDetails
{
    [Table("EdFiRuleExecutionLogDetails", Schema = "destination")]
    public class EdFiRuleExecutionLogDetail
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int? EducationOrganizationId { get; set; }
        public string StudentUniqueId { get; set; }
        public string CourseCode { get; set; }
        public string Discriminator { get; set; }
        public string ProgramName { get; set; }
        public string StaffUniqueId { get; set; }
        public string OtherDetails { get; set; }
        public int RuleExecutionLogId { get; set; }
        [ForeignKey("RuleExecutionLogId")]
        public RuleExecutionLog RuleExecutionLog { get; set; }
    }

    public class DestinationTableColumn
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public bool IsNullable { get; set; }
    }
}
