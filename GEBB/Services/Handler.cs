using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Com.GitHub.PatBatTB.GEBB.Services;

internal class Handler
{
    internal async Task UpdateHandler(ITelegramBotClient botClient, Update update, CancellationToken token)
    {

        try
        {
            switch (update.Type)
            {
                case UpdateType.Message:
                    if (update.Message is not { } message)
                    {
                        Console.WriteLine($"Update doesn't have message");
                        return;
                    }

                    long chatId = message.Chat.Id;
                    string? username = message.Chat.Username;

                    if (message.Text is not { } text)
                    {
                        Console.WriteLine($"[{chatId}] {username} : message doesn't have text");
                        return;
                    }
                
                    Console.WriteLine($"[{chatId}] {username} : {text}");
                    await botClient.SendMessage(chatId,
                        text,
                        replyParameters: new ReplyParameters() { MessageId = message.MessageId },
                        cancellationToken: token
                    );
                    break;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    internal Task ErrorHandler(ITelegramBotClient botClient, Exception ex, CancellationToken token)
    {
        Console.WriteLine(ex.Message);
        return Task.CompletedTask;
    }

    internal void ExitHandler(CancellationTokenSource cts)
    {
        cts.Cancel();
    }
}