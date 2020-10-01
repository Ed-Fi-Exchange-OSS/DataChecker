// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using Microsoft.EntityFrameworkCore;
using MSDF.DataChecker.Persistence.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MSDF.DataChecker.Persistence.DatabaseEnvironments
{
    public interface IDatabaseEnvironmentQueries
    {
        Task<List<DatabaseEnvironment>> GetAsync();
        Task<DatabaseEnvironment> GetAsync(Guid id);
    }
    public class DatabaseEnvironmentQueries : IDatabaseEnvironmentQueries
    {
        private readonly DatabaseContext _db;
        public DatabaseEnvironmentQueries(DatabaseContext db)
        {
            _db = db;
        }

        public async Task<List<DatabaseEnvironment>> GetAsync()
        {
            var models = await _db.DatabaseEnvironments
                .Include(rec=>rec.UserParams)
                .OrderByDescending(m => m.CreatedDate)
                .ToListAsync();

            if(models != null) models.ForEach(p => { 
                p.Password = p.DecryptPassword();
                if (p.SecurityIntegrated == null) p.SecurityIntegrated = false;
            });

            return models;
        }

        public async Task<DatabaseEnvironment> GetAsync(Guid id)
        {
            var model = await _db.DatabaseEnvironments
                .Where(rec => rec.Id == id)
                .Include(rec => rec.UserParams)
                .FirstOrDefaultAsync();

            if (model != null)
            {
                model.Password = model.DecryptPassword();
                if (model.SecurityIntegrated == null) model.SecurityIntegrated = false;
            }
            return model;
        }
    }
}
