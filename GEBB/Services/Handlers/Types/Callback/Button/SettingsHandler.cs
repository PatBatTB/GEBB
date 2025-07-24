using System.ComponentModel;
using Com.Github.PatBatTB.GEBB.DataBase.Alarm;
using Com.Github.PatBatTB.GEBB.Domain;
using Com.Github.PatBatTB.GEBB.Domain.Enums;
using Com.Github.PatBatTB.GEBB.Services.Providers;
using log4net;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace Com.Github.PatBatTB.GEBB.Services.Handlers.Types.Callback.Button;

public static class SettingsHandler
{
    private static Dictionary<CallbackButton, Action<UpdateContainer>> ButtonHandlerDict = new()
    {
        [CallbackButton.Alarm] = HandleAlarm,
        [CallbackButton.Back] = HandleBack,
    };

    private static Dictionary<CallbackButton, Action<UpdateContainer>> AlarmButtonHandlerDict = new()
    {
        [CallbackButton.ThreeDays] = HandleThreeDays,
        [CallbackButton.OneDay] = HandleOneDay,
        [CallbackButton.Hours] = HandleHours,
        [CallbackButton.Back] = HandleAlarmBack,
    };

    private static ILog Log = LogManager.GetLogger(typeof(SettingsHandler));
    private static IAlarmService aService = new DbAlarmService();

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

    private static void HandleAlarm(UpdateContainer container)
    {
        Thread.Sleep(200);
        container.BotClient.EditMessageText(
            chatId: container.ChatId,
            messageId: container.Message.Id,
            text: CallbackMenu.Alarm.Text(),
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

    private static void HandleThreeDays(UpdateContainer container)
    {
        AppAlarm alarm = aService.Get(container.AppUser.UserId);
    }

    private static void HandleOneDay(UpdateContainer obj)
    {
        throw new NotImplementedException();
    }

    private static void HandleHours(UpdateContainer obj)
    {
        throw new NotImplementedException();
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
}