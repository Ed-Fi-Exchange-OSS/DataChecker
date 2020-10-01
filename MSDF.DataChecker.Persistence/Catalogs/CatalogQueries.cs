// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using Microsoft.EntityFrameworkCore;
using MSDF.DataChecker.Persistence.EntityFramework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MSDF.DataChecker.Persistence.Catalogs
{
    public interface ICatalogQueries 
    {
        Task<List<Catalog>> GetAsync();
        Task<List<Catalog>> GetByTypeAsync(string type);
        Task<Catalog> GetAsync(int id);
    }
    public class CatalogQueries : ICatalogQueries
    {
        private readonly DatabaseContext _db;
        public CatalogQueries(DatabaseContext db)
        {
            _db = db;
        }

        public async Task<List<Catalog>> GetAsync()
        {
            var catalogs = await _db.Catalogs.ToListAsync();
            return catalogs;
        }

        public async Task<Catalog> GetAsync(int id)
        {
            var catalogs = await _db.Catalogs
                .FirstOrDefaultAsync(rec => rec.Id == id);

            return catalogs;
        }

        public async Task<List<Catalog>> GetByTypeAsync(string type)
        {
            var catalogs = await _db.Catalogs
                .Where(rec=>rec.CatalogType==type)
                .ToListAsync();

            return catalogs;
        }
    }
}
