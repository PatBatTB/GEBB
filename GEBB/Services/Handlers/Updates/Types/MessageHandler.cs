using Com.Github.PatBatTB.GEBB.Domain;
using Com.Github.PatBatTB.GEBB.Domain.Enums;
using Com.GitHub.PatBatTB.GEBB.Extensions;
using Com.Github.PatBatTB.GEBB.Services.Handlers.Updates.Types.Text;
using Telegram.Bot;

namespace Com.Github.PatBatTB.GEBB.Services.Handlers.Updates.Types;

public static class MessageHandler
{
    private static readonly Dictionary<ContentMessageType, Action<UpdateContainer>> TypeDict = new()
    {
        [ContentMessageType.Command] = CommandHandler.Handle,
        [ContentMessageType.Text] = HandleText,
        [ContentMessageType.Unknown] = HandleUnknown,
    };

    public static void Handle(UpdateContainer container)
    {
        TypeDict[container.Message.TextType()].Invoke(container);
    }

    private static void HandleText(UpdateContainer container)
    {
        if (container.AppUser.UserStatus == UserStatus.Stop)
        {
            container.BotClient.SendMessage(
                chatId: container.ChatId,
                text: "Вы приостановили активность бота.\n" +
                      "Для возобновления воспользуйтесь командой /start");
            return;
        }

        if (container.AppUser.UserStatus == UserStatus.CreatingEvent)
        {
            CreateEventStatusHandler.Handle(container);
            return;
        }

        Console.WriteLine($"{container.AppUser.Username} [{container.AppUser.UserId}] : {container.Message.Text}");
    }

    private static void HandleUnknown(UpdateContainer container)
    {
        Console.WriteLine("UpdateTypeHandler.MessageTypeUnknownHandle()");
    }
}