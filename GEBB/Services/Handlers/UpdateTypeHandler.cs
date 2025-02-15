using Com.Github.PatBatTB.GEBB.Domain;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Com.Github.PatBatTB.GEBB.Services.Handlers;

public class UpdateTypeHandler
{
    private readonly Dictionary<UpdateType, Process> _handlerDict = new()
    {
        [UpdateType.Message] = MessageHandle,
        [UpdateType.CallbackQuery] = CallbackQueryHandle
    };

    public void Handle(UpdateContainer container)
    {
        _handlerDict.GetValueOrDefault(container.Update.Type, Unknown).Invoke(container);
    }

    private static void MessageHandle(UpdateContainer container)
    {
        InlineKeyboardMarkup markup = new()
        {
            InlineKeyboard = new[]
            {
                new[]
                {
                    new InlineKeyboardButton { Text = "Test", CallbackData = "test_data" }
                }
            }
        };
        container.BotClient.SendMessage(
            container.ChatId,
            "test message",
            replyMarkup: markup,
            cancellationToken: container.Token
        );
    }

    private static void CallbackQueryHandle(UpdateContainer container)
    {
        if (container.Update.CallbackQuery!.Data == "test_data")
            container.BotClient.EditMessageReplyMarkup(
                container.ChatId,
                container.Message.Id,
                InlineKeyboardMarkup.Empty(),
                cancellationToken: container.Token
            );
    }

    private static void Unknown(UpdateContainer container)
    {
        Console.WriteLine("UpdateTypeHandler - unknown");
    }

    private delegate void Process(UpdateContainer container);
}