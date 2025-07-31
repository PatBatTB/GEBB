using Com.Github.PatBatTB.GEBB.DataBase.Alarm;
using Com.Github.PatBatTB.GEBB.DataBase.Event;
using Telegram.Bot;

namespace Com.Github.PatBatTB.GEBB.Services;

public class AlarmSendService
{
    private readonly int _defaultDelay = 60_000; //in millis.
    private readonly string _dayAlarmTime = "10:00:00"; //time pattern like 10:00:00
    
    private readonly IAlarmService AService = new DbAlarmService();
    private readonly IAlarmSettingsService AsService = new DbAlarmSettingsService();
    private readonly IEventService EService = new DbEventService();

    public void Start(ITelegramBotClient botClient, CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            List<AppAlarm> alarms = GetAlarms();
            SendNotifications(botClient, alarms, token);
            Thread.Sleep(_defaultDelay);
        }
    }

    private List<AppAlarm> GetAlarms()
    {
        ICollection<AppEvent> events = EService.GetActiveEvents();
        IEnumerable<AppAlarm> creatorAlarms = events
            .Select(e => new AppAlarm { Event = e, LastAlert = null, User = e.Creator });
        IEnumerable<AppAlarm> registeredAlarms = events.SelectMany(
            e => e.RegisteredUsers,
            (e, u) => new AppAlarm { Event = e, LastAlert = null, User = u });
        List<AppAlarm> alarms = creatorAlarms.Union(registeredAlarms, new AlarmComparator()).ToList();
        List<AppAlarm> resultAlarms = AService.GetAlarmsForActiveEvents().ToList();
        resultAlarms = resultAlarms
            .Intersect(alarms, new AlarmComparator())
            .Union(alarms, new AlarmComparator())
            .ToList();
        return resultAlarms;
    }

    private void SendNotifications(ITelegramBotClient botClient, List<AppAlarm> alarms, CancellationToken token)
    {
        ICollection<AppAlarmSettings> settings = AsService.Get(alarms.Select(e => e.User.UserId).ToArray());
        foreach (AppAlarm alarm in alarms)
        {
            AppAlarmSettings? userSettings = settings.FirstOrDefault(e => e!.UserId == alarm.User.UserId, null);
            if (userSettings is null)
            {
                continue;
            }
            if (CheckNeedToNotify(alarm, userSettings))
            {
                SendNotificationMessage(alarm, botClient, token);
                alarm.LastAlert = DateTime.Now;
                AService.Update(alarm);
            }
        }
    }

    private void SendNotificationMessage(AppAlarm alarm, ITelegramBotClient botClient, CancellationToken token)
    {
        string headerMessage = "Напоминаю, что вы зарегистрированы на мероприятие:\n\n";
        string eventDescription = MessageService.GetEventShortDescription(alarm.Event);
        Thread.Sleep(200);
        botClient.SendMessage(
            chatId: alarm.User.UserId,
            text: headerMessage + eventDescription,
            cancellationToken: token);

    }

    private bool CheckNeedToNotify(AppAlarm alarm, AppAlarmSettings userSettings)
    {
        if (alarm.Event.DateTimeOf is null)
        {
            return false;
        }
        
        DateTime dateTimeOf = (DateTime) alarm.Event.DateTimeOf;
        if (userSettings.ThreeDays)
        {
            DateTime alarmDate = new DateTime(
                DateOnly.FromDateTime(dateTimeOf.AddDays(-3)), 
                TimeOnly.Parse(_dayAlarmTime));
            if (alarmDate < DateTime.Now && alarmDate > alarm.LastAlert)
            {
                return true;
            }
        }

        if (userSettings.OneDay)
        {
            DateTime alarmDate = new DateTime(
                DateOnly.FromDateTime(dateTimeOf.AddDays(-1)), 
                TimeOnly.Parse(_dayAlarmTime));
            if (alarmDate < DateTime.Now && alarmDate > alarm.LastAlert)
            {
                return true;
            }
        }

        if (userSettings.Hours > 0)
        {
            DateTime alarmDate = dateTimeOf.AddHours(userSettings.Hours * -1);
            if (alarmDate < DateTime.Now && alarmDate > alarm.LastAlert)
            {
                return true;
            }
        }

        return false;
    }

    private class AlarmComparator : IEqualityComparer<AppAlarm>
    {
        public bool Equals(AppAlarm? x, AppAlarm? y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (x is null) return false;
            if (y is null) return false;
            if (x.GetType() != y.GetType()) return false;
            return x.User.UserId.Equals(y.User.UserId) && x.Event.Id.Equals(y.Event.Id);
        }

        public int GetHashCode(AppAlarm obj)
        {
            return HashCode.Combine(obj.User.UserId, obj.Event.Id);
        }
    }
}