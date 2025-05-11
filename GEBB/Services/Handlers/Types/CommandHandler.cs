using System.Data;
using Com.Github.PatBatTB.GEBB.DataBase.Event;
using Com.Github.PatBatTB.GEBB.DataBase.User;
using Com.Github.PatBatTB.GEBB.Domain;
using Com.Github.PatBatTB.GEBB.Domain.Enums;
using Com.Github.PatBatTB.GEBB.Services.Providers;
using Telegram.Bot;

namespace Com.Github.PatBatTB.GEBB.Services.Handlers.Updates.Types;

public static class CommandHandler
{
    private static readonly Dictionary<string, Action<UpdateContainer>> TypeHandlerDict = new()
    {
        [Command.Start.Name()] = HandleStart,
        [Command.Stop.Name()] = HandleStop,
        [Command.Menu.Name()] = HandleMenu,
        [Command.CancelCreate.Name()] = HandleCancel,
    };

    private static readonly IUserService UService = new DbUserService();
    private static readonly IEventService EService = new DbEventService();

    public static void Handle(UpdateContainer container)
    {
        TypeHandlerDict.GetValueOrDefault(container.Message.Text!, HandleUnknown).Invoke(container);
    }

    private static void HandleStart(UpdateContainer container)
    {
        string text;
        if (!Command.Start.Scope().Contains(container.AppUser.UserStatus))
        {
            text = "Вам недоступна данная команда.";
            container.BotClient.SendMessage(
                container.ChatId,
                text,
                cancellationToken: container.Token);
            return;
        }

        text = container.AppUser.UserStatus switch
        {
            UserStatus.Newuser => "Добро пожаловать!\nДля вызова меню воспользуйтесь командой /menu",
            UserStatus.Stop => "С возвращением!\nДля вызова меню воспользуйтесь командой /menu",
            _ => throw new InvalidConstraintException("Value must be newuser or stop")
        };
        container.BotClient.SendMessage(
            container.ChatId,
            text,
            cancellationToken: container.Token);

        DataService.UpdateUserStatus(container, UserStatus.Active, UService);
    }

    private static void HandleStop(UpdateContainer container)
    {
        string text;
        if (!Command.Stop.Scope().Contains(container.AppUser.UserStatus))
        {
            text = "Вам недоступна данная команда.";
            container.BotClient.SendMessage(
                container.ChatId,
                text,
                cancellationToken: container.Token);
            return;
        }

        text = "Вы приостановили действия бота.\n" +
               "Вы больше не будете получать уведомления о новых мероприятиях.\n" +
               "Для возобновления участия отправьте команду /start";
        container.BotClient.SendMessage(
            container.ChatId,
            text,
            cancellationToken: container.Token);
        
        DataService.UpdateUserStatus(container, UserStatus.Stop, UService);
        ICollection<int> idList = EService.RemoveInBuilding(container.AppUser.UserId);
        container.BotClient.DeleteMessages(
            chatId: container.ChatId,
            messageIds: idList,
            cancellationToken: container.Token);
    }

    private static void HandleMenu(UpdateContainer container)
    {
        string text;
        if (!Command.Menu.Scope().Contains(container.AppUser.UserStatus))
        {
            text = "Вам недоступна данная команда.";
            container.BotClient.SendMessage(
                container.ChatId,
                text,
                cancellationToken: container.Token);
            return;
        }

        DataService.UpdateUserStatus(container, UserStatus.OpenedMenu, UService);

        text = CallbackMenu.Main.Text();
        container.BotClient.SendMessage(
            container.ChatId,
            text,
            replyMarkup: InlineKeyboardProvider.GetMarkup(CallbackMenu.Main),
            cancellationToken: container.Token);
    }

    private static void HandleUnknown(UpdateContainer container)
    {
        Console.WriteLine("UpdateTypeHandler.CommandUnknownHandle()");
    }

    private static void HandleCancel(UpdateContainer container)
    {
        string text;
        if (!Command.CancelCreate.Scope().Contains(container.AppUser.UserStatus))
        {
            text = "Вам недоступна данная команда.";
            container.BotClient.SendMessage(
                container.ChatId,
                text,
                cancellationToken: container.Token);
            return;
        }
        ICollection<int> idList = EService.RemoveInBuilding(container.AppUser.UserId);
        DataService.UpdateUserStatus(container, UserStatus.Active, UService);
        Thread.Sleep(200);
        container.BotClient.DeleteMessages(
            chatId: container.ChatId,
            messageIds: idList,
            cancellationToken: container.Token);
    }
}