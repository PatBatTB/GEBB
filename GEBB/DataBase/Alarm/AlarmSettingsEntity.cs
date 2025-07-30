using Com.Github.PatBatTB.GEBB.DataBase.User;

namespace Com.Github.PatBatTB.GEBB.DataBase.Alarm;

public class AlarmSettingsEntity
{
    public long UserId { get; set; }
    public UserEntity? User { get; set; }
    public bool ThreeDays { get; set; }
    public bool OneDay { get; set; }
    public int Hours { get; set; }
}