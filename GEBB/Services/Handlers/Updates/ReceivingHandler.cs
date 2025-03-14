using Com.Github.PatBatTB.GEBB.Domain;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Com.Github.PatBatTB.GEBB.Services.Handlers.Updates;

public class ReceivingHandler
{
    public Task HandleUpdate(ITelegramBotClient botClient, Update update, CancellationToken token)
    {
        try
        {
            long chatId;
            string? username;
            User user;
            Message message;
            CallbackQuery? callbackQuery = null;
            switch (update.Type)
            {
                case UpdateType.Message:
                    if (update.Message is null)
                    {
                        Console.WriteLine("Update doesn't have message");
                        return Task.CompletedTask;
                    }

                    message = update.Message;
                    chatId = message.Chat.Id;

                    if (message.From is null)
                    {
                        Console.WriteLine($"[{chatId}] : message doesn't have User");
                        return Task.CompletedTask;
                    }

                    user = message.From;
                    username = user.Username;

                    if (message.Text is not { } text)
                    {
                        Console.WriteLine($"[chat:{chatId}] [user:{username ?? ""}] : message doesn't have text");
                        return Task.CompletedTask;
                    }

                    break;
                case UpdateType.CallbackQuery:
                    if (update.CallbackQuery is null)
                    {
                        Console.WriteLine("Update doesn't have callbackQuery");
                        return Task.CompletedTask;
                    }

                    callbackQuery = update.CallbackQuery;
                    user = callbackQuery.From;

                    if (callbackQuery.Message is null)
                    {
                        Console.WriteLine("CallbackQuery doesn't have message");
                        return Task.CompletedTask;
                    }

                    message = callbackQuery.Message;
                    chatId = callbackQuery.Message.Chat.Id;
                    break;
                default:
                    Console.WriteLine("Unknown update type. Ignoring.");
                    return Task.CompletedTask;
            }

            var userEntity = DatabaseHandler.Update(user);
            var callbackData = CallbackData.GetInstance(callbackQuery);
            var alterCbData = new AlterCbData(callbackQuery);
            UpdateContainer updateContainer =
                new(botClient, update, chatId, user, message, userEntity, token, callbackData);
            TypeHandler.Handle(updateContainer);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return Task.CompletedTask;
    }

    internal Task HandleError(ITelegramBotClient botClient, Exception ex, CancellationToken token)
    {
        Console.WriteLine(ex.Message);
        return Task.CompletedTask;
    }

    internal void HandleExitSignal
        (CancellationTokenSource cts)
    {
        cts.Cancel();
    }
}