using System.Globalization;
using Com.Github.PatBatTB.GEBB.DataBase.Event;
using Com.Github.PatBatTB.GEBB.DataBase.User;
using Com.Github.PatBatTB.GEBB.Domain;
using Com.Github.PatBatTB.GEBB.Domain.Enums;
using Com.Github.PatBatTB.GEBB.Services.Providers;
using Telegram.Bot;

namespace Com.Github.PatBatTB.GEBB.Services.Handlers.Updates.Types.Callback.Menu;

public static class CreateEventHandler
{
    //TODO Изменить способ идентификации мероприятия. Не по MessageID, а что бы брался из БД.
    //Необходимо для унификации формы создания эвента и изменения. 
    private static readonly Dictionary<CallbackButton, Action<UpdateContainer>> ButtonHandlerDict = new()
    {
        [CallbackButton.Title] = HandleTitle,
        [CallbackButton.TitleDone] = HandleTitleDone,
        [CallbackButton.DateTimeOf] = HandleDateTime,
        [CallbackButton.DateTimeOfDone] = HandleDateTimeDone,
        [CallbackButton.Address] = HandleAddress,
        [CallbackButton.AddressDone] = HandleAddressDone,
        [CallbackButton.Cost] = HandleCost,
        [CallbackButton.CostDone] = HandleCostDone,
        [CallbackButton.PartLimit] = HandleParticipantLimit,
        [CallbackButton.PartLimitDone] = HandleParticipantLimitDone,
        [CallbackButton.Descr] = HandleDescription,
        [CallbackButton.DescrDone] = HandleDescriptionDone,
        [CallbackButton.FinishCreating] = HandleFinishCreating,
        [CallbackButton.Close] = HandleClose,
    };

    private static readonly IUserService UService = new DbUserService();
    private static readonly IEventService EService = new DbEventService();

    public static void Handle(UpdateContainer container)
    {
        container.Events.AddRange(EService.GetInCreating(container.AppUser.UserId));

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
                ? "Ошибка. Не обнаружено мероприятий в режиме создания.\nПопробуйте снова через команду /menu"
                : "Ошибка. Обнаружено несколько мероприятий в режиме создания.\nПопробуйте снова через команду /menu";
            Thread.Sleep(200);
            container.BotClient.SendMessage(
                chatId: container.ChatId,
                text: message,
                cancellationToken: container.Token);
            return;
        }

        if (container.CallbackData?.Button is not { } button)
            throw new NullReferenceException("CallbackData doesn't have button");
        ButtonHandlerDict.GetValueOrDefault(button, CreateEventMenuUnknownButton)
            .Invoke(container);
    }

    private static void HandleTitle(UpdateContainer container)
    {
        MessageSender.SendEnterDataRequest(container, CreateEventStatus.Title);
    }

    private static void HandleTitleDone(UpdateContainer container)
    {
        MessageSender.SendReplaceDataMenu(container, CallbackMenu.EventTitleReplace);
    }

    private static void HandleDateTime(UpdateContainer container)
    {
        MessageSender.SendEnterDataRequest(container, CreateEventStatus.DateTimeOf);
    }

    private static void HandleDateTimeDone(UpdateContainer container)
    {
        MessageSender.SendReplaceDataMenu(container, CallbackMenu.EventDateTimeOfReplace);
    }

    private static void HandleAddress(UpdateContainer container)
    {
        MessageSender.SendEnterDataRequest(container, CreateEventStatus.Address);
    }

    private static void HandleAddressDone(UpdateContainer container)
    {
        MessageSender.SendReplaceDataMenu(container, CallbackMenu.EventAddressReplace);
    }

    private static void HandleCost(UpdateContainer container)
    {
        MessageSender.SendEnterDataRequest(container, CreateEventStatus.Cost);
    }

    private static void HandleCostDone(UpdateContainer container)
    {
        MessageSender.SendReplaceDataMenu(container, CallbackMenu.EventCostReplace);
    }

    private static void HandleParticipantLimit(UpdateContainer container)
    {
        MessageSender.SendEnterDataRequest(container, CreateEventStatus.ParticipantLimit);
    }

    private static void HandleParticipantLimitDone(UpdateContainer container)
    {
        MessageSender.SendReplaceDataMenu(container, CallbackMenu.EventPartLimitReplace);
    }

    private static void HandleDescription(UpdateContainer container)
    {
        MessageSender.SendEnterDataRequest(container, CreateEventStatus.Description);
    }

    private static void HandleDescriptionDone(UpdateContainer container)
    {
        MessageSender.SendReplaceDataMenu(container, CallbackMenu.EventDescrReplace);
    }

    private static void HandleFinishCreating(UpdateContainer container)
    {
        //TODO должен быть флаг мероприятие создается или меняется. От этого зависит список рассылки и форма сообщения.
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

        string text = $"@{container.AppUser.Username} приглашает на мероприятие!\n" +
                      $"Название: {appEvent.Title}\n" +
                      $"Дата: {appEvent.DateTimeOf!.Value.ToString("ddd dd MMMM yyyy", new CultureInfo("ru-RU"))}\n" +
                      $"Время: {appEvent.DateTimeOf!.Value:HH:mm}\n" +
                      $"Место: {appEvent.Address}\n" +
                      $"Максимум человек: {appEvent.ParticipantLimit}\n" +
                      $"Планируемые затраты: {appEvent.Cost}\n" +
                      (string.IsNullOrEmpty(appEvent.Description)
                          ? ""
                          : $"Дополнительная информация: {appEvent.Description}");
        CallbackData data = new()
            { Button = CallbackButton.Reg, Menu = CallbackMenu.RegisterToEvent, EventId = appEvent.Id };

        foreach (AppUser user in UService.GetInviteList(appEvent))
        {
            Thread.Sleep(200);
            container.BotClient.SendMessage(
                chatId: user.UserId,
                text: text,
                replyMarkup: InlineKeyboardProvider.RegistrationMarkup(data),
                cancellationToken: container.Token);
        }
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

        EService.RemoveInCreating(container.ChatId);
    }

    private static void CreateEventMenuUnknownButton(UpdateContainer container)
    {
        Console.WriteLine("MenuButtonHandler.CreateEventMenuUnknownButton()");
    }
}