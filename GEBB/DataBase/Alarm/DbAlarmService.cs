using Com.GitHub.PatBatTB.GEBB.Exceptions;

namespace Com.Github.PatBatTB.GEBB.DataBase.Alarm;

public class DbAlarmService : IAlarmService
{
    public AppAlarm? Get(long userId)
    {
        using TgBotDbContext db = new();
        return (db.Alarms.Find(userId) is { } alarm) ? EntityToAlarm(alarm) : null; 
    }

    public void Update(AppAlarm alarm)
    {
        using TgBotDbContext db = new();
        if (db.Alarms.Find(alarm.UserId) is { } entity)
        {
            entity.HoursAlarm = alarm.HoursAlarm;
            entity.OneDayAlarm = alarm.OneDayAlarm;
            entity.ThreeDaysAlarm = alarm.ThreeDaysAlarm;
            db.Update(entity);
        }
        else
        {
            db.Add(AlarmToEntity(alarm));
        }
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