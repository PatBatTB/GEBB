using Com.Github.PatBatTB.GEBB.DataBase;
using Com.Github.PatBatTB.GEBB.DataBase.Entity;
using TgUser = Telegram.Bot.Types.User;

namespace Com.Github.PatBatTB.GEBB.Services;

public class DatabaseHandler
{
    public async Task Update(TgUser tgUser)
    {
        using TgbotContext db = new();
        var user = db.Find<User>(tgUser.Id);
        if (user is not null)
        {
            user.Username = tgUser.Username ?? "";
            db.Update(user);
        }
        else
        {
            user = new User
            {
                UserId = tgUser.Id,
                Username = tgUser.Username,
                IsActive = true,
                RegisteredAt = DateTime.Now
            };
            db.Add(user);
            await db.SaveChangesAsync();
        }
    }
}