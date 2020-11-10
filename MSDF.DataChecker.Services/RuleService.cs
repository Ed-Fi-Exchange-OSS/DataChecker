// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using MSDF.DataChecker.Persistence.Catalogs;
using MSDF.DataChecker.Persistence.Collections;
using MSDF.DataChecker.Persistence.RuleExecutionLogs;
using MSDF.DataChecker.Persistence.Rules;
using MSDF.DataChecker.Persistence.Tags;
using MSDF.DataChecker.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MSDF.DataChecker.Services
{
    public interface IRuleService
    {
        Task<List<RuleBO>> GetAsync();
        Task<RuleBO> GetAsync(Guid ruleId);
        Task<RuleBO> AddAsync(RuleBO model);
        Task<RuleBO> UpdateAsync(RuleBO model);
        Task DeleteAsync(Guid ruleId);
        Task<List<RuleBO>> GetWithLogsAsync();
        Task<List<RuleBO>> GetWithLogsByContainerIdAsync(Guid containerId);
        Task<List<RuleBO>> GetWithLogsByDatabaseEnvironmentIdAndContainerIdAsync(Guid databaseEnvironmentId, Guid containerId);
        Task<List<RuleTestResult>> GetTopResults(Guid id, Guid databaseEnvironmentId);
        Task<RuleBO> CopyToAsync(Guid ruleId, Guid containerId);
        Task<SearchTagBO> SearchRulesAsync(List<Guid> collectionsSelected, List<Guid> containersSelected,List<int> tags, string name, string diagnostic, int? detailsDestination, int? severityId);
        Task AssignTagsToRule(Guid ruleId, List<TagBO> tags);
        Task<List<RuleBO>> GetByContainerIdAsync(Guid containerId);
        Task<bool> MoveRuleToContainer(List<Guid> rules, ContainerBO container);
    }

    public class RuleService : IRuleService
    {
        private IRuleQueries _ruleQueries;
        private IRuleCommands  _ruleCommands;
        private IRuleExecutionLogQueries _ruleExecutionLogQueries;
        private ITagCommands _tagCommands;
        private ITagQueries _tagQueries;
        private ICollectionQueries _collectionQueries;
        private ICatalogQueries _catalogQueries;

        public RuleService(
            IRuleQueries ruleQueries,
            IRuleExecutionLogQueries ruleExecutionLogQueries,
            IRuleCommands ruleCommands,
            ITagCommands tagCommands,
            ITagQueries tagQueries,
            ICollectionQueries collectionQueries,
            ICatalogQueries catalogQueries)
        {
            _ruleQueries = ruleQueries;
            _ruleExecutionLogQueries = ruleExecutionLogQueries;
            _ruleCommands = ruleCommands;
            _tagCommands = tagCommands;
            _tagQueries = tagQueries;
            _collectionQueries = collectionQueries;
            _catalogQueries = catalogQueries;
        }

        public async Task<List<RuleBO>> GetAsync()
        {
            var rulesDatabase = await this._ruleQueries
                .GetAsync();

            if (rulesDatabase != null)
                return rulesDatabase
                    .Select(rec => MapEntityToModel(rec))
                    .ToList();

            return null;
        }

        public async Task<RuleBO> GetAsync(Guid ruleId)
        {
            var rule = await _ruleQueries
                .GetAsync(ruleId);

            return rule != null ? MapEntityToModel(rule) : null;
        }

        public async Task<List<RuleBO>> GetWithLogsAsync()
        {
            var rulesDatabase = await this._ruleQueries
                .GetAsync();

            if (rulesDatabase != null)
            {
                List<RuleBO> rules = rulesDatabase
                    .Select(rec => MapEntityToModel(rec))
                    .ToList();

                foreach (RuleBO rule in rules)
                {
                    var logs = await _ruleExecutionLogQueries.GetByRuleIdAsync(rule.Id);
                    rule.Counter = logs.Any() ? logs.FirstOrDefault().Result : 0;
                    rule.LastExecution = logs.Any() ? logs.FirstOrDefault().ExecutionDate : (DateTime?)(null);
                    rule.LastStatus = logs.Any() ? logs.FirstOrDefault().StatusId : 0;
                }
                return rules;
            }
            return null;
        }

        public async Task<List<RuleBO>> GetWithLogsByContainerIdAsync(Guid containerId)
        {
            var rulesDatabase = await this._ruleQueries
                .GetByContainerIdAsync(containerId);

            if (rulesDatabase != null)
            {
                List<RuleBO> rules = rulesDatabase.
                    Select(rec => MapEntityToModel(rec))
                    .ToList();

                foreach (RuleBO rule in rules)
                {
                    var logs = await _ruleExecutionLogQueries.GetByRuleIdAsync(rule.Id);
                    rule.Counter = logs.Any() ? logs.FirstOrDefault().Result : 0;
                    rule.LastExecution = logs.Any() ? logs.FirstOrDefault().ExecutionDate : (DateTime?)(null);
                    rule.LastStatus = logs.Any() ? logs.FirstOrDefault().StatusId : 0;
                }
                return rules;
            }

            return null;
        }

        public async Task<List<RuleBO>> GetWithLogsByDatabaseEnvironmentIdAndContainerIdAsync(Guid databaseEnvironmentId , Guid containerId)
        {
            var rulesDatabase = await this._ruleQueries
                .GetByContainerIdAsync(containerId);

            if (rulesDatabase != null)
            {
                List<RuleBO> rules = rulesDatabase.
                    Select(rec => MapEntityToModel(rec))
                    .ToList();

                foreach (RuleBO rule in rules)
                {
                    var logs = await _ruleExecutionLogQueries.GetByRuleIdAndDatabaseEnvironmentIdAsync(rule.Id, databaseEnvironmentId);
                    var latestLogRecord = logs.FirstOrDefault();

                    rule.Counter = 0;
                    rule.LastExecution = (DateTime?)(null);
                    rule.LastStatus = 0;

                    if (latestLogRecord != null)
                    {
                        rule.Counter = latestLogRecord.Result;
                        rule.LastExecution = latestLogRecord.ExecutionDate;
                        rule.LastStatus = latestLogRecord.StatusId;
                    }                    
                }
                return rules;
            }

            return null;
        }

        public async Task<List<RuleTestResult>> GetTopResults(Guid id,Guid databaseEnvironmentId)
        {
            List<RuleTestResult> results = new List<RuleTestResult>();
            var ruleExecutionLogs = await _ruleExecutionLogQueries
                .GetByRuleIdAndDatabaseEnvironmentIdAsync(id, databaseEnvironmentId);            

            foreach (var item in ruleExecutionLogs)
            {
                results.Add(new RuleTestResult
                {
                    Id = item.Id,
                    Result = item.Result,
                    Status = (Status)item.StatusId,
                    LastExecuted = item.ExecutionDate,
                    ExecutionTimeMs = item.ExecutionTimeMs,
                    ErrorMessage = item.Response,
                    Evaluation = item.Evaluation,
                    ExecutedSql = item.ExecutedSql,
                    DiagnosticSql = item.DiagnosticSql,
                    RuleDetailsDestinationId = item.RuleDetailsDestinationId
                });
            }
            return results;
        }

        public async Task<RuleBO> AddAsync(RuleBO model)
        {
            var result = await this._ruleCommands
                .AddAsync(MapModelToEntity(model));

            await CreateUpdateTags(model, result);
            return result != null ? MapEntityToModel(result) : null;
        }

        public async Task<RuleBO> UpdateAsync(RuleBO model)
        {
            var result = await this._ruleCommands
                .UpdateAsync(MapModelToEntity(model));

            await CreateUpdateTags(model, result);
            return result != null ? MapEntityToModel(result) : null;
        }

        public async Task DeleteAsync(Guid ruleId)
        {
            await this._tagCommands.DeleteTagFromEntityIdAsync(ruleId);
            await this._ruleCommands.DeleteRule(ruleId);
        }

        private async Task CreateUpdateTags(RuleBO ruleBO, Rule ruleDB)
        {
            var listTags = await this._tagQueries.GetByRuleIdAsync(ruleDB.Id);
            if (ruleBO.Tags != null && ruleBO.Tags.Count > 0)
            {
                foreach (var tag in listTags)
                {
                    var existTag = ruleBO.Tags.FirstOrDefault(rec => rec.Id == tag.Id);
                    if (existTag == null)
                        await this._tagCommands.DeleteTagFromEntityAsync(tag.Id, ruleDB.Id);
                }

                foreach (var tag in ruleBO.Tags)
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
                            RuleId = ruleDB.Id,
                            TagId = tag.Id
                        });
                    }
                }
            }
            else if (listTags != null && listTags.Count > 0)
            {
                foreach (var tag in listTags)
                    await this._tagCommands.DeleteTagFromEntityAsync(tag.Id, ruleDB.Id);
            }
        }

        public async Task<RuleBO> CopyToAsync(Guid ruleId, Guid containerId)
        {
            RuleBO result = new RuleBO();
            RuleBO ruleToCopy = MapEntityToModel(await _ruleQueries.GetAsync(ruleId));

            string ruleName = ruleToCopy.Name;
            ruleToCopy.ContainerId = containerId;

            var rulesFromContainer = await _ruleQueries.GetByContainerIdAsync(containerId);
            if (rulesFromContainer != null && rulesFromContainer.Count > 0)
            {
                while (true)
                {
                    var existRuleWithName = rulesFromContainer.FirstOrDefault(rec => rec.Name == ruleName);
                    if (existRuleWithName == null) break;
                    ruleName += " - Copy";
                }
            }

            ruleToCopy.Id = Guid.Empty;
            ruleToCopy.Name = ruleName;
            result = MapEntityToModel(await _ruleCommands.AddAsync(MapModelToEntity(ruleToCopy)));

            var listTags = await _tagQueries.GetByRuleIdAsync(ruleId);
            if (listTags != null && listTags.Count > 0)
            {
                foreach (var tag in listTags)
                {
                    await _tagCommands.AddTagToEntityAsync(new TagEntity
                    {
                        RuleId = result.Id,
                        TagId = tag.Id
                    });
                }
            }

            result.Tags = _tagQueries.GetByRuleIdAsync(result.Id).Result.Select(rec => new TagBO(rec)).ToList();
            return result;
        }

        public async Task<SearchTagBO> SearchRulesAsync(List<Guid> collectionsSelected, List<Guid> containersSelected, List<int> tagsSelected, string name, string diagnostic, int? detailsDestination, int? severityId)
        {
            SearchTagBO result = new SearchTagBO();
            List<Rule> allRules = await _ruleQueries
                .GetByCollectionOrContainerOrTagAsync(collectionsSelected, containersSelected, tagsSelected
                .ToList());

            if (!string.IsNullOrEmpty(name))
            {
                allRules = allRules
                    .Where(rec => rec.Name.ToLower().Contains(name.ToLower()))
                    .ToList();
            }

            if (!string.IsNullOrEmpty(diagnostic))
            {
                allRules = allRules
                    .Where(rec => rec.DiagnosticSql.ToLower().Contains(diagnostic.ToLower()))
                    .ToList();
            }

            if (detailsDestination != null)
            {
                var collectionsByDestination = await _collectionQueries.GetByRulesDestinationIdAsync(detailsDestination.Value);
                if (collectionsByDestination != null && collectionsByDestination.Any())
                {
                    List<Guid> containersByDestination = new List<Guid>();
                    foreach (var collection in collectionsByDestination)
                    {
                        if (collection.ChildContainers != null && collection.ChildContainers.Any())
                        {
                            containersByDestination.AddRange(collection.ChildContainers.Select(rec => rec.Id));
                        }
                    }
                    
                    if (containersByDestination.Any())
                    {
                        containersByDestination = containersByDestination.Distinct().ToList();
                        allRules = allRules
                            .Where(rec => containersByDestination.Contains(rec.ContainerId))
                            .ToList();
                    }
                }
            }

            if (severityId != null)
            {
                allRules = allRules
                    .Where(rec => rec.ErrorSeverityLevel == severityId.Value)
                    .ToList();
            }

            result.Rules = allRules
                .Select(rec => MapEntityToModel(rec))
                .ToList();

            if (result.Rules != null && result.Rules.Count > 0)
            {
                var listEnvironmentType = await _catalogQueries.GetByTypeAsync("EnvironmentType");

                List<Guid> containersParents = result
                    .Rules
                    .Select(rec => rec.ContainerId)
                    .ToList();

                containersParents = containersParents.Distinct().ToList();

                List<Container> allContainersParents = await _collectionQueries
                    .GetByListAsync(containersParents);

                List<Container> allContainersMain = await _collectionQueries
                    .GetByListAsync(allContainersParents.Select(rec=>rec.ParentContainerId.Value).ToList());

                foreach (RuleBO rule in result.Rules)
                {
                    Container existParent = allContainersParents.FirstOrDefault(rec => rec.Id == rule.ContainerId);
                    if (existParent != null && existParent.ParentContainerId != null)
                    {
                        Container existParentMain = allContainersMain.FirstOrDefault(rec => rec.Id == existParent.ParentContainerId.Value);
                        if (existParentMain != null)
                        {
                            Catalog existType = listEnvironmentType.FirstOrDefault(rec => rec.Id == existParentMain.EnvironmentType);
                            if (existType != null)
                            {
                                rule.ParentContainer = existParentMain.Id;
                                rule.EnvironmentTypeText = existType.Name;
                                rule.CollectionContainerName = $"{existParentMain.Name}/{existParent.Name}";
                                rule.ContainerName = existParent.Name;
                                rule.CollectionName = existParentMain.Name;
                            }
                        }
                    }
                }

                result.Rules = result.Rules.OrderBy(rec => rec.CollectionContainerName).ToList();
            }

            return result;
        }

        public async Task AssignTagsToRule(Guid ruleId, List<TagBO> tags)
        {
            List<Tag> listOfTags = await _tagQueries.GetByRuleIdAsync(ruleId);
            foreach (var tag in tags)
            {
                if (tag.Id == -1)
                {
                    var existTag = await _tagQueries.GetByNameAsync(tag.Name.ToUpper());
                    if (existTag != null)
                    {
                        await _tagCommands.AddTagToEntityAsync(new TagEntity
                        {
                            TagId = existTag.Id,
                            RuleId = ruleId
                        });
                    }
                    else
                    {
                        var newTag = await this._tagCommands.AddAsync(new Tag
                        {
                            Name = tag.Name.ToUpper(),
                            Description = tag.Name.ToUpper(),
                            IsPublic = false
                        });

                        await _tagCommands.AddTagToEntityAsync(new TagEntity
                        {
                            TagId = newTag.Id,
                            RuleId = ruleId
                        });
                    }
                }
                else
                {
                    var existTag = listOfTags.FirstOrDefault(rec => rec.Id == tag.Id);
                    if (existTag == null)
                    {
                        await _tagCommands.AddTagToEntityAsync(new TagEntity
                        {
                            TagId = tag.Id,
                            RuleId = ruleId
                        });
                    }
                }
            }
        }

        private RuleBO MapEntityToModel(Rule entity)
        {
            RuleBO model = new RuleBO
            {
                Id = entity.Id,
                ContainerId = entity.ContainerId,
                //CreatedByUserId = entity.CreatedByUserId,
                //CreatedByUserName = entity.CreatedByUserName,
                Description = entity.Description,
                DiagnosticSql = entity.DiagnosticSql,
                //Enabled = entity.Enabled,
                ErrorMessage = entity.ErrorMessage,
                ErrorSeverityLevel = entity.ErrorSeverityLevel,
                Name = entity.Name,
                Resolution = entity.Resolution,
                RuleIdentification = entity.RuleIdentification,
                Version = entity.Version,
                Tags = new List<TagBO>(),
                MaxNumberResults = entity.MaxNumberResults
            };

            model.Tags = _tagQueries.GetByRuleIdAsync(model.Id).Result.Select(rec => new TagBO(rec)).ToList();

            if (entity.RuleExecutionLogs != null)
                model.RuleExecutionLogs = entity.RuleExecutionLogs
                    .Select(rec => new RuleExecutionLogBO
                    {
                        DatabaseEnvironmentId = rec.DatabaseEnvironmentId,
                        DiagnosticSql = rec.DiagnosticSql,
                        Evaluation = rec.Evaluation,
                        ExecutedSql = rec.ExecutedSql,
                        ExecutionDate = rec.ExecutionDate,
                        ExecutionTimeMs = rec.ExecutionTimeMs,
                        Id = rec.Id,
                        Response = rec.Response,
                        Result = rec.Result,
                        RuleId = rec.RuleId,
                        StatusId = rec.StatusId
                    })
                    .ToList();

            return model;
        }

        private Rule MapModelToEntity(RuleBO model)
        {
            Rule entity = new Rule
            {
                Id = model.Id,
                ContainerId = model.ContainerId,
                //CreatedByUserId = model.CreatedByUserId,
                Description = model.Description,
                DiagnosticSql = model.DiagnosticSql,
                //Enabled = model.Enabled,
                ErrorMessage = model.ErrorMessage,
                ErrorSeverityLevel = model.ErrorSeverityLevel,
                Name = model.Name,
                Resolution = model.Resolution,
                RuleIdentification = model.RuleIdentification,
                Version = model.Version,
                MaxNumberResults = model.MaxNumberResults
            };

            if (model.RuleExecutionLogs != null)
                entity.RuleExecutionLogs = model.RuleExecutionLogs
                    .Select(rec => new RuleExecutionLog
                    {
                        DatabaseEnvironmentId = rec.DatabaseEnvironmentId,
                        DiagnosticSql = rec.DiagnosticSql,
                        Evaluation = rec.Evaluation,
                        ExecutedSql = rec.ExecutedSql,
                        ExecutionDate = rec.ExecutionDate,
                        ExecutionTimeMs = rec.ExecutionTimeMs,
                        Id = rec.Id,
                        Response = rec.Response,
                        Result = rec.Result,
                        RuleId = rec.RuleId,
                        StatusId = rec.StatusId
                    })
                    .ToList();

            return entity;
        }

        public async Task<List<RuleBO>> GetByContainerIdAsync(Guid containerId)
        {
            var rulesDatabase = await this._ruleQueries
                .GetByContainerIdAsync(containerId);

            if (rulesDatabase != null)
            {
                List<RuleBO> rules = rulesDatabase.
                    Select(rec => MapEntityToModel(rec))
                    .ToList();

                return rules;
            }

            return null;
        }

        public async Task<bool> MoveRuleToContainer(List<Guid> rules, ContainerBO container)
        {
            bool result = true;
            foreach (Guid idRule in rules)
            {
                Rule existRule = await _ruleQueries.GetAsync(idRule);
                if (existRule != null && existRule.ContainerId != container.Id)
                {
                    existRule.ContainerId = container.Id;
                    await _ruleCommands.UpdateContainerIdAsync(existRule);
                }
            }
            return result;
        }
    }
}
