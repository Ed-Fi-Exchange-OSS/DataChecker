// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using Microsoft.EntityFrameworkCore;
using MSDF.DataChecker.Persistence.EntityFramework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MSDF.DataChecker.Persistence.Logs
{
    public interface ILogQueries
    {
        Task<List<Log>> GetAsync();
        Task<Log> GetAsync(int id);
    }

    public class LogQueries : ILogQueries
    {
        private readonly DatabaseContext _db;
        public LogQueries(DatabaseContext db)
        {
            _db = db;
        }

        public async Task<List<Log>> GetAsync()
        {
            var models = await _db.Logs
                .ToListAsync();

            return models;
        }

        public async Task<Log> GetAsync(int id)
        {
            var model = await _db.Logs
                .Where(rec => rec.Id == id)
                .FirstOrDefaultAsync();

            return model;
        }
    }
}
