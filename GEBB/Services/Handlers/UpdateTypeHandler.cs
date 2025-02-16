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
        [UpdateType.CallbackQuery] = CallbackQueryHandle
    };

    private static readonly Dictionary<MessageType, Action<UpdateContainer>> MessageTypeHandlerDict = new()
    {
        [MessageType.Command] = CommandHandle,
        [MessageType.Text] = TextHandle,
        [MessageType.Unknown] = MessageTypeUnknownHandle
    };

    private static readonly Dictionary<string, Action<UpdateContainer>> CommandTypeHandlerDict = new()
    {
        [Command.Start.Name()] = CommandStartHandle,
        [Command.Stop.Name()] = CommandStopHandle,
        [Command.Menu.Name()] = CommandMenuHandle
    };

    public static void Handle(UpdateContainer container)
    {
        UpdateTypeHandlerDict.GetValueOrDefault(container.Update.Type, UpdateTypeUnknown).Invoke(container);
    }

    private static void MessageHandle(UpdateContainer container)
    {
        MessageTypeHandlerDict[container.Message.TextType()].Invoke(container);
    }

    private static void CallbackQueryHandle(UpdateContainer container)
    {
        throw new NotImplementedException();
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
        throw new NotImplementedException();
    }

    private static void MessageTypeUnknownHandle(UpdateContainer container)
    {
        throw new NotImplementedException();
    }

    private static void CommandStartHandle(UpdateContainer container)
    {
        string text;
        //проверка, что пользователи не могут вызывать команду, если ее нет в их меню.
        if (!Command.Start.Scope().Contains(container.UserStatus))
        {
            text = "Вам недоступна данная команда, воспользуйтесь кнопкой \"меню\".";
            container.BotClient.SendMessage(
                container.ChatId,
                text,
                cancellationToken: container.Token);
            return;
        }

        //отправить сообщение (разделить на сообщения для новых пользователей и для старых (по статусу можно))
        text = container.UserStatus switch
        {
            UserStatus.Newuser => "Добро пожаловать, новый пользователь.",
            UserStatus.Stop => "С возвращением, старый пользователь.",
            _ => throw new InvalidConstraintException("Value must be newuser or stop")
        };
        container.BotClient.SendMessage(
            container.ChatId,
            text,
            cancellationToken: container.Token);

        //изменить статус isActive и userStatus
        container.UserEntity.IsActive = true;
        container.DatabaseHandler.Update(container.UserEntity);
        container.UserStatus = UserStatus.Active;

        //отправить меню
        container.BotClient.SetMyCommands(
            BotCommandProvider.GetCommandMenu(container.UserStatus),
            BotCommandScope.Chat(container.ChatId),
            cancellationToken: container.Token
        );
    }

    private static void CommandStopHandle(UpdateContainer container)
    {
        string text;
        //верификация пользователя
        if (!Command.Stop.Scope().Contains(container.UserStatus))
        {
            text = "Вам недоступна данная команда.";
            container.BotClient.SendMessage(
                container.ChatId,
                text,
                cancellationToken: container.Token);
        }

        //     - отправляется прощальное сообщение
        text = "Вы приостановили действия бота.\n" +
               "Вы больше не будете получать уведомлений.\n" +
               "Для возобновления участия отправьте команду /start";
        container.BotClient.SendMessage(
            container.ChatId,
            text,
            cancellationToken: container.Token);
        //         - меняется статус в базе на false
        container.UserEntity.IsActive = false;
        container.DatabaseHandler.Update(container.UserEntity);
        //         - меняется usersstatus на stop
        container.UserStatus = UserStatus.Stop;
        //         - отправляется меню
        container.BotClient.SetMyCommands(
            BotCommandProvider.GetCommandMenu(container.UserStatus),
            BotCommandScope.Chat(container.ChatId),
            cancellationToken: container.Token);
    }

    private static void CommandMenuHandle(UpdateContainer container)
    {
        string text;
        if (!Command.Menu.Scope().Contains(container.UserStatus))
        {
            text = "Вам недоступна данная команда.";
            container.BotClient.SendMessage(
                container.ChatId,
                text,
                cancellationToken: container.Token);
            return;
        }

        text = "Меню пользователя.\nВ котором в дальнейшем будут кнопки с самим меню.";
        container.BotClient.SendMessage(
            container.ChatId,
            text,
            cancellationToken: container.Token);
    }

    private static void CommandUnknownHandle(UpdateContainer container)
    {
        Console.WriteLine("Unknown command.");
        throw new NotImplementedException();
    }
}