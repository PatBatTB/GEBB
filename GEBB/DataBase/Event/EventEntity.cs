﻿using Com.Github.PatBatTB.GEBB.DataBase.User;

namespace Com.Github.PatBatTB.GEBB.DataBase.Event;

public sealed class EventEntity
{
    //EventId = MessageId from chat, that initialized event.
    public int EventId { get; set; }

    public long CreatorId { get; set; }
    public UserEntity? Creator { get; set; }

    public string? Title { get; set; }

    public DateTime? DateTimeOf { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? Address { get; set; }

    public int? ParticipantLimit { get; set; }

    public int? Cost { get; set; }

    public string? Description { get; set; }

    public bool IsCreateCompleted { get; set; }

    public bool IsActive { get; set; }

    public ICollection<UserEntity> RegisteredUsers { get; set; } = new List<UserEntity>();
}