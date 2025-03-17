using System.Globalization;
using Com.Github.PatBatTB.GEBB.DataBase;
using Com.Github.PatBatTB.GEBB.DataBase.Entity;
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

    public static void Handle(UpdateContainer container)
    {
        using (TgBotDbContext db = new())
        {
            container.EventEntities.AddRange(
                db.Events.AsEnumerable()
                    .Where(elem => elem.CreatorId == container.UserEntity.UserId &&
                                   elem.IsCreateCompleted == false));
        }

        int count = container.EventEntities.Count;
        if (count is 0 or > 1)
        {
            container.BotClient.DeleteMessage(
                container.ChatId,
                container.Message.MessageId,
                container.Token);
            container.UserEntity.UserStatus = UserStatus.Active;
            Thread.Sleep(200);
            DatabaseHandler.Update(container.UserEntity);
            string message = (count == 0)
                ? "Ошибка. Не обнаружено мероприятий в режиме создания.\nПопробуйте снова через команду /menu"
                : "Ошибка. Обнаружено несколько мероприятий в режиме создания.\nПопробуйте снова через команду /menu";
            container.BotClient.SendMessage(
                chatId: container.ChatId,
                text: message,
                cancellationToken: container.Token);
            return;
        }

        if (container.AlterCbData?.Button is not { } button)
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
        EventEntity entity = container.EventEntities[0];
        string message;
        if (entity.Title is null ||
            entity.DateTimeOf is null ||
            entity.Address is null ||
            entity.Cost is null ||
            entity.ParticipantLimit is null)
        {
            message = "Сначала укажите все данные для мероприятия.";
            container.BotClient.AnswerCallbackQuery(container.AlterCbData!.CallbackId!, message, true,
                cancellationToken: container.Token);
            return;
        }

        //отправить уведомление, что мероприятие создано
        message = "Мероприятие создано. Рассылаю приглашения.";
        container.BotClient.AnswerCallbackQuery(container.AlterCbData!.CallbackId!, message, true,
            cancellationToken: container.Token);
        Thread.Sleep(200);
        //Изменить статус IsCreateComplete на true
        entity.IsCreateCompleted = true;
        DatabaseHandler.Update(entity);
        //изменить статус пользователя
        container.UserEntity.UserStatus = UserStatus.Active;
        DatabaseHandler.Update(container.UserEntity);
        container.BotClient.SetMyCommands(
            BotCommandProvider.GetCommandMenu(container.UserEntity.UserStatus),
            BotCommandScope.Chat(container.ChatId),
            cancellationToken: container.Token);
        Thread.Sleep(200);
        container.BotClient.DeleteMessage(
            container.ChatId,
            container.Message.Id,
            container.Token);

        //Запросить лист пользователей (все пользователи, кроме инициатора, со статусом не stop)
        //Отправить приглашения всем пользователям из списка.
        string text = $"@{container.UserEntity.Username} приглашает на мероприятие!\n" +
                      $"Название: {entity.Title}\n" +
                      $"Дата: {entity.DateTimeOf!.Value.ToString("ddd dd MMMM yyyy", new CultureInfo("ru-RU"))}\n" +
                      $"Время: {entity.DateTimeOf!.Value:HH:mm}\n" +
                      $"Место: {entity.Address}\n" +
                      $"Максимум человек: {entity.ParticipantLimit}\n" +
                      $"Планируемые затраты: {entity.Cost}\n" +
                      (string.IsNullOrEmpty(entity.Description)
                          ? ""
                          : $"Дополнительная информация: {entity.Description}");
        //TODO Test sending message
        AlterCbData data = new()
            { Button = CallbackButton.Registration, Menu = CallbackMenu.EventRegister, EventId = entity.EventId.ToString() };
        foreach (long id in DatabaseHandler.GetInviteList(entity))
        {
            Thread.Sleep(200);
            container.BotClient.SendMessage(
                chatId: id,
                text: text,
                replyMarkup: InlineKeyboardProvider.RegistrationMarkup(data),
                cancellationToken: container.Token);
        }
    }

    private static void HandleClose(UpdateContainer container)
    {
        var chatId = container.ChatId;
        var messageId = container.Message.Id;
        container.BotClient.DeleteMessage(
            chatId,
            messageId,
            container.Token);

        container.UserEntity.UserStatus = UserStatus.Active;
        DatabaseHandler.Update(container.UserEntity);

        container.BotClient.SetMyCommands(
            BotCommandProvider.GetCommandMenu(container.UserEntity.UserStatus),
            BotCommandScope.Chat(container.ChatId),
            cancellationToken: container.Token);
        using TgBotDbContext db = new();
        if (db.Find<EventEntity>(container.Message.MessageId, container.ChatId) is { } currentEvent)
        {
            db.Remove(currentEvent);
            db.SaveChanges();
        }
    }

    private static void CreateEventMenuUnknownButton(UpdateContainer container)
    {
        Console.WriteLine("MenuButtonHandler.CreateEventMenuUnknownButton()");
    }
}