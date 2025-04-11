using Com.Github.PatBatTB.GEBB.Domain;
using Com.Github.PatBatTB.GEBB.Domain.Enums;
using Telegram.Bot;

namespace Com.Github.PatBatTB.GEBB.Services.Handlers.Updates.Types.Callback.Menu;

public static class EventListHandler
{
    private static readonly Dictionary<CallbackButton, Action<UpdateContainer>> ButtonHandlerDict = new()
    {
        [CallbackButton.Edit] = HandleEdit,
        [CallbackButton.Cancel] = HandleCancel,
        [CallbackButton.Close] = HandleClose,
    };
    
    public static void Handle(UpdateContainer container)
    {
        if (container.CallbackData?.Button is not { } button)
            throw new NullReferenceException("CallbackData doesn't have button");
        ButtonHandlerDict.GetValueOrDefault(button, HandleUnknown).Invoke(container);
    }

    private static void HandleEdit(UpdateContainer container)
    {
        //TODO открыть форму редактирования мероприятия (аналогично созданию)
    }

    private static void HandleCancel(UpdateContainer container)
    {
        //TODO Получить список зарегистрированных пользователей
        //TODO Удалить мероприятие из базы
        //TODO Удалить всех зарегистрированных на мероприятие пользователей из таблицы Registrations
        //TODO Отправить всем удаленным пользователям сообщение, что мероприятие удалено.
        HandleClose(container);
    }

    private static void HandleClose(UpdateContainer container)
    {
        Thread.Sleep(200);
        container.BotClient.DeleteMessage(
            chatId: container.ChatId,
            messageId: container.Message.Id,
            cancellationToken: container.Token);
    }

    private static void HandleUnknown(UpdateContainer container)
    {
        Console.WriteLine("EventListHandler.UnknownButton");
    }
}