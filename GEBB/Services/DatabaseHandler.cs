using Com.Github.PatBatTB.GEBB.DataBase;
using Com.Github.PatBatTB.GEBB.DataBase.Entity;
using Com.Github.PatBatTB.GEBB.Domain;
using Telegram.Bot.Types;

namespace Com.Github.PatBatTB.GEBB.Services;

public static class DatabaseHandler
{
    public static UserEntity Update(User tgUser)
    {
        using TgBotDBContext db = new();
        var user = db.Find<UserEntity>(tgUser.Id);
        if (user is not null)
        {
            user.Username = tgUser.Username ?? "";
        }
        else
        {
            user = new UserEntity
            {
                UserId = tgUser.Id,
                Username = tgUser.Username,
                UserStatus = UserStatus.Newuser,
                RegisteredAt = DateTime.Now
            };
            db.Add(user);
        }

        db.SaveChanges();
        return user;
    }

    public static void Update(UserEntity user)
    {
        using TgBotDBContext db = new();
        db.Update(user);
        db.SaveChanges();
    }
}