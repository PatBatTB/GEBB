using Com.Github.PatBatTB.GEBB.DataBase.Event;
using Com.Github.PatBatTB.GEBB.DataBase.User;
using Com.Github.PatBatTB.GEBB.Domain;
using Com.Github.PatBatTB.GEBB.Domain.Enums;
using Com.Github.PatBatTB.GEBB.Services.Providers;
using log4net;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Com.Github.PatBatTB.GEBB.Services.Handlers.Types.Callback.Button;

public static class MainHandler
{
    private static readonly Dictionary<CallbackButton, Action<UpdateContainer>> ButtonHandlerDict = new()
    {
        [CallbackButton.Create] = HandleCreate,
        [CallbackButton.List] = HandleList,
        [CallbackButton.Settings] = HandleSettings,
        [CallbackButton.Close] = HandleClose,
    };

    private static readonly IUserService UService = new DbUserService();
    private static readonly IEventService EService = new DbEventService();
    private static readonly ILog Log = LogManager.GetLogger(typeof(MainHandler));

    public static void Handle(UpdateContainer container)
    {
        if (container.CallbackData?.Button is not { } button)
            throw new NullReferenceException("CallbackData doesn't have button");
        ButtonHandlerDict.GetValueOrDefault(button, HandleUnknown)
            .Invoke(container);
    }
    
    private static void HandleCreate(UpdateContainer container)
    {
        long chatId = container.ChatId;
        int messageId = container.Message.Id;
        CancellationToken token = container.Token;
        
        container.Events.AddRange(EService.GetBuildEvents(container.AppUser.UserId, EventStatus.Creating));
        
        Thread.Sleep(200);
        container.BotClient.DeleteMessage(
            chatId: chatId,
            messageId: messageId,
            cancellationToken: token);
        
        if (container.Events.Count > 0)
        {
            List<int> messageIds = container.Events.Select(dto => dto.MessageId).ToList();

            Thread.Sleep(200);
            container.BotClient.DeleteMessages(
                chatId: chatId,
                messageIds: messageIds,
                cancellationToken: token);
            
            Thread.Sleep(200);
            container.BotClient.SendMessage(
                chatId: chatId,
                text: "Ошибка. Обнаружено мероприятие в режиме создания.\nПопробуйте снова через команду /menu",
                cancellationToken: token);

            EService.RemoveInBuilding(chatId, EventStatus.Creating);

            Log.Warn("Attempting to create multiple events at the same time");
        }
        
        Thread.Sleep(200);
        Message sent = container.BotClient.SendMessage(
            chatId,
            CallbackMenu.CreateEvent.Text(),
            cancellationToken: token).Result;
        AppEvent appEvent = EService.Create(container.AppUser.UserId, sent.Id);
        Thread.Sleep(100);
        container.BotClient.EditMessageReplyMarkup(
            chatId: container.ChatId,
            messageId: sent.Id,
            replyMarkup: InlineKeyboardProvider.GetMarkup(CallbackMenu.CreateEvent, appEvent.Id),
            cancellationToken: container.Token);

        DataService.UpdateUserStatus(container, UserStatus.CreatingEvent, UService);
    }

    private static void HandleList(UpdateContainer container)
    {
        container.BotClient.EditMessageText(
            chatId: container.ChatId,
            messageId: container.Message.Id,
            text: CallbackMenu.EventsList.Text(),
            replyMarkup: InlineKeyboardProvider.GetMarkup(CallbackMenu.EventsList),
            cancellationToken: container.Token);
    }

    private static void HandleSettings(UpdateContainer container)
    {
        Thread.Sleep(200);
        container.BotClient.EditMessageText(
            chatId: container.ChatId,
            messageId: container.Message.Id,
            text: CallbackMenu.Settings.Text(),
            replyMarkup: InlineKeyboardProvider.GetMarkup(CallbackMenu.Settings),
            cancellationToken: container.Token);
    }

    private static void HandleClose(UpdateContainer container)
    {
        var chatId = container.ChatId;
        var messageId = container.Message.Id;
        container.BotClient.DeleteMessage(
            chatId,
            messageId,
            container.Token);
        DataService.UpdateUserStatus(container, UserStatus.Active, UService);
    }

    private static void HandleUnknown(UpdateContainer container)
    {
        Log.Error("Unknown button");
    }
}