using Com.Github.PatBatTB.GEBB.DataBase.Event;
using Com.Github.PatBatTB.GEBB.Domain.Enums;

namespace Com.Github.PatBatTB.GEBB.DataBase.User;

public class DbUserService : IUserService
{
    public AppUser Update(Telegram.Bot.Types.User tgUser)
    {
        using TgBotDbContext db = new();
        if (db.Users.Find(tgUser.Id) is { } user)
        {
            user.Username = tgUser.Username;
        }
        else
        {
            user = new UserEntity
            {
                UserId = tgUser.Id,
                Username = tgUser.Username,
                Status = UserStatus.Newuser,
                RegisteredAt = DateTime.Now
            };
            db.Add(user);
        }

        db.SaveChanges();
        return EntityToUser(user);
    }

    public void Update(AppUser appUser)
    {
        using TgBotDbContext db = new();
        db.Update(UserToEntity(appUser));
        db.SaveChanges();
    }

    public void Remove(AppUser appUser)
    {
        using TgBotDbContext db = new();
        db.Remove(UserToEntity(appUser));
        db.SaveChanges();
    }

    /// <summary>
    /// Returns a list of all active users from the database, excluding the creator, to be invited to the event.
    /// </summary>
    /// <param name="entity">Event</param>
    /// <returns>List of usersID</returns>
    public ICollection<AppUser> GetInviteList(AppEvent appEvent)
    {
        using TgBotDbContext db = new();
        ICollection<UserEntity> entities = db.Users
            .Where(user => user.Status != UserStatus.Stop && user.UserId != appEvent.Creator.UserId)
            .ToList();
        return entities.Select(EntityToUser).ToList();
    }

    public AppUser EntityToUser(UserEntity entity)
    {
        return new()
        {
            UserId = entity.UserId,
            Username = entity.Username,
            UserStatus = entity.Status,
            RegisteredAt = entity.RegisteredAt,
        };
    }

    public UserEntity UserToEntity(AppUser appUser)
    {
        return new()
        {
            UserId = appUser.UserId,
            Username = appUser.Username,
            Status = appUser.UserStatus,
            RegisteredAt = appUser.RegisteredAt
        };
    }
}