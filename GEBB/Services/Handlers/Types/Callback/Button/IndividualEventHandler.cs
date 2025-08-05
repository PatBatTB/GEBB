using System.Globalization;
using System.Text;
using Com.GitHub.PatBatTB.GEBB;
using Com.Github.PatBatTB.GEBB.DataBase.Alarm;
using Com.Github.PatBatTB.GEBB.DataBase.Event;
using Com.Github.PatBatTB.GEBB.DataBase.Message;
using Com.Github.PatBatTB.GEBB.DataBase.User;
using Com.Github.PatBatTB.GEBB.Domain;
using Com.Github.PatBatTB.GEBB.Domain.Enums;
using Com.GitHub.PatBatTB.GEBB.Exceptions;
using Com.Github.PatBatTB.GEBB.Services.Providers;
using log4net;
using Microsoft.Data.Sqlite;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Com.Github.PatBatTB.GEBB.Services.Handlers.Types.Callback.Button;

public class IndividualEventHandler
{
    private readonly Dictionary<CallbackButton, Action<UpdateContainer>> _myOwnButtonDict;
    private readonly Dictionary<CallbackButton, Action<UpdateContainer>> _myOwnButtonPartDict;
    private readonly Dictionary<CallbackButton, Action<UpdateContainer>> _registeredButtonDescrDict;
    private readonly Dictionary<CallbackButton, Action<UpdateContainer>> _registeredButtonPartDict;
    private readonly Dictionary<CallbackButton, Action<UpdateContainer>> _registerButtonDist;
    
    private readonly IEventService _eService = App.ServiceFactory.GetEventService();
    private readonly IUserService _uService = App.ServiceFactory.GetUserService();
    private readonly IAlarmService _aService = App.ServiceFactory.GetAlarmService();
    private readonly IEventMessageService _esService = App.ServiceFactory.GetEventMessageService();
    
    private readonly ILog _log = LogManager.GetLogger(typeof(IndividualEventHandler));

    public IndividualEventHandler()
    {
        _myOwnButtonDict = new Dictionary<CallbackButton, Action<UpdateContainer>>
        {
            [CallbackButton.PartList] = HandleMyOwnParticipantList,
            [CallbackButton.EventMessage] = HandleEventMessage,
            [CallbackButton.Edit] = HandleEdit,
            [CallbackButton.Cancel] = HandleCancel,
            [CallbackButton.Close] = HandleClose,
        };
        _myOwnButtonPartDict = new Dictionary<CallbackButton, Action<UpdateContainer>>
        {
            [CallbackButton.ToDescr] = HandleMyOwnToDescription,
            [CallbackButton.EventMessage] = HandleEventMessage,
            [CallbackButton.Edit] = HandleEdit,
            [CallbackButton.Cancel] = HandleCancel,
            [CallbackButton.Close] = HandleClose,
        };
        _registeredButtonDescrDict = new Dictionary<CallbackButton, Action<UpdateContainer>>
        {
            [CallbackButton.PartList] = HandleParticipantList,
            [CallbackButton.EventMessage] = HandleEventMessage,
            [CallbackButton.CancelReg] = HandleCancelRegistration,
            [CallbackButton.Close] = HandleClose,
        };
        _registeredButtonPartDict = new Dictionary<CallbackButton, Action<UpdateContainer>>
        {
            [CallbackButton.ToDescr] = HandleToDescription,
            [CallbackButton.EventMessage] = HandleEventMessage,
            [CallbackButton.CancelReg] = HandleCancelRegistration,
            [CallbackButton.Close] = HandleClose,
        };
        _registerButtonDist = new Dictionary<CallbackButton, Action<UpdateContainer>>
        {
            [CallbackButton.Reg] = HandleRegisterToEvent,
            [CallbackButton.Close] = HandleClose,
        };
    }

