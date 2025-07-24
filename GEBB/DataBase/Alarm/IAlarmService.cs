namespace Com.Github.PatBatTB.GEBB.DataBase.Alarm;

public interface IAlarmService
{
    AppAlarm Get(long userId);
    void Update(AppAlarm alarm);
}