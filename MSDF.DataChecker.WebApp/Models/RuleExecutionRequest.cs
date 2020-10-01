// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using MSDF.DataChecker.Services.Models;
using System;
using System.Collections.Generic;

namespace MSDF.DataChecker.WebApp.Models
{
    public class RuleExecutionRequest
    {
        public Guid RuleId { get; set; }

        public Guid DatabaseEnvironmentId { get; set; }
    }

    public class RuleExecutionDiagnosticRequest
    {
        public int RuleExecutionLogId { get; set; }

        public Guid DatabaseEnvironmentId { get; set; }
    }

    public class RuleExecutionTestRequest
    {
        public RuleBO RuleToTest { get; set; }

        public Guid DatabaseEnvironmentId { get; set; }
    }

    public class RuleCopyToBO
    {
        public Guid RuleId { get; set; }

        public Guid ContainerId { get; set; }
    }

    public class SearchRulesBO
    {
        public List<Guid> Containers { get; set; }
        public List<Guid> Collections { get; set; }
        public List<int> Tags { get; set; }
        public string Name { get; set; }
        public string DiagnosticSql { get; set; }
        public int? DetailsDestination { get; set; }
        public int? Severity { get; set; }
    }

    public class RulesDeleteByIdsBO
    {
        public List<Guid> RuleIds { get; set; }
    }

    public class RulesAssignTagsByIdsBO
    {
        public List<Guid> RuleIds { get; set; }
        public List<TagBO> Tags { get; set; }
    }

    public class RulesCopyToContainerByIdsBO
    {
        public List<Guid> RuleIds { get; set; }
        public List<Guid> ContainerIds { get; set; }
    }

    public class MoveRuleToContainerBO
    {
        public List<Guid> Rules { get; set; }
        public ContainerBO ContainerTo { get; set; }
    }
}