    public void HandleMyOwn(UpdateContainer container)
    {
        if (container.CallbackData?.Button is not { } button)
            throw new NullReferenceException("CallbackData doesn't have button");
        _myOwnButtonDict.GetValueOrDefault(button, HandleUnknown).Invoke(container);
    }

    public void HandleMyOwnPart(UpdateContainer container)
    {
        if (container.CallbackData?.Button is not { } button)
            throw new NullReferenceException("CallbackData doesn't have button");
        _myOwnButtonPartDict.GetValueOrDefault(button, HandleUnknown).Invoke(container);
    }

    public void HandleRegisteredDescr(UpdateContainer container)
    {
        if (container.CallbackData?.Button is not { } button)
            throw new NullReferenceException("CallbackData doesn't have button");
        _registeredButtonDescrDict.GetValueOrDefault(button, HandleUnknown).Invoke(container);
    }

    public void HandleRegisteredPart(UpdateContainer container)
    {
        if (container.CallbackData?.Button is not { } button)
            throw new NullReferenceException("CallbackData doesn't have button");
        _registeredButtonPartDict.GetValueOrDefault(button, HandleUnknown).Invoke(container);
    }

    public void HandleRegister(UpdateContainer container)
    {
        if (container.CallbackData?.Button is not { } button)
            throw new NullReferenceException("CallbackData doesn't have button");
        _registerButtonDist.GetValueOrDefault(button, HandleUnknown).Invoke(container);
    }

    private void HandleMyOwnParticipantList(UpdateContainer container)
    {
        AppEvent appEvent = _eService.Get(container.CallbackData!.EventId!);
        string participantList = GetParticipantList(appEvent);
        string text = $"Название: {appEvent.Title}\n" +
                      $"Участники:\n" +
                      $"{participantList}";
        Thread.Sleep(200);
        container.BotClient.EditMessageText(
            container.ChatId,
            container.Message.Id,
            text: text,
            replyMarkup: InlineKeyboardProvider.GetMarkup(CallbackMenu.CreEventPart, appEvent.Id),
            cancellationToken: container.Token);
    }

    private void HandleMyOwnToDescription(UpdateContainer container)
    {
        AppEvent appEvent = _eService.Get(container.CallbackData!.EventId!);
        Thread.Sleep(200);
        container.BotClient.EditMessageText(
            chatId: container.ChatId,
            messageId: container.Message.Id,
            text: MessageService.GetMyEventDescription(appEvent),
            replyMarkup: InlineKeyboardProvider.GetMarkup(CallbackMenu.CreatedEvent, appEvent.Id),  
            cancellationToken: container.Token);
    }

    private void HandleEdit(UpdateContainer container)
    {
        try
        {
            container.Events.Add(_eService.Get(container.CallbackData!.EventId!));
            List<AppEvent> appEvents = new();
            appEvents.AddRange(_eService.GetBuildEvents(container.ChatId, EventStatus.Editing));
            if (appEvents.Count > 0) throw new InvalidOperationException();
            Thread.Sleep(200);
            Message sent = container.BotClient.SendMessage(
                chatId: container.ChatId,
                text: $"Редактирование мероприятия:\n{container.Events[0].Title}",
                cancellationToken: container.Token).Result;
            container.Events[0].MessageId = sent.Id;
            container.Events[0] = _eService.Edit(container.Events[0]);
            Thread.Sleep(100);
            container.BotClient.EditMessageReplyMarkup(
                chatId: container.ChatId,
                messageId: sent.Id,
                replyMarkup: InlineKeyboardProvider.GetMarkup(CallbackMenu.EditEvent, container.Events[0].Id),
                cancellationToken: container.Token);
            DataService.UpdateUserStatus(container, UserStatus.EditingEvent, _uService);
        }
        catch (EntityNotFoundException)
        {
            Thread.Sleep(200);
            container.BotClient.AnswerCallbackQuery(
                callbackQueryId: container.CallbackData!.CallbackId!,
                text: "Произошла ошибка, текущее мероприятие не найдено в базе.",
                showAlert: true,
                cancellationToken: container.Token);
        }
        catch (InvalidOperationException)
        {
            Thread.Sleep(200);
            container.BotClient.AnswerCallbackQuery(
                callbackQueryId: container.CallbackData!.CallbackId!,
                text: "Произошла ошибка. В базе найдено другое мероприятие, которые вы редактируете. Попробуйте снова.",
                showAlert: true,
                cancellationToken: container.Token);
            ICollection<int> editingIds = _eService.RemoveInBuilding(container.AppUser.UserId, EventStatus.Editing);
            container.BotClient.DeleteMessages(container.ChatId, editingIds, container.Token);
        }
        Thread.Sleep(200);
        container.BotClient.DeleteMessage(container.ChatId, container.Message.Id, container.Token);
    }

