// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using Microsoft.EntityFrameworkCore;
using MSDF.DataChecker.Persistence.EntityFramework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MSDF.DataChecker.Persistence.DatabaseEnvironments
{
    public interface IDatabaseEnvironmentCommands
    {
        Task<DatabaseEnvironment> AddAsync(DatabaseEnvironment model);
        Task<DatabaseEnvironment> UpdateAsync(DatabaseEnvironment model);
        Task DeleteAsync(Guid id);
    }
    public class DatabaseEnvironmentCommands : IDatabaseEnvironmentCommands
    {
        private readonly DatabaseContext _db;
        public DatabaseEnvironmentCommands(DatabaseContext db)
        {
            _db = db;
        }

        public async Task<DatabaseEnvironment> AddAsync(DatabaseEnvironment model)
        {
            if (model.SecurityIntegrated == null || !model.SecurityIntegrated.Value)
                model.Password = Tools.CryptoTools.EncryptString(model.Password);

            model.CreatedDate = DateTime.Now;

            var result = _db.DatabaseEnvironments.Add(model);
            await _db.SaveChangesAsync();
            return result.Entity;
        }

        public async Task<DatabaseEnvironment> UpdateAsync(DatabaseEnvironment model)
        {
            var entity = await _db
                .DatabaseEnvironments
                .Where(rec => rec.Id == model.Id)
                .FirstOrDefaultAsync();

            if (model.SecurityIntegrated == null || !model.SecurityIntegrated.Value)
                entity.Password = Tools.CryptoTools.EncryptString(model.Password);

            entity.Database = model.Database;
            entity.DataSource = model.DataSource;
            entity.ExtraData = model.ExtraData;
            entity.Name = model.Name;
            entity.SecurityIntegrated = model.SecurityIntegrated;
            entity.User = model.User;
            entity.Version = model.Version;
            entity.TimeoutInMinutes = model.TimeoutInMinutes;

            var result = _db.DatabaseEnvironments.Update(entity);
            await _db.SaveChangesAsync();

            return result.Entity;
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _db
                .DatabaseEnvironments
                .Where(rec => rec.Id == id)
                .Include(rec => rec.UserParams)
                .FirstOrDefaultAsync();

            if (entity != null)
            {
                var ruleExecutionLogs = await _db.RuleExecutionLogs
                    .Where(rec => rec.DatabaseEnvironmentId == entity.Id)
                    .ToListAsync();

                if (ruleExecutionLogs != null)
                    _db.RuleExecutionLogs.RemoveRange(ruleExecutionLogs);

                if (entity.UserParams != null)
                    _db.UserParams.RemoveRange(entity.UserParams);

                _db.DatabaseEnvironments.Remove(entity);
                await _db.SaveChangesAsync();
            }
        }
    }
}
