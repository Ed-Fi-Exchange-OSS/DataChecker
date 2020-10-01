// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MSDF.DataChecker.Services;
using MSDF.DataChecker.Services.Models;
using System;
using System.Threading.Tasks;

namespace MSDF.DataChecker.WebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogsController : ControllerBase
    {
        private readonly ILogService _logService;
        private readonly ILogger<LogsController> _logger;

        public LogsController(ILogService logService, ILogger<LogsController> logger)
        {
            _logService = logService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult> AddAsync([FromBody] LogBO model)
        {
            _logger.LogError(model.Information);

            model.DateCreated = DateTime.UtcNow;
            var log = await _logService
                .AddAsync(model);

            if (log != null)
                return CreatedAtAction("Get", new { id = log.Id }, log);

            return BadRequest();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetAsync(int id)
        {
            var model = await _logService
                .GetAsync(id);

            if (model != null)
                return Ok(model);

            return NotFound();
        }
    }
}
