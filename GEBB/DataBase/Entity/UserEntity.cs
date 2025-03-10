using Com.Github.PatBatTB.GEBB.Domain.Enums;

namespace Com.Github.PatBatTB.GEBB.DataBase.Entity;

public sealed class UserEntity
{
    public long UserId { get; set; }

    public string? Username { get; set; }

    public DateTime RegisteredAt { get; set; }

    public UserStatus UserStatus { get; set; }

    public ICollection<EventEntity> Events { get; set; } = new List<EventEntity>();

    public ICollection<EventEntity> EventsNavigation { get; set; } = new List<EventEntity>();
}