namespace Com.Github.PatBatTB.GEBB.DataBase.Alarm;

public class DbAlarmSettingsService : IAlarmSettingsService
{
    public AppAlarmSettings? Get(long userId)
    {
        using TgBotDbContext db = new();
        return (db.AlarmSettings.Find(userId) is { } alarm) ? EntityToAlarmSettings(alarm) : null; 
    }

    public ICollection<AppAlarmSettings> Get(params long[] userId)
    {
        using TgBotDbContext db = new();
        List<AlarmSettingsEntity> alarmSettingsEntities = db.AlarmSettings
            .Where(e => userId.Contains(e.UserId))
            .ToList();
        return alarmSettingsEntities.Select(EntityToAlarmSettings).ToList();
    }

    public void Update(AppAlarmSettings alarmSettings)
    {
        using TgBotDbContext db = new();
        if (db.AlarmSettings.Find(alarmSettings.UserId) is { } entity)
        {
            entity.Hours = alarmSettings.Hours;
            entity.OneDay = alarmSettings.OneDay;
            entity.ThreeDays = alarmSettings.ThreeDays;
            db.Update(entity);
        }
        else
        {
            db.Add(AlarmSettingsToEntity(alarmSettings));
        }
        db.SaveChanges();
    }

    private AppAlarmSettings EntityToAlarmSettings(AlarmSettingsEntity settingsEntity)
    {
        return new()
        {
            UserId = settingsEntity.UserId,
            ThreeDays = settingsEntity.ThreeDays,
            OneDay = settingsEntity.OneDay,
            Hours = settingsEntity.Hours
        };
    }

    private AlarmSettingsEntity AlarmSettingsToEntity(AppAlarmSettings alarmSettings)
    {
        return new()
        {
            UserId = alarmSettings.UserId,
            ThreeDays = alarmSettings.ThreeDays,
            OneDay = alarmSettings.OneDay,
            Hours = alarmSettings.Hours
        };
    }
    
    
}