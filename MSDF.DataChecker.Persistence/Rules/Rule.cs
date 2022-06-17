// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using MSDF.DataChecker.Persistence.RuleExecutionLogs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MSDF.DataChecker.Persistence.Rules
{
    [Table("Rules", Schema = "datachecker")]
    public class Rule
    {            
        /// <summary>
        /// The unique identifier for the SQL rule.
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// Id from the Container that groups the rule.
        /// </summary>
        public Guid ContainerId { get; set; }
        /// <summary>
        /// User Id that created the rule.
        /// </summary>
        //public Guid? CreatedByUserId { get; set; }
        //public string CreatedByUserName { get; internal set; }
        /// <summary>
        /// The short name that describes the rule.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The category of the rule.
        /// </summary>

        /// <summary>
        /// A more elaborate narrative of what the rule does.
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// The friendly error message to display if the execution of the Sql and the evaluation are negative.
        /// </summary>
        public string ErrorMessage { get; set; }
        /// <summary>
        /// The severity of the error.
        /// </summary>
        public int ErrorSeverityLevel { get; set; }
        /// <summary>
        /// A description of how to solve an issue if there is one.
        /// </summary>
        public string Resolution { get; set; }
        /// <summary>
        /// The Sql statement to be run against the database.
        /// </summary>
        public string DiagnosticSql { get; set; }        
        /// <summary>
        /// The Ed-Fi ODS version this script is compatible with.
        /// </summary>
        //public string EdFiODSCompatibilityVersion { get; set; }
        /// <summary>
        /// The version for this rule.
        /// </summary>
        public string Version { get; set; }    
        /// <summary>
        /// Defines if this rule is enabled or not. If false the rule will not run.
        /// </summary>
        //public bool Enabled { get; set; }
        public string RuleIdentification { get; set; }
        public virtual List<RuleExecutionLog> RuleExecutionLogs { get; set; }
        public int? MaxNumberResults { get; set; }

        public DateTime? DateUpdated { get; set; }
    }
}
