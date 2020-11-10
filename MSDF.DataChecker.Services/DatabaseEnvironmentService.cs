// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using MSDF.DataChecker.Persistence.DatabaseEnvironments;
using MSDF.DataChecker.Services.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace MSDF.DataChecker.Services
{
    public interface IDatabaseEnvironmentService
    {
        Task<DatabaseEnvironmentBO> AddAsync(DatabaseEnvironmentBO model);
        Task<DatabaseEnvironmentBO> UpdateAsync(DatabaseEnvironmentBO model);
        Task DeleteAsync(Guid databaseEnvironmentId);
        Task<List<DatabaseEnvironmentBO>> GetAsync();
        Task<DatabaseEnvironmentBO> GetAsync(Guid databaseEnviromentId);
        Task<Dictionary<string, List<string>>> GetMapTableAsync(Guid id);
        Task<GenericResponse> TestConnectionAsync(string connectionString);
        Task<string> TestConnectionByIdAsync(Guid id);
        Task<DatabaseEnvironmentBO> DuplicateAsync(Guid id);
    }

    public class DatabaseEnvironmentService : IDatabaseEnvironmentService
    {
        private IDatabaseEnvironmentQueries _databaseEnvironmentQueries;
        private IDatabaseEnvironmentCommands _databaseEnvironmentCommands;
        private IUserParamService _userParamsService;

        public DatabaseEnvironmentService(
            IDatabaseEnvironmentQueries databaseEnvironmentQueries,
            IDatabaseEnvironmentCommands databaseEnvironmentCommands,
            IUserParamService userParamsService)

        {
            _databaseEnvironmentQueries = databaseEnvironmentQueries;            
            _databaseEnvironmentCommands = databaseEnvironmentCommands;
            _userParamsService = userParamsService;
        }

        public async Task<DatabaseEnvironmentBO> AddAsync(DatabaseEnvironmentBO model)
        {            
            var map = await this.GetTablesAndColumnsByConnectionString(model.GetConnectionString());
            if(map != null)
                model.MapTables = JsonConvert.SerializeObject(map, Formatting.Indented);

            var result  = await this._databaseEnvironmentCommands.AddAsync(MapModelToEntity(model));
            return MapEntityToModel(result);
        }

        public async Task DeleteAsync(Guid databaseEnvironmentId)
        {
            await this._databaseEnvironmentCommands.DeleteAsync(databaseEnvironmentId);
        }

        public async Task<Dictionary<string, List<string>>> GetMapTableAsync(Guid databaseEnvironmentId)
        {
            var databaseEnvironment = await this.GetAsync(databaseEnvironmentId);

            if (databaseEnvironment.MapTables !=null && databaseEnvironment.MapTables !="")
            {
                var conversion = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(databaseEnvironment.MapTables);
                return conversion;
            }
            else
            {
                var map = await this.GetTablesAndColumnsByConnectionString(databaseEnvironment.GetConnectionString());
                databaseEnvironment.MapTables = JsonConvert.SerializeObject(map, Formatting.Indented);
                await this.UpdateAsync(databaseEnvironment);
                return map;
            }            
        }

        private async Task<Dictionary<string,List<string>>> GetTablesAndColumnsByConnectionString(string connectionString)
        {
            var listObject = new Dictionary<string, List<string>>();
            using (var conn = new SqlConnection(connectionString))
            {                
                try
                {
                    await conn.OpenAsync();
                    DataTable schema = conn.GetSchema("Tables");
                    var sourceTableRows = conn.GetSchema("Columns");

                    var list = new List<string>();
                    foreach (DataRow row in schema.Rows)
                    {
                        var columns= row.Table.Rows;
                        var table = row[2].ToString();

                        if (!list.Contains(table))
                        {
                            var rowList = new List<string>();
                            foreach (DataRow column in sourceTableRows.Rows) 
                            {
                                if (table == column[2].ToString())
                                    if(!list.Contains(column[3].ToString()))
                                        rowList.Add(column[3].ToString());

                            }
                            listObject.Add(table,rowList);
                            list.Add(table);
                        }
                    }
                    return listObject;
                }
                catch
                {
                }
            }
            return null;
        }

        public async Task<DatabaseEnvironmentBO> UpdateAsync(DatabaseEnvironmentBO model)
        {
            var map = await this.GetTablesAndColumnsByConnectionString(model.GetConnectionString());
            if(map != null)
                model.MapTables = JsonConvert.SerializeObject(map, Formatting.Indented);

            var environment = await this._databaseEnvironmentQueries.GetAsync(model.Id);

            environment.Name = model.Name;
            environment.User = model.User;
            environment.DataSource = model.DataSource;            
            environment.ExtraData = model.ExtraData;
            environment.Database = model.Database;
            environment.SecurityIntegrated = model.SecurityIntegrated;
            environment.MapTables= model.MapTables;
            environment.Version = model.Version;
            environment.TimeoutInMinutes = model.TimeoutInMinutes;

            if (environment.SecurityIntegrated != null && environment.SecurityIntegrated.Value)
            {
                environment.User = string.Empty;
                environment.Password = string.Empty;
            }
            else if (!string.IsNullOrEmpty(model.Password)) {
                environment.Password = model.Password;
            }

            var result = await this._databaseEnvironmentCommands.UpdateAsync(environment);
            return MapEntityToModel(result);
        }

        public async Task<GenericResponse> TestConnectionAsync(string connectionString)
        {
            var genericResponse = new GenericResponse();
            try
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    await conn.OpenAsync();
                    genericResponse.IsValid = true;
                }
            }
            catch (Exception ex)
            {
                genericResponse.IsValid = false;
                genericResponse.Message = ex.Message;
            }
            return genericResponse;
        }

        public async Task<string> TestConnectionByIdAsync(Guid id)
        {
            string result = string.Empty;
            try
            {
                var existEnvironment = MapEntityToModel(await _databaseEnvironmentQueries.GetAsync(id));
                string sqlConnection = existEnvironment.GetConnectionString();
                if (!sqlConnection.ToLower().Contains("timeout"))
                    sqlConnection += " Connection Timeout=10";

                using (var conn = new SqlConnection(sqlConnection))
                {
                    await conn.OpenAsync();
                    await conn.CloseAsync();
                    await conn.DisposeAsync();
                }
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }

        public async Task<List<DatabaseEnvironmentBO>> GetAsync()
        {
            var environments= await this._databaseEnvironmentQueries.GetAsync();
            if (environments != null) 
                return environments
                    .Select(rec => MapEntityToModel(rec))
                    .ToList();

            return null;
        }

        public async Task<DatabaseEnvironmentBO> GetAsync(Guid id)
        {
            var environment = await this._databaseEnvironmentQueries
                .GetAsync(id);

            if (environment != null)
                return MapEntityToModel(environment);

            return null;
        }

        public async Task<DatabaseEnvironmentBO> DuplicateAsync(Guid databaseEnvironmentId)
        {
            var model = await this.GetAsync(databaseEnvironmentId);
            var databaseEnvironment = new DatabaseEnvironment()
            {
                User = model.User,
                Password = model.Password,
                Database = model.Database,
                DataSource = model.DataSource,
                ExtraData = model.ExtraData,
                Name = string.Format("{0}_Dup", model.Name),
                Version = model.Version,
                MapTables = model.MapTables,
                MaxNumberResults = model.MaxNumberResults,
                SecurityIntegrated=model.SecurityIntegrated,
                TimeoutInMinutes=model.TimeoutInMinutes
            };

            var result = await this._databaseEnvironmentCommands.AddAsync(databaseEnvironment);
            if (result != null)
            {
                var listParams = await _userParamsService.GetByEnvironmentIdAsync(databaseEnvironmentId);
                if (listParams != null)
                {
                    listParams.ForEach(rec => {
                        rec.Id = Guid.Empty;
                        rec.DatabaseEnvironmentId = result.Id;
                    });
                    var newListParams = await _userParamsService.UpdateListAsync(listParams, result.Id);
                }
            }

            return MapEntityToModel(result);
        }

        private DatabaseEnvironment MapModelToEntity(DatabaseEnvironmentBO model)
        {
            DatabaseEnvironment entity = new DatabaseEnvironment
            {
                CreatedDate = model.CreatedDate,
                Database = model.Database,
                DataSource = model.DataSource,
                ExtraData = model.ExtraData,
                Id = model.Id,
                MapTables = model.MapTables,
                Name = model.Name,
                Password = model.Password,
                SecurityIntegrated = model.SecurityIntegrated == null ? false : model.SecurityIntegrated.Value,
                User = model.User,
                Version = model.Version,
                MaxNumberResults=model.MaxNumberResults,
                TimeoutInMinutes = model.TimeoutInMinutes
            };

            if (model.UserParams != null)
                entity.UserParams = model.UserParams.Select(rec => new Persistence.UserParams.UserParam
                {
                    DatabaseEnvironmentId = rec.DatabaseEnvironmentId,
                    Id = rec.Id,
                    Name = rec.Name,
                    Value = rec.Value
                }).ToList();

            return entity;
        }

        private DatabaseEnvironmentBO MapEntityToModel(DatabaseEnvironment entity)
        {
            DatabaseEnvironmentBO model = new DatabaseEnvironmentBO
            {
                CreatedDate = entity.CreatedDate,
                Database = entity.Database,
                DataSource = entity.DataSource,
                ExtraData = entity.ExtraData,
                Id = entity.Id,
                MapTables = entity.MapTables,
                Name = entity.Name,
                Password = entity.Password,
                SecurityIntegrated = entity.SecurityIntegrated == null ? false : entity.SecurityIntegrated.Value,
                User = entity.User,
                Version = entity.Version,
                MaxNumberResults=entity.MaxNumberResults,
                TimeoutInMinutes = entity.TimeoutInMinutes
            };

            if (entity.UserParams != null)
                model.UserParams = entity.UserParams.Select(rec => new UserParamBO
                {
                    DatabaseEnvironmentId = rec.DatabaseEnvironmentId,
                    Id = rec.Id,
                    Name = rec.Name,
                    Value = rec.Value
                }).ToList();

            return model;
        }
    }
}
