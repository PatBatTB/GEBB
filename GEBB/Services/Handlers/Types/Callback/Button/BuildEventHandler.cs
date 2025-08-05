using System.Globalization;
using System.Text;
using Com.GitHub.PatBatTB.GEBB;
using Com.Github.PatBatTB.GEBB.DataBase.Alarm;
using Com.Github.PatBatTB.GEBB.DataBase.Event;
using Com.Github.PatBatTB.GEBB.DataBase.User;
using Com.Github.PatBatTB.GEBB.Domain;
using Com.Github.PatBatTB.GEBB.Domain.Enums;
using Com.GitHub.PatBatTB.GEBB.Exceptions;
using Com.Github.PatBatTB.GEBB.Services.Providers;
using log4net;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace Com.Github.PatBatTB.GEBB.Services.Handlers.Types.Callback.Button;

public  class BuildEventHandler
{
    private readonly Dictionary<(CallbackButton, EventStatus), Action<UpdateContainer>> _buttonHandlerDict;

    private readonly IUserService _uService;
    private readonly IEventService _eService;
    private readonly IAlarmService _aService;

    private readonly ILog _log;

    public BuildEventHandler()
    {
        _uService = App.ServiceFactory.GetUserService();
        _eService = App.ServiceFactory.GetEventService();
        _aService = App.ServiceFactory.GetAlarmService();
        _log = LogManager.GetLogger(typeof(BuildEventHandler));
        _buttonHandlerDict = new Dictionary<(CallbackButton, EventStatus), Action<UpdateContainer>>
        {
            [(CallbackButton.Title, EventStatus.Creating)] = HandleTitle,
            [(CallbackButton.TitleDone, EventStatus.Creating)] = HandleTitleDone,
            [(CallbackButton.DateTimeOf, EventStatus.Creating)] = HandleDateTime,
            [(CallbackButton.DateTimeOfDone, EventStatus.Creating)] = HandleDateTimeDone,
            [(CallbackButton.Address, EventStatus.Creating)] = HandleAddress,
            [(CallbackButton.AddressDone, EventStatus.Creating)] = HandleAddressDone,
            [(CallbackButton.Cost, EventStatus.Creating)] = HandleCost,
            [(CallbackButton.CostDone, EventStatus.Creating)] = HandleCostDone,
            [(CallbackButton.PartLimit, EventStatus.Creating)] = HandleParticipantLimit,
            [(CallbackButton.PartLimitDone, EventStatus.Creating)] = HandleParticipantLimitDone,
            [(CallbackButton.Descr, EventStatus.Creating)] = HandleDescription,
            [(CallbackButton.DescrDone, EventStatus.Creating)] = HandleDescriptionDone,
            [(CallbackButton.FinishBuilding, EventStatus.Creating)] = HandleFinishCreating,
            [(CallbackButton.Close, EventStatus.Creating)] = HandleClose,
            [(CallbackButton.Title, EventStatus.Editing)] = HandleEditTitle,
            [(CallbackButton.DateTimeOf, EventStatus.Editing)] = HandleEditDateTimeOf,
            [(CallbackButton.Address, EventStatus.Editing)] = HandleEditAddress,
            [(CallbackButton.Cost, EventStatus.Editing)] = HandleEditCost,
            [(CallbackButton.PartLimit, EventStatus.Editing)] = HandleEditParticipantLimit,
            [(CallbackButton.Descr, EventStatus.Editing)] = HandleEditDescription,
            [(CallbackButton.FinishBuilding, EventStatus.Editing)] = HandleFinishEditing,
            [(CallbackButton.Close, EventStatus.Editing)] = HandleClose,
        };
    }

    public void Handle(UpdateContainer container)
    {
        (EventStatus status, string text) = (container.CallbackData!.Menu!) switch
        {
            CallbackMenu.CreateEvent => (EventStatus.Creating,  "создания"),
            CallbackMenu.EditEvent => (EventStatus.Editing, "редактирования" ),
            _ => throw new InvalidOperationException("Invalid menu in event building.")
        };
        container.Events.AddRange(_eService.GetBuildEvents(container.AppUser.UserId, status)); 

        int count = container.Events.Count;
        if (count is 0 or > 1)
        {
            Thread.Sleep(200);
            container.BotClient.DeleteMessage(
                container.ChatId,
                container.Message.MessageId,
                container.Token);
            
            _eService.Remove(container.Events);
            DataService.UpdateUserStatus(container, UserStatus.Active, _uService);
            
            string message = (count == 0)
                ? $"Ошибка. Не обнаружено мероприятий в режиме {text}.\nПопробуйте снова через команду /menu"
                : $"Ошибка. Обнаружено несколько мероприятий в режиме {text}.\nПопробуйте снова через команду /menu";
            Thread.Sleep(200);
            container.BotClient.SendMessage(
                chatId: container.ChatId,
                text: message,
                cancellationToken: container.Token);
            return;
        }

        if (container.CallbackData?.Button is not { } button)
        {
            _log.Error("CallbackData doesn't have button");
            throw new NullReferenceException("CallbackData doesn't have button");
        }
        _buttonHandlerDict.GetValueOrDefault((button, container.Events[0].Status), CreateEventMenuUnknownButton)
            .Invoke(container);
    }

