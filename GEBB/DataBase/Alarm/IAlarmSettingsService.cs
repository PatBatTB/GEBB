

namespace Com.Github.PatBatTB.GEBB.DataBase.Alarm;

public interface IAlarmSettingsService
{
    AppAlarmSettings? Get(long userId);
    ICollection<AppAlarmSettings> Get(params long[] userId);
    void Update(AppAlarmSettings alarmSettings);
}