using Com.Github.PatBatTB.GEBB.DataBase.User;
using Com.Github.PatBatTB.GEBB.Domain.Enums;

namespace Com.Github.PatBatTB.GEBB.DataBase.Event;

public class AppEvent
{
    public required string Id { get; set; }
    public int MessageId { get; set; }
    public required AppUser Creator { get; set; }
    public string? Title { get; set; }
    public DateTime? DateTimeOf { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? Address { get; set; }
    public int? ParticipantLimit { get; set; }
    public int? Cost { get; set; }
    public string? Description { get; set; }
    public ICollection<AppUser> RegisteredUsers { get; set; } = [];
    public EventStatus Status { get; set; }
}