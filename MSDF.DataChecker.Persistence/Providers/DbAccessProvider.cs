using Hangfire;
using Hangfire.PostgreSql;
using Hangfire.SqlServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MSDF.DataChecker.Persistence.EntityFramework;
using Npgsql;
using System;
using System.Data.SqlClient;
namespace MSDF.DataChecker.Persistence.Providers
{
    public interface IDbAccessProvider
    {
        public void SQLServer(IServiceCollection configuration, string ConnectionStrings);
        public void PostgresSQL(IServiceCollection configuration, string ConnectionStrings);
        public bool TestPostgresConnection(string ConnectionStrings);
        public bool TestSqlServerConnection(string ConnectionStrings);

    }
    public class DbAccessProvider : IDbAccessProvider
    {
        public void PostgresSQL(IServiceCollection configuration, string ConnectionStrings)
        {
            configuration.AddEntityFrameworkNpgsql().AddDbContext<DatabaseContext>(opt => opt.UseNpgsql(ConnectionStrings));
            configuration.AddHangfire(globalConfiguration => globalConfiguration
                 .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                 .UseSimpleAssemblyNameTypeSerializer()
                 .UseRecommendedSerializerSettings()
                 .UsePostgreSqlStorage(ConnectionStrings, new PostgreSqlStorageOptions
                 {
                     InvisibilityTimeout = TimeSpan.FromMinutes(5)
                 }));
            configuration.AddHangfireServer();
        }

        public void SQLServer(IServiceCollection configuration, string ConnectionStrings)
        {
            configuration.AddDbContext<DatabaseContext>(options => options.UseSqlServer(ConnectionStrings), ServiceLifetime.Transient);
            configuration.AddHangfire(globalConfiguration => globalConfiguration
                 .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                 .UseSimpleAssemblyNameTypeSerializer()
                 .UseRecommendedSerializerSettings()
                 .UseSqlServerStorage(ConnectionStrings, new SqlServerStorageOptions
                 {
                     CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                     SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                     QueuePollInterval = TimeSpan.Zero,
                     UseRecommendedIsolationLevel = true,
                     DisableGlobalLocks = true
                 }));
            configuration.AddHangfireServer();
        }


        public bool TestPostgresConnection(string ConnectionStrings)
        {
            var success = false;
            try
            {
                using (var conn = new NpgsqlConnection(ConnectionStrings))
                {
                    conn.Open();
                    success = true;
                }
            }
            catch (Exception ex)
            {
                success = false;
            }
            return success;
        }

        public bool TestSqlServerConnection(string ConnectionStrings)
        {
            var success = false;
            try
            {
                using (var conn = new SqlConnection(ConnectionStrings))
                {
                    conn.OpenAsync();
                    conn.CloseAsync();
                    conn.DisposeAsync();
                    success = true;
                }
            }
            catch (Exception ex)
            {
                success = false;
            }
            return success;

        }
    }
}
