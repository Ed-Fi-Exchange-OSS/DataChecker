// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using MSDF.DataChecker.Persistence.Logs;
using MSDF.DataChecker.Services.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MSDF.DataChecker.Services
{
    public interface ILogService
    {
        Task<LogBO> AddAsync(LogBO model);
        Task<LogBO> UpdateAsync(LogBO model);
        Task DeleteAsync(int id);
        Task<List<LogBO>> GetAsync();
        Task<LogBO> GetAsync(int id);
    }

    public class LogService : ILogService
    {
        private ILogQueries _queries;
        private ILogCommands _commands;

        public LogService(
            ILogQueries logQueries,
            ILogCommands logCommands
            )
        {
            _queries = logQueries;
            _commands = logCommands;
        }

        public async Task<LogBO> AddAsync(LogBO model)
        {
            var result = await this._commands
                .AddAsync(MapModelToEntity(model));

            return result != null ? MapEntityToModel(result) : null;
        }

        public async Task<LogBO> UpdateAsync(LogBO model)
        {
            var result = await this._commands
                .UpdateAsync(MapModelToEntity(model));

            return result != null ? MapEntityToModel(result) : null;
        }

        public async Task DeleteAsync(int logId)
        {
            await this._commands.DeleteAsync(logId);
        }

        public async Task<List<LogBO>> GetAsync()
        {
            var logs = await this._queries.GetAsync();
            if (logs != null)
                return logs
                    .Select(rec => MapEntityToModel(rec))
                    .ToList();

            return null;
        }

        public async Task<LogBO> GetAsync(int id)
        {
            var log = await this._queries
                .GetAsync(id);

            if (log != null)
                return MapEntityToModel(log);

            return null;
        }

        private LogBO MapEntityToModel(Log entity)
        {
            LogBO model = new LogBO
            {
                Id = entity.Id,
                Information= entity.Information,
                DateCreated = entity.DateCreated,
                Source=entity.Source
            };

            return model;
        }

        private Log MapModelToEntity(LogBO model)
        {
            Log entity = new Log
            {
                Id = model.Id,
                Information=model.Information,
                Source=model.Source,
                DateCreated = model.DateCreated
            };

            return entity;
        }
    }
}
