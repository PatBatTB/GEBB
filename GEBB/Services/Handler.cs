using Com.Github.PatBatTB.GEBB.DataBase;
using Com.GitHub.PatBatTB.GEBB.Domain;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using UserDb = Com.Github.PatBatTB.GEBB.DataBase.Entity.User;

namespace Com.GitHub.PatBatTB.GEBB.Services;

internal class Handler
{
    internal async Task UpdateHandler(ITelegramBotClient botClient, Update update, CancellationToken token)
    {

        try
        {
            await using TgbotContext db = new();
            switch (update.Type)
            {
                case UpdateType.Message:
                    if (update.Message is not { } message)
                    {
                        Console.WriteLine($"Update doesn't have message");
                        return;
                    }

                    long chatId = message.Chat.Id;
                    long userId = message.From!.Id;
                    string? username = message.Chat.Username;

                    UserDb? userDb = db.Find<UserDb>(userId);
                    if (userDb is null)
                    {
                        userDb = new()
                            { UserId = (int)userId, Username = username, IsActive = true, RegisteredAt = DateTime.Now };
                        db.Add(userDb);
                    }
                    else
                    {
                        userDb.Username = username;
                    }

                    await db.SaveChangesAsync(token);

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