using System.ComponentModel.DataAnnotations;

namespace Com.Github.PatBatTB.GEBB.DataBase.Entity;

public sealed class EventEntity
{
    [Key] public int EventId { get; set; }

    public string Title { get; set; }

    public DateTime DateTimeOf { get; set; }

    public string Address { get; set; }

    public int? ParticipantLimit { get; set; }

    public int? Cost { get; set; }

    public string? Description { get; set; }

    public bool IsActive { get; set; }

    public ICollection<UserEntity> Users { get; set; } = new List<UserEntity>();
}