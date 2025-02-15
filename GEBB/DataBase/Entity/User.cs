using System;
using System.Collections.Generic;

namespace Com.Github.PatBatTB.GEBB.DataBase.Entity;

public partial class User
{
    public long UserId { get; set; }

    public string? Username { get; set; }

    public DateTime? RegisteredAt { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<Event> Events { get; set; } = new List<Event>();
}
