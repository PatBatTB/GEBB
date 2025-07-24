namespace Com.Github.PatBatTB.GEBB.DataBase.Alarm;

public class AppAlarm
{
    public long UserId { get; set; }
    public bool ThreeDaysAlarm { get; set; }
    public bool OneDayAlarm { get; set; }
    public int HoursAlarm { get; set; }
}