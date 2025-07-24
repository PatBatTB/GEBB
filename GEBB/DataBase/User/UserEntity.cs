using System.ComponentModel.DataAnnotations;
using Com.Github.PatBatTB.GEBB.DataBase.Alarm;
using Com.Github.PatBatTB.GEBB.DataBase.Event;
using Com.Github.PatBatTB.GEBB.Domain.Enums;

namespace Com.Github.PatBatTB.GEBB.DataBase.User;

public sealed class UserEntity
{
    public long UserId { get; set; }

    [StringLength(100)]
    public string? Username { get; set; }

    public DateTime RegisteredAt { get; set; }

    public UserStatus Status { get; set; }

    public ICollection<EventEntity> Events { get; set; } = new List<EventEntity>();

    public ICollection<BuildEventEntity> TempEvents { get; set; } = new List<BuildEventEntity>();

    public ICollection<EventEntity> EventsNavigation { get; set; } = new List<EventEntity>();

    public AlarmEntity? Alarm { get; set; }
}