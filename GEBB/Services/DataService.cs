using Com.Github.PatBatTB.GEBB.DataBase.User;
using Com.Github.PatBatTB.GEBB.Domain;
using Com.Github.PatBatTB.GEBB.Domain.Enums;
using Com.Github.PatBatTB.GEBB.Services.Providers;
using log4net;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Com.Github.PatBatTB.GEBB.Services;

public static class DataService
{
    private static readonly ILog Log = LogManager.GetLogger(typeof(DataService));
    
    public static void UpdateUserStatus(UpdateContainer container, UserStatus newStatus, IUserService uService)
    {
        Log.Debug($"Updating {container.AppUser.Username}[{container.AppUser.UserId}] status from {container.AppUser.UserStatus} to {newStatus}");
        container.AppUser.UserStatus = newStatus;
        uService.Update(container.AppUser);
        UpdateBotCommandsMenu(container.BotClient, newStatus, container.ChatId, container.Token);
    }

    public static void UpdateBotCommandsMenu(ITelegramBotClient botClient, UserStatus status, 
        long chatId, CancellationToken token)
    {
        Thread.Sleep(200);
        botClient.SetMyCommands(
            BotCommandProvider.GetCommandMenu(status),
            BotCommandScope.Chat(chatId),
            cancellationToken: token);
    }
}