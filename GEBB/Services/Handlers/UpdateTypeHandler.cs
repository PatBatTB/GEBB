using System.Data;
using Com.Github.PatBatTB.GEBB.Domain;
using Com.GitHub.PatBatTB.GEBB.Extensions;
using Com.Github.PatBatTB.GEBB.Services.Providers;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using MessageType = Com.Github.PatBatTB.GEBB.Domain.MessageType;

namespace Com.Github.PatBatTB.GEBB.Services.Handlers;

public static class UpdateTypeHandler
{
    private static readonly Dictionary<UpdateType, Action<UpdateContainer>> UpdateTypeHandlerDict = new()
    {
        [UpdateType.Message] = MessageHandle,
        [UpdateType.CallbackQuery] = CallbackQueryHandle,
    };

    private static readonly Dictionary<MessageType, Action<UpdateContainer>> MessageTypeHandlerDict = new()
    {
        [MessageType.Command] = CommandHandle,
        [MessageType.Text] = TextHandle,
        [MessageType.Unknown] = MessageTypeUnknownHandle,
    };

    private static readonly Dictionary<string, Action<UpdateContainer>> CommandTypeHandlerDict = new()
    {
        [Command.Start.Name()] = CommandStartHandle,
        [Command.Stop.Name()] = CommandStopHandle,
        [Command.Menu.Name()] = CommandMenuHandle,
        [Command.CreateCancel.Name()] = CommandCreateCancelHandle,
    };

    public static void Handle(UpdateContainer container)
    {
        container.BotClient.SetMyCommands(
            BotCommandProvider.GetCommandMenu(container.UserEntity.UserStatus),
            BotCommandScope.Chat(container.ChatId),
            cancellationToken: container.Token
        );
        UpdateTypeHandlerDict.GetValueOrDefault(container.UpdateType, UpdateTypeUnknown).Invoke(container);
    }

    private static void MessageHandle(UpdateContainer container)
    {
        MessageTypeHandlerDict[container.Message.TextType()].Invoke(container);
    }

    private static void CallbackQueryHandle(UpdateContainer container)
    {
        if (container.UserEntity.UserStatus == UserStatus.Stop)
        {
            container.BotClient.SendMessage(
                chatId: container.ChatId,
                text: "Вы приостановили активность бота.\n" +
                      "Для возобновления воспользуйтесь командой /start");
            return;
        }

        Console.WriteLine("CallbackQuery was taken: " + container.CallbackData!.Data);
        MenuButtonHandler.Handle(container);
    }

    private static void UpdateTypeUnknown(UpdateContainer container)
    {
        Console.WriteLine("UpdateTypeHandler - unknown");
    }

    private static void CommandHandle(UpdateContainer container)
    {
        CommandTypeHandlerDict.GetValueOrDefault(container.Message.Text!, CommandUnknownHandle).Invoke(container);
    }

    private static void TextHandle(UpdateContainer container)
    {
        if (container.UserEntity.UserStatus == UserStatus.Stop)
        {
            container.BotClient.SendMessage(
                chatId: container.ChatId,
                text: "Вы приостановили активность бота.\n" +
                      "Для возобновления воспользуйтесь командой /start");
            return;
        }

        //Проверить, что пользователь в статусе создания мероприятия
        if (container.UserEntity.UserStatus == UserStatus.CreatingEvent)
        {
            CreateEventHandler.Handle(container);
            return;
        }

        Console.WriteLine($"{container.User.Username} [{container.User.Id}] : {container.Message.Text}");
    }

    private static void MessageTypeUnknownHandle(UpdateContainer container)
    {
        Console.WriteLine("UpdateTypeHandler.MessageTypeUnknownHandle()");
    }

    private static void CommandStartHandle(UpdateContainer container)
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

    private static void CommandStopHandle(UpdateContainer container)
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

    private static void CommandMenuHandle(UpdateContainer container)
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

    private static void CommandUnknownHandle(UpdateContainer container)
    {
        Console.WriteLine("UpdateTypeHandler.CommandUnknownHandle()");
    }

    private static void CommandCreateCancelHandle(UpdateContainer container)
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