    private void HandleTitle(UpdateContainer container)
    {
        MessageSender.SendEnterDataRequest(container, BuildEventStatus.CreateTitle);
    }

    private void HandleTitleDone(UpdateContainer container)
    {
        MessageSender.SendReplaceDataMenu(container, CallbackMenu.EventTitleReplace);
    }

    private void HandleDateTime(UpdateContainer container)
    {
        MessageSender.SendEnterDataRequest(container, BuildEventStatus.CreateDateTimeOf);
    }

    private void HandleDateTimeDone(UpdateContainer container)
    {
        MessageSender.SendReplaceDataMenu(container, CallbackMenu.EventDateTimeOfReplace);
    }

    private void HandleAddress(UpdateContainer container)
    {
        MessageSender.SendEnterDataRequest(container, BuildEventStatus.CreateAddress);
    }

    private void HandleAddressDone(UpdateContainer container)
    {
        MessageSender.SendReplaceDataMenu(container, CallbackMenu.EventAddressReplace);
    }

    private void HandleCost(UpdateContainer container)
    {
        MessageSender.SendEnterDataRequest(container, BuildEventStatus.CreateCost);
    }

    private void HandleCostDone(UpdateContainer container)
    {
        MessageSender.SendReplaceDataMenu(container, CallbackMenu.EventCostReplace);
    }

    private void HandleParticipantLimit(UpdateContainer container)
    {
        MessageSender.SendEnterDataRequest(container, BuildEventStatus.CreateParticipantLimit);
    }

    private void HandleParticipantLimitDone(UpdateContainer container)
    {
        MessageSender.SendReplaceDataMenu(container, CallbackMenu.EventPartLimitReplace);
    }

    private void HandleDescription(UpdateContainer container)
    {
        MessageSender.SendEnterDataRequest(container, BuildEventStatus.CreateDescription);
    }

    private void HandleDescriptionDone(UpdateContainer container)
    {
        MessageSender.SendReplaceDataMenu(container, CallbackMenu.EventDescrReplace);
    }

    private void HandleEditTitle(UpdateContainer container)
    {
        MessageSender.SendEnterDataRequest(container, BuildEventStatus.EditTitle);
    }

    private void HandleEditDateTimeOf(UpdateContainer container)
    {
        MessageSender.SendEnterDataRequest(container, BuildEventStatus.EditDateTimeOf);
    }

    private void HandleEditAddress(UpdateContainer container)
    {
        MessageSender.SendEnterDataRequest(container, BuildEventStatus.EditAddress);
    }

    private void HandleEditCost(UpdateContainer container)
    {
        MessageSender.SendEnterDataRequest(container, BuildEventStatus.EditCost);
    }

    private void HandleEditParticipantLimit(UpdateContainer container)
    {
        MessageSender.SendEnterDataRequest(container, BuildEventStatus.EditParticipantLimit);
    }

    private void HandleEditDescription(UpdateContainer container)
    {
        MessageSender.SendEnterDataRequest(container, BuildEventStatus.EditDescription);
    }

    private void HandleFinishCreating(UpdateContainer container)
    {
        AppEvent appEvent = container.Events[0];
        string message;
        if (appEvent.Title is null ||
            appEvent.DateTimeOf is null ||
            appEvent.Address is null ||
            appEvent.Cost is null ||
            appEvent.ParticipantLimit is null)
        {
            message = "Сначала укажите все данные для мероприятия.";
            container.BotClient.AnswerCallbackQuery(container.CallbackData!.CallbackId!, message, true,
                cancellationToken: container.Token);
            return;
        }

        message = "Мероприятие создано. Рассылаю приглашения.";
        container.BotClient.AnswerCallbackQuery(container.CallbackData!.CallbackId!, message, true,
            cancellationToken: container.Token);
        Thread.Sleep(200);
        _eService.FinishCreating(appEvent);
        DataService.UpdateUserStatus(container, UserStatus.Active, _uService);
        Thread.Sleep(200);
        container.BotClient.DeleteMessage(
            container.ChatId,
            container.Message.Id,
            container.Token);
        _aService.Update(new AppAlarm { Event = appEvent, User = container.AppUser, LastAlert = DateTime.Now} );

        foreach (AppUser user in _uService.GetInviteList(appEvent))
        {
            Thread.Sleep(200);
            container.BotClient.SendMessage(
                chatId: user.UserId,
                text: MessageService.GetUserInvitesToEventString(container.AppUser, appEvent),
                replyMarkup: InlineKeyboardProvider.GetMarkup(CallbackMenu.RegisterToEvent, appEvent.Id),
                cancellationToken: container.Token);
        }
    }

