using Com.Github.PatBatTB.GEBB.DataBase.Event;
using Com.Github.PatBatTB.GEBB.DataBase.User;

namespace Com.Github.PatBatTB.GEBB.DataBase.Alarm;

public class AlarmEntity
{
    public long UserId { get; set; }
    public UserEntity? User { get; set; }
    public int EventId { get; set; }
    public EventEntity Event { get; set; }
    public DateTime? LastAlert { get; set; }
}