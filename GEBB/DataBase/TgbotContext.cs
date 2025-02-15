using Com.Github.PatBatTB.GEBB.DataBase.Entity;
using Com.GitHub.PatBatTB.GEBB.Domain;
using Com.Github.PatBatTB.GEBB.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Com.Github.PatBatTB.GEBB.DataBase;

public partial class TgbotContext : DbContext
{
    public virtual DbSet<Event> Events { get; set; }
    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlite(AppSettings.DbConnString);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Event>(entity =>
        {
            entity.Property(e => e.EventId)
                .ValueGeneratedNever()
                .HasColumnType("INT");
            entity.Property(e => e.Address).HasColumnType("varchar");
            entity.Property(e => e.Cost).HasColumnType("INT");
            entity.Property(e => e.DateTimeOf).HasColumnType("timestamp");
            entity.Property(e => e.Description).HasColumnType("varchar");
            entity.Property(e => e.IsActive).HasColumnType("bool");
            entity.Property(e => e.ParticipantLimit).HasColumnType("INT");
            entity.Property(e => e.Title).HasColumnType("varchar");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.UserId)
                .ValueGeneratedNever()
                .HasColumnType("BIGINT");
            entity.Property(e => e.IsActive).HasColumnType("bool");
            entity.Property(e => e.RegisteredAt).HasColumnType("timestamp");
            entity.Property(e => e.Username).HasColumnType("varchar");

            entity.HasMany(d => d.Events).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "UsersXEvent",
                    r => r.HasOne<Event>().WithMany()
                        .HasForeignKey("EventId")
                        .OnDelete(DeleteBehavior.ClientSetNull),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.ClientSetNull),
                    j =>
                    {
                        j.HasKey("UserId", "EventId");
                        j.ToTable("Users_x_Events");
                        j.IndexerProperty<long>("UserId").HasColumnType("BIGINT");
                        j.IndexerProperty<int>("EventId").HasColumnType("INT");
                    });
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
