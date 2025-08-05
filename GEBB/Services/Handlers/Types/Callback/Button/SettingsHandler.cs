using Com.GitHub.PatBatTB.GEBB;
using Com.Github.PatBatTB.GEBB.DataBase.Alarm;
using Com.Github.PatBatTB.GEBB.Domain;
using Com.Github.PatBatTB.GEBB.Domain.Enums;
using Com.Github.PatBatTB.GEBB.Services.Providers;
using log4net;
using Telegram.Bot;

namespace Com.Github.PatBatTB.GEBB.Services.Handlers.Types.Callback.Button;

public static class SettingsHandler
{
    private static readonly Dictionary<CallbackButton, Action<UpdateContainer>> ButtonHandlerDict = new()
    {
        [CallbackButton.Alarm] = HandleAlarm,
        [CallbackButton.Back] = HandleBack,
    };

    private static readonly Dictionary<CallbackButton, Action<UpdateContainer>> AlarmButtonHandlerDict = new()
    {
        [CallbackButton.ThreeDays] = HandleThreeDays,
        [CallbackButton.OneDay] = HandleOneDay,
        [CallbackButton.Hours] = HandleHours,
        [CallbackButton.Back] = HandleAlarmBack,
    };

    private static readonly Dictionary<CallbackButton, Action<UpdateContainer>> AlarmHoursHandlerDict = new()
    {
        [CallbackButton.Disable] = HandleDisableHours,
        [CallbackButton.One] = HandleOneHour,
        [CallbackButton.Two] = HandleTwoHours,
        [CallbackButton.Three] = HandleThreeHours,
        [CallbackButton.Four] = HandleFourHours,
        [CallbackButton.Five] = HandleFiveHours,
        [CallbackButton.Back] = HandleAlarmHoursBack,
    };
    
    private static readonly ILog Log = LogManager.GetLogger(typeof(SettingsHandler));
    private static readonly IAlarmSettingsService AsService = App.ServiceFactory.GetAlarmSettingsService();

    private static void UpdateAlarmHours(AppAlarmSettings alarmSettings, int hours)
    {
        alarmSettings.Hours = hours;
        AsService.Update(alarmSettings);
    }

    private static void HandleFiveHours(UpdateContainer container)
    {
        AppAlarmSettings? alarm = AsService.Get(container.AppUser.UserId);
        if (alarm is null)
        {
            throw new Exception("Alarm for user doesn't exist");
        }
        UpdateAlarmHours(alarm, 5);
        HandleAlarmHoursBack(container);
    }

    private static void HandleFourHours(UpdateContainer container)
    {
        AppAlarmSettings? alarm = AsService.Get(container.AppUser.UserId);
        if (alarm is null)
        {
            throw new Exception("Alarm for user doesn't exist");
        }
        UpdateAlarmHours(alarm, 4);
        HandleAlarmHoursBack(container);
    }

    private static void HandleThreeHours(UpdateContainer container)
    {
        AppAlarmSettings? alarm = AsService.Get(container.AppUser.UserId);
        if (alarm is null)
        {
            throw new Exception("Alarm for user doesn't exist");
        }
        UpdateAlarmHours(alarm, 3);
        HandleAlarmHoursBack(container);
    }

    private static void HandleTwoHours(UpdateContainer container)
    {
        AppAlarmSettings? alarm = AsService.Get(container.AppUser.UserId);
        if (alarm is null)
        {
            throw new Exception("Alarm for user doesn't exist");
        }
        UpdateAlarmHours(alarm, 2);
        HandleAlarmHoursBack(container);
    }

    private static void HandleOneHour(UpdateContainer container)
    {
        AppAlarmSettings? alarm = AsService.Get(container.AppUser.UserId);
        if (alarm is null)
        {
            throw new Exception("Alarm for user doesn't exist");
        }
        UpdateAlarmHours(alarm, 1);
        HandleAlarmHoursBack(container);
    }

    private static void HandleDisableHours(UpdateContainer container)
    {
        AppAlarmSettings? alarm = AsService.Get(container.AppUser.UserId);
        if (alarm is null)
        {
            throw new Exception("Alarm for user doesn't exist");
        }
        UpdateAlarmHours(alarm, 0);
        HandleAlarmHoursBack(container);
    }

