// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using Microsoft.AspNetCore.Mvc;
using MSDF.DataChecker.Services;
using MSDF.DataChecker.Services.Models;
using System;
using System.Threading.Tasks;

namespace MSDF.DataChecker.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContainersController : ControllerBase
    {
        private readonly IContainerService _containerService;

        public ContainersController(IContainerService containersService)
        {
            _containerService = containersService;
        }

        [HttpGet]
        public async Task<ActionResult> GetAsync()
        {
            var model = await _containerService
                .GetAsync();

            if (model != null) 
                return Ok(model);

            return NotFound();
        }

        [HttpGet("ParentContainers")]
        public async Task<ActionResult> GetParentContainersAsync()
        {
            var model = await _containerService
                .GetParentContainersAsync();

            if (model != null)
                return Ok(model);

            return NotFound();
        }

        [HttpGet("ChildContainers")]
        public async Task<ActionResult> GetChildContainersAsync()
        {
            var model = await _containerService
                .GetChildContainersAsync();

            if (model != null)
                return Ok(model);

            return NotFound();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetAsync(Guid id)
        {
            var model = await _containerService
                .GetAsync(id);

            if (model != null) 
                return Ok(model);

            return NotFound();
        }

        [HttpGet("{containerId}/details/{databaseEnvironmentId}")]
        public async Task<ActionResult> GetCollectionDetails(Guid databaseEnvironmentId, Guid? containerId = null)
        {
            var model = await _containerService
                .GetByDatabaseEnvironmentIdAndContainerIdAsync(databaseEnvironmentId, containerId);

            if (model != null) 
                return Ok(model);

            return NotFound();
        }

        [HttpPost("AddContainerFromCommunity")]
        public async Task<ActionResult> AddContainerFromCommunity([FromBody] ContainerBO container)
        {
            string message = await _containerService.AddFromCommunityAsync(container);
            return Ok(message);
        }

        [HttpGet("GetToCommunity/{id}")]
        public async Task<ActionResult> GetToCommunity(Guid id)
        {
            var model = await _containerService
                .GetToCommunityAsync(id);

            if (model != null)
                return Ok(model);

            return NotFound();
        }

        [HttpPost]
        public async Task<ActionResult> AddAsync([FromBody] ContainerBO container)
        {
            var result = await _containerService
                .AddAsync(container);

            if (result != null) 
                return CreatedAtAction("Get", new { containerId = result.Id }, result);

            return BadRequest();
        }

        [HttpGet("Delete/{id}")]
        public async Task<ActionResult> DeleteAsync(Guid id)
        {
            await _containerService
                .DeleteAsync(id);

            return Accepted();
        }

        [HttpPost("Update")]
        public async Task<ActionResult> UpdateAsync([FromBody] ContainerBO container)
        {
            await _containerService
                .UpdateAsync(container);

            return NoContent();
        }

        [HttpPost("SetDefaultAsync")]
        public async Task<ActionResult> SetDefaultAsync([FromBody] ContainerBO container)
        {
            await _containerService
                .SetDefaultAsync(container.Id);

            return NoContent();
        }

        [HttpPost("GetByName")]
        public async Task<ActionResult> GetByNameAsync([FromBody] ContainerBO container)
        {
            ContainerBO result = await _containerService.GetByNameAsync(container);
            return Ok(result);
        }

        [HttpPost("ValidateDestinationTable")]
        public async Task<ActionResult> ValidateDestinationTableAsync([FromBody] ContainerDestinationBO model)
        {
            if (model == null) return Ok(true);
            bool result = await _containerService.ValidateDestinationTableAsync(model);
            return Ok(result);
        }
    }
}
