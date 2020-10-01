// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using MSDF.DataChecker.Persistence.EntityFramework;
using System;
using System.Threading.Tasks;

namespace MSDF.DataChecker.Persistence.RuleExecutionLogs
{
    public interface IRuleExecutionLogCommands
    {
        Task<RuleExecutionLog> AddAsync(RuleExecutionLog ruleExecutionLog);
        Task<RuleExecutionLog> UpdateAsync(RuleExecutionLog ruleExecutionLog);
    }

    public class RuleExecutionLogCommands : IRuleExecutionLogCommands
    {
        private readonly DatabaseContext _db;
        public RuleExecutionLogCommands(DatabaseContext db)
        {
            _db = db;
        }

        public async Task<RuleExecutionLog> AddAsync(RuleExecutionLog ruleExecutionLog)
        {
            ruleExecutionLog.ExecutionDate = DateTime.UtcNow;
            var result = this._db.Add(ruleExecutionLog);
            await this._db.SaveChangesAsync();
            return result.Entity;
        }

        public async Task<RuleExecutionLog> UpdateAsync(RuleExecutionLog ruleExecutionLog)
        {
            ruleExecutionLog.ExecutionDate = DateTime.UtcNow;
            this._db.RuleExecutionLogs.Update(ruleExecutionLog);
            await this._db.SaveChangesAsync();
            return ruleExecutionLog;
        }
    }
}
