using Com.Github.PatBatTB.GEBB.DataBase.Entity;
using Com.Github.PatBatTB.GEBB.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Com.Github.PatBatTB.GEBB.Domain;

public class UpdateContainer
{
    public UpdateContainer(ITelegramBotClient botClient, Update update,
        long chatId, User user, Message message, CancellationToken token)
    {
        BotClient = botClient;
        UpdateType = update.Type;
        Token = token;
        ChatId = chatId;
        DatabaseHandler = new DatabaseHandler();
        User = user;
        Message = message;
        CallbackData = CallbackData.GetInstance(update.CallbackQuery);
        UserEntity = DatabaseHandler.Update(user);
    }

    public ITelegramBotClient BotClient { get; }
    public UpdateType UpdateType { get; }
    public long ChatId { get; }
    public User User { get; }
    public DatabaseHandler DatabaseHandler { get; }
    public Message Message { get; }
    public CallbackData? CallbackData { get; }
    public UserEntity UserEntity { get; }
    public CancellationToken Token { get; }
}