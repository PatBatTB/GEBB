using Com.Github.PatBatTB.GEBB.DataBase.Entity;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Com.Github.PatBatTB.GEBB.Domain;

public class UpdateContainer(
    ITelegramBotClient botClient,
    Update update,
    long chatId,
    User user,
    Message message,
    UserEntity userEntity,
    CancellationToken token,
    CallbackData? callbackData = null)
{
    public ITelegramBotClient BotClient { get; } = botClient;
    public UpdateType UpdateType { get; } = update.Type;
    public long ChatId { get; } = chatId;
    public User User { get; } = user;
    public Message Message { get; } = message;
    public CallbackData? CallbackData { get; } = callbackData;
    public UserEntity UserEntity { get; } = userEntity;
    public CancellationToken Token { get; } = token;
    public List<EventEntity> EventEntity { get; } = [];
}