using Hangfire;
using Hangfire.PostgreSql;
using Hangfire.SqlServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MSDF.DataChecker.Persistence.EntityFramework;
using Npgsql;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace MSDF.DataChecker.Persistence.Providers
{
    public interface IDbAccessProvider
    {
        public void SQLServer(IServiceCollection configuration, string ConnectionStrings);
        public void PostgresSQL(IServiceCollection configuration, string ConnectionStrings);
        public string TestPostgresConnection(string ConnectionStrings);
        public  Task<string> TestSqlServerConnection(string ConnectionStrings);

    }
    public class DbAccessProvider : IDbAccessProvider
    {
        public void PostgresSQL(IServiceCollection configuration, string ConnectionStrings)
        {
            configuration.AddDbContext<DatabaseContext>(opt => opt.UseNpgsql(ConnectionStrings, x => x.MigrationsAssembly("MSDF.DataChecker.Migrations.Postgres")));
            //configuration.AddDbContext<DatabaseContext>(opt => opt.UseNpgsql(ConnectionStrings));
            configuration.AddHangfire(globalConfiguration => globalConfiguration
                 .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                 .UseSimpleAssemblyNameTypeSerializer()
                 .UseRecommendedSerializerSettings()
                 .UsePostgreSqlStorage(ConnectionStrings, new PostgreSqlStorageOptions
                 {
                     InvisibilityTimeout = TimeSpan.FromMinutes(5),
                     
                 }));
            configuration.AddHangfireServer();
        }

        public void SQLServer(IServiceCollection configuration, string ConnectionStrings)

        {

            configuration.AddDbContext<DatabaseContext>(options => options.UseSqlServer(ConnectionStrings, x => x.MigrationsAssembly("MSDF.DataChecker.Migrations.SqlServer")) ,ServiceLifetime.Transient);
           //configuration.AddDbContext<DatabaseContext>(options => options.UseSqlServer(ConnectionStrings), ServiceLifetime.Transient);
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


        public  string TestPostgresConnection(string ConnectionStrings)
        {
            string result = string.Empty;
            try
            {
                using (var conn = new NpgsqlConnection(ConnectionStrings))
                {
                    conn.Open();
                }
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }

        public async Task<string> TestSqlServerConnection(string ConnectionStrings)
        {
            string result = string.Empty;
            try
            {
                using (var conn = new SqlConnection(ConnectionStrings))
                {
                  await  conn.OpenAsync();
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
    }
}
