using Com.GitHub.PatBatTB.GEBB;
using Com.Github.PatBatTB.GEBB.DataBase.Alarm;
using Com.Github.PatBatTB.GEBB.Domain;
using Com.Github.PatBatTB.GEBB.Domain.Enums;
using Com.Github.PatBatTB.GEBB.Services.Providers;
using log4net;
using Telegram.Bot;

namespace Com.Github.PatBatTB.GEBB.Services.Handlers.Types.Callback.Button;

public class SettingsHandler
{
    private readonly Dictionary<CallbackButton, Action<UpdateContainer>> _buttonHandlerDict;
    private readonly Dictionary<CallbackButton, Action<UpdateContainer>> _alarmButtonHandlerDict;
    private readonly Dictionary<CallbackButton, Action<UpdateContainer>> _alarmHoursHandlerDict;

    private readonly IAlarmSettingsService _asService = App.ServiceFactory.GetAlarmSettingsService();
    
    private readonly ILog _log = LogManager.GetLogger(typeof(SettingsHandler));

    public SettingsHandler()
    {
        _buttonHandlerDict = new Dictionary<CallbackButton, Action<UpdateContainer>>
        {
            [CallbackButton.Alarm] = HandleAlarm,
            [CallbackButton.Back] = HandleBack,
        };
        _alarmButtonHandlerDict = new Dictionary<CallbackButton, Action<UpdateContainer>>
        {
            [CallbackButton.ThreeDays] = HandleThreeDays,
            [CallbackButton.OneDay] = HandleOneDay,
            [CallbackButton.Hours] = HandleHours,
            [CallbackButton.Back] = HandleAlarmBack,
        };
        _alarmHoursHandlerDict = new Dictionary<CallbackButton, Action<UpdateContainer>>
        {
            [CallbackButton.Disable] = HandleDisableHours,
            [CallbackButton.One] = HandleOneHour,
            [CallbackButton.Two] = HandleTwoHours,
            [CallbackButton.Three] = HandleThreeHours,
            [CallbackButton.Four] = HandleFourHours,
            [CallbackButton.Five] = HandleFiveHours,
            [CallbackButton.Back] = HandleAlarmHoursBack,
        };
    }

    public void Handle(UpdateContainer container)
    {
        if (container.CallbackData?.Button is not { } button)
        {
            throw new NullReferenceException("CallbackData doesn't have button");   
        }
        _buttonHandlerDict.GetValueOrDefault(button, HandleUnknown)
            .Invoke(container);
    }

    public void HandleAlarmMenu(UpdateContainer container)
    {
        if (container.CallbackData?.Button is not { } button)
        {
            throw new NullReferenceException("CallbackData doesn't have button.");
        }
        _alarmButtonHandlerDict.GetValueOrDefault(button, HandleUnknown).Invoke(container);
    }

    public void HandleAlarmHours(UpdateContainer container)
    {
        if (container.CallbackData?.Button is not { } button)
        {
            throw new NullReferenceException("CallbackData doesn't have button.");
        }
        _alarmHoursHandlerDict.GetValueOrDefault(button, HandleUnknown).Invoke(container);
    }

    private void HandleAlarm(UpdateContainer container)
    {
        AppAlarmSettings? alarm = _asService.Get(container.ChatId);
        if (alarm is null)
        {
            alarm = new()
            {
                UserId = container.AppUser.UserId,
                ThreeDays = false,
                OneDay = false,
                Hours = 0
            };
            _asService.Update(alarm);
        }
        
        string message = $"{CallbackMenu.Alarm.Text()}\n\n" +
                         GetAlarmInfoMessage(alarm);
        Thread.Sleep(200);
        container.BotClient.EditMessageText(
            chatId: container.ChatId,
            messageId: container.Message.Id,
            text: message,
            replyMarkup: InlineKeyboardProvider.GetMarkup(CallbackMenu.Alarm),
            cancellationToken: container.Token);
    }

    private void UpdateAlarmHours(AppAlarmSettings alarmSettings, int hours)
    {
        alarmSettings.Hours = hours;
        _asService.Update(alarmSettings);
    }

    private void HandleFiveHours(UpdateContainer container)
    {
        AppAlarmSettings? alarm = _asService.Get(container.AppUser.UserId);
        if (alarm is null)
        {
            throw new Exception("Alarm for user doesn't exist");
        }
        UpdateAlarmHours(alarm, 5);
        HandleAlarmHoursBack(container);
    }

    private void HandleFourHours(UpdateContainer container)
    {
        AppAlarmSettings? alarm = _asService.Get(container.AppUser.UserId);
        if (alarm is null)
        {
            throw new Exception("Alarm for user doesn't exist");
        }
        UpdateAlarmHours(alarm, 4);
        HandleAlarmHoursBack(container);
    }

    private void HandleThreeHours(UpdateContainer container)
    {
        AppAlarmSettings? alarm = _asService.Get(container.AppUser.UserId);
        if (alarm is null)
        {
            throw new Exception("Alarm for user doesn't exist");
        }
        UpdateAlarmHours(alarm, 3);
        HandleAlarmHoursBack(container);
    }

    private void HandleTwoHours(UpdateContainer container)
    {
        AppAlarmSettings? alarm = _asService.Get(container.AppUser.UserId);
        if (alarm is null)
        {
            throw new Exception("Alarm for user doesn't exist");
        }
        UpdateAlarmHours(alarm, 2);
        HandleAlarmHoursBack(container);
    }

