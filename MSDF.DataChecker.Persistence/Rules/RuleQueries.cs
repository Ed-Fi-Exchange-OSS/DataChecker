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

namespace MSDF.DataChecker.Persistence.Rules
{
    public interface IRuleQueries
    {
        Task<List<Rule>> GetAsync();
        Task<List<Rule>> GetByContainerIdAsync(Guid containerId);
        Task<Rule> GetAsync(Guid ruleId);
        Task<List<Rule>> GetByListAsync(List<Guid> rulesList);
        Task<List<Rule>> GetByContainerListAsync(List<Guid> containersList);
        Task<List<Rule>> GetByCollectionOrContainerOrTagAsync(List<Guid> conllections, List<Guid> containers, List<int> tags);
    }
    public class RuleQueries : IRuleQueries
    {
        private readonly DatabaseContext _db;
        public RuleQueries(DatabaseContext db)
        {
            _db = db;
        }

        public async Task<List<Rule>> GetAsync()
        {
            var rules = await _db.Rules.ToListAsync();
            return rules;
        }

        public async Task<Rule> GetAsync(Guid ruleId)
        {
            var rule = await _db.Rules
                .Where(rec => rec.Id == ruleId)
                .FirstOrDefaultAsync();

            return rule;
        }

        public async Task<List<Rule>> GetByCollectionOrContainerOrTagAsync(List<Guid> collections, List<Guid> containers, List<int> tags)
        {
            List<Rule> result = new List<Rule>();

            var rules = await (from r in _db.Rules
                        join c in _db.Containers on r.ContainerId equals c.Id
                        join p in _db.Containers on c.ParentContainerId equals p.Id
                        where ((collections.Count == 0 || collections.Contains(p.Id)) && (containers.Count == 0 || containers.Contains(c.Id)))
                        select new
                        {
                            Rule = r,
                            RuleParent = c,
                            ContainerParent = p
                        }).ToListAsync();

            if (tags.Any())
            {
                List<Guid> rulesByTag = _db.TagEntities
                    .Where(rec => rec.RuleId != null && tags.Contains(rec.TagId))
                    .Select(rec=>rec.RuleId.Value)
                    .ToList();

                List<Guid> containersByTag = _db.TagEntities
                    .Where(rec => rec.ContainerId != null && tags.Contains(rec.TagId))
                    .Select(rec => rec.ContainerId.Value)
                    .ToList();

                rules = rules
                        .Where(rec => containersByTag.Contains(rec.ContainerParent.Id) || 
                        containersByTag.Contains(rec.RuleParent.Id) || rulesByTag.Contains(rec.Rule.Id))
                        .ToList();
            }

            if (rules.Any())
                result = rules.Select(rec => rec.Rule).ToList();

            return result;
        }

        public async Task<List<Rule>> GetByContainerIdAsync(Guid containerId)
        {
            var rules = await _db.Rules
                .Where(rec=>rec.ContainerId == containerId)
                .ToListAsync();

            return rules;
        }

        public async Task<List<Rule>> GetByContainerListAsync(List<Guid> containersList)
        {
            var rules = await _db.Rules
                .Where(rec => containersList.Contains(rec.ContainerId))
                .ToListAsync();

            return rules;
        }

        public async Task<List<Rule>> GetByListAsync(List<Guid> rulesList)
        {
            var rules = await _db.Rules
                .Where(rec => rulesList.Contains(rec.Id))
                .ToListAsync();

            return rules;
        }
    }
}
