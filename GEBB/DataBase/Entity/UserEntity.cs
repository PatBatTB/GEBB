using System.ComponentModel.DataAnnotations;

namespace Com.Github.PatBatTB.GEBB.DataBase.Entity;

public class UserEntity
{
    [Key] public long UserId { get; set; }

    public string? Username { get; set; }

    public DateTime RegisteredAt { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<EventEntity> Events { get; set; } = new List<EventEntity>();
}