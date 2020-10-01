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
    public class TagsController : ControllerBase
    {
        private readonly ITagService _tagService;

        public TagsController(ITagService tagService)
        {
            _tagService = tagService;
        }

        [HttpPost]
        public async Task<ActionResult> AddAsync([FromBody] TagBO model)
        {
            var tag = await _tagService
                .AddAsync(model);

            if (tag != null)
                return CreatedAtAction("Get", new { id = tag.Id }, tag);

            return BadRequest();
        }

        [HttpPost("Update")]
        public async Task<ActionResult> UpdateAsync([FromBody] TagBO model)
        {
            await _tagService
               .UpdateAsync(model);

            return NoContent();
        }

        [HttpPost("SearchByTags")]
        public async Task<ActionResult> SearchByTagsAsync([FromBody] List<TagBO> model)
        {
            var result = await _tagService
                .SearchByTags(model);

            if (result != null)
                return Ok(result);

            return NotFound();
        }

        [HttpGet("Delete/{id}")]
        public async Task<ActionResult> DeleteAsync(int id)
        {
            await _tagService
                .DeleteAsync(id);

            return Accepted();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetAsync(int id)
        {
            var model = await _tagService
                .GetAsync(id);

            if (model != null)
                return Ok(model);

            return NotFound();
        }

        [HttpGet()]
        public async Task<ActionResult> GetAsync()
        {
            var model = await _tagService
                .GetAsync();

            if (model != null)
                return Ok(model);

            return NotFound();
        }

        [HttpGet("GetByContainer/{id}")]
        public async Task<ActionResult> GetByContainerAsync(Guid id)
        {
            var model = await _tagService
                .GetByContainerIdAsync(id);

            if (model != null)
                return Ok(model);

            return NotFound();
        }

        [HttpGet("GetByRule/{id}")]
        public async Task<ActionResult> GetByRuleAsync(Guid id)
        {
            var model = await _tagService
                .GetByRuleIdAsync(id);

            if (model != null)
                return Ok(model);

            return NotFound();
        }
    }
}
