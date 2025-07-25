namespace Com.Github.PatBatTB.GEBB.DataBase.Alarm;

public interface IAlarmSettingsService
{
    AppAlarmSettings? Get(long userId);
    void Update(AppAlarmSettings alarmSettings);
}