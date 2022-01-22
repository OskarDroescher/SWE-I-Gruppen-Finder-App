using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;

namespace Speet.Models
{
    public class DatabaseContext : DbContext
    {
        private static Timer _serverActionsTimer;

        static DatabaseContext()
        {
            SetUpServerActionsTimer();
        }

        private static void SetUpServerActionsTimer()
        {
            int[] dailyTimeParts = ApplicationConstants.DailyServerActionsTime.Split(':').Select(p => int.Parse(p)).ToArray();
            TimeSpan dailyTimeSpan = new TimeSpan(dailyTimeParts[0], dailyTimeParts[1], dailyTimeParts[2]);

            int oneDayPeriod = (int)new TimeSpan(24, 0, 0).TotalMilliseconds;
            int dailyTime = (int)dailyTimeSpan.TotalMilliseconds;
            int currentTime = (int)DateTime.Now.TimeOfDay.TotalMilliseconds;

            int dueTime;
            if (dailyTime > currentTime)
                dueTime = dailyTime - currentTime;
            else
                dueTime = oneDayPeriod - currentTime + dailyTime;

            _serverActionsTimer = new Timer(
                callback: ExecuteServerActions,
                state: null,
                dueTime: dueTime,
                period: oneDayPeriod);
        }

        private static void ExecuteServerActions(object timerState)
        {
            using (DatabaseContext databaseContext = new DatabaseContext())
            {
                UpdateMeetupDates(databaseContext);
                CleanUpExpiredGroups(databaseContext);
                databaseContext.SaveChanges();
            }
        }

        private static void UpdateMeetupDates(DatabaseContext databaseContext)
        {
            foreach (var group in databaseContext.SportGroup)
            {
                if (group.MeetupDate.CompareTo(DateTime.Now) <= 0)
                {
                    switch(group.MeetupRecurrence)
                    {
                        case MeetupRecurrenceType.Daily:
                            {
                                group.MeetupDate = group.MeetupDate.AddDays(1);
                                break;
                            }
                        case MeetupRecurrenceType.Weekly:
                            {
                                group.MeetupDate = group.MeetupDate.AddDays(7);
                                break;
                            }
                        case MeetupRecurrenceType.Monthly:
                            {
                                group.MeetupDate = group.MeetupDate.AddMonths(1);
                                break;
                            }
                    }
                }
            }
        }

        private static void CleanUpExpiredGroups(DatabaseContext databaseContext)
        {
            DateTime expiredDate = DateTime.Now.AddDays(-ApplicationConstants.SportGroupsExpirationDays);

            foreach (var group in databaseContext.SportGroup)
                if (group.MeetupDate.CompareTo(expiredDate) <= 0)
                    databaseContext.SportGroup.Remove(group);
        }

        private DatabaseContext()
        {
            //Necessary for instantiating local database context for ExecuteServerActions()
        }

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
