using Com.Github.PatBatTB.GEBB.DataBase.Event;
using Com.Github.PatBatTB.GEBB.DataBase.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Com.Github.PatBatTB.GEBB.DataBase.Alarm;

public class DbAlarmService : IAlarmService
{
    private readonly DbUserService _dbUserService = new();
    private readonly DbEventService _dbEventService = new();
    
    public ICollection<AppAlarm> GetAlarmsForUser(long userId)
    {
        using TgBotDbContext db = new();
        ICollection<AlarmEntity> alarmEntities = db.Alarms
            .Include(e => e.User)
            .Include(e => e.Event)
            .Where(e => e.UserId == userId).ToList();
        return alarmEntities.Select(EntityToAlarm).ToList();
    }

    public ICollection<AppAlarm> GetAlarmsForActiveEvents()
    {
        using TgBotDbContext db = new();
        ICollection<AlarmEntity> alarmEntities = db.Alarms
            .Include(e => e.User)
            .Include(e => e.Event)
            .Where(e => e.Event.DateTimeOf > DateTime.Now)
            .ToList();
        return alarmEntities.Select(EntityToAlarm).ToList();
    }

    public void Update(AppAlarm appAlarm)
    {
        (int eventId, long creatorId) = _dbEventService.ParseEventId(appAlarm.Event.Id);
        using TgBotDbContext db = new();
        if (db.Alarms.Find(appAlarm.User.UserId, eventId, creatorId) is { } alarm)
        {
            alarm.LastAlert = appAlarm.LastAlert;
            db.Update(alarm);
        }
        else
        {
            db.Add(AlarmToEntity(appAlarm));
        }
        db.SaveChanges();
    }

    private AlarmEntity AlarmToEntity(AppAlarm alarm)
    {
        (int eventId, long creatorId) = _dbEventService.ParseEventId(alarm.Event.Id);
        return new()
        {
            UserId = alarm.User.UserId,
            CreatorId = creatorId,
            EventId = eventId,
            LastAlert = alarm.LastAlert
        };
    }

    private AppAlarm EntityToAlarm(AlarmEntity entity)
    {
        string eventId = _dbEventService.CreateEventId(entity.EventId, entity.CreatorId);
        return new()
        {
            User = _dbUserService.EntityToUser(entity.User),
            Event = _dbEventService.EntityToEvent(entity.Event),
            LastAlert = entity.LastAlert
        };
    }
}