    private void HandleOneHour(UpdateContainer container)
    {
        AppAlarmSettings? alarm = _asService.Get(container.AppUser.UserId);
        if (alarm is null)
        {
            throw new Exception("Alarm for user doesn't exist");
        }
        UpdateAlarmHours(alarm, 1);
        HandleAlarmHoursBack(container);
    }

    private void HandleDisableHours(UpdateContainer container)
    {
        AppAlarmSettings? alarm = _asService.Get(container.AppUser.UserId);
        if (alarm is null)
        {
            throw new Exception("Alarm for user doesn't exist");
        }
        UpdateAlarmHours(alarm, 0);
        HandleAlarmHoursBack(container);
    }

    private void HandleAlarmHoursBack(UpdateContainer container)
    {
        AppAlarmSettings? alarm = _asService.Get(container.AppUser.UserId);
        if (alarm is null)
        {
            throw new Exception("Alarm for user doesn't exist");
        }
        string message = $"{CallbackMenu.Alarm.Text()}\n\n" +
                         GetAlarmInfoMessage(alarm);
        Thread.Sleep(200);
        container.BotClient.EditMessageText(
            chatId: container.ChatId,
            messageId: container.Message.Id,
            text: message,
            replyMarkup: InlineKeyboardProvider.GetMarkup(CallbackMenu.Alarm),
            cancellationToken: container.Token);
    }

    private void HandleBack(UpdateContainer container)
    {
        Thread.Sleep(200);
        container.BotClient.EditMessageText(
            chatId: container.ChatId,
            messageId: container.Message.Id,
            text: CallbackMenu.Main.Text(),
            replyMarkup: InlineKeyboardProvider.GetMarkup(CallbackMenu.Main),
            cancellationToken: container.Token);
    }

    private void HandleHours(UpdateContainer container)
    {
        Thread.Sleep(200);
        container.BotClient.EditMessageText(
            chatId: container.ChatId,
            messageId: container.Message.Id,
            text: CallbackMenu.AlarmHours.Text(),
            replyMarkup: InlineKeyboardProvider.GetMarkup(CallbackMenu.AlarmHours),
            cancellationToken: container.Token);
    }

    private void HandleOneDay(UpdateContainer container)
    {
        AppAlarmSettings? alarm = _asService.Get(container.AppUser.UserId);
        if (alarm is null)
        {
            throw new Exception("Alarm for user doesn't exist");
        }
        alarm.OneDay = alarm.OneDay != true;
        _asService.Update(alarm);
        string message = $"{CallbackMenu.Alarm.Text()}\n\n" +
                         GetAlarmInfoMessage(alarm);
        Thread.Sleep(200);
        container.BotClient.EditMessageText(
            chatId: container.ChatId,
            text: message,
            messageId: container.Message.Id,
            replyMarkup: InlineKeyboardProvider.GetMarkup(CallbackMenu.Alarm),
            cancellationToken: container.Token);
    }

    private void HandleThreeDays(UpdateContainer container)
    {
        AppAlarmSettings? alarm = _asService.Get(container.AppUser.UserId);
        if (alarm is null)
        {
            throw new Exception("Alarm for user doesn't exist");
        }
        alarm.ThreeDays = alarm.ThreeDays != true;
        _asService.Update(alarm);
        string message = $"{CallbackMenu.Alarm.Text()}\n\n" +
                         GetAlarmInfoMessage(alarm);
        Thread.Sleep(200);
        container.BotClient.EditMessageText(
            chatId: container.ChatId,
            text: message,
            messageId: container.Message.Id,
            replyMarkup: InlineKeyboardProvider.GetMarkup(CallbackMenu.Alarm),
            cancellationToken: container.Token);
    }

    private void HandleAlarmBack(UpdateContainer container)
    {
        Thread.Sleep(200);
        container.BotClient.EditMessageText(
            chatId: container.ChatId,
            messageId: container.Message.Id,
            text: CallbackMenu.Settings.Text(),
            replyMarkup: InlineKeyboardProvider.GetMarkup(CallbackMenu.Settings),
            cancellationToken: container.Token);
    }

    private void HandleUnknown(UpdateContainer container)
    {
        _log.Error("Unknown button");
    }

    private string GetAlarmInfoMessage(AppAlarmSettings alarmSettings)
    {
        return $"Текущие настройки:\n" +
               $"{CallbackButton.Hours.Text()}: {GetHoursAlarmText(alarmSettings.Hours)}\n" +
               $"{CallbackButton.OneDay.Text()}: {GetBooleanAlarmText(alarmSettings.OneDay)}\n" + 
               $"{CallbackButton.ThreeDays.Text()}: {GetBooleanAlarmText(alarmSettings.ThreeDays)}";
    }

    private string GetBooleanAlarmText(bool value)
    {
        return value == true ? "Вкл." : "Выкл.";
    }

    private string GetHoursAlarmText(int value)
    {
        string hoursName = value switch
        {
            (>= 10 and <= 20) => "часов",
            _ when (value % 10 == 1) => "час",
            _ when (value % 10 >= 2 && value <= 4) => "часа",
            _ => "часов"
        };

        return value == 0 ? "Выкл." : $"За {value} {hoursName}.";
    }
}