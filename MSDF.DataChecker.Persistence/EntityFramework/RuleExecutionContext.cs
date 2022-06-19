using Microsoft.EntityFrameworkCore;
namespace MSDF.DataChecker.Persistence.EntityFramework
{
  public class RuleExecutionContext : DbContext
    {
        private string _connectionString { get; set; }
        private string _engine { get; }

        public RuleExecutionContext(string connectionString, string engine, DbContextOptions<RuleExecutionContext> options) : base(options)
        {
            _engine = engine;
            _connectionString = connectionString;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!string.IsNullOrEmpty(_connectionString))
                if (_engine.ToLower().Contains("npgsqlconnection"))
                {
                    _connectionString = Utility.GetFixedConnectionString(_connectionString);
                    optionsBuilder.UseNpgsql(_connectionString, builder => { });
                }
                else
                    optionsBuilder.UseSqlServer(_connectionString, builder => { });

        }
    }
}
