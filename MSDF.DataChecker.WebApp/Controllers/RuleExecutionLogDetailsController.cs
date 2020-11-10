// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using Microsoft.AspNetCore.Mvc;
using MSDF.DataChecker.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MSDF.DataChecker.WebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RuleExecutionLogDetailsController : ControllerBase
    {
        private readonly IRuleExecutionLogDetailService _ruleExecutionLogDetailService;

        public RuleExecutionLogDetailsController(
            IRuleExecutionLogDetailService ruleExecutionLogDetailService)
        {
            _ruleExecutionLogDetailService = ruleExecutionLogDetailService;
        }

        [HttpGet("LastRuleExecutionLogByEnvironmentAndRule/{environmentId}/{ruleId}")]
        public async Task<ActionResult> LastRuleExecutionLogByEnvironmentAndRule(Guid environmentId, Guid ruleId)
        {
            var model = await _ruleExecutionLogDetailService
                .GetLastRuleExecutionLogByEnvironmentAndRuleAsync(environmentId,ruleId);

            return Ok(model);
        }

        [HttpGet("RuleExecutionLogAsync/{id}")]
        public async Task<ActionResult> GetByRuleExecutionLogAsync(int id)
        {
            var model = await _ruleExecutionLogDetailService
                .GetByRuleExecutionLogIdAsync(id);

            if (model != null && model.Columns != null && model.Columns.Count > 0)
                return Ok(model);

            return Ok(null);
        }

        [HttpGet("ExportToCsvAsync/{id}")]
        public async Task<ActionResult> ExportToCsvAsync(int id)
        {
            StringBuilder builder = new StringBuilder();

            var model = await _ruleExecutionLogDetailService
                .GetByRuleExecutionLogIdAsync(id);

            if (model != null && model.Columns != null && model.Columns.Count > 0)
            {
                string headers = string.Empty;
                for (int i = 0; i < model.Columns.Count; i++)
                {
                    string column = model.Columns[i];
                    headers += $"{column}";
                    if ((i + 1) < model.Columns.Count)
                        headers += ",";
                }
                builder.AppendLine(headers);

                for (int i = 0; i < model.Rows.Count; i++)
                {
                    var row = model.Rows[i];
                    string rowsValues = string.Empty;
                    for (int j = 0; j < model.Columns.Count; j++)
                    {
                        string column = model.Columns[j];
                        string value = row.GetValueOrDefault(column);

                        rowsValues += $"\"{value}\"";
                        if ((j + 1) < model.Columns.Count)
                            rowsValues += ",";
                    }
                    builder.AppendLine(rowsValues);
                }

                return File(Encoding.UTF8.GetBytes(builder.ToString()), "text/csv", $"RuleExecutionLogDetails-{model.RuleExecutionLogId}.csv");
            }

            return NotFound();
        }

        [HttpGet("ExportToTableAsync/{id}")]
        public async Task<ActionResult> ExportToTableAsync(int id)
        {
            var model = await _ruleExecutionLogDetailService
                .ExportToTableByRuleExecutionLogIdAsync(id);

            if (model != null)
                return Ok(model);

            return NotFound();
        }

        [HttpGet("ExecuteDiagnosticSqlFromLog/{id}")]
        public async Task<ActionResult> ExecuteDiagnosticSqlFromLog(int id)
        {
            var model = await _ruleExecutionLogDetailService
                .ExecutionDiagnosticSqlByLogIdAsync(id);

            if (model != null && model.Columns != null && model.Columns.Count > 0)
                return Ok(model);

            return Ok(null);
        }
    }
}
