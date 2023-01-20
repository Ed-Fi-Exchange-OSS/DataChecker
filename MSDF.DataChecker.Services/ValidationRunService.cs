using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MSDF.DataChecker.Persistence.Providers;
using MSDF.DataChecker.Persistence.Settings;
using MSDF.DataChecker.Persistence.ValidationsRun;
using MSDF.DataChecker.Services.Models;

namespace MSDF.DataChecker.Services
{
    public interface IValidationRunService
    {
        Task<ValidationRunBO> AddAsync(ValidationRunBO model);
        Task<ValidationRunBO> UpdateAsync(ValidationRunBO validationRun);
        Task<List<ValidationRunBO>> GetAsync();
        Task<ValidationRunBO> GetAsync(int id);
    }
    public class ValidationRunService : IValidationRunService
    {
        private IValidationRunCommands _validationnRunCommands;
        private IValidationRunQueries _validationRunQueries;
        private readonly DataBaseSettings _appSettings;
        private IDbAccessProvider _dataAccessProvider;

        public ValidationRunService(
            IValidationRunCommands validationRunCommands,
            IValidationRunQueries validationRunQueries,
        IOptions<DataBaseSettings> appSettings,
            IDbAccessProvider dataAccessProvider)

        {
            _validationnRunCommands = validationRunCommands;
            _validationRunQueries = validationRunQueries;
            _appSettings = appSettings.Value;
            _dataAccessProvider = dataAccessProvider;
        }
        public async Task<ValidationRunBO> AddAsync(ValidationRunBO model)
        {
            var result = await  this._validationnRunCommands.AddAsync(MapModelToEntity(model));
            return MapEntityToModel(result);
        }

        public async Task<ValidationRunBO> UpdateAsync(ValidationRunBO model)
        {
            var result = await this._validationnRunCommands.UpdateAsync(MapModelToEntity(model));
            return MapEntityToModel(result);
        }
        public async Task<List<ValidationRunBO>> GetAsync()
        {
            var validationsRun = await this._validationRunQueries.GetAsync();
            if (validationsRun != null)
                return validationsRun
                    .Select(rec => MapEntityToModel(rec))
                    .ToList();

            return null;
        }

        public async Task<ValidationRunBO> GetAsync(int id)
        {
            var validationsRun = await this._validationRunQueries.GetAsync(id);
            if (validationsRun != null)
                return MapEntityToModel(validationsRun);

            return null;
        }
        private ValidationRun MapModelToEntity(ValidationRunBO model)
        {
            var entity = new ValidationRun
            {
                EndTime = model.EndTime,
                HostDatabase = model.HostDatabase,
                HostServer = model.HostServer,
                RunStatus = model.RunStatus,
                Id = model.Id,
                Source = model.Source,
                StartTime = model.StartTime
            };
            return entity;
        }

        private ValidationRunBO MapEntityToModel(ValidationRun entity)
        {
            var model = new ValidationRunBO
            {
                EndTime = entity.EndTime,
                HostDatabase = entity.HostDatabase,
                HostServer = entity.HostServer,
                RunStatus = entity.RunStatus,
                Id = entity.Id,
                Source = entity.Source,
                StartTime = entity.StartTime
            };
            return model;
        }

    }
}
