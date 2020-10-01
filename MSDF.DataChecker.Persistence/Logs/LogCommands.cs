// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using Microsoft.EntityFrameworkCore;
using MSDF.DataChecker.Persistence.EntityFramework;
using System.Linq;
using System.Threading.Tasks;

namespace MSDF.DataChecker.Persistence.Logs
{
    public interface ILogCommands
    {
        Task<Log> AddAsync(Log model);
        Task<Log> UpdateAsync(Log model);
        Task DeleteAsync(int id);
    }

    public class LogCommands : ILogCommands
    {
        private readonly DatabaseContext _db;
        public LogCommands(DatabaseContext db)
        {
            _db = db;
        }

        public async Task<Log> AddAsync(Log model)
        {
            var result = _db.Logs.Add(model);
            await _db.SaveChangesAsync();
            return result.Entity;
        }

        public async Task<Log> UpdateAsync(Log model)
        {
            var entity = await _db
                .Logs
                .Where(rec => rec.Id == model.Id)
                .FirstOrDefaultAsync();

            entity.Information = model.Information;

            var result = _db.Logs.Update(entity);
            await _db.SaveChangesAsync();

            return result.Entity;
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _db
                .Logs
                .Where(rec => rec.Id == id)
                .FirstOrDefaultAsync();

            if (entity != null)
            {
                _db.Logs.Remove(entity);
                await _db.SaveChangesAsync();
            }
        }
    }
}
