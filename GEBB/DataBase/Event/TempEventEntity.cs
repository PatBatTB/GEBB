using System.ComponentModel.DataAnnotations;
using Com.Github.PatBatTB.GEBB.DataBase.User;
using Com.Github.PatBatTB.GEBB.Domain.Enums;

namespace Com.Github.PatBatTB.GEBB.DataBase.Event;

public sealed class TempEventEntity
{
    public int EventId { get; set; }

    public long CreatorId { get; set; }
    public UserEntity? Creator { get; set; }

    public int MessageId { get; set; }

    [StringLength(100)]
    public string? Title { get; set; }

    public DateTime? DateTimeOf { get; set; }

    public DateTime CreatedAt { get; set; }

    [StringLength(100)]
    public string? Address { get; set; }

    public int? ParticipantLimit { get; set; }

    public int? Cost { get; set; }

    [StringLength(250)]
    public string? Description { get; set; }

    public EventStatus Status { get; set; }
}