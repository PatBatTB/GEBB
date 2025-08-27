using Com.Github.PatBatTB.GEBB.Domain;
using Com.Github.PatBatTB.GEBB.Domain.Enums;
using Com.Github.PatBatTB.GEBB.Services.Handlers.Types;
using Com.Github.PatBatTB.GEBB.Services.Handlers.Types.Callback;
using log4net;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace Com.Github.PatBatTB.GEBB.Services.Handlers;

public class TypeHandler
{
    private readonly Dictionary<UpdateType, Action<UpdateContainer>> _updateTypeHandlerDict;
    private readonly ILog _log;
    private readonly MessageHandler _messageHandler;
    private readonly MenuHandler _menuHandler;

    public TypeHandler()
    {
        _messageHandler = new MessageHandler();
        _updateTypeHandlerDict = new Dictionary<UpdateType, Action<UpdateContainer>>
        {
            [UpdateType.Message] = _messageHandler.Handle,
            [UpdateType.CallbackQuery] = CallbackQueryHandle,
        };
        _log = LogManager.GetLogger(typeof(TypeHandler));
        _menuHandler = new MenuHandler();
    }

    public void Handle(UpdateContainer container)
    {
        DataService.UpdateBotCommandsMenu(container.BotClient, container.AppUser.UserStatus, 
            container.ChatId, container.Token);
        if (container.AppUser.UserStatus == UserStatus.Stop)
        {
            Thread.Sleep(200);
            container.BotClient.SendMessage(
                chatId: container.ChatId,
                text: "Вы приостановили активность бота.\n" +
                      "Для возобновления воспользуйтесь командой /start");
            return;
        }
        _updateTypeHandlerDict.GetValueOrDefault(container.UpdateType, UpdateTypeUnknown).Invoke(container);
    }

    private void CallbackQueryHandle(UpdateContainer container)
    {
        _menuHandler.Handle(container);
    }

    private void UpdateTypeUnknown(UpdateContainer container)
    {
        _log.Error("Unknown update type");
    }
}