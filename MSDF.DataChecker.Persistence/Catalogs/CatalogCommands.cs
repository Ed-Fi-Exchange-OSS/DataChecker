// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using MSDF.DataChecker.Persistence.EntityFramework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MSDF.DataChecker.Persistence.Catalogs
{
    public interface ICatalogCommands
    {
        Task<Catalog> AddAsync(Catalog catalog);
        Task<Catalog> UpdateAsync(Catalog catalog);
        Task DeleteAsync(int id);
    }

    public class CatalogCommands : ICatalogCommands
    {
        private readonly DatabaseContext _db;
        public CatalogCommands(DatabaseContext db)
        {
            _db = db;
        }

        public async Task<Catalog> AddAsync(Catalog catalog)
        {
            catalog.Updated = DateTime.UtcNow;
            var newEntity = this._db.Add(catalog);
            await this._db.SaveChangesAsync();
            return newEntity.Entity;
        }

        public async Task DeleteAsync(int id)
        {
            var catalogToDelete = this._db.Catalogs
                .FirstOrDefault(m => m.Id == id);

            this._db.Catalogs.Remove(catalogToDelete);
            await this._db.SaveChangesAsync();
        }

        public async Task<Catalog> UpdateAsync(Catalog catalog)
        {
            var catalogInfo = this._db.Catalogs
                .FirstOrDefault(m => m.Id == catalog.Id);

            catalogInfo.Name = catalog.Name;
            catalogInfo.Description = catalog.Description;
            catalogInfo.Updated = DateTime.UtcNow;

            var catalogUpdated = this._db.Catalogs.Update(catalog);
            await this._db.SaveChangesAsync();
            return catalogUpdated.Entity;
        }
    }
}
