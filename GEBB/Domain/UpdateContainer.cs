using Com.Github.PatBatTB.GEBB.Services;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Com.Github.PatBatTB.GEBB.Domain;

public class UpdateContainer
{
    public UpdateContainer(ITelegramBotClient botClient, Update update,
        CancellationToken token, long chatId, User user, Message message)
    {
        BotClient = botClient;
        Update = update;
        Token = token;
        ChatId = chatId;
        DatabaseHandler = new DatabaseHandler();
        User = user;
        Message = message;
        _ = DatabaseHandler.Update(user);
    }

    public ITelegramBotClient BotClient { get; init; }
    public Update Update { get; init; }
    public CancellationToken Token { get; init; }
    public long ChatId { get; init; }
    public User User { get; init; }
    public DatabaseHandler DatabaseHandler { get; init; }
    public Message Message { get; init; }
}