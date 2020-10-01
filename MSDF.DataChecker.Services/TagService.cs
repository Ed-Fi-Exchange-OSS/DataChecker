// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using MSDF.DataChecker.Persistence.Collections;
using MSDF.DataChecker.Persistence.Rules;
using MSDF.DataChecker.Persistence.Tags;
using MSDF.DataChecker.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MSDF.DataChecker.Services
{
    public interface ITagService
    {
        Task<TagBO> AddAsync(TagBO model);
        Task UpdateAsync(TagBO model);
        Task DeleteAsync(int id);
        Task<List<TagBO>> GetAsync();
        Task<List<TagBO>> GetByContainerIdAsync(Guid containerId);
        Task<List<TagBO>> GetByRuleIdAsync(Guid ruleId);
        Task<TagBO> GetAsync(int id);
        Task<SearchTagBO> SearchByTags(List<TagBO> tags);
    }

    public class TagService : ITagService
    {
        private ITagQueries _TagQueries;
        private ITagCommands _TagCommands;
        private ICollectionQueries _collectionQueries;
        private IRuleQueries _ruleQueries;

        public TagService(
            ITagQueries TagQueries,
            ITagCommands TagCommands,
            ICollectionQueries collectionQueries,
            IRuleQueries ruleQueries)
        {
            _TagQueries = TagQueries;
            _TagCommands = TagCommands;
            _collectionQueries = collectionQueries;
            _ruleQueries = ruleQueries;
        }

        public async Task<TagBO> AddAsync(TagBO model)
        {
            var result = await _TagCommands.AddAsync(MapModelToEntity(model));
            if (result != null)
                return MapEntityToModel(result);

            return null;
        }

        public async Task UpdateAsync(TagBO model)
        {
            await _TagCommands.UpdateAsync(MapModelToEntity(model));
        }

        public async Task DeleteAsync(int id)
        {
            await _TagCommands.DeleteAsync(id);
        }

        public async Task<List<TagBO>> GetAsync()
        {
            var result = await _TagQueries.GetAsync();
            if (result != null)
            {
                var listTags = result
                    .Select(rec => MapEntityToModel(rec))
                    .ToList();

                await DetermineIfTagIsUsed(listTags);
                listTags = listTags.OrderBy(rec => rec.Name).ToList();
                return listTags;
            }
            return null;
        }

        public async Task<List<TagBO>> GetByContainerIdAsync(Guid containerId)
        {
            var result = await _TagQueries.GetByContainerIdAsync(containerId);
            if (result != null)
            {
                var listTags = result
                    .Select(rec => MapEntityToModel(rec))
                    .ToList();

                await DetermineIfTagIsUsed(listTags);
                listTags = listTags.OrderBy(rec => rec.Name).ToList();
                return listTags;
            }
            return null;
        }

        public async Task<List<TagBO>> GetByRuleIdAsync(Guid ruleId)
        {
            var result = await _TagQueries.GetByRuleIdAsync(ruleId);
            if (result != null)
            {
                var listTags = result
                    .Select(rec => MapEntityToModel(rec))
                    .ToList();

                await DetermineIfTagIsUsed(listTags);
                listTags = listTags.OrderBy(rec => rec.Name).ToList();
                return listTags;
            }
            return null;
        }

        public async Task<TagBO> GetAsync(int id)
        {
            var result = await _TagQueries.GetAsync(id);
            if (result != null)
                return MapEntityToModel(result);

            return null;
        }

        private Tag MapModelToEntity(TagBO model)
        {
            return new Tag
            {
                Created = model.Created,
                Description = model.Description,
                Id = model.Id,
                Name = model.Name.ToUpper(),
                IsPublic = model.IsPublic,
                Updated = model.Updated
            };
        }

        private TagBO MapEntityToModel(Tag entity)
        {
            return new TagBO
            {
                Created = entity.Created,
                Description = entity.Description,
                Id = entity.Id,
                Name = entity.Name.ToUpper(),
                IsPublic = entity.IsPublic,
                Updated = entity.Updated
            };
        }

        public async Task<SearchTagBO> SearchByTags(List<TagBO> tags)
        {
            SearchTagBO result = new SearchTagBO();

            var entitiesFound = await _TagQueries
                .GetByTagsAsync(tags.Select(rec => rec.Id)
                .ToList());

            if (entitiesFound != null && entitiesFound.Any())
            {
                List<Guid> containersList = entitiesFound
                    .Where(rec => rec.ContainerId != null)
                    .Select(rec => rec.ContainerId.Value)
                    .ToList();

                List<Guid> rulesList = entitiesFound
                    .Where(rec => rec.RuleId != null)
                    .Select(rec => rec.RuleId.Value)
                    .ToList();

                List<Container> containers = await _collectionQueries
                    .GetByListAsync(containersList);

                List<Rule> rules = await _ruleQueries
                    .GetByListAsync(rulesList);

                rules.AddRange((await _ruleQueries
                    .GetByContainerListAsync(containers
                    .Select(rec => rec.Id)
                    .ToList())));

                rules = rules
                    .Distinct()
                    .ToList();

                result.Collections = containers
                    .Where(rec => rec.ParentContainerId == null)
                    .Select(rec => new ContainerBO
                    {
                        Name = rec.Name,
                        Description = rec.Name,
                        IsDefault = rec.IsDefault,
                        TagIsInherited = false
                    })
                    .ToList();

                result.Containers = containers
                    .Where(rec => rec.ParentContainerId != null)
                    .Select(rec => new ContainerBO
                    {
                        Name = rec.Name,
                        Description = rec.Name,
                        IsDefault = rec.IsDefault,
                        ParentContainerId = rec.ParentContainerId,
                        TagIsInherited = entitiesFound.FirstOrDefault(aux=>aux.ContainerId == rec.Id) == null
                    })
                    .ToList();

                result.Rules = rules
                    .Select(rec => new RuleBO { 
                        RuleIdentification = rec.RuleIdentification,
                        Name=rec.Name,
                        Description=rec.Description,
                        ContainerId=rec.ContainerId,
                        TagIsInherited = entitiesFound.FirstOrDefault(aux => aux.RuleId == rec.Id) == null
                    })
                    .ToList();

                result.TagsSelected = tags;
            }

            return result;
        }

        private async Task DetermineIfTagIsUsed(List<TagBO> tags)
        {
            if (tags != null && tags.Any())
            {
                var listTagEntities = await _TagQueries.GetByTagsAsync(tags.Select(rec => rec.Id).ToList());
                foreach (var tag in tags)
                {
                    tag.IsUsed = listTagEntities.FirstOrDefault(rec => rec.TagId == tag.Id) != null;
                }
            }
        }
    }
}
