// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using Microsoft.EntityFrameworkCore.Internal;
using MSDF.DataChecker.Persistence.Catalogs;
using MSDF.DataChecker.Persistence.Collections;
//using MSDF.DataChecker.Persistence.CommunityUser;
using MSDF.DataChecker.Persistence.RuleExecutionLogDetails;
using MSDF.DataChecker.Persistence.Rules;
using MSDF.DataChecker.Persistence.Tags;
using MSDF.DataChecker.Services.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MSDF.DataChecker.Services
{
    public interface IContainerService
    {
        Task<CollectionCategory> AddAsync(ContainerBO container);
        Task<CollectionCategory> UpdateAsync(ContainerBO container);
        Task DeleteAsync(Guid containerId);
        Task DuplicateAsync(ContainerBO container);
        Task<List<CollectionCategory>> GetAsync();
        Task<CollectionCategory> GetAsync(Guid? containerId = null);
        Task<CollectionCategory> GetByDatabaseEnvironmentIdAndContainerIdAsync(Guid databaseEnvironmentId, Guid? containerId = null);
        Task SetDefaultAsync(Guid containerId);
        Task<List<ContainerBO>> GetChildContainersAsync();
        Task<List<ContainerBO>> GetParentContainersAsync();
        Task<ContainerBO> GetToCommunityAsync(Guid containerId);
        Task<ContainerBO> GetByNameAsync(ContainerBO model);
        Task<string> AddFromCommunityAsync(ContainerBO model);
        Task<bool> ValidateDestinationTableAsync(ContainerDestinationBO container);
    }
    public class ContainerService : IContainerService
    {
        private ICollectionQueries _collectionQueries;
        private IRuleService _ruleService;
        private ICollectionCommands _collectionCommands;
        private ITagCommands _tagCommands;
        private ITagQueries _tagQueries;
        private ICatalogQueries _catalogQueries;
        private IRuleExecutionLogDetailQueries _edFiRuleExecutionLogDetailQueries;
        private IRuleExecutionLogDetailCommands _edFiRuleExecutionLogDetailCommands;
        private ICatalogService _catalogService;

        public ContainerService(
            ICollectionQueries collectionQueries,
            IRuleService ruleService,
            ICollectionCommands collectionCommands,
            ITagCommands tagCommands,
            ITagQueries tagQueries,
            ICatalogQueries catalogQueries,
            IRuleExecutionLogDetailQueries edFiRuleExecutionLogDetailQueries,
            ICatalogService catalogService,
            IRuleExecutionLogDetailCommands edFiRuleExecutionLogDetailCommands)
        {
            _collectionQueries = collectionQueries;
            _ruleService = ruleService;
            _collectionCommands = collectionCommands;
            _tagCommands = tagCommands;
            _tagQueries = tagQueries;
            _catalogQueries = catalogQueries;
            _edFiRuleExecutionLogDetailQueries = edFiRuleExecutionLogDetailQueries;
            _catalogService = catalogService;
            _edFiRuleExecutionLogDetailCommands = edFiRuleExecutionLogDetailCommands;
        }

        public async Task<CollectionCategory> AddAsync(ContainerBO container)
        {
            var containerReturn = await this._collectionCommands
                .AddAsync(MapModelToEntity(container));

            await CreateUpdateTags(container, containerReturn);

            var collectionCategory = await this
                .GetAsync(containerReturn.Id);

            return collectionCategory;
        }

        public async Task<CollectionCategory> UpdateAsync(ContainerBO container)
        {
            var containerReturn = await this._collectionCommands
                .UpdateAsync(MapModelToEntity(container));

            await CreateUpdateTags(container, containerReturn);

            var collectionCategory = await this
                .GetAsync(containerReturn.Id);

            return collectionCategory;
        }

        public async Task DuplicateAsync(ContainerBO container)
        {
            await this.ProcessToDuplicate(container);
            var containerReturn = await this._collectionCommands.AddAsync(MapModelToEntity(container));
            await CreateUpdateTags(container, containerReturn);
        }

        public async Task DeleteAsync(Guid id)
        {
            await this._collectionCommands.DeleteAsync(id);
        }

        public async Task<List<CollectionCategory>> GetAsync()
        {
            List<CollectionCategory> serviceCollections = new List<CollectionCategory>();
            var collections = await this._collectionQueries.GetAsync();

            if (collections != null)
            {
                foreach (var collection in collections)
                {
                    CollectionCategory currentCollection = new CollectionCategory(collection);
                    currentCollection.Tags = (await _tagQueries.GetByContainerIdAsync(currentCollection.Id)).Select(rec => new TagBO(rec)).ToList();

                    foreach (var childContainer in currentCollection.ChildContainers)
                    {
                        childContainer.Rules = await _ruleService.GetWithLogsByContainerIdAsync(childContainer.Id);
                        childContainer.AmountRules = childContainer.Rules != null ? childContainer.Rules.Count : 0;
                        childContainer.Tags = (await _tagQueries.GetByContainerIdAsync(childContainer.Id)).Select(rec => new TagBO(rec)).ToList();

                        if (collection.RuleDetailsDestinationId != null && childContainer.Rules != null && childContainer.Rules.Any())
                            childContainer.Rules.ForEach(rec => rec.CollectionRuleDetailsDestinationId = collection.RuleDetailsDestinationId);

                    }
                    serviceCollections.Add(currentCollection);
                }
            }
            return serviceCollections;
        }

        public async Task<CollectionCategory> GetByDatabaseEnvironmentIdAndContainerIdAsync(Guid databaseEnvironmentId, Guid? containerId = null)
        {
            Container container = null;
            if (containerId == null)
                container = await this._collectionQueries.GetDefaultAsync();
            else
                container = await this._collectionQueries.GetAsync(containerId.Value);

            var collectionCategory = new CollectionCategory();
            collectionCategory.Id = container.Id;
            collectionCategory.Name = container.Name;
            collectionCategory.EnvironmentType = container.EnvironmentType;
            collectionCategory.Description = container.Description;
            collectionCategory.ContainerTypeId = container.ContainerTypeId;
            //collectionCategory.CreatedByUserId = container.CreatedByUserId;
            collectionCategory.RuleDetailsDestinationId = container.RuleDetailsDestinationId;
            collectionCategory.ChildContainers = new List<CollectionCategory>();
            collectionCategory.Tags = new List<TagBO>();
            collectionCategory.Tags = (await _tagQueries.GetByContainerIdAsync(collectionCategory.Id)).Select(rec => new TagBO(rec)).ToList();

            foreach (var itemContainer in container.ChildContainers)
            {
                var category = new CollectionCategory
                {
                    Id = itemContainer.Id,
                    Name = itemContainer.Name,
                    Rules = new List<RuleBO>(),
                    ContainerTypeId = container.ContainerTypeId,
                    //CreatedByUserId = container.CreatedByUserId,
                    LastStatus = 0,
                    Tags = new List<TagBO>()
                };

                category.Tags = (await _tagQueries.GetByContainerIdAsync(itemContainer.Id)).Select(rec => new TagBO(rec)).ToList();
                category.Rules = await _ruleService.GetWithLogsByDatabaseEnvironmentIdAndContainerIdAsync(databaseEnvironmentId, itemContainer.Id);
                category.ValidRules = category.Rules.Count(m => m.LastStatus == 2);
                if (category.Rules.Any())
                {
                    if (category.Rules.Count(rec => rec.LastStatus == 3) > 0)
                        category.LastStatus = 3;
                    else if (category.Rules.Count(rec => rec.LastStatus == 2) > 0)
                        category.LastStatus = 2;
                    else if (category.Rules.Count(rec => rec.LastStatus == 1) > 0)
                        category.LastStatus = 1;
                }

                if (collectionCategory.RuleDetailsDestinationId != null && category.Rules != null && category.Rules.Any())
                    category.Rules.ForEach(rec => rec.CollectionRuleDetailsDestinationId = collectionCategory.RuleDetailsDestinationId);

                collectionCategory.ChildContainers.Add(category);
            }
            return collectionCategory;
        }

        public async Task<CollectionCategory> GetAsync(Guid? containerId = null)
        {
            Container container = null;
            if (containerId == null)
                container = await this._collectionQueries.GetDefaultAsync();
            else
                container = await this._collectionQueries.GetAsync(containerId.Value);

            var collectionCategory = new CollectionCategory();
            collectionCategory.Id = container.Id;
            collectionCategory.Name = container.Name;
            collectionCategory.Description = container.Description;
            collectionCategory.EnvironmentType = container.EnvironmentType;
            collectionCategory.ChildContainers = new List<CollectionCategory>();
            collectionCategory.Rules = await _ruleService.GetWithLogsByContainerIdAsync(container.Id);
            collectionCategory.ValidRules = collectionCategory.Rules.Count(m => m.LastStatus == 2);
            collectionCategory.LastStatus = collectionCategory.Rules.Any(m => m.LastStatus == 2) ? 2 : 1;
            collectionCategory.ContainerTypeId = container.ContainerTypeId;
            //collectionCategory.CreatedByUserId = container.CreatedByUserId;
            collectionCategory.Tags = new List<TagBO>();
            collectionCategory.Tags = (await _tagQueries.GetByContainerIdAsync(collectionCategory.Id)).Select(rec => new TagBO(rec)).ToList();
            collectionCategory.RuleDetailsDestinationId = container.RuleDetailsDestinationId;

            foreach (var itemContainer in container.ChildContainers)
            {
                var category = new CollectionCategory
                {
                    Id = itemContainer.Id,
                    Name = itemContainer.Name,
                    Rules = new List<RuleBO>(),
                    Tags = new List<TagBO>()
                };

                category.Tags = (await _tagQueries.GetByContainerIdAsync(itemContainer.Id)).Select(rec => new TagBO(rec)).ToList();
                category.Rules = await _ruleService.GetWithLogsByContainerIdAsync(itemContainer.Id);
                category.ValidRules = category.Rules.Count(m => m.LastStatus == 2);
                category.LastStatus = category.Rules.Any(m => m.LastStatus == 2) ? 2 : 1;

                if (collectionCategory.RuleDetailsDestinationId != null && category.Rules != null && category.Rules.Any())
                    category.Rules.ForEach(rec => rec.CollectionRuleDetailsDestinationId = collectionCategory.RuleDetailsDestinationId);

                collectionCategory.ChildContainers.Add(category);
            }
            return collectionCategory;
        }

        public async Task SetDefaultAsync(Guid containerId)
        {
            var container = await this.GetContainer(containerId);
            var defaultContainer = await this.GetDefaultContainer();

            defaultContainer.IsDefault = false;
            container.IsDefault = true;

            await this.UpdateAsync(container);
            await this.UpdateAsync(defaultContainer);
        }

        private async Task<ContainerBO> GetContainer(Guid containerId)
        {
            var container = await this._collectionQueries.GetAsync(containerId);
            var containerBO = MapEntityToModel(container);
            return containerBO;
        }

        private async Task<ContainerBO> GetDefaultContainer()
        {
            var container = await this._collectionQueries.GetDefaultAsync();
            var containerBO = MapEntityToModel(container);
            return containerBO;
        }

        private async Task ProcessToDuplicate(ContainerBO container)
        {
            container.CommunityUser = null;
            container.Id = new Guid();

            if (container.Rules != null)
                container.Rules.ForEach(m => m.Id = new Guid());

            container.IsDefault = false;
            if (container.ChildContainers != null)
            {
                foreach (var m in container.ChildContainers)
                {
                    await ProcessToDuplicate(m);
                }
            }
        }

        private Container MapModelToEntity(ContainerBO model)
        {
            Container entity = new Container
            {
                ContainerTypeId = model.ContainerTypeId,
                //CreatedByUserId = model.CreatedByUserId,
                Description = model.Description,
                Id = model.Id,
                IsDefault = model.IsDefault,
                Name = model.Name,
                ParentContainerId = model.ParentContainerId,
                EnvironmentType = model.EnvironmentType,
                RuleDetailsDestinationId = model.RuleDetailsDestinationId == 0 ? null : model.RuleDetailsDestinationId,
            };

            if (model.ContainerType != null)
                entity.ContainerType = new ContainerType
                {
                    Description = model.ContainerType.Description,
                    Id = model.ContainerType.Id,
                    Name = model.ContainerType.Name
                };

            //if (model.CommunityUser != null)
            //    entity.CommunityUser = new CommunityUserModel
            //    {
            //        CommunityOrganizationId = model.CommunityUser.CommunityOrganizationId,
            //        CreateByUserId = model.CommunityUser.CreateByUserId,
            //        CreatedDate = model.CommunityUser.CreatedDate,
            //        Email = model.CommunityUser.Email,
            //        Id = model.CommunityUser.Id,
            //        LastUpdatedDate = model.CommunityUser.LastUpdatedDate,
            //        LastUpdatedUserId = model.CommunityUser.LastUpdatedUserId,
            //        Name = model.Name
            //    };

            if (model.ChildContainers != null)
                entity.ChildContainers = model.ChildContainers.Select(rec => MapModelToEntity(rec)).ToList();

            if (model.Rules != null)
                entity.Rules = model.Rules.Select(rec => new Rule
                {
                    ContainerId = rec.ContainerId,
                    //CreatedByUserId = rec.CreatedByUserId,
                    Description = rec.Description,
                    DiagnosticSql = rec.DiagnosticSql,
                    //Enabled = rec.Enabled,
                    ErrorMessage = rec.ErrorMessage,
                    ErrorSeverityLevel = rec.ErrorSeverityLevel,
                    Id = rec.Id,
                    Name = rec.Name,
                    Resolution = rec.Name,
                    RuleIdentification = rec.RuleIdentification,
                    Version = rec.Version
                }).ToList();

            return entity;
        }

        private ContainerBO MapEntityToModel(Container entity)
        {
            ContainerBO model = new ContainerBO
            {
                ContainerTypeId = entity.ContainerTypeId,
                //CreatedByUserId = entity.CreatedByUserId,
                Description = entity.Description,
                Id = entity.Id,
                IsDefault = entity.IsDefault,
                Name = entity.Name,
                ParentContainerId = entity.ParentContainerId,
                EnvironmentType = entity.EnvironmentType,
                Tags = new List<TagBO>(),
                RuleDetailsDestinationId = entity.RuleDetailsDestinationId == 0 ? null : entity.RuleDetailsDestinationId,
            };

            model.Tags = _tagQueries.GetByContainerIdAsync(model.Id).Result.Select(rec => new TagBO(rec)).ToList();

            if (entity.ContainerType != null)
                model.ContainerType = new ContainerTypeBO
                {
                    Description = entity.ContainerType.Description,
                    Id = entity.ContainerType.Id,
                    Name = entity.ContainerType.Name
                };

            //if (entity.CommunityUser != null)
            //    model.CommunityUser = new CommunityUserBO
            //    {
            //        CommunityOrganizationId = entity.CommunityUser.CommunityOrganizationId,
            //        CreateByUserId = entity.CommunityUser.CreateByUserId,
            //        CreatedDate = entity.CommunityUser.CreatedDate,
            //        Email = entity.CommunityUser.Email,
            //        Id = entity.CommunityUser.Id,
            //        LastUpdatedDate = entity.CommunityUser.LastUpdatedDate,
            //        LastUpdatedUserId = entity.CommunityUser.LastUpdatedUserId,
            //        Name = entity.Name
            //    };

            if (entity.Rules != null)
                model.Rules = entity.Rules.Select(rec => new RuleBO
                {
                    ContainerId = rec.ContainerId,
                    //CreatedByUserId = rec.CreatedByUserId,
                    Description = rec.Description,
                    DiagnosticSql = rec.DiagnosticSql,
                    //Enabled = rec.Enabled,
                    ErrorMessage = rec.ErrorMessage,
                    ErrorSeverityLevel = rec.ErrorSeverityLevel,
                    Id = rec.Id,
                    Name = rec.Name,
                    Resolution = rec.Name,
                    RuleIdentification = rec.RuleIdentification,
                    Version = rec.Version
                }).ToList();

            if (entity.ChildContainers != null)
                model.ChildContainers = entity.ChildContainers.Select(rec => MapEntityToModel(rec)).ToList();

            return model;
        }

        private async Task CreateUpdateTags(ContainerBO container, Container collectionCategory)
        {
            var listTags = await this._tagQueries.GetByContainerIdAsync(collectionCategory.Id);
            if (container.Tags != null && container.Tags.Count > 0)
            {
                foreach (var tag in listTags)
                {
                    var existTag = container.Tags.FirstOrDefault(rec => rec.Id == tag.Id);
                    if (existTag == null)
                        await this._tagCommands.DeleteTagFromEntityAsync(tag.Id, collectionCategory.Id);
                }

                foreach (var tag in container.Tags)
                {
                    var existTag = listTags.FirstOrDefault(rec => rec.Id == tag.Id);
                    if (existTag == null)
                    {
                        if (tag.Id == -1)
                        {
                            var newTag = await this._tagCommands.AddAsync(new Tag
                            {
                                Name = tag.Name.ToUpper(),
                                Description = tag.Name.ToUpper(),
                                IsPublic = false
                            });
                            tag.Id = newTag.Id;
                        }

                        await this._tagCommands.AddTagToEntityAsync(new TagEntity
                        {
                            ContainerId = collectionCategory.Id,
                            TagId = tag.Id
                        });
                    }
                }
            }
            else if (listTags != null && listTags.Count > 0)
            {
                foreach (var tag in listTags)
                    await this._tagCommands.DeleteTagFromEntityAsync(tag.Id, collectionCategory.Id);
            }
        }

        public async Task<List<ContainerBO>> GetChildContainersAsync()
        {
            List<ContainerBO> result = new List<ContainerBO>();
            var listParentContainers = await _collectionQueries.GetAsync();

            if (listParentContainers != null && listParentContainers.Any())
            {
                foreach (Container parentContainer in listParentContainers)
                {
                    if (parentContainer.ChildContainers != null && parentContainer.ChildContainers.Any())
                    {
                        foreach (Container childContainer in parentContainer.ChildContainers)
                        {
                            result.Add(new ContainerBO
                            {
                                Id = childContainer.Id,
                                ParentContainerId = parentContainer.Id,
                                Name = childContainer.Name,
                                ParentContainerName = parentContainer.Name
                            });
                        }
                    }
                }
            }

            result = result.OrderBy(rec => rec.Name).ToList();
            return result;
        }

        public async Task<List<ContainerBO>> GetParentContainersAsync()
        {
            List<ContainerBO> result = new List<ContainerBO>();
            var listParentContainers = await _collectionQueries.GetAsync();

            if (listParentContainers != null && listParentContainers.Any())
            {
                foreach (Container parentContainer in listParentContainers)
                {
                    result.Add(new ContainerBO
                    {
                        Id = parentContainer.Id,
                        Name = parentContainer.Name
                    });
                }
            }

            result = result.OrderBy(rec => rec.Name).ToList();
            return result;
        }

        public async Task<ContainerBO> GetToCommunityAsync(Guid containerId)
        {
            Container container = await this._collectionQueries.GetAsync(containerId);

            var collectionCategory = new ContainerBO();
            collectionCategory.Id = container.Id;
            collectionCategory.Name = container.Name;
            collectionCategory.Description = container.Description;
            collectionCategory.EnvironmentType = container.EnvironmentType;
            collectionCategory.ChildContainers = new List<ContainerBO>();
            collectionCategory.ContainerTypeId = container.ContainerTypeId;
            //collectionCategory.CreatedByUserId = container.CreatedByUserId;
            collectionCategory.Tags = new List<TagBO>();
            collectionCategory.Tags = (await _tagQueries.GetByContainerIdAsync(collectionCategory.Id)).Select(rec => new TagBO(rec)).ToList();
            collectionCategory.RuleDetailsDestinationId = container.RuleDetailsDestinationId;

            var environmentCatalog = await _catalogQueries.GetAsync(container.EnvironmentType);
            if (environmentCatalog != null)
                collectionCategory.CatalogEnvironmentType = new CatalogBO
                {
                    CatalogType = environmentCatalog.CatalogType,
                    Description = environmentCatalog.Description,
                    Name = environmentCatalog.Name
                };

            if (collectionCategory.Tags.Any())
                collectionCategory.Tags = collectionCategory.Tags.Where(rec => rec.IsPublic).ToList();

            if (collectionCategory.RuleDetailsDestinationId != null)
            {
                var existCatalog = await _catalogQueries.GetAsync(collectionCategory.RuleDetailsDestinationId.Value);
                if (existCatalog != null)
                {
                    var listColumns = await _edFiRuleExecutionLogDetailQueries.GetColumnsByTableAsync(existCatalog.Name, "destination");
                    if (listColumns != null)
                    {
                        collectionCategory.ContainerDestination = new ContainerDestinationBO
                        {
                            CatalogId = existCatalog.Id,
                            ContainerId = collectionCategory.Id,
                            DestinationName = existCatalog.Name,
                            DestinationStructure = JsonConvert.SerializeObject(listColumns)
                        };
                    }
                }
            }

            foreach (var itemContainer in container.ChildContainers)
            {
                var category = new ContainerBO
                {
                    Id = itemContainer.Id,
                    Name = itemContainer.Name,
                    Description = itemContainer.Description,
                    ParentContainerId = itemContainer.ParentContainerId,
                    Rules = new List<RuleBO>(),
                    Tags = new List<TagBO>()
                };

                category.Tags = (await _tagQueries.GetByContainerIdAsync(itemContainer.Id)).Select(rec => new TagBO(rec)).ToList();
                category.Rules = await _ruleService.GetByContainerIdAsync(itemContainer.Id);

                if (category.Tags.Any())
                    category.Tags = category.Tags.Where(rec => rec.IsPublic).ToList();

                if (category.Rules.Any())
                {
                    category.Rules.ForEach(rec =>
                    {
                        if (rec.Tags != null && rec.Tags.Any())
                            rec.Tags = rec.Tags.Where(recTags => recTags.IsPublic).ToList();
                    });
                }

                collectionCategory.ChildContainers.Add(category);
            }
            return collectionCategory;
        }

        public async Task<ContainerBO> GetByNameAsync(ContainerBO model)
        {
            var existContainer = await _collectionQueries.GetByNameAsync(model.Name.ToLower());
            if(existContainer != null)
                return MapEntityToModel(existContainer);

            return null;
        }

        public async Task<string> AddFromCommunityAsync(ContainerBO model)
        {
            string result = string.Empty;
            try
            {
                if (!model.CreateNewCollection)
                {
                    ContainerBO collectionToDelete = await GetByNameAsync(model);
                    if (collectionToDelete != null)
                        await DeleteAsync(collectionToDelete.Id);
                }

                string collectionName = model.Name;
                int counter = 1;
                while (true)
                {
                    var existCollection = await _collectionQueries.GetByNameAsync(model.Name.ToLower());
                    if (existCollection == null) break;
                    model.Name = $"{collectionName} - ({counter})";
                    counter++;
                }

                var catalogsInformation = await _catalogQueries.GetAsync();
                var existEnvironment = catalogsInformation.FirstOrDefault(rec => rec.CatalogType == "EnvironmentType" && rec.Name.ToLower() == model.CatalogEnvironmentType.Name.ToLower());
                if (existEnvironment != null)
                    model.EnvironmentType = existEnvironment.Id;
                else
                {
                    var newEnvironmentType = await _catalogService.AddAsync(new CatalogBO
                    {
                        CatalogType = "EnvironmentType",
                        Description=model.CatalogEnvironmentType.Name,
                        Name=model.CatalogEnvironmentType.Name
                    });
                    model.EnvironmentType = newEnvironmentType.Id;
                }

                if (model.RuleDetailsDestinationId != null && model.ContainerDestination != null && !string.IsNullOrEmpty(model.ContainerDestination.DestinationName))
                {
                    bool createDestinationTable = true;
                    var existDestinationTable = catalogsInformation.FirstOrDefault(rec => rec.CatalogType == "RuleDetailsDestinationType" && rec.Name.ToLower() == model.ContainerDestination.DestinationName.ToLower());
                    if (existDestinationTable != null)
                    {
                        List<DestinationTableColumn> destinationTableInDbColumns = JsonConvert.DeserializeObject<List<DestinationTableColumn>>(model.ContainerDestination.DestinationStructure);
                        var listColumnsFromDestination = await _edFiRuleExecutionLogDetailQueries.GetColumnsByTableAsync(model.ContainerDestination.DestinationName, "destination");

                        if (destinationTableInDbColumns.Count == listColumnsFromDestination.Count)
                        {
                            createDestinationTable = false;
                            foreach (var columnInDestinationTable in destinationTableInDbColumns)
                            {
                                var existColumn = listColumnsFromDestination.FirstOrDefault(rec =>
                                rec.Name.ToLower() == columnInDestinationTable.Name.ToLower() &&
                                rec.Type == columnInDestinationTable.Type.ToLower() &&
                                rec.IsNullable == columnInDestinationTable.IsNullable);

                                if (existColumn == null)
                                {
                                    createDestinationTable = true;
                                    break;
                                }
                            }
                            if (!createDestinationTable)
                                model.RuleDetailsDestinationId = existDestinationTable.Id;
                            else
                            {
                                int counterTable = 1;
                                string newDestinationTableName = $"{model.ContainerDestination.DestinationName}_{counterTable}";
                                while (true)
                                {
                                    existDestinationTable = catalogsInformation.FirstOrDefault(rec => 
                                    rec.CatalogType == "RuleDetailsDestinationType" && 
                                    rec.Name.ToLower() == newDestinationTableName.ToLower());
                                    if (existDestinationTable == null) break;
                                    counterTable++;
                                    newDestinationTableName = $"{model.ContainerDestination.DestinationName}_{counterTable}";
                                }
                                model.ContainerDestination.DestinationName = newDestinationTableName;
                            }
                        }
                    }

                    if(createDestinationTable)
                    {
                        var newDestinationTable = await _catalogService.AddAsync(new CatalogBO
                        {
                            CatalogType = "RuleDetailsDestinationType",
                            Description = model.ContainerDestination.DestinationName,
                            Name = model.ContainerDestination.DestinationName
                        });
                        model.RuleDetailsDestinationId = newDestinationTable.Id;

                        bool existDestinationTableInDb = await _edFiRuleExecutionLogDetailQueries.ExistExportTableFromRuleExecutionLogAsync(model.ContainerDestination.DestinationName, "destination");
                        if (!existDestinationTableInDb)
                        {
                            List<DestinationTableColumn> destinationTableInDbColumns = JsonConvert.DeserializeObject<List<DestinationTableColumn>>(model.ContainerDestination.DestinationStructure);
                            List<string> sqlColumns = new List<string>();
                            foreach (var column in destinationTableInDbColumns)
                            {
                                string isNull = column.IsNullable ? "NULL" : "NOT NULL";
                                if (column.Name == "id" && column.Type == "int")
                                    sqlColumns.Add("[Id] [int] IDENTITY(1,1) NOT NULL");
                                else if (column.Type.Contains("varchar"))
                                    sqlColumns.Add($"[{column.Name}] [{column.Type}](max) {isNull}");
                                else if (column.Type.Contains("datetime"))
                                    sqlColumns.Add($"[{column.Name}] [{column.Type}](7) {isNull}");
                                else
                                    sqlColumns.Add($"[{column.Name}] [{column.Type}] {isNull}");
                            }
                            string sqlCreate = $"CREATE TABLE [destination].[{model.ContainerDestination.DestinationName}]({string.Join(",", sqlColumns)}) ";
                            await _edFiRuleExecutionLogDetailCommands.ExecuteSqlAsync(sqlCreate);
                        }
                    }
                }

                Container newCollection = new Container
                {
                    ContainerTypeId = 1,
                    Description = model.Description,
                    Name = model.Name,
                    IsDefault = false,
                    EnvironmentType = model.EnvironmentType,
                    RuleDetailsDestinationId = model.RuleDetailsDestinationId,
                    ChildContainers = new List<Container>()
                };

                if (model.ChildContainers != null && model.ChildContainers.Any())
                {
                    model.ChildContainers.ForEach(rec => {
                        Container newChildContainer = new Container
                        {
                            ContainerTypeId = 2,
                            Description = rec.Description,
                            Name = rec.Name,
                            IsDefault = false,
                            EnvironmentType = 0,
                            RuleDetailsDestinationId = null,
                            Rules= new List<Rule>()
                        };

                        if (rec.Rules != null && rec.Rules.Any())
                        {
                            rec.Rules.ForEach(recRule =>
                            {
                                newChildContainer.Rules.Add(new Rule
                                {
                                    Description = recRule.Description,
                                    DiagnosticSql = recRule.DiagnosticSql,
                                    //Enabled = true,
                                    ErrorMessage = recRule.ErrorMessage,
                                    MaxNumberResults = recRule.MaxNumberResults,
                                    Name = recRule.Name,
                                    ErrorSeverityLevel = recRule.ErrorSeverityLevel,
                                    Resolution = recRule.Resolution,
                                    RuleIdentification = recRule.RuleIdentification,
                                    Version = recRule.Version
                                });
                            });
                        }

                        newCollection.ChildContainers.Add(newChildContainer);
                    });
                }

                var newCollectionFromDatabase = await _collectionCommands.AddAsync(newCollection);
                if (newCollectionFromDatabase != null)
                {
                    var listTags = await _tagQueries.GetAsync();

                    if (model.Tags != null && model.Tags.Any())
                    {
                        foreach (var tag in model.Tags)
                        {
                            var existTag = listTags.FirstOrDefault(rec => rec.Name.ToLower() == tag.Name.ToLower());
                            if (existTag == null)
                            {
                                existTag = await _tagCommands.AddAsync(new Tag { 
                                    Description=tag.Description,
                                    IsPublic=true,
                                    Name=tag.Name.ToUpper()
                                });
                                listTags.Add(existTag);
                            }

                            await _tagCommands.AddTagToEntityAsync(new TagEntity { 
                                ContainerId = newCollectionFromDatabase.Id,
                                TagId = existTag.Id
                            });
                        }
                    }

                    if (model.ChildContainers != null && model.ChildContainers.Any())
                    {
                        foreach (var childContainer in model.ChildContainers)
                        {
                            var newChildContainer = newCollectionFromDatabase.ChildContainers.FirstOrDefault(rec => rec.Name == childContainer.Name);
                            if (childContainer.Tags != null && childContainer.Tags.Any())
                            {
                                foreach (var tag in childContainer.Tags)
                                {
                                    var existTag = listTags.FirstOrDefault(rec => rec.Name.ToLower() == tag.Name.ToLower());
                                    if (existTag == null)
                                    {
                                        existTag = await _tagCommands.AddAsync(new Tag
                                        {
                                            Description = tag.Description,
                                            IsPublic = true,
                                            Name = tag.Name.ToUpper()
                                        });
                                        listTags.Add(existTag);
                                    }

                                    await _tagCommands.AddTagToEntityAsync(new TagEntity
                                    {
                                        ContainerId = newChildContainer.Id,
                                        TagId = existTag.Id
                                    });
                                }
                            }

                            if (childContainer.Rules != null && childContainer.Rules.Any())
                            {
                                foreach (var rule in childContainer.Rules)
                                {
                                    var newRule = newChildContainer.Rules.FirstOrDefault(rec=>rec.Name==rule.Name);
                                    if (rule.Tags != null && rule.Tags.Any())
                                    {
                                        foreach (var tag in rule.Tags)
                                        {
                                            var existTag = listTags.FirstOrDefault(rec => rec.Name.ToLower() == tag.Name.ToLower());
                                            if (existTag == null)
                                            {
                                                existTag = await _tagCommands.AddAsync(new Tag
                                                {
                                                    Description = tag.Description,
                                                    IsPublic = true,
                                                    Name = tag.Name.ToUpper()
                                                });
                                                listTags.Add(existTag);
                                            }

                                            await _tagCommands.AddTagToEntityAsync(new TagEntity
                                            {
                                                RuleId = newRule.Id,
                                                TagId = existTag.Id
                                            });
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }

        public async Task<bool> ValidateDestinationTableAsync(ContainerDestinationBO container)
        {
            try
            {
                var columnsLocal = await _edFiRuleExecutionLogDetailQueries.GetColumnsByTableAsync(container.DestinationName, "destination");
                List<DestinationTableColumn> columnsCommunity = JsonConvert.DeserializeObject<List<DestinationTableColumn>>(container.DestinationStructure);

                if (columnsLocal == null || columnsLocal.Count == 0)
                    return true;

                if (columnsCommunity.Count != columnsLocal.Count)
                    return false;

                foreach (var column in columnsLocal)
                {
                    var existColumnCommunity = columnsCommunity
                        .FirstOrDefault(rec =>
                        rec.Name.ToLower() == column.Name.ToLower() &&
                        rec.Type.ToLower() == column.Type.ToLower() &&
                        rec.IsNullable == column.IsNullable);

                    if (existColumnCommunity == null) return false;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
