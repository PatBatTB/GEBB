using Com.GitHub.PatBatTB.GEBB;
using Com.Github.PatBatTB.GEBB.DataBase.Event;
using Com.Github.PatBatTB.GEBB.DataBase.User;
using Com.Github.PatBatTB.GEBB.Domain;
using Com.Github.PatBatTB.GEBB.Domain.Enums;
using Com.Github.PatBatTB.GEBB.Services.Providers;
using log4net;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Com.Github.PatBatTB.GEBB.Services.Handlers.Types.Callback.Button;

public class MainHandler
{
    private readonly Dictionary<CallbackButton, Action<UpdateContainer>> _buttonHandlerDict;

    private readonly IUserService _uService;
    private readonly IEventService _eService;
    
    private readonly ILog _log;

    public MainHandler()
    {
        _uService = App.ServiceFactory.GetUserService();
        _eService = App.ServiceFactory.GetEventService();
        _log = LogManager.GetLogger(typeof(MainHandler));
        _buttonHandlerDict = new Dictionary<CallbackButton, Action<UpdateContainer>>
        {
            [CallbackButton.Create] = HandleCreate,
            [CallbackButton.List] = HandleList,
            [CallbackButton.Settings] = HandleSettings,
            [CallbackButton.Close] = HandleClose,
        };
    }

    public void Handle(UpdateContainer container)
    {
        if (container.CallbackData?.Button is not { } button)
            throw new NullReferenceException("CallbackData doesn't have button");
        _buttonHandlerDict.GetValueOrDefault(button, HandleUnknown)
            .Invoke(container);
    }
    
    private void HandleCreate(UpdateContainer container)
    {
        long chatId = container.ChatId;
        int messageId = container.Message.Id;
        CancellationToken token = container.Token;
        
        container.Events.AddRange(_eService.GetBuildEvents(container.AppUser.UserId, EventStatus.Creating));
        
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

            _eService.RemoveInBuilding(chatId, EventStatus.Creating);

            _log.Warn("Attempting to create multiple events at the same time");
        }
        
        Thread.Sleep(200);
        Message sent = container.BotClient.SendMessage(
            chatId,
            CallbackMenu.CreateEvent.Text(),
            cancellationToken: token).Result;
        AppEvent appEvent = _eService.Create(container.AppUser.UserId, sent.Id);
        Thread.Sleep(100);
        container.BotClient.EditMessageReplyMarkup(
            chatId: container.ChatId,
            messageId: sent.Id,
            replyMarkup: InlineKeyboardProvider.GetMarkup(CallbackMenu.CreateEvent, appEvent.Id),
            cancellationToken: container.Token);

        DataService.UpdateUserStatus(container, UserStatus.CreatingEvent, _uService);
    }

    private void HandleList(UpdateContainer container)
    {
        container.BotClient.EditMessageText(
            chatId: container.ChatId,
            messageId: container.Message.Id,
            text: CallbackMenu.EventsList.Text(),
            replyMarkup: InlineKeyboardProvider.GetMarkup(CallbackMenu.EventsList),
            cancellationToken: container.Token);
    }

    private void HandleSettings(UpdateContainer container)
    {
        Thread.Sleep(200);
        container.BotClient.EditMessageText(
            chatId: container.ChatId,
            messageId: container.Message.Id,
            text: CallbackMenu.Settings.Text(),
            replyMarkup: InlineKeyboardProvider.GetMarkup(CallbackMenu.Settings),
            cancellationToken: container.Token);
    }

    private void HandleClose(UpdateContainer container)
    {
        var chatId = container.ChatId;
        var messageId = container.Message.Id;
        container.BotClient.DeleteMessage(
            chatId,
            messageId,
            container.Token);
        DataService.UpdateUserStatus(container, UserStatus.Active, _uService);
    }

    private void HandleUnknown(UpdateContainer container)
    {
        _log.Error("Unknown button");
    }
}