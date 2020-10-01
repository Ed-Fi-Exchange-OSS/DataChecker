// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using MSDF.DataChecker.Services;
using MSDF.DataChecker.Services.Models;

namespace MSDF.DataChecker.WebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobsController : ControllerBase
    {
        private readonly IJobService _jobService;
        public JobsController(IJobService jobService) {
          _jobService = jobService;
        }

        [HttpGet]
        public ActionResult Get()
        {
          var model = _jobService.Get();

          if (model != null) return Ok(model);

          return NotFound();
        }

        [HttpGet("{id}")]
        public ActionResult Get(int id)
        {
            var model = _jobService.Get(id);

            if (model != null) return Ok(model);

            return NotFound();
        }

        [HttpPost]
        public ActionResult Add([FromBody] JobBO model)
        {
            var job = _jobService.Add(model);

            if (job != null) return CreatedAtAction("Get", new { id = job.Id }, job);

            return BadRequest();
        }

        [HttpPost("Update")]
        public ActionResult Update([FromBody] JobBO model)
        {
            _jobService.Update(model);

            return NoContent();
        }

        [HttpGet("Delete/{id}")]
        public ActionResult Delete(int id)
        {
            _jobService.Delete(id);
            return Accepted();
        }

        [HttpGet("Enqueue/{id}")]
        public ActionResult Enqueue(int id)
        {
            _jobService.Enqueue(id);
            return Accepted();
        }

        [HttpPost("RunAndForget")]
        public ActionResult RunAndForget([FromBody] JobBO model)
        {
          _jobService.RunAndForget(model);
          return Accepted();
        }
    }
}
