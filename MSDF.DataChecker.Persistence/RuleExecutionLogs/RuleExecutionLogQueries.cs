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

namespace MSDF.DataChecker.Persistence.RuleExecutionLogs
{
    public interface IRuleExecutionLogQueries
    {
        Task<List<RuleExecutionLog>> GetByRuleIdAsync(Guid ruleId);
        Task<List<RuleExecutionLog>> GetByRuleIdAndDatabaseEnvironmentIdAsync(Guid ruleId,Guid databaseEnvironmentId);
        Task<RuleExecutionLog> GetAsync(int id);
    }
    public class RuleExecutionLogQueries : IRuleExecutionLogQueries
    {
        private readonly DatabaseContext _db;
        public RuleExecutionLogQueries(DatabaseContext db)
        {
            _db = db;
        }

        public async Task<List<RuleExecutionLog>> GetByRuleIdAsync(Guid ruleId)
        {
            var elements = await _db.RuleExecutionLogs
                .Where(m => m.RuleId == ruleId)
                .OrderByDescending(m=>m.ExecutionDate)
                .Take(5)
                .ToListAsync();

            return elements;
        }

        public async Task<List<RuleExecutionLog>> GetByRuleIdAndDatabaseEnvironmentIdAsync(Guid ruleId, Guid databaseEnvironmentId)
        {
            var elements = await _db.RuleExecutionLogs
                .Where(m => m.RuleId == ruleId && m.DatabaseEnvironmentId == databaseEnvironmentId)
                .OrderByDescending(m => m.ExecutionDate)
                .Take(5)
                .ToListAsync();
            
            return elements;
        }

        public async Task<RuleExecutionLog> GetAsync(int id)
        {
            var element = await _db.RuleExecutionLogs
                .FirstOrDefaultAsync(p => p.Id == id);
            
            return element;
        }
    }
}
