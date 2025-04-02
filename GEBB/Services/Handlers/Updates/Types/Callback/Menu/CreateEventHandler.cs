using System.Globalization;
using Com.Github.PatBatTB.GEBB.DataBase.Event;
using Com.Github.PatBatTB.GEBB.DataBase.User;
using Com.Github.PatBatTB.GEBB.Domain;
using Com.Github.PatBatTB.GEBB.Domain.Enums;
using Com.Github.PatBatTB.GEBB.Services.Providers;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Com.Github.PatBatTB.GEBB.Services.Handlers.Updates.Types.Callback.Menu;

public static class CreateEventHandler
{
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
        [CallbackButton.ParticipantLimit] = HandleParticipantLimit,
        [CallbackButton.ParticipantLimitDone] = HandleParticipantLimitDone,
        [CallbackButton.Description] = HandleDescription,
        [CallbackButton.DescriptionDone] = HandleDescriptionDone,
        [CallbackButton.FinishCreating] = HandleFinishCreating,
        [CallbackButton.Close] = HandleClose,
    };

    private static readonly IUserService UService = new DbUserService();
    private static readonly IEventService EService = new DbEventService();

    public static void Handle(UpdateContainer container)
    {
        container.Events.AddRange(EService.GetInCreating(container.UserDto.UserId));

        int count = container.Events.Count;
        if (count is 0 or > 1)
        {
            container.BotClient.DeleteMessage(
                container.ChatId,
                container.Message.MessageId,
                container.Token);
            Thread.Sleep(200);

            EService.Remove(container.Events);

            container.UserDto.UserStatus = UserStatus.Active;
            UService.Merge(container.UserDto);
            string message = (count == 0)
                ? "Ошибка. Не обнаружено мероприятий в режиме создания.\nПопробуйте снова через команду /menu"
                : "Ошибка. Обнаружено несколько мероприятий в режиме создания.\nПопробуйте снова через команду /menu";
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
        MessageSender.SendReplaceDataMenu(container, CallbackMenu.EventParticipantLimitReplace);
    }

    private static void HandleDescription(UpdateContainer container)
    {
        MessageSender.SendEnterDataRequest(container, CreateEventStatus.Description);
    }

    private static void HandleDescriptionDone(UpdateContainer container)
    {
        MessageSender.SendReplaceDataMenu(container, CallbackMenu.EventDescriptionReplace);
    }

    private static void HandleFinishCreating(UpdateContainer container)
    {
        //Проверить все ли данные введены в EventEntity?
        EventDto eventDto = container.Events[0];
        string message;
        if (eventDto.Title is null ||
            eventDto.DateTimeOf is null ||
            eventDto.Address is null ||
            eventDto.Cost is null ||
            eventDto.ParticipantLimit is null)
        {
            message = "Сначала укажите все данные для мероприятия.";
            container.BotClient.AnswerCallbackQuery(container.CallbackData!.CallbackId!, message, true,
                cancellationToken: container.Token);
            return;
        }

        //отправить уведомление, что мероприятие создано
        message = "Мероприятие создано. Рассылаю приглашения.";
        container.BotClient.AnswerCallbackQuery(container.CallbackData!.CallbackId!, message, true,
            cancellationToken: container.Token);
        Thread.Sleep(200);
        //Изменить статус IsCreateComplete на true
        EService.FinishCreating(eventDto);
        //изменить статус пользователя
        container.UserDto.UserStatus = UserStatus.Active;
        UService.Merge(container.UserDto);
        container.BotClient.SetMyCommands(
            BotCommandProvider.GetCommandMenu(container.UserDto.UserStatus),
            BotCommandScope.Chat(container.ChatId),
            cancellationToken: container.Token);
        Thread.Sleep(200);
        container.BotClient.DeleteMessage(
            container.ChatId,
            container.Message.Id,
            container.Token);

        //Запросить лист пользователей (все пользователи, кроме инициатора, со статусом не stop)
        //Отправить приглашения всем пользователям из списка.
        string text = $"@{container.UserDto.Username} приглашает на мероприятие!\n" +
                      $"Название: {eventDto.Title}\n" +
                      $"Дата: {eventDto.DateTimeOf!.Value.ToString("ddd dd MMMM yyyy", new CultureInfo("ru-RU"))}\n" +
                      $"Время: {eventDto.DateTimeOf!.Value:HH:mm}\n" +
                      $"Место: {eventDto.Address}\n" +
                      $"Максимум человек: {eventDto.ParticipantLimit}\n" +
                      $"Планируемые затраты: {eventDto.Cost}\n" +
                      (string.IsNullOrEmpty(eventDto.Description)
                          ? ""
                          : $"Дополнительная информация: {eventDto.Description}");
        //TODO Test sending message
        CallbackData data = new()
            { Button = CallbackButton.Registration, Menu = CallbackMenu.EventRegister, EventId = eventDto.EventId };

        foreach (UserDto user in UService.GetInviteList(eventDto))
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

        container.UserDto.UserStatus = UserStatus.Active;
        UService.Merge(container.UserDto);

        container.BotClient.SetMyCommands(
            BotCommandProvider.GetCommandMenu(container.UserDto.UserStatus),
            BotCommandScope.Chat(container.ChatId),
            cancellationToken: container.Token);
        EService.Remove(eventId);
    }

    private static void CreateEventMenuUnknownButton(UpdateContainer container)
    {
        Console.WriteLine("MenuButtonHandler.CreateEventMenuUnknownButton()");
    }
}