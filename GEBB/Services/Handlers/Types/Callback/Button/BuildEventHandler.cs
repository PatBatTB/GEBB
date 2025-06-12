using System.Globalization;
using System.Text;
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

public static class BuildEventHandler
{
    private static readonly ILog Log = LogManager.GetLogger(typeof(BuildEventHandler)); 
    
    private static readonly Dictionary<(CallbackButton, EventStatus), Action<UpdateContainer>> ButtonHandlerDict = new()
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

    private static readonly IUserService UService = new DbUserService();
    private static readonly IEventService EService = new DbEventService();

    public static void Handle(UpdateContainer container)
    {
        (EventStatus status, string text) = (container.CallbackData!.Menu!) switch
        {
            CallbackMenu.CreateEvent => (EventStatus.Creating,  "создания"),
            CallbackMenu.EditEvent => (EventStatus.Editing, "редактирования" ),
            _ => throw new InvalidOperationException("Invalid menu in event building.")
        };
        container.Events.AddRange(EService.GetBuildEvents(container.AppUser.UserId, status)); 

        int count = container.Events.Count;
        if (count is 0 or > 1)
        {
            Thread.Sleep(200);
            container.BotClient.DeleteMessage(
                container.ChatId,
                container.Message.MessageId,
                container.Token);
            
            EService.Remove(container.Events);
            DataService.UpdateUserStatus(container, UserStatus.Active, UService);
            
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
            Log.Error("CallbackData doesn't have button");
            throw new NullReferenceException("CallbackData doesn't have button");
        }
        ButtonHandlerDict.GetValueOrDefault((button, container.Events[0].Status), CreateEventMenuUnknownButton)
            .Invoke(container);
    }

    private static void HandleTitle(UpdateContainer container)
    {
        MessageSender.SendEnterDataRequest(container, BuildEventStatus.CreateTitle);
    }

    private static void HandleTitleDone(UpdateContainer container)
    {
        MessageSender.SendReplaceDataMenu(container, CallbackMenu.EventTitleReplace);
    }

    private static void HandleDateTime(UpdateContainer container)
    {
        MessageSender.SendEnterDataRequest(container, BuildEventStatus.CreateDateTimeOf);
    }

    private static void HandleDateTimeDone(UpdateContainer container)
    {
        MessageSender.SendReplaceDataMenu(container, CallbackMenu.EventDateTimeOfReplace);
    }

    private static void HandleAddress(UpdateContainer container)
    {
        MessageSender.SendEnterDataRequest(container, BuildEventStatus.CreateAddress);
    }

    private static void HandleAddressDone(UpdateContainer container)
    {
        MessageSender.SendReplaceDataMenu(container, CallbackMenu.EventAddressReplace);
    }

    private static void HandleCost(UpdateContainer container)
    {
        MessageSender.SendEnterDataRequest(container, BuildEventStatus.CreateCost);
    }

    private static void HandleCostDone(UpdateContainer container)
    {
        MessageSender.SendReplaceDataMenu(container, CallbackMenu.EventCostReplace);
    }

    private static void HandleParticipantLimit(UpdateContainer container)
    {
        MessageSender.SendEnterDataRequest(container, BuildEventStatus.CreateParticipantLimit);
    }

    private static void HandleParticipantLimitDone(UpdateContainer container)
    {
        MessageSender.SendReplaceDataMenu(container, CallbackMenu.EventPartLimitReplace);
    }

    private static void HandleDescription(UpdateContainer container)
    {
        MessageSender.SendEnterDataRequest(container, BuildEventStatus.CreateDescription);
    }

    private static void HandleDescriptionDone(UpdateContainer container)
    {
        MessageSender.SendReplaceDataMenu(container, CallbackMenu.EventDescrReplace);
    }

    private static void HandleEditTitle(UpdateContainer container)
    {
        MessageSender.SendEnterDataRequest(container, BuildEventStatus.EditTitle);
    }

    private static void HandleEditDateTimeOf(UpdateContainer container)
    {
        MessageSender.SendEnterDataRequest(container, BuildEventStatus.EditDateTimeOf);
    }

    private static void HandleEditAddress(UpdateContainer container)
    {
        MessageSender.SendEnterDataRequest(container, BuildEventStatus.EditAddress);
    }

    private static void HandleEditCost(UpdateContainer container)
    {
        MessageSender.SendEnterDataRequest(container, BuildEventStatus.EditCost);
    }

    private static void HandleEditParticipantLimit(UpdateContainer container)
    {
        MessageSender.SendEnterDataRequest(container, BuildEventStatus.EditParticipantLimit);
    }

    private static void HandleEditDescription(UpdateContainer container)
    {
        MessageSender.SendEnterDataRequest(container, BuildEventStatus.EditDescription);
    }

    private static void HandleFinishCreating(UpdateContainer container)
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
        EService.FinishCreating(appEvent);
        DataService.UpdateUserStatus(container, UserStatus.Active, UService);
        Thread.Sleep(200);
        container.BotClient.DeleteMessage(
            container.ChatId,
            container.Message.Id,
            container.Token);

        foreach (AppUser user in UService.GetInviteList(appEvent))
        {
            Thread.Sleep(200);
            container.BotClient.SendMessage(
                chatId: user.UserId,
                text: MessageService.GetUserInvitesToEventString(container.AppUser, appEvent),
                replyMarkup: InlineKeyboardProvider.GetMarkup(CallbackMenu.RegisterToEvent, appEvent.Id),
                cancellationToken: container.Token);
        }
    }

    private static void HandleFinishEditing(UpdateContainer container)
    {
        try
        {
            EService.FinishEditing(container.Events[0], out AppEvent oldEvent, out AppEvent newEvent);
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
            }
            else
            {
                container.BotClient.AnswerCallbackQuery(
                    callbackQueryId: container.CallbackData!.CallbackId!,
                    text: "Редактирование отменено.\nВы не внесли никаких изменений.",
                    showAlert: true,
                    cancellationToken: container.Token);
            }
            
            DataService.UpdateUserStatus(container, UserStatus.Active, UService);
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

    private static void HandleClose(UpdateContainer container)
    {
        var chatId = container.ChatId;
        var messageId = container.Message.Id;
        var eventId = container.CallbackData!.EventId!;
        container.BotClient.DeleteMessage(
            chatId,
            messageId,
            container.Token);

        DataService.UpdateUserStatus(container, UserStatus.Active, UService);
        EService.RemoveInBuilding(container.ChatId, container.Events[0].Status);
    }

    private static bool GetChangesHeader(AppEvent oldEvent, AppEvent newEvent, out string messageText)
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

    private static void CreateEventMenuUnknownButton(UpdateContainer container)
    {
        Log.Error("Unknown button");
    }
}