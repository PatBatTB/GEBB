namespace Com.Github.PatBatTB.GEBB.DataBase.Alarm;

public class DbAlarmService : IAlarmService
{
    public AppAlarm Get(long userId)
    {
        using TgBotDbContext db = new();
        AlarmEntity entity = db.Alarms.First(alarm => alarm.UserId == userId);
        return EntityToAlarm(entity);
    }

    public void Update(AppAlarm alarm)
    {
        using TgBotDbContext db = new();
        db.Add(AlarmToEntity(alarm));
        db.SaveChanges();
    }

    private AppAlarm EntityToAlarm(AlarmEntity entity)
    {
        return new()
        {
            UserId = entity.UserId,
            ThreeDaysAlarm = entity.ThreeDaysAlarm,
            OneDayAlarm = entity.OneDayAlarm,
            HoursAlarm = entity.HoursAlarm
        };
    }

    private AlarmEntity AlarmToEntity(AppAlarm alarm)
    {
        return new()
        {
            UserId = alarm.UserId,
            ThreeDaysAlarm = alarm.ThreeDaysAlarm,
            OneDayAlarm = alarm.OneDayAlarm,
            HoursAlarm = alarm.HoursAlarm
        };
    }
    
    
}