// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using Microsoft.AspNetCore.Mvc;
using MSDF.DataChecker.Services;
using System.Threading.Tasks;

namespace MSDF.DataChecker.WebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CatalogsController : ControllerBase
    {
        private ICatalogService _catalogService;

        public CatalogsController(ICatalogService catalogService)
        {
            _catalogService = catalogService;
        }

        [HttpGet]
        public async Task<ActionResult> Get()
        {
            var list = await _catalogService
                .GetAsync();

            if (list != null)
                return Ok(list);

            return NotFound();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> Get(int id)
        {
            var result = await _catalogService
                .GetAsync(id);

            if (result != null)
                return Ok(result);

            return NotFound();
        }

        [HttpGet("GetByType")]
        public async Task<ActionResult> GetByType(string type)
        {
            var result = await _catalogService
                .GetByTypeAsync(type);

            if (result != null)
                return Ok(result);

            return NotFound();
        }
    }
}
