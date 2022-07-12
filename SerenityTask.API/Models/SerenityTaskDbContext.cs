using System.Linq;
using Microsoft.EntityFrameworkCore;
using SerenityTask.API.DataSeed;

namespace SerenityTask.API.Models.Entities
{
    public class SerenityTaskDbContext : DbContext
    {
        public SerenityTaskDbContext(DbContextOptions<SerenityTaskDbContext> options) : base(options) { }

        public DbSet<Role> Roles { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<UserDetails> UserDetails { get; set; }

        public DbSet<AchievementType> AchievementTypes { get; set; }

        public DbSet<Achievement> Achievements { get; set; }

        public DbSet<TimeZoneType> TimeZoneTypes { get; set; }

        public DbSet<Session> Sessions { get; set; }

        public DbSet<GoogleCredential> GoogleCredentials { get; set; }

        public DbSet<GoogleCalendarAccessRequest> GoogleCalendarAccessRequests { get; set; }

        public DbSet<SessionRequest> SessionRequests { get; set; }

        public DbSet<HubConnection> HubConnections { get; set; }

        public DbSet<UserConnector> UserConnectors { get; set; }

        public DbSet<ConfirmationToken> ConfirmationTokens { get; set; }

        public DbSet<UserNotification> UserNotifications { get; set; }

        public DbSet<SystemNotification> SystemNotifications { get; set; }

        public DbSet<SettingsNotification> SettingsNotifications { get; set; }

        public DbSet<UserSettings> UserSettings { get; set; }

        public DbSet<ProblemReport> ProblemReports { get; set; }

        public DbSet<Task> Tasks { get; set; }

        public DbSet<TaskHistoryNote> TaskHistory { get; set; }

        public DbSet<File> Files { get; set; }

        public DbSet<PlantType> PlantTypes { get; set; }

        public DbSet<Plant> Plants { get; set; }

        public DbSet<PlantHistoryNote> PlantHistory { get; set; }

        public DbSet<Changelog> Changelog { get; set; }

        public DbSet<Quote> Quotes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasOne(x => x.Role)
                .WithMany(x => x.Users);

            modelBuilder.Entity<UserConnector>()
                .HasOne(x => x.User)
                .WithMany(x => x.UserConnectors);

            modelBuilder.Entity<UserDetails>()
                .HasOne(x => x.User)
                .WithOne(x => x.UserDetails);

            modelBuilder.Entity<UserDetails>()
                .HasOne(x => x.TimeZone)
                .WithMany(x => x.UserDetails);

            modelBuilder.Entity<Achievement>()
                .HasOne(x => x.UserDetails)
                .WithMany(x => x.Achievements);

            modelBuilder.Entity<Achievement>()
                .HasOne(x => x.Type)
                .WithMany(x => x.Achievements);

            modelBuilder.Entity<Session>()
                .HasOne(x => x.Owner)
                .WithMany(x => x.OwnedSessions);

            modelBuilder.Entity<Session>()
                .HasMany(x => x.Participants)
                .WithMany(x => x.Sessions);

            modelBuilder.Entity<GoogleCredential>()
                .HasOne(x => x.User)
                .WithOne(x => x.GoogleCredential);

            modelBuilder.Entity<GoogleCalendarAccessRequest>()
                .HasOne(x => x.User)
                .WithOne(x => x.GoogleCalendarAccessRequest);

            modelBuilder.Entity<SessionRequest>()
                .HasOne(x => x.Sender)
                .WithMany(x => x.SendedSessionRequests);

            modelBuilder.Entity<SessionRequest>()
                .HasOne(x => x.Receiver)
                .WithMany(x => x.ReceivedSessionRequests);

            modelBuilder.Entity<Session>()
                .HasOne(x => x.Owner)
                .WithMany(x => x.OwnedSessions);

            modelBuilder.Entity<Session>()
                .HasMany(x => x.Participants)
                .WithMany(x => x.Sessions);

            modelBuilder.Entity<HubConnection>()
                .HasOne(x => x.User)
                .WithMany(x => x.HubConnections);

            modelBuilder.Entity<ConfirmationToken>()
                .HasOne(x => x.User)
                .WithMany(x => x.AccountConfirmationTokens);

            modelBuilder.Entity<UserNotification>()
                .HasOne(x => x.Receiver)
                .WithMany(x => x.UserNotifications);

            modelBuilder.Entity<SystemNotification>()
                .HasOne(x => x.Receiver)
                .WithMany(x => x.SystemNotifications);

            modelBuilder.Entity<SettingsNotification>()
                .HasOne(x => x.User)
                .WithMany(x => x.SettingsNotifications);

            modelBuilder.Entity<UserSettings>()
                .HasOne(x => x.User)
                .WithOne(x => x.UserSettings);

            modelBuilder.Entity<ProblemReport>()
                .HasOne(x => x.User)
                .WithMany(x => x.ProblemReports);

            modelBuilder.Entity<Task>()
                .HasOne(x => x.User)
                .WithMany(x => x.Tasks);

            modelBuilder.Entity<File>()
                .HasOne(x => x.User)
                .WithMany(x => x.Files);

            modelBuilder.Entity<TaskHistoryNote>()
                .HasOne(x => x.Task)
                .WithMany(x => x.History);

            modelBuilder.Entity<File>()
                .HasOne(x => x.Task)
                .WithMany(x => x.Files);

            modelBuilder.Entity<Task>()
                .HasOne(x => x.ParentTask)
                .WithMany(x => x.Subtasks);

            modelBuilder.Entity<Plant>()
                .HasOne(x => x.PlantType)
                .WithMany(x => x.Plants);

            modelBuilder.Entity<Plant>()
                .HasOne(x => x.User)
                .WithMany(x => x.Plants);

            modelBuilder.Entity<PlantHistoryNote>()
                .HasOne(x => x.Plant)
                .WithMany(x => x.PlantHistory);

            modelBuilder.Entity<PlantHistoryNote>()
                .HasOne(x => x.Task)
                .WithMany(x => x.PlantHistory);

            modelBuilder.ApplyConfiguration(new RolesConfiguration());
            modelBuilder.ApplyConfiguration(new UsersConfiguration());
            modelBuilder.ApplyConfiguration(new UserDetailsConfiguration());
            modelBuilder.ApplyConfiguration(new AchievementTypesConfiguration());
            modelBuilder.ApplyConfiguration(new TimeZonesConfiguration());
            modelBuilder.ApplyConfiguration(new UserSettingsConfiguration());
            modelBuilder.ApplyConfiguration(new QuotesConfiguration());
            modelBuilder.ApplyConfiguration(new PlantTypesConfiguration());

            foreach (var foreignKey in modelBuilder.Model.GetEntityTypes().SelectMany(x => x.GetForeignKeys()))
            {
                foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }
    }
}
