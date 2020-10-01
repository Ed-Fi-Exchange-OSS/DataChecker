// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using Microsoft.AspNetCore.Mvc;
using MSDF.DataChecker.Services;
using MSDF.DataChecker.Services.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MSDF.DataChecker.WebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserParamController : ControllerBase
    {
        private IUserParamService _userParamService;

        public UserParamController(IUserParamService enviromentService)
        {
            _userParamService = enviromentService;
        }

        [HttpPost]
        public async Task<ActionResult> AddDatabaseEnviroment([FromBody] UserParamBO model)
        {
            var userParam = await _userParamService
                .AddAsync(model);

            if (model != null) 
                return CreatedAtAction("Get", new { databaseEnvironmentId = userParam.DatabaseEnvironmentId }, userParam);

            return BadRequest();
        }

        [HttpPost("Update")]
        public async Task<ActionResult> Update([FromBody] UserParamBO model)
        {
            await _userParamService
                .UpdateAsync(model);

            return NoContent();
        }

        [HttpGet("Delete/{id}")]
        public async Task<ActionResult> Delete([FromBody] UserParamBO model)
        {
            await _userParamService
                .DeleteAsync(model);

            return Accepted();
        }

        [HttpGet("{databaseEnvironmentId}")]
        public async Task<ActionResult> Get(Guid databaseEnvironmentId)
        {
            var model = await _userParamService
                .GetByEnvironmentIdAsync(databaseEnvironmentId);

            if (model != null) 
                return Ok(model);

            return NotFound();
        }

        [HttpPost("UpdateUserParams/{databaseEnvironmentId}")]
        public async Task<ActionResult> UpdateUserParams([FromBody] List<UserParamBO> models,Guid databaseEnvironmentId)
        {
            var listUpdated = await _userParamService
                .UpdateListAsync(models,databaseEnvironmentId);

            if (listUpdated != null)
                return Ok(listUpdated);

            return BadRequest();
        }
    }
}
