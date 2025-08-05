using Com.Github.PatBatTB.GEBB.Domain;
using Com.Github.PatBatTB.GEBB.Domain.Enums;
using Com.GitHub.PatBatTB.GEBB.Extensions;
using Com.Github.PatBatTB.GEBB.Services.Handlers.Types.Text;
using log4net;

namespace Com.Github.PatBatTB.GEBB.Services.Handlers.Types;

public class MessageHandler
{
    private readonly Dictionary<ContentMessageType, Action<UpdateContainer>> _typeDict;
    private readonly ILog _log = LogManager.GetLogger(typeof(MessageHandler));

    public MessageHandler()
    {
        _typeDict = new Dictionary<ContentMessageType, Action<UpdateContainer>>
        {
            [ContentMessageType.Command] = new CommandHandler().Handle,
            [ContentMessageType.Text] = HandleText,
            [ContentMessageType.Unknown] = HandleUnknown,
        };
    }

    public void Handle(UpdateContainer container)
    {
        _typeDict[container.Message.TextType()].Invoke(container);
    }

    private void HandleText(UpdateContainer container)
    {
        switch (container.AppUser.UserStatus)
        {
            case UserStatus.CreatingEvent: case UserStatus.EditingEvent:
                new BuildingEventStatusHandler().Handle(container);
                break;
            case UserStatus.SendingMessage:
                new SendEventMessageHandler().Handle(container);
                break;
            default:
                _log.Error("Unknown UserStatus");
                break;
        }
    }

    private void HandleUnknown(UpdateContainer container)
    {
        _log.Error("Unknown message type");
    }
}