// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using MSDF.DataChecker.Persistence.Catalogs;
using MSDF.DataChecker.Services.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MSDF.DataChecker.Services
{
    public interface ICatalogService
    {
        Task<CatalogBO> AddAsync(CatalogBO model);
        Task<List<CatalogBO>> GetAsync();
        Task<CatalogBO> GetAsync(int id);
        Task<List<CatalogBO>> GetByTypeAsync(string type);
    }

    public class CatalogService: ICatalogService
    {
        ICatalogQueries _queries;
        ICatalogCommands _commands;

        public CatalogService(
            ICatalogQueries queries,
            ICatalogCommands commands)
        {
            _queries = queries;
            _commands = commands;
        }

        public async Task<List<CatalogBO>> GetAsync()
        {
            var catalogList = await _queries.GetAsync();
            if (catalogList != null)
                return catalogList
                    .Select(rec => MapEntityToModel(rec))
                    .ToList();

            return null;
        }

        public async Task<CatalogBO> GetAsync(int id)
        {
            var catalog = await _queries.GetAsync(id);
            if (catalog != null)
                return MapEntityToModel(catalog);

            return null;
        }

        public async Task<List<CatalogBO>> GetByTypeAsync(string type)
        {
            var catalogList = await _queries.GetByTypeAsync(type);
            if (catalogList != null)
                return catalogList
                    .Select(rec => MapEntityToModel(rec))
                    .ToList();

            return null;
        }

        private Catalog MapModelToEntity(CatalogBO model)
        {
            Catalog entity = new Catalog
            {
                CatalogType = model.CatalogType,
                Description = model.Description,
                Id = model.Id,
                Name = model.Name,
                Updated = model.Updated
            };

            return entity;
        }

        private CatalogBO MapEntityToModel(Catalog entity)
        {
            CatalogBO model = new CatalogBO
            {
                CatalogType = entity.CatalogType,
                Description = entity.Description,
                Id = entity.Id,
                Name = entity.Name,
                Updated = entity.Updated
            };

            return model;
        }

        public async Task<CatalogBO> AddAsync(CatalogBO model)
        {
            Catalog result = await _commands.AddAsync(MapModelToEntity(model));
            return MapEntityToModel(result);
        }
    }
}
