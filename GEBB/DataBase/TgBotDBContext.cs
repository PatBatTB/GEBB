﻿using Com.Github.PatBatTB.GEBB.DataBase.Event;
using Com.Github.PatBatTB.GEBB.DataBase.User;
using Com.GitHub.PatBatTB.GEBB.Domain;
using Microsoft.EntityFrameworkCore;

namespace Com.Github.PatBatTB.GEBB.DataBase;

public partial class TgBotDbContext : DbContext
{
    public virtual DbSet<EventEntity> Events { get; set; }
    public virtual DbSet<UserEntity> Users { get; set; }
    public virtual DbSet<BuildEventEntity> TempEvents { get; set; }

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
            entity.HasOne(d => d.Creator).WithMany(p => p.TempEvents).HasForeignKey(d => d.CreatorId);
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
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}