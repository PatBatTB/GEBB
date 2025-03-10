using System.Data;
using Com.Github.PatBatTB.GEBB.Domain;
using Com.Github.PatBatTB.GEBB.Domain.Enums;
using Com.Github.PatBatTB.GEBB.Services.Providers;
using Telegram.Bot;
using Telegram.Bot.Types;

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

    public static void Handle(UpdateContainer container)
    {
        TypeHandlerDict.GetValueOrDefault(container.Message.Text!, HandleUnknown).Invoke(container);
    }

    private static void HandleStart(UpdateContainer container)
    {
        string text;
        //проверка, что пользователи не могут вызывать команду, если ее нет в их меню.
        if (!Command.Start.Scope().Contains(container.UserEntity.UserStatus))
        {
            text = "Вам недоступна данная команда.";
            container.BotClient.SendMessage(
                container.ChatId,
                text,
                cancellationToken: container.Token);
            return;
        }

        text = container.UserEntity.UserStatus switch
        {
            UserStatus.Newuser => "Добро пожаловать!\nДля вызова меню воспользуйтесь командой /menu",
            UserStatus.Stop => "С возвращением!\nДля вызова меню воспользуйтесь командой /menu",
            _ => throw new InvalidConstraintException("Value must be newuser or stop")
        };
        container.BotClient.SendMessage(
            container.ChatId,
            text,
            cancellationToken: container.Token);

        //изменить userStatus, обновить БД
        container.UserEntity.UserStatus = UserStatus.Active;
        DatabaseHandler.Update(container.UserEntity);

        //отправить меню
        container.BotClient.SetMyCommands(
            BotCommandProvider.GetCommandMenu(container.UserEntity.UserStatus),
            BotCommandScope.Chat(container.ChatId),
            cancellationToken: container.Token
        );
    }

    private static void HandleStop(UpdateContainer container)
    {
        string text;
        //верификация пользователя
        if (!Command.Stop.Scope().Contains(container.UserEntity.UserStatus))
        {
            text = "Вам недоступна данная команда.";
            container.BotClient.SendMessage(
                container.ChatId,
                text,
                cancellationToken: container.Token);
            return;
        }

        //     - отправляется прощальное сообщение
        text = "Вы приостановили действия бота.\n" +
               "Вы больше не будете получать уведомлений.\n" +
               "Для возобновления участия отправьте команду /start";
        container.BotClient.SendMessage(
            container.ChatId,
            text,
            cancellationToken: container.Token);
        //         - меняется usersstatus на stop
        container.UserEntity.UserStatus = UserStatus.Stop;
        //         - меняется статус в базе
        DatabaseHandler.Update(container.UserEntity);

        //         - отправляется меню
        container.BotClient.SetMyCommands(
            BotCommandProvider.GetCommandMenu(container.UserEntity.UserStatus),
            BotCommandScope.Chat(container.ChatId),
            cancellationToken: container.Token);

        //удаляются все незавершенные создания мероприятий с удалением сообщений с меню.
        List<int> idList = DatabaseHandler.DeleteCreatingEvents(container.UserEntity.UserId);
        container.BotClient.DeleteMessages(
            chatId: container.ChatId,
            messageIds: idList,
            cancellationToken: container.Token);
    }

    private static void HandleMenu(UpdateContainer container)
    {
        string text;
        if (!Command.Menu.Scope().Contains(container.UserEntity.UserStatus))
        {
            text = "Вам недоступна данная команда.";
            container.BotClient.SendMessage(
                container.ChatId,
                text,
                cancellationToken: container.Token);
            return;
        }

        container.UserEntity.UserStatus = UserStatus.OpenedMenu;
        DatabaseHandler.Update(container.UserEntity);
        container.BotClient.SetMyCommands(
            BotCommandProvider.GetCommandMenu(container.UserEntity.UserStatus),
            BotCommandScope.Chat(container.ChatId),
            cancellationToken: container.Token);

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
        //получить список мероприятий в режиме создания.
        List<int> idList = DatabaseHandler.DeleteCreatingEvents(container.UserEntity.UserId);
        //удалить связанные с мероприятиями сообщения.

        //изменить юзерстатус на Active
        container.UserEntity.UserStatus = UserStatus.Active;
        DatabaseHandler.Update(container.UserEntity);

        container.BotClient.DeleteMessages(
            chatId: container.ChatId,
            messageIds: idList,
            cancellationToken: container.Token);
        //отправить актуальное меню
        container.BotClient.SetMyCommands(
            BotCommandProvider.GetCommandMenu(container.UserEntity.UserStatus),
            BotCommandScope.Chat(container.ChatId),
            cancellationToken: container.Token);
    }
}