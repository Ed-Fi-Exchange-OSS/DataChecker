// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using Microsoft.EntityFrameworkCore;
using MSDF.DataChecker.Persistence.EntityFramework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MSDF.DataChecker.Persistence.Tags
{
    public interface ITagCommands
    {
        Task<Tag> AddAsync(Tag model);
        Task UpdateAsync(Tag model);
        Task DeleteAsync(int id);
        Task<TagEntity> AddTagToEntityAsync(TagEntity model);
        Task DeleteTagFromEntityAsync(int id, Guid idEntity);
        Task DeleteTagFromEntityIdAsync(Guid idEntity);
    }

    public class TagCommands : ITagCommands
    {
        private readonly DatabaseContext _db;
        public TagCommands(DatabaseContext db)
        {
            _db = db;
        }

        public async Task<Tag> AddAsync(Tag model)
        {
            model.Created = DateTime.UtcNow;
            model.Updated = DateTime.UtcNow;
            var result = await _db.Tags.AddAsync(model);
            await _db.SaveChangesAsync();

            return result.Entity;
        }

        public async Task DeleteAsync(int id)
        {
            var entitiesRelated = await _db.TagEntities
                .Where(rec => rec.TagId == id).ToListAsync();

            if (entitiesRelated != null && entitiesRelated.Any())
            {
                _db.TagEntities.RemoveRange(entitiesRelated);
                await _db.SaveChangesAsync();
            }

            var entityToDelete = await _db.Tags
                .FirstOrDefaultAsync(rec => rec.Id == id);

            if (entityToDelete != null)
            {
                _db.Tags.Remove(entityToDelete);
                await _db.SaveChangesAsync();
            }
        }

        public async Task UpdateAsync(Tag model)
        {
            var entityToUpdate = await _db.Tags
                .FirstOrDefaultAsync(rec => rec.Id == model.Id);

            if (entityToUpdate != null)
            {
                entityToUpdate.Name = model.Name;
                entityToUpdate.Description = model.Description;
                entityToUpdate.IsPublic = model.IsPublic;
                entityToUpdate.Updated = DateTime.UtcNow;

                _db.Tags.Update(entityToUpdate);
                await _db.SaveChangesAsync();
            }
        }

        public async Task<TagEntity> AddTagToEntityAsync(TagEntity model)
        {
            var result = await _db.TagEntities.AddAsync(model);
            await _db.SaveChangesAsync();
            return result.Entity;
        }

        public async Task DeleteTagFromEntityAsync(int id, Guid idEntity)
        {
            var entityToDelete = await _db.TagEntities
                .FirstOrDefaultAsync(rec => rec.TagId == id && (rec.ContainerId == idEntity || rec.RuleId == idEntity));

            if (entityToDelete != null)
            {
                _db.TagEntities.Remove(entityToDelete);
                await _db.SaveChangesAsync();
            }
        }

        public async Task DeleteTagFromEntityIdAsync(Guid idEntity)
        {
            var tagsToDelete = await _db.TagEntities
                .Where(rec => (rec.ContainerId == idEntity || rec.RuleId == idEntity))
                .ToListAsync();

            if (tagsToDelete != null)
            {
                tagsToDelete.ForEach(rec => _db.TagEntities.Remove(rec));
                await _db.SaveChangesAsync();
            }
        }
    }
}
