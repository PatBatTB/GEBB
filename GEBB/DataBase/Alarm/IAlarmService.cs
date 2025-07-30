namespace Com.Github.PatBatTB.GEBB.DataBase.Alarm;

public interface IAlarmService
{
    ICollection<AppAlarm> GetAlarmsForUser(long userId);
    ICollection<AppAlarm> GetAlarmsForActiveEvents();
    void Update(AppAlarm appAlarm);
}