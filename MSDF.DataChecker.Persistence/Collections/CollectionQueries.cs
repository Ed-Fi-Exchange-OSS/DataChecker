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

namespace MSDF.DataChecker.Persistence.Collections
{
    public interface ICollectionQueries
    {
        Task<List<Container>> GetAsync();
        Task<Container> GetAsync(Guid id);
        Task<Container> GetByNameAsync(string name);
        Task<Container> GetDefaultAsync();
        Task<List<Container>> GetByListAsync(List<Guid> containersList);
        Task<List<Container>> GetByRulesDestinationIdAsync(int ruleDestinationId);
    }
    public class CollectionQueries : ICollectionQueries
    {
        private readonly DatabaseContext _db;
        public CollectionQueries(DatabaseContext db)
        {
            _db = db;
        }

        public async Task<List<Container>> GetAsync()
        {
            var containers = await _db.Containers
                    .Where(m => m.ParentContainerId == null)
                    .Include(m => m.ChildContainers)
                    .ToListAsync();

            return containers;
        }

        public async Task<Container> GetAsync(Guid id)
        {
            var container = await _db.Containers
                    .Where(m => m.Id == id)
                    .Include(m => m.ChildContainers)
                    .FirstOrDefaultAsync();

            return container;
        }

        public async Task<List<Container>> GetByListAsync(List<Guid> containersList)
        {
            var containers = await _db.Containers
                    .Where(m => containersList.Contains(m.Id) || (m.ParentContainerId != null && containersList.Contains(m.ParentContainerId.Value)))
                    .Include(m => m.ChildContainers)
                    .ToListAsync();

            return containers;
        }

        public async Task<Container> GetByNameAsync(string name)
        {
            return await _db.Containers.FirstOrDefaultAsync(rec => rec.Name.ToLower() == name.ToLower());
        }

        public async Task<List<Container>> GetByRulesDestinationIdAsync(int ruleDestinationId)
        {
            var containers = await _db.Containers
                    .Where(m => m.RuleDetailsDestinationId != null && m.RuleDetailsDestinationId.Value == ruleDestinationId)
                    .Include(m => m.ChildContainers)
                    .ToListAsync();

            return containers;
        }

        public async Task<Container> GetDefaultAsync()
        {
            var container = await _db.Containers
                    .Where(m => m.IsDefault)
                    .Include(m => m.ChildContainers)
                    .FirstOrDefaultAsync();

            return container;
        }
    }
}
