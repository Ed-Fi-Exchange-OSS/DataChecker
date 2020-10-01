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

namespace MSDF.DataChecker.Persistence.Tags
{
    public interface ITagQueries
    {
        Task<List<Tag>> GetAsync();
        Task<List<Tag>> GetByContainerIdAsync(Guid containerId);
        Task<List<Tag>> GetByRuleIdAsync(Guid ruleId);
        Task<Tag> GetAsync(int id);
        Task<Tag> GetByNameAsync(string name);
        Task<List<TagEntity>> GetByTagsAsync(List<int> tagIds);
    }
    public class TagQueries : ITagQueries
    {
        private readonly DatabaseContext _db;
        public TagQueries(DatabaseContext db)
        {
            _db = db;
        }

        public async Task<List<Tag>> GetAsync()
        {
            return await _db.Tags.ToListAsync();
        }

        public async Task<Tag> GetAsync(int id)
        {
            return await _db.Tags.FirstOrDefaultAsync(rec => rec.Id == id);
        }

        public async Task<Tag> GetByNameAsync(string name)
        {
            return await _db.Tags.FirstOrDefaultAsync(rec => rec.Name.ToLower() == name.ToLower());
        }

        public async Task<List<Tag>> GetByContainerIdAsync(Guid containerId)
        {
            return await _db.TagEntities
                .Where(rec => rec.ContainerId == containerId)
                .Select(rec => rec.Tag).ToListAsync();
        }

        public async Task<List<Tag>> GetByRuleIdAsync(Guid ruleId)
        {
            return await _db.TagEntities
                .Where(rec => rec.RuleId == ruleId)
                .Select(rec => rec.Tag).ToListAsync();
        }

        public async Task<List<TagEntity>> GetByTagsAsync(List<int> tagIds)
        {
            return await _db.TagEntities
                .Where(rec => tagIds.Contains(rec.TagId))
                .ToListAsync();
        }
    }
}