    private void HandleCancel(UpdateContainer container)
    {
        AppEvent appEvent = _eService.Get(container.CallbackData!.EventId!);
        _eService.Remove(container.CallbackData!.EventId!);
        if (appEvent.DateTimeOf > DateTime.Now)
        {
            Thread.Sleep(200);
            container.BotClient.AnswerCallbackQuery(
                callbackQueryId: container.CallbackData.CallbackId!,
                text: "Мероприятие отменено.",
                showAlert: true,
                cancellationToken: container.Token);
            string text = $"Мероприятие {appEvent.Title}\n" +
                          $"{appEvent.DateTimeOf!.Value.ToString("ddd dd MMMM yyyy", new CultureInfo("ru-RU"))}\n" +
                          $"{appEvent.DateTimeOf!.Value:HH:mm}\n" +
                          $"отменено организатором.";
            foreach (AppUser appUser in appEvent.RegisteredUsers)
            {
                Thread.Sleep(200);
                container.BotClient.SendMessage(
                    chatId: appUser.UserId,
                    text: text,
                    cancellationToken: container.Token);
            }
        }
        else
        {
            Thread.Sleep(200);
            container.BotClient.AnswerCallbackQuery(
                callbackQueryId: container.CallbackData.CallbackId!,
                text: "Мероприятие уже завершилось.",
                showAlert: true,
                cancellationToken: container.Token);
        }
        

        HandleClose(container);
    }

    private void HandleParticipantList(UpdateContainer container)
    {
        AppEvent appEvent = _eService.Get(container.CallbackData?.EventId!);
        string participantList = GetParticipantList(appEvent);
        
        string text = $"Название: {appEvent.Title}\n" +
                      $"Организатор: @{appEvent.Creator.Username}\n" +
                      $"Участники:\n" +
                      $"{participantList}";
        Thread.Sleep(200);
        container.BotClient.EditMessageText(
            container.ChatId,
            container.Message.Id,
            text: text,
            replyMarkup: InlineKeyboardProvider.GetMarkup(CallbackMenu.RegEventPart, appEvent.Id),
            cancellationToken: container.Token);
    }

    private void HandleCancelRegistration(UpdateContainer container)
    {
        AppEvent appEvent = _eService.Get(container.CallbackData?.EventId!);
        _eService.CancelRegistration(appEvent, container.AppUser);
        container.BotClient.DeleteMessage(
            chatId: container.ChatId,
            messageId: container.Message.Id,
            cancellationToken: container.Token);
        Thread.Sleep(200);
        container.BotClient.AnswerCallbackQuery(
            callbackQueryId: container.CallbackData!.CallbackId!,
            text: "Вы отменили регистрацию на мероприятие.",
            showAlert: true,
            cancellationToken: container.Token);
    }

