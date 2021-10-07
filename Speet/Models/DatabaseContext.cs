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

            options.UseLazyLoadingProxies()
                   .UseSqlite(connection);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //Configure n to m SportGroup-User relation and join table name
            modelBuilder.Entity<SportGroup>()
                        .HasMany<User>(sg => sg.Participants)
                        .WithMany(u => u.JoinedGroups)
                        .UsingEntity(j => j.ToTable("Joins"));

            //Configure n to m SportGroup-User relation and join table name
            modelBuilder.Entity<SportGroup>()
                        .HasMany<Tag>(sg => sg.Tags)
                        .WithMany(t => t.AssignedGroups)
                        .UsingEntity(j => j.ToTable("Assigned"));

            //Configure n to 1 SportGroup-User relation
            modelBuilder.Entity<SportGroup>()
                        .HasOne<User>(sg => sg.CreatedBy)
                        .WithMany(u => u.CreatedGroups)
                        .IsRequired();
        }

        public DbSet<SportGroup> SportGroup { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<Tag> Tag { get; set; }
    }
}
