using Com.Github.PatBatTB.GEBB.Domain.Enums;

namespace Com.Github.PatBatTB.GEBB.DataBase.User;

public class AppUser
{
    public long UserId { get; set; }
    public string? Username { get; set; }
    public DateTime RegisteredAt { get; set; }
    public UserStatus UserStatus { get; set; }
}