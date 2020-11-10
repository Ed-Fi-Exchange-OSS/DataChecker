// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using Microsoft.AspNetCore.Mvc;
using MSDF.DataChecker.Services;
using MSDF.DataChecker.Services.Models;
using MSDF.DataChecker.WebApp.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MSDF.DataChecker.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RulesController : ControllerBase
    {
        private readonly IDatabaseEnvironmentService _databaseEnvironmentService;
        private readonly IRuleService _rulesService;
        private readonly IRuleExecutionService _executionService;

        public RulesController(IRuleService rulesService,
            IRuleExecutionService executionService,
            IDatabaseEnvironmentService databaseEnvironmentService)
        {
            _rulesService = rulesService;
            _databaseEnvironmentService = databaseEnvironmentService;
            _executionService = executionService;
        }

        [HttpGet]
        public async Task<ActionResult> GetAsync()
        {
            var model = await _rulesService
                .GetWithLogsAsync();

            if (model != null)
                return Ok(model);

            return NotFound();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetAsync(Guid id)
        {
            var model = await _rulesService
                .GetAsync(id);

            if (model != null)
                return Ok(model);

            return NotFound();
        }

        [HttpGet("Results/{id}/{databaseEnvironmentId}")]
        public async Task<ActionResult> GetResultsAsync(Guid id, Guid databaseEnvironmentId)
        {
            var model = await _rulesService
                .GetTopResults(id, databaseEnvironmentId);

            if (model != null)
                return Ok(model);

            return NotFound();
        }

        [HttpPost]
        public async Task<ActionResult> AddAsync([FromBody] RuleBO model)
        {
            var rule = await _rulesService
                .AddAsync(model);

            if (rule != null)
                return CreatedAtAction("Get", new { id = rule.Id }, rule);

            return BadRequest();
        }

        [HttpPost("Update")]
        public async Task<ActionResult> UpdateAsync([FromBody] RuleBO model)
        {
            await _rulesService
               .UpdateAsync(model);

            return NoContent();
        }

        [HttpGet("Delete/{id}")]
        public async Task<ActionResult> DeleteAsync(Guid id)
        {
            await _rulesService
                .DeleteAsync(id);

            return Accepted();
        }

        [HttpPost("Run")]
        public async Task<ActionResult> RunAsync([FromBody] RuleExecutionRequest model)
        {
            var databaseEnvironment = await _databaseEnvironmentService
                .GetAsync(model.DatabaseEnvironmentId);

            var result = await _executionService
                .ExecuteRuleByEnvironmentIdAsync(model.RuleId, databaseEnvironment);

            if (result != null)
                return Ok(result);

            return BadRequest();
        }

        [HttpPost("RunDiagnosticAndReturnTable")]
        public async Task<ActionResult> RunAndReturnTableAsync(RuleExecutionDiagnosticRequest model)
        {
            var databaseEnvironment = await _databaseEnvironmentService
                .GetAsync(model.DatabaseEnvironmentId);

            var result = await _executionService
                .ExecuteRuleDiagnosticByRuleLogIdAndEnvironmentIdAsync(model.RuleExecutionLogId, databaseEnvironment);

            if (result != null && result.Information != null)
                result.Information = result.Information.Take(100).ToList();

            return Ok(result);
        }

        [HttpPost("TestRun")]
        public async Task<ActionResult> TestRunAsync([FromBody] RuleExecutionTestRequest model)
        {
            var databaseEnvironment = await _databaseEnvironmentService
                .GetAsync(model.DatabaseEnvironmentId);

            string connectionString = databaseEnvironment
                .GetConnectionString();

            var result = await _executionService
                .ExecuteRuleAsync(model.RuleToTest, connectionString, databaseEnvironment.UserParams, databaseEnvironment.TimeoutInMinutes);

            if (result != null)
                return Ok(result);

            return BadRequest();
        }

        [HttpPost("CopyRuleTo")]
        public async Task<ActionResult> CopyRuleToAsync([FromBody] RuleCopyToBO model)
        {
            var rule = await _rulesService
                .CopyToAsync(model.RuleId, model.ContainerId);

            if (rule != null)
                return CreatedAtAction("Get", new { id = rule.Id }, rule);

            return BadRequest();
        }

        [HttpPost("SearchRules")]
        public async Task<ActionResult> SearchRulesAsync([FromBody] SearchRulesBO model)
        {
            var result = await _rulesService
                .SearchRulesAsync(model.Collections, model.Containers, model.Tags, model.Name, model.DiagnosticSql, model.DetailsDestination, model.Severity);

            if (result != null)
                return Ok(result);

            return NotFound();
        }

        [HttpPost("DeleteByIds")]
        public async Task<ActionResult> DeleteByIdsAsync([FromBody] RulesDeleteByIdsBO model)
        {
            if (model != null && model.RuleIds != null && model.RuleIds.Count > 0)
            {
                foreach (Guid ruleId in model.RuleIds)
                {
                    await _rulesService.DeleteAsync(ruleId);
                }
            }

            return Ok();
        }

        [HttpPost("AssignTagsByIds")]
        public async Task<ActionResult> AssignTagsByIdsAsync([FromBody] RulesAssignTagsByIdsBO model)
        {
            if (model != null && model.RuleIds != null && model.RuleIds.Count > 0
                && model.Tags != null && model.Tags.Count > 0)
            {
                foreach (Guid ruleId in model.RuleIds)
                {
                    await _rulesService.AssignTagsToRule(ruleId, model.Tags);
                }
            }

            return Ok();
        }

        [HttpPost("CopyToByIds")]
        public async Task<ActionResult> CopyToByIdsAsync([FromBody] RulesCopyToContainerByIdsBO model)
        {
            if (model != null && model.RuleIds != null && model.RuleIds.Count > 0
                && model.ContainerIds != null && model.ContainerIds.Count > 0)
            {
                foreach (Guid ruleId in model.RuleIds)
                {
                    foreach (Guid containerId in model.ContainerIds)
                    {
                        await _rulesService.CopyToAsync(ruleId, containerId);
                    }
                }
            }

            return Ok();
        }

        [HttpPost("MoveRulesToContainer")]
        public async Task<ActionResult> MoveRuleToContainerAsync([FromBody] MoveRuleToContainerBO model)
        {
            bool result = await _rulesService.MoveRuleToContainer(model.Rules, model.ContainerTo);
            return Ok(result);
        }
    }
}
