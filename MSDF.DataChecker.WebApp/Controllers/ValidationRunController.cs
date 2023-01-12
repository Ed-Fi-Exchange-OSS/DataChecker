using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MSDF.DataChecker.Persistence.Settings;
using MSDF.DataChecker.Services;
using MSDF.DataChecker.Services.Models;
using MSDF.DataChecker.Services.RuleExecution;

namespace MSDF.DataChecker.WebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValidationRunController : ControllerBase
    {

        private IValidationRunService _validationRunService;
        private IConfiguration _configuration;
        private readonly DataBaseSettings _appSettings;
        public ValidationRunController(
            IValidationRunService validationRunService,
            IConfiguration configuration,
            IOptionsSnapshot<DataBaseSettings> appSettings)
        {
            _validationRunService = validationRunService;
            _configuration = configuration;
            _appSettings = appSettings.Value;
        }

        [HttpGet]
        public async Task<ActionResult> Get()
        {
            var list = await _validationRunService
                .GetAsync();

            if (list != null)
            {
                return Ok(list);
            }

            return NotFound();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> Get(int id)
        {
            var result = await _validationRunService
                .GetAsync(id);

            if (result != null)
            {
                return Ok(result);
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<ActionResult> Add([FromBody] ValidationRunBO model)
        {
               var validationRun = await _validationRunService
                .AddAsync(model);

            if (validationRun != null)
                return Ok(validationRun.Id);
            return BadRequest();
        }


        [HttpPost("Finish")]
        public async Task<ActionResult> Finish([FromBody] ValidationRunBO model)
        {

            model.RunStatus = "Finished";
            var validationRun = await _validationRunService
             .UpdateAsync(model);

            if (validationRun != null)
                return Ok(validationRun.Id);
            return BadRequest();
        }

        [HttpPost("ValidationRunError")]
        public async Task<ActionResult> ValidationRunError([FromBody] ValidationRunBO model)
        {

            model.RunStatus = "Stopped-Error";
            var validationRun = await _validationRunService
             .UpdateAsync(model);

            if (validationRun != null)
                return Ok(validationRun.Id);
            return BadRequest();
        }

        
    }
}
