using Com.Github.PatBatTB.GEBB.DataBase.Alarm;
using Com.Github.PatBatTB.GEBB.DataBase.Event;
using Com.Github.PatBatTB.GEBB.DataBase.Message;
using Com.Github.PatBatTB.GEBB.DataBase.User;

namespace Com.Github.PatBatTB.GEBB.DataBase;

public class DbServiceFactory : IServiceFactory
{
    public IUserService GetUserService() => new DbUserService();
    public IEventMessageService GetEventMessageService() => new DbEventMessageService();
    public IEventService GetEventService() => new DbEventService();
    public IAlarmService GetAlarmService() => new DbAlarmService();
    public IAlarmSettingsService GetAlarmSettingsService() => new DbAlarmSettingsService();
}