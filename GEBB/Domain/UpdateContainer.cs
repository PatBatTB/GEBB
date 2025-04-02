using Com.Github.PatBatTB.GEBB.DataBase.Event;
using Com.Github.PatBatTB.GEBB.DataBase.User;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Com.Github.PatBatTB.GEBB.Domain;

public class UpdateContainer(
    ITelegramBotClient botClient,
    Update update,
    long chatId,
    Message message,
    UserDto userDto,
    CancellationToken token,
    CallbackData? callbackData = null)
{
    public ITelegramBotClient BotClient { get; } = botClient;
    public UpdateType UpdateType { get; } = update.Type;
    public long ChatId { get; } = chatId;
    public Message Message { get; } = message;
    public CallbackData? CallbackData { get; } = callbackData;
    public UserDto UserDto { get; } = userDto;
    public CancellationToken Token { get; } = token;
    public List<EventDto> Events { get; } = [];
}