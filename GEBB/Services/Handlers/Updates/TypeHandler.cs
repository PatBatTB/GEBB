using Com.Github.PatBatTB.GEBB.Domain;
using Com.Github.PatBatTB.GEBB.Domain.Enums;
using Com.Github.PatBatTB.GEBB.Services.Handlers.Updates.Types;
using Com.Github.PatBatTB.GEBB.Services.Handlers.Updates.Types.Callback;
using Com.Github.PatBatTB.GEBB.Services.Providers;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Com.Github.PatBatTB.GEBB.Services.Handlers.Updates;

public static class TypeHandler
{
    private static readonly Dictionary<UpdateType, Action<UpdateContainer>> UpdateTypeHandlerDict = new()
    {
        [UpdateType.Message] = MessageHandler.Handle,
        [UpdateType.CallbackQuery] = CallbackQueryHandle,
    };

    public static void Handle(UpdateContainer container)
    {
        container.BotClient.SetMyCommands(
            BotCommandProvider.GetCommandMenu(container.UserDto.UserStatus),
            BotCommandScope.Chat(container.ChatId),
            cancellationToken: container.Token
        );
        UpdateTypeHandlerDict.GetValueOrDefault(container.UpdateType, UpdateTypeUnknown).Invoke(container);
    }


    private static void CallbackQueryHandle(UpdateContainer container)
    {
        if (container.UserDto.UserStatus == UserStatus.Stop)
        {
            container.BotClient.SendMessage(
                chatId: container.ChatId,
                text: "Вы приостановили активность бота.\n" +
                      "Для возобновления воспользуйтесь командой /start");
            return;
        }

        Console.WriteLine("CallbackQuery was taken: " + container.CallbackData!.GetDataString());
        MenuHandler.Handle(container);
    }

    private static void UpdateTypeUnknown(UpdateContainer container)
    {
        Console.WriteLine("UpdateTypeHandler - unknown");
    }
}