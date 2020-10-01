// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using Microsoft.EntityFrameworkCore;
using MSDF.DataChecker.Persistence.EntityFramework;
using System;
using System.Threading.Tasks;

namespace MSDF.DataChecker.Persistence.UserParams
{
    public interface IUserParamCommands
    {
        Task<UserParam> AddAsync(UserParam model);
        Task UpdateAsync(UserParam model);
        Task DeleteAsync(Guid id);
    }
    public class UserParamCommands : IUserParamCommands
    {
        private readonly DatabaseContext _db;
        public UserParamCommands(DatabaseContext db)
        {
            _db = db;
        }

        public async Task<UserParam> AddAsync(UserParam model)
        {
            _db.UserParams.Add(model);
            await _db.SaveChangesAsync();
            return model;
        }

        public async Task UpdateAsync(UserParam model)
        {
            var entity = await _db.UserParams
                .FirstOrDefaultAsync(rec => rec.Id == model.Id);

            if (entity != null)
            {
                entity.Name = model.Name;
                entity.Value = model.Value;

                _db.UserParams.Update(entity);
                await _db.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _db.UserParams
                .FirstOrDefaultAsync(rec => rec.Id == id);

            if (entity != null)
            {
                _db.UserParams.Remove(entity);
                await _db.SaveChangesAsync();
            }
        }        
    }
}
