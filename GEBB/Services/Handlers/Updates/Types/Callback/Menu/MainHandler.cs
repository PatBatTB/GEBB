using Com.Github.PatBatTB.GEBB.Domain;
using Com.Github.PatBatTB.GEBB.Domain.Enums;
using Com.Github.PatBatTB.GEBB.Services.Providers;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Com.Github.PatBatTB.GEBB.Services.Handlers.Updates.Types.Callback.Menu;

public static class MainHandler
{
    private static readonly Dictionary<CallbackButton, Action<UpdateContainer>> ButtonHandlerDict = new()
    {
        [CallbackButton.MyEvents] = HandleEvents,
        [CallbackButton.MyRegistrations] = HandleMyRegistrations,
        [CallbackButton.AvailableEvents] = HandleAvailableEvents,
        [CallbackButton.Close] = HandleClose,
    };

    public static void Handle(UpdateContainer container)
    {
        ButtonHandlerDict.GetValueOrDefault(container.CallbackData!.DataButton, HandleUnknown)
            .Invoke(container);
    }

    private static void HandleEvents(UpdateContainer container)
    {
        container.BotClient.EditMessageText(
            container.ChatId,
            container.Message.Id,
            CallbackMenu.MyEvents.Text(),
            replyMarkup: InlineKeyboardProvider.GetMarkup(CallbackMenu.MyEvents),
            cancellationToken: container.Token);
    }

    private static void HandleMyRegistrations(UpdateContainer container)
    {
        throw new NotImplementedException();
    }

    private static void HandleAvailableEvents(UpdateContainer container)
    {
        throw new NotImplementedException();
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
    }

    private static void HandleUnknown(UpdateContainer container)
    {
        Console.WriteLine("MenuButtonHandler.MainMenuUnknownButtonHandle()");
    }
}