using System.Globalization;
using Com.Github.PatBatTB.GEBB.DataBase.Event;
using Com.Github.PatBatTB.GEBB.DataBase.User;
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

    private static readonly IEventService EService = new DbEventService();
    
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
        EventDto eventDto = EService.Get(container.CallbackData!.EventId!)!;
        EService.Remove(container.CallbackData!.EventId!);
        string text = $"Мероприятие {eventDto.Title}\n" +
                      $"{eventDto.DateTimeOf!.Value.ToString("ddd dd MMMM yyyy", new CultureInfo("ru-RU"))}\n" +
                      $"{eventDto.DateTimeOf!.Value:HH:mm}\n" +
                      $"отменено организатором.";
        foreach (UserDto userDto in eventDto.RegisteredUsers)
        {
            Thread.Sleep(200);
            container.BotClient.SendMessage(
                chatId: userDto.UserId,
                text: text,
                cancellationToken: container.Token);
        }

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