    private void HandleToDescription(UpdateContainer container)
    {
        AppEvent appEvent = _eService.Get(container.CallbackData?.EventId!);
        Thread.Sleep(200);
        container.BotClient.EditMessageText(
            chatId: container.ChatId,
            messageId: container.Message.Id,
            text: MessageService.GetEventDescription(appEvent),
            replyMarkup: InlineKeyboardProvider.GetMarkup(CallbackMenu.RegEventDescr, appEvent.Id),
            cancellationToken: container.Token);
    }

    private void HandleRegisterToEvent(UpdateContainer container)
    {
        try
        {
            AppEvent appEvent = _eService.Get(container.CallbackData!.EventId!);
            
            if (appEvent.Status == EventStatus.Deleted)
            {
                throw new EventNotValidException("Данное мероприятие было отменено организатором.");
            }
            
            if (appEvent.DateTimeOf <= DateTime.Now)
            {
                throw new EventNotValidException("Данное мероприятие уже завершилось.");
            }

            if (appEvent.ParticipantLimit > 0 && appEvent.ParticipantLimit <= appEvent.RegisteredUsers.Count)
            {
                throw new EventNotValidException("К сожалению, места на это мероприятие закончились.");
            }
            
            _eService.RegisterUser(appEvent, container.AppUser);
            Thread.Sleep(200);
            container.BotClient.AnswerCallbackQuery(
                callbackQueryId: container.CallbackData!.CallbackId!,
                text: "Вы успешно зарегистрировались на мероприятие.",
                showAlert: true,
                cancellationToken: container.Token);
            _aService.Update(new AppAlarm { Event = appEvent, User = container.AppUser, LastAlert = DateTime.Now} );
        }
        catch (EntityNotFoundException)
        {
            Thread.Sleep(200);
            container.BotClient.AnswerCallbackQuery(
                callbackQueryId: container.CallbackData!.CallbackId!,
                text: "Произошла ошибка. Пожалуйста сообщите администратору.",
                showAlert: true,
                cancellationToken: container.Token);
        }
        catch (SqliteException e) when (e.SqliteErrorCode == 19 && e.Message.Contains("UNIQUE constraint failed"))
        {
            Thread.Sleep(200);
            container.BotClient.AnswerCallbackQuery(
                callbackQueryId: container.CallbackData!.CallbackId!,
                text: "Вы уже зарегистрированы на это мероприятие",
                showAlert: true,
                cancellationToken: container.Token);
        }
        catch (EventNotValidException e)
        {
            Thread.Sleep(200);
            container.BotClient.AnswerCallbackQuery(
                callbackQueryId: container.CallbackData!.CallbackId!,
                text: e.Message,
                showAlert: true,
                cancellationToken: container.Token);
        }
        
        Thread.Sleep(200);
        container.BotClient.DeleteMessage(container.ChatId, container.Message.Id, container.Token);
    }

    private void HandleEventMessage(UpdateContainer container)
    {
        DataService.UpdateUserStatus(container, UserStatus.SendingMessage, _uService);
        _esService.Update(new AppEventMessage
        {
            Event = _eService.Get(container.CallbackData!.EventId!),
            User = _uService.Get(container.AppUser.UserId)
        });
        Thread.Sleep(200);
        container.BotClient.SendMessage(
            chatId: container.ChatId,
            text: "Введите сообщение для отправки:",
            replyMarkup: new ForceReplyMarkup { InputFieldPlaceholder = "Текст сообщения" },
            cancellationToken: container.Token);
    }

    private void HandleClose(UpdateContainer container)
    {
        Thread.Sleep(200);
        container.BotClient.DeleteMessage(
            chatId: container.ChatId,
            messageId: container.Message.Id,
            cancellationToken: container.Token);
    }

    private string GetParticipantList(AppEvent appEvent)
    {
        StringBuilder participantList = new();
        foreach (AppUser appUser in appEvent.RegisteredUsers)
        {
            participantList.Append("@" + appUser.Username + "\n");
        }

        return participantList.ToString();
    }

    private void HandleUnknown(UpdateContainer container)
    {
        _log.Error("Unknown button");
    }
}