using Com.Github.PatBatTB.GEBB.Domain;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Com.Github.PatBatTB.GEBB.Services.Handlers;

internal class ReceivingHandler
{
    internal Task UpdateHandler(ITelegramBotClient botClient, Update update, CancellationToken token)
    {
        try
        {
            UpdateContainer updateContainer;
            switch (update.Type)
            {
                case UpdateType.Message:
                    if (update.Message is not { } message)
                    {
                        Console.WriteLine("Update doesn't have message");
                        return Task.CompletedTask;
                    }

                    var chatId = message.Chat.Id;

                    if (message.From is not { } user)
                    {
                        Console.WriteLine($"[{chatId}] : message doesn't have User");
                        return Task.CompletedTask;
                    }

                    var username = user.Username;
                    var userId = user.Id;

                    if (message.Text is not { } text)
                    {
                        Console.WriteLine($"[chat:{chatId}] [user:{username ?? ""}] : message doesn't have text");
                        return Task.CompletedTask;
                    }

                    updateContainer = new UpdateContainer(botClient, update, token, chatId, user, message);
                    break;
                case UpdateType.CallbackQuery:
                    if (update.CallbackQuery is not { } callbackQuery)
                    {
                        Console.WriteLine("Update doesn't have callbackQuery");
                        return Task.CompletedTask;
                    }

                    user = callbackQuery.From;

                    if (callbackQuery.Message is null)
                    {
                        Console.WriteLine("CallbackQuery doesn't have message");
                        return Task.CompletedTask;
                    }

                    message = callbackQuery.Message;
                    chatId = callbackQuery.Message.Chat.Id;
                    updateContainer = new UpdateContainer(botClient, update, token, chatId, user, message);
                    break;
                default:
                    Console.WriteLine("Unknown update type. Ignoring.");
                    return Task.CompletedTask;
            }

            UpdateTypeHandler updateTypeHandler = new();
            updateTypeHandler.Handle(updateContainer);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }

        return Task.CompletedTask;
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