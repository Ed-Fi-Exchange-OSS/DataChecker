// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MSDF.DataChecker.Persistence.EntityFramework;
using System.Data;
using System.Threading.Tasks;

namespace MSDF.DataChecker.Persistence.RuleExecutionLogDetails
{
    public interface IRuleExecutionLogDetailCommands
    {
        Task ExecuteSqlBulkCopy(DataTable table, string tableName);
        Task ExecuteSqlAsync(string sqlScript);
    }

    public class RuleExecutionLogDetailCommands : IRuleExecutionLogDetailCommands
    {
        private readonly DatabaseContext _db;
        public RuleExecutionLogDetailCommands(DatabaseContext db)
        {
            _db = db;
        }

        public async Task ExecuteSqlAsync(string sqlScript)
        {
            string connectionString = _db.Database.GetDbConnection().ConnectionString;
            //using (SqlConnection destinationConnection = new SqlConnection(connectionString))
            //{
            //    await destinationConnection.OpenAsync();
            //    using (var sqlCommand = new SqlCommand(sqlScript, destinationConnection))
            //    {
            //        await sqlCommand.ExecuteNonQueryAsync();
            //    }
            //}


            //New code to use EntityFrmaework
            _db.Database.ExecuteSqlRaw(sqlScript);
        }

        public async Task ExecuteSqlBulkCopy(DataTable table, string tableName)
        {
            string connectionString = _db.Database.GetDbConnection().ConnectionString;
            using (SqlConnection destinationConnection = new SqlConnection(connectionString))
            {
                await destinationConnection.OpenAsync();
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(destinationConnection))
                {
                    bulkCopy.DestinationTableName = tableName;
                    await bulkCopy.WriteToServerAsync(table);
                }
            }
        }
    }
}
