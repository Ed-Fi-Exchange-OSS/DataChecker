// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MSDF.DataChecker.Services.Models
{
    public class JobBO
    {
        public int Id { get; set; }
        public string Cron { get; set; }
        public string Status { get; set; }
        public DateTime? LastFinishedDateTime { get; set; }
        public string Name { get; set; }
        public string TypeName { get; set; }
        public JobType Type { get; set; }
        public Guid? DatabaseEnvironmentId { get; set; }
        public int? TagId { get; set; }
        public Guid? ContainerId { get; set; }

        public enum JobType
        {
            Tag = 1,
            Container
        }

        public class JobRunner
        {
            private readonly IRuleService _ruleService;
            private readonly ITagService _tagService;
            private readonly IContainerService _containerService;
            private readonly IDatabaseEnvironmentService _databaseEnvironmentService;
            private readonly IRuleExecutionService _executionService;

            public JobRunner(ITagService tagService,
                             IContainerService containerService,
                             IDatabaseEnvironmentService databaseEnvironmentService,
                             IRuleExecutionService executionService,
                             IRuleService ruleService)
            {
                _tagService = tagService;
                _containerService = containerService;
                _databaseEnvironmentService = databaseEnvironmentService;
                _executionService = executionService;
                _ruleService = ruleService;
            }

            public async Task Run(JobBO job)
            {
                if (job == null) return;
                if (job.DatabaseEnvironmentId == null) return;

                List<RuleBO> toRun = new List<RuleBO>();

                var databaseEnvironment = await _databaseEnvironmentService.GetAsync(job.DatabaseEnvironmentId.Value);

                List<Guid> collections = new List<Guid>();
                List<Guid> containers = new List<Guid>();
                List<int> tags = new List<int>();

                switch (job.Type)
                {
                    case JobType.Tag:
                        if (!job.TagId.HasValue) return;
                        tags.Add(job.TagId.Value);
                        break;
                    case JobType.Container:
                        var containerType = await _containerService.GetAsync(job.ContainerId.Value);
                        if(containerType.ContainerTypeId == 1)
                            collections.Add(job.ContainerId.Value);
                        else
                            containers.Add(job.ContainerId.Value);                       
                        break;
                }

                var result = await _ruleService.SearchRulesAsync(collections, containers, tags, string.Empty, string.Empty, null, null);
                toRun.AddRange(result.Rules);
                toRun = toRun.Where(r => r.Id != Guid.Empty).ToList();

                foreach (var r in toRun)
                {
                    await _executionService.ExecuteRuleByEnvironmentIdAsync(r.Id, databaseEnvironment);
                }
            }
        }
    }
}
