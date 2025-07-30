using Com.Github.PatBatTB.GEBB.Domain;
using Com.Github.PatBatTB.GEBB.Domain.Enums;
using Com.GitHub.PatBatTB.GEBB.Extensions;
using Com.Github.PatBatTB.GEBB.Services.Handlers.Types.Text;
using log4net;
using Telegram.Bot;

namespace Com.Github.PatBatTB.GEBB.Services.Handlers.Types;

public static class MessageHandler
{
    private static readonly Dictionary<ContentMessageType, Action<UpdateContainer>> TypeDict = new()
    {
        [ContentMessageType.Command] = CommandHandler.Handle,
        [ContentMessageType.Text] = HandleText,
        [ContentMessageType.Unknown] = HandleUnknown,
    };
    
    private static readonly ILog Log = LogManager.GetLogger(typeof(MessageHandler));

    public static void Handle(UpdateContainer container)
    {
        TypeDict[container.Message.TextType()].Invoke(container);
    }

    private static void HandleText(UpdateContainer container)
    {
        switch (container.AppUser.UserStatus)
        {
            case UserStatus.Stop:
                container.BotClient.SendMessage(
                    chatId: container.ChatId,
                    text: "Вы приостановили активность бота.\n" +
                          "Для возобновления воспользуйтесь командой /start");
                break;
            case UserStatus.CreatingEvent: case UserStatus.EditingEvent:
                BuildingEventStatusHandler.Handle(container);
                break;
            case UserStatus.SendingMessage:
                new SendEventMessageHandler().Handle(container);
                break;
        }
    }

    private static void HandleUnknown(UpdateContainer container)
    {
        Log.Error("Unknown message type");
    }
}