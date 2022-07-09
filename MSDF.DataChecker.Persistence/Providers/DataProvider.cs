using Microsoft.EntityFrameworkCore;
using MSDF.DataChecker.Persistence.EntityFramework;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace MSDF.DataChecker.Persistence.Providers
{
    public interface IDataProvider
    {
        string ConnectionString { get; set; }
        IDataProvider ProviderData { get; }
        DataTable ExecuteReader(string newConection, string sqlToRun, Dictionary<string, string> parameters = null);
        int ExecuteScalar(string newConection, string sqlToRun, Dictionary<string, string> parameters = null);
        Task<NpgsqlDataReader> ExecutePostgresReaderAsync(string connectionString, string sqlToRun, Dictionary<string, string> parameters = null, int? timeout = null);
        int ExecutePostgresScalarAsync(string connectionString, string sqlToRun = "", Dictionary<string, string> parameters = null, int? timeout = null);
        Task<SqlDataReader> ExecuteSqlServerReaderAsync(string connectionString, string sqlToRun = "", Dictionary<string, string> parameters = null, int? timeout = null);
        int ExecuteSqlServerScalarAsync(string connectionString, string sqlToRun = "", Dictionary<string, string> parameters = null, int? timeout = null);

    }
    public class DataProvider : IDataProvider
    {
        public IDataProvider _dataProvider;
        public IDataProvider ProviderData { get { return _dataProvider; } }
        private readonly DatabaseContext _dbCurrent;
        private RuleExecutionContext _dbtoConect;
        private string _connectionType = "NpgsqlConnection";
        private string _connectionString; // field
        public string ConnectionString   // property
        {
            get
            {
                return _dbtoConect.GetConnectionString();
            }
            set
            {
                _connectionString = value;
                _dbtoConect = new RuleExecutionContext(value, _connectionType, new DbContextOptions<RuleExecutionContext>());
            }
        }

        public DataProvider(DatabaseContext dbCurrent)
        {
            //_dbCurrent = dbCurrent;
            //_dbtoConect = new RuleExecutionContext("", "", new DbContextOptions<RuleExecutionContext>());
            //_connectionType = dbCurrent.Database.GetDbConnection().GetType().Name;
        }
        public int ExecuteScalar(string newConection, string sqlToRun, Dictionary<string, string> parameters = null)
        {
            var rows = 0;
            using (var command = _dbtoConect.Database.GetDbConnection().CreateCommand())
            {
                _dbtoConect.Database.OpenConnection();
                command.CommandText = sqlToRun;
                command.CommandTimeout = 120;
                if (parameters != null)
                    command.AddDbCommandParameters(_connectionType, parameters);
                rows = Convert.ToInt32(command.ExecuteScalar());
                command.Connection.Close();
            }
            return rows;
        }


        public DataTable ExecuteReader(string newConection, string sqlToRun, Dictionary<string, string> parameters = null)
        {
            var dt = new DataTable();
            using (var command = _dbtoConect.Database.GetDbConnection().CreateCommand())
            {
                _dbtoConect.Database.OpenConnection();
                command.CommandTimeout = 120;
                command.CommandText = sqlToRun;
                if (parameters != null)
                    command.AddDbCommandParameters(_connectionType, parameters);
                using (var dr = command.ExecuteReader())
                {
                    if (dr.HasRows)
                    {
                        dt.Load(dr);
                    }
                    command.Connection.Close();
                }
            }
            return dt;
        }

        public Task<NpgsqlDataReader> ExecutePostgresReaderAsync(string connectionString, string sqlToRun = "", Dictionary<string, string> parameters = null, int? timeout = null)
        {
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.OpenAsync();
                using (var cmd = new NpgsqlCommand(sqlToRun, conn))
                {
                    if (timeout != null)
                        cmd.CommandTimeout = (timeout.Value * 60);
                    AddParameters(sqlToRun, cmd, parameters);
                    return cmd.ExecuteReaderAsync();
                }
            }

        }

        public int ExecutePostgresScalarAsync(string connectionString, string sqlToRun = "", Dictionary<string, string> parameters = null, int? timeout = null)
        {
            var rows = 0;
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(sqlToRun, conn))
                {
                    if (timeout != null)
                        cmd.CommandTimeout = (timeout.Value * 60);
                    AddParameters(sqlToRun, cmd, parameters);
                    rows = Convert.ToInt32(cmd.ExecuteScalar());
                }
                conn.Close();
            }
            return rows;
        }


        public Task<SqlDataReader> ExecuteSqlServerReaderAsync(string connectionString, string sqlToRun = "", Dictionary<string, string> parameters = null, int? timeout = null)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.OpenAsync();
                using (var cmd = new SqlCommand(sqlToRun, conn))
                {
                    if (timeout != null)
                        cmd.CommandTimeout = (timeout.Value * 60);
                    AddParameters(sqlToRun, cmd, parameters);
                    return cmd.ExecuteReaderAsync();
                }
            }
        }


        public int ExecuteSqlServerScalarAsync(string connectionString, string sqlToRun = "", Dictionary<string, string> parameters = null, int? timeout = null)
        {
            var rows = 0;
            using (var conn = new SqlConnection(connectionString))
            {
                conn.OpenAsync();
                using (var cmd = new SqlCommand(sqlToRun, conn))
                {
                    if (timeout != null)
                        cmd.CommandTimeout = (timeout.Value * 60);
                    AddParameters(sqlToRun, cmd, parameters);
                    rows = Convert.ToInt32(cmd.ExecuteScalar());
                }
                conn.Close();
            }

            return rows;
        }

        private void AddParameters(string sql, NpgsqlCommand sqlCommand, Dictionary<string, string> parameters)
        {
            if (parameters != null)
            {
                foreach (KeyValuePair<string, string> param in parameters)
                {
                    if (sql.Contains("@" + param.Key))
                        sqlCommand.Parameters.AddWithValue("@" + param.Key, param.Value);
                }
            }
        }
        private void AddParameters(string sql, SqlCommand sqlCommand, Dictionary<string, string> parameters)
        {
            if (parameters != null)
            {
                foreach (KeyValuePair<string, string> param in parameters)
                {
                    if (sql.Contains("@" + param.Key))
                        sqlCommand.Parameters.AddWithValue("@" + param.Key, param.Value);
                }
            }
        }


    }
}
