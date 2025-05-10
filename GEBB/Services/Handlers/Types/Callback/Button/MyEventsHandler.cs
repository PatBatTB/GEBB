using System.Globalization;
using Com.Github.PatBatTB.GEBB.DataBase.Event;
using Com.Github.PatBatTB.GEBB.DataBase.User;
using Com.Github.PatBatTB.GEBB.Domain;
using Com.Github.PatBatTB.GEBB.Domain.Enums;
using Com.Github.PatBatTB.GEBB.Services.Providers;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Com.Github.PatBatTB.GEBB.Services.Handlers.Types.Callback.Button;

public static class MyEventsHandler
{
    private static readonly Dictionary<CallbackButton, Action<UpdateContainer>> ButtonHandlerDict = new()
    {
        [CallbackButton.Create] = HandleCreate,
        [CallbackButton.List] = HandleList,
        [CallbackButton.Back] = HandleBack,
    };

    private static readonly IUserService UService = new DbUserService();

    private static IEventService EService = new DbEventService();

    public static void Handle(UpdateContainer container)
    {
        if (container.CallbackData?.Button is not { } button)
            throw new NullReferenceException("CallbackData doesn't have button");
        ButtonHandlerDict.GetValueOrDefault(button, HandleUnknown).Invoke(container);
    }

    private static void HandleCreate(UpdateContainer container)
    {
        long chatId = container.ChatId;
        int messageId = container.Message.Id;
        CancellationToken token = container.Token;
        
        container.Events.AddRange(EService.GetInCreating(container.AppUser.UserId));
        
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

            EService.RemoveInCreating(chatId);

            throw new Exception("Multiple event creating doesn't work;");
        }
        
        Thread.Sleep(200);
        Message sent = container.BotClient.SendMessage(
            chatId,
            CallbackMenu.CreateEvent.Text(),
            replyMarkup: InlineKeyboardProvider.GetMarkup(CallbackMenu.CreateEvent),
            cancellationToken: token).Result;
        
        EService.Create(container.AppUser.UserId, sent.Id);
        DataService.UpdateUserStatus(container, UserStatus.CreatingEvent, UService);
    }

    private static void HandleList(UpdateContainer container)
    {
        container.Events.AddRange(EService.GetMyOwnEvents(container.AppUser.UserId));
        if (container.Events.Count > 0)
        {
            foreach (AppEvent appEvent in container.Events)
            {
                string text = $"Название: {appEvent.Title}\n" +
                              $"Дата: {appEvent.DateTimeOf!.Value.ToString("ddd dd MMMM yyyy", new CultureInfo("ru-RU"))}\n" +
                              $"Время: {appEvent.DateTimeOf!.Value:HH:mm}\n" +
                              $"Место: {appEvent.Address}\n" +
                              $"Максимум человек: {appEvent.ParticipantLimit}\n" +
                              $"Зарегистрировалось: {appEvent.RegisteredUsers.Count}\n" +
                              $"Планируемые затраты: {appEvent.Cost}\n" +
                              (string.IsNullOrEmpty(appEvent.Description)
                                  ? ""
                                  : $"Дополнительная информация: {appEvent.Description}");
                Thread.Sleep(200);
                container.BotClient.SendMessage(
                    chatId: container.ChatId,
                    text: text,
                    replyMarkup: InlineKeyboardProvider.GetMarkup(CallbackMenu.CreatedEvent, appEvent.Id),  
                    cancellationToken: container.Token);
            }
        }
        else
        {
            Thread.Sleep(200);
            container.BotClient.AnswerCallbackQuery(
                callbackQueryId: container.CallbackData!.CallbackId!,
                text: "Созданных мероприятий нет.",
                showAlert: true,
                cancellationToken: container.Token);
        }
        Thread.Sleep(200);
        container.BotClient.DeleteMessage(
            chatId: container.ChatId,
            messageId: container.Message.Id,
            cancellationToken: container.Token);
        DataService.UpdateUserStatus(container, UserStatus.Active, UService);
    }

    private static void HandleBack(UpdateContainer container)
    {
        container.BotClient.EditMessageText(
            container.ChatId,
            container.Message.Id,
            CallbackMenu.Main.Text(),
            replyMarkup: InlineKeyboardProvider.GetMarkup(CallbackMenu.Main),
            cancellationToken: container.Token);
    }

    private static void HandleUnknown(UpdateContainer container)
    {
        Console.WriteLine("MenuButtonHandler.MyEventsMenuUnknownButton()");
    }
}