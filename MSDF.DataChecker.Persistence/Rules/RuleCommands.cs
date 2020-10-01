// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using Microsoft.EntityFrameworkCore;
using MSDF.DataChecker.Persistence.EntityFramework;
using System;
using System.Threading.Tasks;

namespace MSDF.DataChecker.Persistence.Rules
{
    public interface IRuleCommands
    {
        Task<Rule> AddAsync(Rule model);
        Task<Rule> UpdateAsync(Rule model);
        Task<Rule> UpdateContainerIdAsync(Rule model);
        Task DeleteRule(Guid ruleId);
    }

    public class RuleCommands : IRuleCommands
    {
        private readonly DatabaseContext _db;
        public RuleCommands(DatabaseContext db)
        {
            _db = db;
        }
        
        public async Task<Rule> AddAsync(Rule model)
        {
            _db.Rules.Add(model);
            await _db.SaveChangesAsync();
            return model;
        }
        
        public async Task<Rule> UpdateAsync(Rule model)
        {
            var entity = await _db.Rules
                .FirstOrDefaultAsync(rec => rec.Id == model.Id);

            if (entity != null)
            {
                entity.Description = model.Description;
                entity.DiagnosticSql = model.DiagnosticSql;
                entity.ErrorMessage = model.ErrorMessage;
                entity.ErrorSeverityLevel = model.ErrorSeverityLevel;
                entity.Name = model.Name;
                entity.Resolution = model.Resolution;
                entity.RuleIdentification = model.RuleIdentification;
                entity.Version = model.Version;
                entity.MaxNumberResults = model.MaxNumberResults;

                _db.Rules.Update(entity);
                await _db.SaveChangesAsync();
                return entity;
            }
            return null;
        }

        public async Task<Rule> UpdateContainerIdAsync(Rule model)
        {
            var entity = await _db.Rules
                .FirstOrDefaultAsync(rec => rec.Id == model.Id);

            if (entity != null)
            {
                entity.ContainerId = model.ContainerId;
                _db.Rules.Update(entity);
                await _db.SaveChangesAsync();
                return entity;
            }
            return null;
        }

        public async Task DeleteRule(Guid ruleId)
        {
            var entityToDelete = await _db.Rules
                .Include(rec=>rec.RuleExecutionLogs)
                .FirstOrDefaultAsync(rec => rec.Id == ruleId);

            if (entityToDelete != null)
            {
                if (entityToDelete.RuleExecutionLogs != null)
                    _db.RuleExecutionLogs.RemoveRange(entityToDelete.RuleExecutionLogs);

                _db.Rules.Remove(entityToDelete);
                await _db.SaveChangesAsync();
            }
        }
    }
}
