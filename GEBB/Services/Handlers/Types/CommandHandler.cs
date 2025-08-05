using System.Data;
using Com.GitHub.PatBatTB.GEBB;
using Com.Github.PatBatTB.GEBB.DataBase.Event;
using Com.Github.PatBatTB.GEBB.DataBase.User;
using Com.Github.PatBatTB.GEBB.Domain;
using Com.Github.PatBatTB.GEBB.Domain.Enums;
using Com.Github.PatBatTB.GEBB.Services.Providers;
using log4net;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace Com.Github.PatBatTB.GEBB.Services.Handlers.Types;

public class CommandHandler
{
    private readonly Dictionary<string, Action<UpdateContainer>> _typeHandlerDict;
    private readonly IUserService _uService;
    private readonly IEventService _eService;
    private readonly ILog _log ;

    public CommandHandler()
    {
        _typeHandlerDict = new Dictionary<string, Action<UpdateContainer>>
        {
            [Command.Start.Name()] = HandleStart,
            [Command.Stop.Name()] = HandleStop,
            [Command.Menu.Name()] = HandleMenu,
            [Command.Report.Name()] = HandleReport,
            [Command.CancelCreate.Name()] = HandleCancel,
        };
        _uService = App.ServiceFactory.GetUserService();
        _eService = App.ServiceFactory.GetEventService();
        _log = LogManager.GetLogger(typeof(CommandHandler));
    }

    public void Handle(UpdateContainer container)
    {
        _typeHandlerDict.GetValueOrDefault(container.Message.Text!, HandleUnknown).Invoke(container);
    }

    private void HandleStart(UpdateContainer container)
    {
        string text;
        if (!Command.Start.Scope().Contains(container.AppUser.UserStatus))
        {
            text = "Вам недоступна данная команда.";
            Thread.Sleep(200);
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
        Thread.Sleep(200);
        container.BotClient.SendMessage(
            container.ChatId,
            text,
            cancellationToken: container.Token);

        DataService.UpdateUserStatus(container, UserStatus.Active, _uService);
    }

    private void HandleStop(UpdateContainer container)
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
        
        DataService.UpdateUserStatus(container, UserStatus.Stop, _uService);
        ICollection<int> idList = _eService.RemoveInBuilding(container.AppUser.UserId);
        Thread.Sleep(200);
        container.BotClient.DeleteMessages(
            chatId: container.ChatId,
            messageIds: idList,
            cancellationToken: container.Token);
    }

    private void HandleMenu(UpdateContainer container)
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

        DataService.UpdateUserStatus(container, UserStatus.OpenedMenu, _uService);

        text = CallbackMenu.Main.Text();
        Thread.Sleep(200);
        container.BotClient.SendMessage(
            container.ChatId,
            text,
            replyMarkup: InlineKeyboardProvider.GetMarkup(CallbackMenu.Main),
            cancellationToken: container.Token);
    }

    private void HandleReport(UpdateContainer container)
    {
        Thread.Sleep(200);
        container.BotClient.SendMessage(
            chatId: container.ChatId,
            text: CallbackMenu.ReportBug.Text() + "\n Вы можете отправить письмо на email\ncontact-project+patbattb-gebb-support@incoming.gitlab.com\n или воспользоваться кнопками ниже.",
            replyMarkup: InlineKeyboardProvider.GetMarkup(CallbackMenu.ReportBug),
            parseMode: ParseMode.Html,
            cancellationToken: container.Token);
    }

    private void HandleCancel(UpdateContainer container)
    {
        string text;
        if (!Command.CancelCreate.Scope().Contains(container.AppUser.UserStatus))
        {
            text = "Вам недоступна данная команда.";
            Thread.Sleep(200);
            container.BotClient.SendMessage(
                container.ChatId,
                text,
                cancellationToken: container.Token);
            return;
        }
        ICollection<int> idList = _eService.RemoveInBuilding(container.AppUser.UserId);
        DataService.UpdateUserStatus(container, UserStatus.Active, _uService);
        Thread.Sleep(200);
        container.BotClient.DeleteMessages(
            chatId: container.ChatId,
            messageIds: idList,
            cancellationToken: container.Token);
    }

    private void HandleUnknown(UpdateContainer container)
    {
        _log.Error("Unknown command");
    }
}