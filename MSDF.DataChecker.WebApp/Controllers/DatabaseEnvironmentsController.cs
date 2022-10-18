// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MSDF.DataChecker.Persistence.Settings;
using MSDF.DataChecker.Services;
using MSDF.DataChecker.Services.Models;
using System;
using System.Threading.Tasks;

namespace MSDF.DataChecker.WebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DatabaseEnvironmentsController : ControllerBase
    {
        private IDatabaseEnvironmentService _databaseEnvironmentService;
        private IConfiguration _configuration;
        private readonly DataBaseSettings _appSettings;
        public DatabaseEnvironmentsController(
            IDatabaseEnvironmentService enviromentService,
            IConfiguration configuration,
            IOptionsSnapshot<DataBaseSettings> appSettings)
        {
            _databaseEnvironmentService = enviromentService;
            _configuration = configuration;
            _appSettings = appSettings.Value;
        }

        [HttpGet]
        public async Task<ActionResult> Get()
        {
            var list = await _databaseEnvironmentService
                .GetAsync();

            if (list != null)
            {
                list.ForEach(p => p.Password = string.Empty);
                return Ok(list);
            }

            return NotFound();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> Get(Guid id)
        {
            var result = await _databaseEnvironmentService
                .GetAsync(id);

            if (result != null)
            {
                result.Password = String.Empty;
                return Ok(result);
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<ActionResult> Add([FromBody] DatabaseEnvironmentBO model)
        {
            var databaseEnvironment = await _databaseEnvironmentService
                .AddAsync(model);

            if (databaseEnvironment != null)
                return CreatedAtAction("Get", new { id = databaseEnvironment.Id }, databaseEnvironment);

            return BadRequest();
        }

        [HttpPost("Update")]
        public async Task<ActionResult> Update([FromBody] DatabaseEnvironmentBO model)
        {
            await _databaseEnvironmentService
                .UpdateAsync(model);

            return NoContent();
        }

        [HttpGet("Delete/{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            await _databaseEnvironmentService
                .DeleteAsync(id);

            return Accepted();
        }

        [HttpPost("Duplicate/{id}")]
        public async Task<ActionResult> DuplicateDatabaseEnvironment(Guid id)
        {
            var result = await _databaseEnvironmentService
                .DuplicateAsync(id);

            if (result != null)
                return Ok(result);

            return BadRequest();
        }

        [HttpGet("MapTableInformation/{id}")]
        public async Task<ActionResult> GetMapTable(Guid id)
        {
            var result = await _databaseEnvironmentService
                .GetMapTableAsync(id);

            if (result != null)
                return Ok(result);

            return NotFound();
        }

        [HttpPost("TestConnection")]
        public async Task<ActionResult> TestConnection([FromBody] DatabaseEnvironmentBO model)
        {
            var result = await _databaseEnvironmentService
                .TestConnectionAsync(model.GetConnectionString(_appSettings.Engine));

            if (result != null)
                return Ok(result);

            return BadRequest();
        }

        [HttpPost("TestConnectionById")]
        public async Task<ActionResult> TestConnectionById([FromBody] DatabaseEnvironmentBO model)
        {
            var result = await _databaseEnvironmentService
                .TestConnectionByIdAsync(model.Id);

            if (result != null)
                return Ok(result);

            return BadRequest();
        }

        [HttpGet("GetMaxNumberResults")]
        public ActionResult GetMaxNumberResults()
        {
            int result = 100;
            string value = _configuration["MaxNumberResults"];

            if (!string.IsNullOrEmpty(value))
                result = Convert.ToInt32(value);

            return Ok(result);
        }
    }
}
