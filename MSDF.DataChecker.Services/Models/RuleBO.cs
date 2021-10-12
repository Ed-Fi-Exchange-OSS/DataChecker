// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using MSDF.DataChecker.Persistence.Rules;
using System;
using System.Collections.Generic;

namespace MSDF.DataChecker.Services.Models
{
    public class RuleBO
    {
        public Guid Id { get; set; }
        public Guid ContainerId { get; set; }
        public Guid? CreatedByUserId { get; set; }
        public string CreatedByUserName { get; internal set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ErrorMessage { get; set; }
        public int ErrorSeverityLevel { get; set; }
        public string Resolution { get; set; }
        public string DiagnosticSql { get; set; }
        public string Version { get; set; }
        public bool Enabled { get; set; }
        public string RuleIdentification { get; set; }
        public List<RuleExecutionLogBO> RuleExecutionLogs { get; set; }
        public List<TagBO> Tags { get; set; }
        public int Counter { get; set; }
        public int LastStatus { get; set; }
        public DateTime? LastExecution { get; set; }
        public bool TagIsInherited { get; set; }
        public int? CollectionRuleDetailsDestinationId { get; set; }
        public int? MaxNumberResults { get; set; }

        public string EnvironmentTypeText { get; set; }
        public Guid ParentContainer { get; set; }
        public string CollectionContainerName { get; set; }


        public string CollectionName { get; set; }
        public string ContainerName { get; set; }

        public DateTime? DateUpdated { get; set; }


        public RuleBO()
        {

        }

        public RuleBO(Rule rule)
        {
            this.Id = rule.Id;
            this.Description = rule.Description;
            this.DiagnosticSql = rule.DiagnosticSql;
            //this.Enabled = rule.Enabled;
            this.ErrorMessage = rule.ErrorMessage;
            this.ErrorSeverityLevel = rule.ErrorSeverityLevel;
            this.Name = rule.Name;
            this.Resolution = rule.Resolution;
            this.Version = rule.Version;
            this.RuleIdentification = rule.RuleIdentification;
            this.MaxNumberResults = rule.MaxNumberResults;
            this.DateUpdated = rule.DateUpdated;
        }
    }

    public class RuleTestResult
    {
        public int Id { get; set; }
        public RuleBO Rule { get; set; }
        public int Result { get; set; }
        public long ExecutionTimeMs { get; set; }
        public bool Evaluation { get; set; }
        public Status Status { get; set; }
        public System.DateTime? LastExecuted { get; set; }
        public List<RuleTestResult> TestResults { get; set; }
        public string ErrorMessage { get; set; }
        public string ExecutedSql { get; set; }
        public string DiagnosticSql { get; set; }
        public int? RuleDetailsDestinationId { get; set; }
    }

    public enum Status
    {
        Succeded = 1,
        Failed = 2,
        Error = 3
    }
}
