using System.ComponentModel.DataAnnotations;
using Com.Github.PatBatTB.GEBB.DataBase.Message;
using Com.Github.PatBatTB.GEBB.DataBase.User;
using Com.Github.PatBatTB.GEBB.Domain.Enums;

namespace Com.Github.PatBatTB.GEBB.DataBase.Event;

public sealed class EventEntity
{
    public int EventId { get; set; }

    public long CreatorId { get; set; }
    public UserEntity? Creator { get; set; }

    [StringLength(100)]
    public string? Title { get; set; }

    public DateTime? DateTimeOf { get; set; }

    public DateTime CreatedAt { get; set; }

    [StringLength(100)]
    public string? Address { get; set; }

    public int? ParticipantLimit { get; set; }

    public int? Cost { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }

    public EventStatus Status { get; set; }

    public ICollection<UserEntity> RegisteredUsers { get; set; } = new List<UserEntity>();

    public ICollection<EventMessageEntity> EventMessages { get; set; } = new List<EventMessageEntity>();
}