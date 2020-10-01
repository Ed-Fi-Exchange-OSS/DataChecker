// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using Microsoft.Extensions.Configuration;
using MSDF.DataChecker.Persistence.UserParams;
using MSDF.DataChecker.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MSDF.DataChecker.Services
{
    public interface IUserParamService
    {
        Task<UserParamBO> AddAsync(UserParamBO model);
        Task UpdateAsync(UserParamBO model);
        Task DeleteAsync(UserParamBO model);
        Task<UserParamBO> GetAsync(Guid id);
        Task<List<UserParamBO>> GetByEnvironmentIdAsync(Guid databaseEnvironmentId);
        Task<List<UserParamBO>> UpdateListAsync(List<UserParamBO> models, Guid databaseEnvironmentId);
    }

    public class UserParamService : IUserParamService
    {
        private IUserParamQueries _userParamQueries;
        private IUserParamCommands _userParamCommands;
        private readonly IConfiguration _config;

        public UserParamService(
            IConfiguration config,
            IUserParamQueries userParamQueries,
            IUserParamCommands userParamCommands)

        {
            _userParamQueries = userParamQueries;            
            _userParamCommands = userParamCommands;
            _config = config;            
        }

        public async Task<UserParamBO> AddAsync(UserParamBO model)
        {               
            var result  = await this._userParamCommands
                .AddAsync(MapModelToEntity(model));

            return result != null ? MapEntityToModel(result) : null;
        }

        public async Task DeleteAsync(UserParamBO model)
        {
            await this._userParamCommands.DeleteAsync(model.Id);
        }        
        
        public async Task UpdateAsync(UserParamBO model)
        {
            await this._userParamCommands.UpdateAsync(MapModelToEntity(model));
        }

        public async Task<List<UserParamBO>> UpdateListAsync(List<UserParamBO> models, Guid databaseEnvironmentId)
        {
            var userParams = await this._userParamQueries
                .GetByDatabaseEnvironmentIdAsync(databaseEnvironmentId);

            foreach (var item in userParams)
            {
                if (models.Any(m => m.Id == item.Id))
                {
                    var userParam = models.First(rec => rec.Id == item.Id);
                    item.Name = userParam.Name;
                    item.Value = userParam.Value;
                    models.Remove(userParam);
                    await UpdateAsync(MapEntityToModel(item));
                }
                else
                {
                    await _userParamCommands.DeleteAsync(item.Id);
                }
            }

            foreach (var item in models)
            {
                item.DatabaseEnvironmentId = databaseEnvironmentId;
                await AddAsync(item);
            }

            var listParams = await _userParamQueries.GetByDatabaseEnvironmentIdAsync(databaseEnvironmentId);
            if (listParams != null)
                return listParams
                    .Select(rec => MapEntityToModel(rec))
                    .ToList();

            return null;
        }

        public async Task<List<UserParamBO>> GetByEnvironmentIdAsync(Guid databaseEnvironmentId)
        {
            var items = await this._userParamQueries
                .GetByDatabaseEnvironmentIdAsync(databaseEnvironmentId);

            if (items != null)
                return items
                    .Select(rec => MapEntityToModel(rec))
                    .ToList();

            return null;
        }

        public async Task<UserParamBO> GetAsync(Guid id)
        {
            var userParam = await this._userParamQueries
                .GetAsync(id);

            return userParam != null ? MapEntityToModel(userParam) : null;
        }

        private UserParamBO MapEntityToModel(UserParam entity)
        {
            UserParamBO model = new UserParamBO
            {
                DatabaseEnvironmentId = entity.DatabaseEnvironmentId,
                Id = entity.Id,
                Name = entity.Name,
                Value = entity.Value
            };

            return model;
        }

        private UserParam MapModelToEntity(UserParamBO model)
        {
            UserParam entity = new UserParam
            {
                DatabaseEnvironmentId = model.DatabaseEnvironmentId,
                Id = model.Id,
                Name = model.Name,
                Value = model.Value
            };

            return entity;
        }
    }
}
