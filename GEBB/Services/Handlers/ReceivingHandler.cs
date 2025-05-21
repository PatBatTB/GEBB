using Com.Github.PatBatTB.GEBB.DataBase.User;
using Com.Github.PatBatTB.GEBB.Domain;
using log4net;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Com.Github.PatBatTB.GEBB.Services.Handlers;

public class ReceivingHandler
{
    private IUserService UService = new DbUserService();
    private readonly ILog log = LogManager.GetLogger(typeof(ReceivingHandler));

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
                        log.Error("Update doesn't have message");
                        return Task.CompletedTask;
                    }

                    message = update.Message;
                    chatId = message.Chat.Id;
                    log.Debug($"Receive message in chat: {chatId} from: {message.From!.Username}[{message.From!.Id}] message: {message.Text!}");

                    if (message.From is null)
                    {
                        log.Error($"[{chatId}] : message doesn't have User");
                        return Task.CompletedTask;
                    }

                    user = message.From;
                    username = user.Username;

                    if (message.Text is not { } text)
                    {
                        log.Error($"[chat:{chatId}] [user:{username ?? ""}] : message doesn't have text");
                        return Task.CompletedTask;
                    }

                    break;
                case UpdateType.CallbackQuery:
                    if (update.CallbackQuery is null)
                    {
                        log.Error("Update doesn't have callbackQuery");
                        return Task.CompletedTask;
                    }

                    callbackQuery = update.CallbackQuery;
                    user = callbackQuery.From;
                    log.Debug($"Receive CallbackQuery from: {user.Username}[{user.Id}] data: {callbackQuery.Data!}");

                    if (callbackQuery.Message is null)
                    {
                        log.Error("CallbackQuery doesn't have message");
                        return Task.CompletedTask;
                    }

                    message = callbackQuery.Message;
                    chatId = callbackQuery.Message.Chat.Id;
                    break;
                default:
                    log.Error("Unknown update type. Ignoring.");
                    return Task.CompletedTask;
            }

            AppUser appUser = UService.Update(user);
            CallbackData callbackData = new(callbackQuery!);
            UpdateContainer updateContainer =
                new(botClient, update, chatId, message, appUser, token, callbackData);
            TypeHandler.Handle(updateContainer);
        }
        catch (Exception e)
        {
            log.Error(e.Message);
        }

        return Task.CompletedTask;
    }

    internal Task HandleError(ITelegramBotClient botClient, Exception e, CancellationToken token)
    {
        log.Error(e.Message);
        return Task.CompletedTask;
    }

    internal void HandleExitSignal
        (CancellationTokenSource cts)
    {
        log.Debug("Exit or cancel signal taken");
        cts.Cancel();
    }
}