using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Speet.Models
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
            //Necessary for passing options to base class
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            var connectionStringBuilder = new SqliteConnectionStringBuilder { DataSource = "./Database/SpeetDB.db" };
            var connectionString = connectionStringBuilder.ToString();
            var connection = new SqliteConnection(connectionString);

            options.UseSqlite(connection);
        }

        public DbSet<SportGroup> SportGroup { get; set; }
    }
}
