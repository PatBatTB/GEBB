using Com.Github.PatBatTB.GEBB.DataBase.Alarm;
using Com.Github.PatBatTB.GEBB.DataBase.Event;
using Com.Github.PatBatTB.GEBB.DataBase.User;
using Com.GitHub.PatBatTB.GEBB.Domain;
using Microsoft.EntityFrameworkCore;

namespace Com.Github.PatBatTB.GEBB.DataBase;

public partial class TgBotDbContext : DbContext
{
    public virtual DbSet<EventEntity> Events { get; set; }
    public virtual DbSet<UserEntity> Users { get; set; }
    public virtual DbSet<BuildEventEntity> TempEvents { get; set; }
    public virtual DbSet<AlarmSettingsEntity> AlarmSettings { get; set; }
    public virtual DbSet<AlarmEntity> Alarms { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlite(AppSettings.DbConnString);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EventEntity>(entity =>
        {
            entity.Property(e => e.EventId).HasColumnType("INTEGER");
            entity.Property(e => e.CreatorId).HasColumnType("BIGINT");
            entity.Property(e => e.Address).HasColumnType("varchar");
            entity.Property(e => e.Cost).HasColumnType("INTEGER");
            entity.Property(e => e.Description).HasColumnType("varchar");
            entity.Property(e => e.ParticipantLimit).HasColumnType("INTEGER");
            entity.Property(e => e.Title).HasColumnType("varchar");
            entity.Property(e => e.Status).HasColumnType("INTEGER");

            entity.HasKey(e => new { e.EventId, e.CreatorId });
            entity.HasOne(d => d.Creator).WithMany(p => p.Events).HasForeignKey(d => d.CreatorId);
        });

        modelBuilder.Entity<BuildEventEntity>(entity =>
        {
            entity.Property(e => e.EventId).HasColumnType("INTEGER");
            entity.Property(e => e.CreatorId).HasColumnType("BIGINT");
            entity.Property(e => e.MessageId).HasColumnType("INTEGER");
            entity.Property(e => e.Address).HasColumnType("varchar");
            entity.Property(e => e.Cost).HasColumnType("INTEGER");
            entity.Property(e => e.Description).HasColumnType("varchar");
            entity.Property(e => e.ParticipantLimit).HasColumnType("INTEGER");
            entity.Property(e => e.Title).HasColumnType("varchar");
            entity.Property(e => e.Status).HasColumnType("INTEGER");
            entity.HasKey(e => new { e.EventId, e.CreatorId });
            entity.HasOne(d => d.Creator).WithMany(p => p.TempEvents).HasForeignKey(d => d.CreatorId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<UserEntity>(entity =>
        {
            entity.Property(e => e.UserId)
                .ValueGeneratedNever()
                .HasColumnType("BIGINT");
            entity.Property(e => e.Status).HasColumnType("INTEGER");
            entity.Property(e => e.RegisteredAt).HasColumnType("timestamp");
            entity.Property(e => e.Username).HasColumnType("varchar");

            entity.HasKey(e => e.UserId);

            entity.HasMany(d => d.EventsNavigation).WithMany(p => p.RegisteredUsers)
                .UsingEntity<Dictionary<string, object>>(
                    "Registrations",
                    r => r.HasOne<EventEntity>().WithMany()
                        .HasForeignKey("EventId", "CreatorId")
                        .OnDelete(DeleteBehavior.Cascade),
                    l => l.HasOne<UserEntity>().WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade),
                    j =>
                    {
                        j.HasKey("UserId", "EventId", "CreatorId");
                        j.ToTable("Registrations");
                        j.IndexerProperty<long>("UserId").HasColumnType("BIGINT");
                        j.IndexerProperty<int>("EventId").HasColumnType("INTEGER");
                        j.IndexerProperty<long>("CreatorId").HasColumnType("BIGINT");
                    });

            entity.HasOne(d => d.AlarmSettings).WithOne(p => p.User).HasForeignKey<AlarmSettingsEntity>(d => d.UserId);
        });

        modelBuilder.Entity<AlarmSettingsEntity>(entity =>
        {
            entity.Property(e => e.UserId).HasColumnType("BIGINT");
            entity.Property(e => e.ThreeDays).HasColumnType("INTEGER");
            entity.Property(e => e.OneDay).HasColumnType("INTEGER");
            entity.Property(e => e.Hours).HasColumnType("INTEGER");

            entity.HasKey(e => e.UserId);
        });

        modelBuilder.Entity<AlarmEntity>(entity =>
            {
                entity.Property(e => e.UserId).HasColumnType("BIGINT");
                entity.Property(e => e.EventId).HasColumnType("INTEGER");
                entity.Property(e => e.CreatorId).HasColumnType("BIGINT");

                entity.HasKey(e => new { e.UserId, e.EventId, e.CreatorId });
                entity.HasOne<UserEntity>(d => d.User).WithMany(p => p.Alarms)
                    .HasForeignKey(d => d.UserId);
                entity.HasOne<EventEntity>(d => d.Event).WithMany(p => p.Alarms)
                    .HasForeignKey(d => new { d.EventId, d.CreatorId });
            }
        );

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}