    private static void HandleAlarmHoursBack(UpdateContainer container)
    {
        AppAlarmSettings? alarm = AsService.Get(container.AppUser.UserId);
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

    public static void Handle(UpdateContainer container)
    {
        if (container.CallbackData?.Button is not { } button)
            throw new NullReferenceException("CallbackData doesn't have button");
        ButtonHandlerDict.GetValueOrDefault(button, HandleUnknown)
            .Invoke(container);
    }

    public static void HandleAlarmMenu(UpdateContainer container)
    {
        if (container.CallbackData?.Button is not { } button)
        {
            throw new NullReferenceException("CallbackData doesn't have button.");
        }
        AlarmButtonHandlerDict.GetValueOrDefault(button, HandleUnknown).Invoke(container);
    }

    public static void HandleAlarmHours(UpdateContainer container)
    {
        if (container.CallbackData?.Button is not { } button)
        {
            throw new NullReferenceException("CallbackData doesn't have button.");
        }
        AlarmHoursHandlerDict.GetValueOrDefault(button, HandleUnknown).Invoke(container);
    }

    private static void HandleAlarm(UpdateContainer container)
    {
        AppAlarmSettings? alarm = AsService.Get(container.ChatId);
        if (alarm is null)
        {
            alarm = new()
            {
                UserId = container.AppUser.UserId,
                ThreeDays = false,
                OneDay = false,
                Hours = 0
            };
            AsService.Update(alarm);
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

    private static void HandleBack(UpdateContainer container)
    {
        Thread.Sleep(200);
        container.BotClient.EditMessageText(
            chatId: container.ChatId,
            messageId: container.Message.Id,
            text: CallbackMenu.Main.Text(),
            replyMarkup: InlineKeyboardProvider.GetMarkup(CallbackMenu.Main),
            cancellationToken: container.Token);
    }

    private static void HandleHours(UpdateContainer container)
    {
        Thread.Sleep(200);
        container.BotClient.EditMessageText(
            chatId: container.ChatId,
            messageId: container.Message.Id,
            text: CallbackMenu.AlarmHours.Text(),
            replyMarkup: InlineKeyboardProvider.GetMarkup(CallbackMenu.AlarmHours),
            cancellationToken: container.Token);
    }

    private static void HandleOneDay(UpdateContainer container)
    {
        AppAlarmSettings? alarm = AsService.Get(container.AppUser.UserId);
        if (alarm is null)
        {
            throw new Exception("Alarm for user doesn't exist");
        }
        alarm.OneDay = alarm.OneDay != true;
        AsService.Update(alarm);
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

    private static void HandleThreeDays(UpdateContainer container)
    {
        AppAlarmSettings? alarm = AsService.Get(container.AppUser.UserId);
        if (alarm is null)
        {
            throw new Exception("Alarm for user doesn't exist");
        }
        alarm.ThreeDays = alarm.ThreeDays != true;
        AsService.Update(alarm);
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

    private static void HandleAlarmBack(UpdateContainer container)
    {
        Thread.Sleep(200);
        container.BotClient.EditMessageText(
            chatId: container.ChatId,
            messageId: container.Message.Id,
            text: CallbackMenu.Settings.Text(),
            replyMarkup: InlineKeyboardProvider.GetMarkup(CallbackMenu.Settings),
            cancellationToken: container.Token);
    }

    private static void HandleUnknown(UpdateContainer container)
    {
        Log.Error("Unknown button");
    }

    private static string GetAlarmInfoMessage(AppAlarmSettings alarmSettings)
    {
        return $"Текущие настройки:\n" +
               $"{CallbackButton.Hours.Text()}: {GetHoursAlarmText(alarmSettings.Hours)}\n" +
               $"{CallbackButton.OneDay.Text()}: {GetBooleanAlarmText(alarmSettings.OneDay)}\n" + 
               $"{CallbackButton.ThreeDays.Text()}: {GetBooleanAlarmText(alarmSettings.ThreeDays)}";
    }

    private static string GetBooleanAlarmText(bool value)
    {
        return value == true ? "Вкл." : "Выкл.";
    }

    private static string GetHoursAlarmText(int value)
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