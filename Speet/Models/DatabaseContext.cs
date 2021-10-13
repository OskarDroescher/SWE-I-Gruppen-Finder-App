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

            //Configure GenderType enum to string conversation in database
            modelBuilder.Entity<User>()
                        .Property(u => u.Gender)
                        .HasConversion<string>();

            //Configure ActivityCategoryType enum to string conversation in database
            modelBuilder.Entity<ActivityTag>()
                        .Property(at => at.ActivityCategory)
                        .HasConversion<string>();

            //Configure GenderRestrictionType enum to string conversation in database
            modelBuilder.Entity<GenderRestrictionTag>()
                        .Property(grt => grt.GenderRestriction)
                        .HasConversion<string>();

            //Configure n to m SportGroup-User relation and join table name
            modelBuilder.Entity<SportGroup>()
                        .HasMany<User>(sg => sg.Participants)
                        .WithMany(u => u.JoinedGroups)
                        .UsingEntity(j => j.ToTable("Joins"));

            //Configure n to m SportGroup-ActivityTag relation and join table name
            modelBuilder.Entity<SportGroup>()
                        .HasMany<ActivityTag>(sg => sg.ActivityTags)
                        .WithMany(at => at.AssignedGroups)
                        .UsingEntity(j => j.ToTable("Assigned"));

            //Configure n to 1 SportGroup-GenderRestrictionTag relation and join table name
            modelBuilder.Entity<SportGroup>()
                        .HasOne<GenderRestrictionTag>(sg => sg.GenderRestrictionTag)
                        .WithMany(grt => grt.AssignedGroups);

            //Configure n to 1 SportGroup-User relation
            modelBuilder.Entity<SportGroup>()
                        .HasOne<User>(sg => sg.CreatedBy)
                        .WithMany(u => u.CreatedGroups)
                        .IsRequired();
        }

        public DbSet<SportGroup> SportGroup { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<ActivityTag> ActivityTag { get; set; }
        public DbSet<GenderRestrictionTag> GenderRestrictionTag { get; set; }
    }
}
