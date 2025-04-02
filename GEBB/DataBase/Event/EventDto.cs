using Com.Github.PatBatTB.GEBB.DataBase.User;

namespace Com.Github.PatBatTB.GEBB.DataBase.Event;

public class EventDto
{
    public required string EventId { get; set; }
    public int MessageId { get; set; }
    public required UserDto Creator { get; set; }
    public string? Title { get; set; }
    public DateTime? DateTimeOf { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? Address { get; set; }
    public int? ParticipantLimit { get; set; }
    public int? Cost { get; set; }
    public string? Description { get; set; }
    public bool IsCreateCompleted { get; set; }
    public bool IsActive { get; set; }
    public ICollection<UserDto> RegisteredUsers { get; set; } = [];
}