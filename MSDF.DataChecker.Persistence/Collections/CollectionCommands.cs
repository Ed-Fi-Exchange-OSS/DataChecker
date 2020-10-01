// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using MSDF.DataChecker.Persistence.EntityFramework;
using MSDF.DataChecker.Persistence.RuleExecutionLogs;
using MSDF.DataChecker.Persistence.Rules;
using MSDF.DataChecker.Persistence.Tags;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MSDF.DataChecker.Persistence.Collections
{
    public interface ICollectionCommands
    {
        Task<Container> AddAsync(Container Collection);
        Task<Container> UpdateAsync(Container container);
        Task DeleteAsync(Guid containerId);        
    }

    public class CollectionCommands : ICollectionCommands
    {
        private readonly DatabaseContext _db;
        public CollectionCommands(DatabaseContext db)
        {
            _db = db;
        }

        public async Task<Container> AddAsync(Container container)
        {
            var newEntity = this._db.Add(container);
            await this._db.SaveChangesAsync();
            return newEntity.Entity;            
        }

        public async Task<Container> UpdateAsync(Container container)
        {
            var containerInfo = this._db.Containers
                .FirstOrDefault(m => m.Id == container.Id);

            containerInfo.Name = container.Name;
            containerInfo.Description = container.Description;
            containerInfo.IsDefault = container.IsDefault;
            containerInfo.EnvironmentType = container.EnvironmentType;
            containerInfo.RuleDetailsDestinationId = container.RuleDetailsDestinationId;

            var containerUpdated = this._db.Containers.Update(containerInfo);
            await this._db.SaveChangesAsync();
            return containerUpdated.Entity;
        }

        public async Task DeleteAsync(Guid containerId)
        {
            List<Container> childContainers = _db.Containers.Where(rec => rec.ParentContainerId != null && rec.ParentContainerId == containerId).ToList();
            List<Guid> childContainersId = new List<Guid>();
            List<Rule> allRules = new List<Rule>();
            List<RuleExecutionLog> ruleLogs = new List<RuleExecutionLog>();
            List<TagEntity> allTags = new List<TagEntity>();

            if (childContainers.Any())
            {
                childContainersId = childContainers.Select(rec => rec.Id).ToList();
                allRules = _db.Rules.Where(rec => childContainersId.Contains(rec.ContainerId)).ToList();
                List<Guid> allRulesId = allRules.Select(rec => rec.Id).ToList();
                ruleLogs = _db.RuleExecutionLogs.Where(rec => allRulesId.Contains(rec.RuleId)).ToList();
                allTags = _db.TagEntities.Where(rec => rec.ContainerId != null && childContainersId.Contains(rec.ContainerId.Value)).ToList();
                allTags.AddRange(_db.TagEntities.Where(rec => rec.RuleId != null && allRulesId.Contains(rec.RuleId.Value)).ToList());
            }
            else
            {
                childContainersId.Add(containerId);
                allRules = _db.Rules.Where(rec => childContainersId.Contains(rec.ContainerId)).ToList();
                List<Guid> allRulesId = allRules.Select(rec => rec.Id).ToList();
                ruleLogs = _db.RuleExecutionLogs.Where(rec => allRulesId.Contains(rec.RuleId)).ToList();
                allTags = _db.TagEntities.Where(rec => rec.ContainerId != null && childContainersId.Contains(rec.ContainerId.Value)).ToList();
                allTags.AddRange(_db.TagEntities.Where(rec => rec.RuleId != null && allRulesId.Contains(rec.RuleId.Value)).ToList());
            }

            allTags.AddRange(_db.TagEntities.Where(rec => rec.ContainerId != null && rec.ContainerId == containerId).ToList());

            _db.RuleExecutionLogs.RemoveRange(ruleLogs);
            _db.TagEntities.RemoveRange(allTags);
            _db.Rules.RemoveRange(allRules);
            _db.Containers.RemoveRange(childContainers);
            await this._db.SaveChangesAsync();

            var collection = this._db.Containers
                .FirstOrDefault(m => m.Id == containerId);

            this._db.Containers.Remove(collection);
            await this._db.SaveChangesAsync();
        }
    }
}
