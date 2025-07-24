using Com.Github.PatBatTB.GEBB.DataBase.User;

namespace Com.Github.PatBatTB.GEBB.DataBase.Alarm;

public class AlarmEntity
{
    public long UserId { get; set; }
    public UserEntity? User { get; set; }
    public bool ThreeDaysAlarm { get; set; }
    public bool OneDayAlarm { get; set; }
    public int HoursAlarm { get; set; }
}