    private void HandleFinishEditing(UpdateContainer container)
    {
        try
        {
            _eService.FinishEditing(container.Events[0], out AppEvent oldEvent, out AppEvent newEvent);
            if (GetChangesHeader(oldEvent, newEvent, out string messageText))
            {
                Thread.Sleep(200);
                container.BotClient.AnswerCallbackQuery(
                    callbackQueryId: container.CallbackData!.CallbackId!,
                    text: "Мероприятие изменено.\nРассылаю уведомления зарегистрировавшимся пользователям.",
                    showAlert: true,
                    cancellationToken: container.Token);
                List<long> userIds = oldEvent.RegisteredUsers.Select(elem => elem.UserId).ToList();
                foreach (long userId in userIds)
                {
                    Thread.Sleep(200);
                    container.BotClient.SendMessage(
                        chatId: userId,
                        text: messageText,
                        parseMode: ParseMode.Html,
                        cancellationToken: container.Token);
                }
                _aService.Update(new AppAlarm { Event = oldEvent, User = container.AppUser, LastAlert = DateTime.Now} );
            }
            else
            {
                Thread.Sleep(200);
                container.BotClient.AnswerCallbackQuery(
                    callbackQueryId: container.CallbackData!.CallbackId!,
                    text: "Редактирование отменено.\nВы не внесли никаких изменений.",
                    showAlert: true,
                    cancellationToken: container.Token);
            }
            
            DataService.UpdateUserStatus(container, UserStatus.Active, _uService);
        }
        catch (EntityNotFoundException)
        {
            Thread.Sleep(200);
            container.BotClient.AnswerCallbackQuery(
                callbackQueryId: container.CallbackData!.CallbackId!,
                text: "Произошла ошибка.\nМероприятие не найдено.",
                showAlert: true,
                cancellationToken: container.Token);
        }
        container.BotClient.DeleteMessage(container.ChatId, container.Message.Id, container.Token);
    }

    private void HandleClose(UpdateContainer container)
    {
        var chatId = container.ChatId;
        var messageId = container.Message.Id;
        Thread.Sleep(200);
        container.BotClient.DeleteMessage(
            chatId,
            messageId,
            container.Token);

        DataService.UpdateUserStatus(container, UserStatus.Active, _uService);
        _eService.RemoveInBuilding(container.ChatId, container.Events[0].Status);
    }

    private bool GetChangesHeader(AppEvent oldEvent, AppEvent newEvent, out string messageText)
    {
        StringBuilder messageTextBuilder = new();
        bool isChanged = false;
        messageTextBuilder.Append("Мероприятие изменено!\n");
        messageTextBuilder.Append("Название: ");
        
        if (oldEvent.Title != newEvent.Title)
        {
            messageTextBuilder.Append($"<s>{oldEvent.Title}</s> -> <b>{newEvent.Title}</b>\n");
            isChanged = true;
        }
        else
        {
            messageTextBuilder.Append(oldEvent.Title + "\n");
        }
        
        string oldDate = oldEvent.DateTimeOf!.Value.ToString("ddd dd MMMM yyyy", new CultureInfo("ru-RU"));
        string newDate = newEvent.DateTimeOf!.Value.ToString("ddd dd MMMM yyyy", new CultureInfo("ru-RU"));
        messageTextBuilder.Append("Дата: ");
        
        if (oldDate != newDate)
        {
            messageTextBuilder.Append($"<s>{oldDate}</s> -> <b>{newDate}</b>\n");
            isChanged = true;
        }
        else
        {
            messageTextBuilder.Append(oldDate + "\n");
        }
        
        messageTextBuilder.Append("Время: ");
        string oldTime = $"{oldEvent.DateTimeOf!.Value:HH:mm}";
        string newTime = $"{newEvent.DateTimeOf!.Value:HH:mm}";
        
        if (oldTime != newTime)
        {
            messageTextBuilder.Append($"<s>{oldTime}</s> -> <b>{newTime}</b>\n");
            isChanged = true;
        }
        
        else
        {
            messageTextBuilder.Append(oldTime + "\n");
        }
        
        if (oldEvent.Address != newEvent.Address)
        {
            messageTextBuilder.Append($"Место: <s>{oldEvent.Address}</s> -> <b>{newEvent.Address}</b>\n");
            
        }
        
        if (oldEvent.ParticipantLimit != newEvent.ParticipantLimit)
        {
            messageTextBuilder.Append($"Максимум человек: <s>{oldEvent.ParticipantLimit}</s> -> <b>{newEvent.ParticipantLimit}</b>\n");
            isChanged = true;
        }

        if (oldEvent.Cost != newEvent.Cost)
        {
            messageTextBuilder.Append($"Максимум человек: <s>{oldEvent.Cost}</s> -> <b>{newEvent.Cost}</b>\n");
            isChanged = true;
        }
        
        if (oldEvent.Description != newEvent.Description)
        {
            messageTextBuilder.Append($"Дополнительная информация: <s>{oldEvent.Description}</s> -> <b>{newEvent.Description}</b>\n");
            isChanged = true;
        }
        
        messageText = isChanged ? messageTextBuilder.ToString() : "";
        return isChanged;
    }

    private void CreateEventMenuUnknownButton(UpdateContainer container)
    {
        _log.Error("Unknown button");
    }
}