using System;
using System.Collections.Generic;

namespace Com.Github.PatBatTB.GEBB.DataBase.Entity;

public partial class Event
{
    public int EventId { get; set; }

    public string? Title { get; set; }

    public DateTime? DateTimeOf { get; set; }

    public string? Address { get; set; }

    public int? ParticipantLimit { get; set; }

    public int? Cost { get; set; }

    public string? Description { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
