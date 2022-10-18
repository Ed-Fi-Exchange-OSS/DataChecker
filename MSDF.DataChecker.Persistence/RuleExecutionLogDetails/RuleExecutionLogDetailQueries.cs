// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MSDF.DataChecker.Persistence.EntityFramework;
using MSDF.DataChecker.Persistence.Providers;
using MSDF.DataChecker.Persistence.Settings;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
namespace MSDF.DataChecker.Persistence.RuleExecutionLogDetails
{
    public interface IRuleExecutionLogDetailQueries
    {
        Task<DataTable> GetByRuleExecutionLogIdAsync(int id, string tableName);
        Task<List<DestinationTableColumn>> GetColumnsByTableAsync(string tableName, string tableSchema);
        Task<bool> ExistExportTableFromRuleExecutionLogAsync(string tableName, string tableSchema);
    }

    public class RuleExecutionLogDetailQueries : IRuleExecutionLogDetailQueries
    {
        private readonly DatabaseContext _db;
        private readonly DataBaseSettings _appSettings;
        private readonly IDataProvider _dataProvider;
        private string connectionString = "";
        public RuleExecutionLogDetailQueries(DatabaseContext db, IOptionsSnapshot<DataBaseSettings> appSettings, IDataProvider dataProvider)
        {
            _db = db;
            _appSettings = appSettings.Value;
            _dataProvider = dataProvider;
            if (_appSettings.Engine == "Postgres")
            {
                connectionString = _db.Database.GetDbConnection().ConnectionString;
                connectionString = Utility.ParseConnectionString(connectionString, _appSettings.Engine);
            }
            else
                connectionString = _db.Database.GetDbConnection().ConnectionString;
        }

        public async Task<DataTable> GetByRuleExecutionLogIdAsync(int id, string tableName)
        {
            //string connectionString = _db.Database.GetDbConnection().ConnectionString;
            string sql = _appSettings.Engine == "Postgres"? string.Format("SELECT * FROM destination.\"{0}\" WHERE \"RuleExecutionLogId\" = @Id ORDER BY \"Id\"", tableName) :
                 string.Format(
                 "SELECT * " +
                 "FROM destination.{0} " +
                 "WHERE RuleExecutionLogId = @Id " +
                 "ORDER BY Id", tableName)  ;
            var parameters = new Dictionary<string, string>();
            parameters.Add("@Id", id.ToString());
            var dataReader = _dataProvider.ExecuteReader(_db, sql, parameters);
            return dataReader;


            //using (SqlConnection destinationConnection = new SqlConnection(connectionString))
            //{
            //    await destinationConnection.OpenAsync();
            //    string sql = string.Format(
            //        "SELECT * " +
            //        "FROM destination.{0} " +
            //        "WHERE RuleExecutionLogId = @Id " +
            //        "ORDER BY Id", tableName);

            //    using (var sqlCommand = new SqlCommand(sql, destinationConnection))
            //    {
            //        sqlCommand.Parameters.AddWithValue("@Id", id);
            //        var reader = await sqlCommand.ExecuteReaderAsync();
            //        if (reader.HasRows)
            //        {
            //            DataTable dt = new DataTable();
            //            dt.Load(reader);
            //            return dt;
            //        }
            //    }
            //}

        }

        public async Task<List<DestinationTableColumn>> GetColumnsByTableAsync(string tableName, string tableSchema)
        {

            var columns = new List<DestinationTableColumn>();
            //string sql = string.Format(
            //      "SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE " +
            //      "FROM INFORMATION_SCHEMA.COLUMNS " +
            //      "WHERE TABLE_NAME = @tablename AND TABLE_SCHEMA = @tableschema " +
            //      "ORDER BY ORDINAL_POSITION");

            string sql = string.Format(
                "SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE " +
                "FROM INFORMATION_SCHEMA.COLUMNS " +
                "WHERE  TABLE_NAME = @tablename AND TABLE_SCHEMA = @tableschema " +
                "ORDER BY ORDINAL_POSITION", tableName, tableSchema);
            var parameters = new Dictionary<string, string>();
            parameters.Add("@tablename", tableName);
            parameters.Add("@tableschema", tableSchema);
            var dataReader = _dataProvider.ExecuteReader(_db, sql, parameters);
            foreach (DataRow row in dataReader.Rows)
            {
                columns.Add(new DestinationTableColumn
                {
                    Name = row[0].ToString().ToLower(),
                    Type = row[1].ToString().ToLower(),
                    IsNullable = row[2].ToString().ToLower() == "no" ? false : true
                });
            }

            //List<DestinationTableColumn> columns = new List<DestinationTableColumn>();


            //using (SqlConnection destinationConnection = new SqlConnection(connectionString))
            //{
            //    await destinationConnection.OpenAsync();
            //    string sql = string.Format(
            //        "SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE " +
            //        "FROM INFORMATION_SCHEMA.COLUMNS " +
            //        "WHERE TABLE_NAME = @tablename AND TABLE_SCHEMA = @tableschema " +
            //        "ORDER BY ORDINAL_POSITION");

            //    using (var sqlCommand = new SqlCommand(sql, destinationConnection))
            //    {
            //        sqlCommand.Parameters.AddWithValue("@tablename", tableName);
            //        sqlCommand.Parameters.AddWithValue("@tableschema", tableSchema);
            //        var reader = await sqlCommand.ExecuteReaderAsync();
            //        while (reader.Read())
            //        {
            //            columns.Add(new DestinationTableColumn { 
            //                Name = reader.GetValue(0).ToString().ToLower(),
            //                Type = reader.GetValue(1).ToString().ToLower(),
            //                IsNullable = reader.GetValue(2).ToString().ToLower() == "no" ? false : true
            //            });
            //        }
            //    }
            //}

            return columns;
        }

        public async Task<bool> ExistExportTableFromRuleExecutionLogAsync(string tableName, string tableSchema)
        {
            bool existTable = false;
            string sql = string.Format(
                          "SELECT COUNT(*) " +
                          "FROM INFORMATION_SCHEMA.TABLES " +
                          "WHERE TABLE_NAME = @tablename AND TABLE_SCHEMA = @tableschema ");
            var parameters = new Dictionary<string, string>();
            parameters.Add("@tablename", tableName);
            parameters.Add("@tableschema", tableSchema);
            int result = _dataProvider.ExecuteScalar(_db, sql, parameters);
            existTable = result > 0;


            ////string connectionString = _db.Database.GetDbConnection().ConnectionString;
            //using (SqlConnection destinationConnection = new SqlConnection(connectionString))
            //{
            //    await destinationConnection.OpenAsync();
            //    string sql = string.Format(
            //        "SELECT COUNT(*) " +
            //        "FROM INFORMATION_SCHEMA.TABLES " +
            //        "WHERE TABLE_NAME = @tablename AND TABLE_SCHEMA = @tableschema ");

            //    using (var sqlCommand = new SqlCommand(sql, destinationConnection))
            //    {
            //        sqlCommand.Parameters.AddWithValue("@tablename", tableName);
            //        sqlCommand.Parameters.AddWithValue("@tableschema", tableSchema);
            //        int result = (int)(await sqlCommand.ExecuteScalarAsync());
            //        existTable = result > 0;
            //    }
            //}

            return existTable;
        }
    }
}
