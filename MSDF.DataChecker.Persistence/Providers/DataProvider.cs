using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MSDF.DataChecker.Persistence.EntityFramework;
using MSDF.DataChecker.Persistence.Settings;
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

        DataTable ExecuteReader(DatabaseContext _db, string sqlToRun, Dictionary<string, string> parameters = null);
        int ExecuteScalar(DatabaseContext _db, string sqlToRun, Dictionary<string, string> parameters = null);

        Task<NpgsqlDataReader> ExecutePostgresReaderAsync(string connectionString, string sqlToRun, Dictionary<string, string> parameters = null);
        NpgsqlDataReader ExecutePostgresReader(string connectionString, string sqlToRun = "", Dictionary<string, string> parameters = null);
        int ExecutePostgresScalarAsync(string connectionString, string sqlToRun = "", Dictionary<string, string> parameters = null, int? timeout = null);
        Task<SqlDataReader> ExecuteSqlServerReaderAsync(string connectionString, string sqlToRun = "", Dictionary<string, string> parameters = null, int? timeout = null);
        int ExecuteSqlServerScalarAsync(string connectionString, string sqlToRun = "", Dictionary<string, string> parameters = null, int? timeout = null);

    }
    public class DataProvider : IDataProvider
    {
        public IDataProvider _dataProvider;
        public IDataProvider ProviderData { get { return _dataProvider; } }
        private RuleExecutionContext _dbtoConect;
        private readonly DataBaseSettings _appSettings;
        private string _connectionType = "Postgres";
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

        public DataProvider(DatabaseContext dbCurrent, IOptionsSnapshot<DataBaseSettings> appSettings)
        {
            _connectionType = appSettings.Value.Engine;
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

        public Task<NpgsqlDataReader> ExecutePostgresReaderAsync(string connectionString, string sqlToRun = "", Dictionary<string, string> parameters = null)
        {
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.OpenAsync();
                using (var cmd = new NpgsqlCommand(sqlToRun, conn))
                {
                    if (parameters != null)
                        cmd.AddDbCommandParameters(_connectionType, parameters);
                    var reader=  cmd.ExecuteReaderAsync();
                    return   reader;
                }
            }

        }

        public NpgsqlDataReader ExecutePostgresReader(string connectionString, string sqlToRun = "", Dictionary<string, string> parameters = null)
        {
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.OpenAsync();
                using (var cmd = new NpgsqlCommand(sqlToRun, conn))
                {
                    if (parameters != null)
                        cmd.AddDbCommandParameters(_connectionType, parameters);
                    var reader = cmd.ExecuteReader();
                    return reader;
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
                    cmd.AddDbCommandParameters(_connectionType, parameters);
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
                    cmd.AddDbCommandParameters(_connectionType, parameters);
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
                    cmd.AddDbCommandParameters(_connectionType, parameters);
                    rows = Convert.ToInt32(cmd.ExecuteScalar());
                }
                conn.Close();
            }

            return rows;
        }

        public DataTable ExecuteReader(DatabaseContext _db, string sqlToRun, Dictionary<string, string> parameters = null)
        {
            var dt = new DataTable();
            using (var command = _db.Database.GetDbConnection().CreateCommand())
            {
                _db.Database.OpenConnection();
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

        public int ExecuteScalar(DatabaseContext _db, string sqlToRun, Dictionary<string, string> parameters = null)
        {
            var rows = 0;
            using (var command = _db.Database.GetDbConnection().CreateCommand())
            {
                _db.Database.OpenConnection();
                command.CommandText = sqlToRun;
                command.CommandTimeout = 120;
                if (parameters != null)
                    command.AddDbCommandParameters(_connectionType, parameters);
                rows = Convert.ToInt32(command.ExecuteScalar());
                command.Connection.Close();
            }
            return rows;
        }
    }
}
