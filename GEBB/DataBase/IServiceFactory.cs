using Com.Github.PatBatTB.GEBB.DataBase.Alarm;
using Com.Github.PatBatTB.GEBB.DataBase.Event;
using Com.Github.PatBatTB.GEBB.DataBase.Message;
using Com.Github.PatBatTB.GEBB.DataBase.User;

namespace Com.Github.PatBatTB.GEBB.DataBase;

public interface IServiceFactory
{
    IUserService GetUserService();
    IEventMessageService GetEventMessageService();
    IEventService GetEventService();
    IAlarmService GetAlarmService();
    IAlarmSettingsService GetAlarmSettingsService();
}