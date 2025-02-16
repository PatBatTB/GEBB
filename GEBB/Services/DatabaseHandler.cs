using Com.Github.PatBatTB.GEBB.DataBase;
using Com.Github.PatBatTB.GEBB.DataBase.Entity;
using Com.Github.PatBatTB.GEBB.Domain;
using Telegram.Bot.Types;

namespace Com.Github.PatBatTB.GEBB.Services;

public class DatabaseHandler
{
    public UserEntity Update(User tgUser, out UserStatus userStatus)
    {
        using TgbotContext db = new();
        var user = db.Find<UserEntity>(tgUser.Id);
        if (user is not null)
        {
            user.Username = tgUser.Username ?? "";
            userStatus = user.IsActive ? UserStatus.Active : UserStatus.Stop;
        }
        else
        {
            user = new UserEntity
            {
                UserId = tgUser.Id,
                Username = tgUser.Username,
                IsActive = true,
                RegisteredAt = DateTime.Now
            };
            db.Add(user);
            userStatus = UserStatus.Newuser;
        }

        db.SaveChanges();
        return user;
    }

    public void Update(UserEntity user)
    {
        using TgbotContext db = new();
        db.Update(user);
        db.SaveChanges();
    }
}