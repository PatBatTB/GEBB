using Com.Github.PatBatTB.GEBB.DataBase;
using Com.Github.PatBatTB.GEBB.DataBase.Entity;
using Com.Github.PatBatTB.GEBB.Domain.Enums;
using Telegram.Bot.Types;

namespace Com.Github.PatBatTB.GEBB.Services.Handlers;

public static class DatabaseHandler
{
    public static UserEntity Update(User tgUser)
    {
        using TgBotDbContext db = new();
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

    public static void Update<TEntity>(TEntity entity)
    {
        using TgBotDbContext db = new();
        db.Update(entity);
        db.SaveChanges();
    }

    public static void Remove<TEntity>(IEnumerable<TEntity> entities)
    {
        using TgBotDbContext db = new();
        db.RemoveRange(entities);
        db.SaveChanges();
    }

    /// <summary>
    /// Delete all events in status "creating" (IsCreateComplete = false) for specify user.
    /// Returned ID list equals ID of messages with creating menus.
    /// </summary>
    /// <param name="userId">ID of event creator.</param> 
    /// <returns>ID list of deleting events.</returns> 
    public static List<int> DeleteCreatingEvents(long userId)
    {
        List<EventEntity> eventList = [];
        List<int> idList = [];
        using TgBotDbContext db = new();
        eventList.AddRange(
            db.Events.AsEnumerable()
                .Where(elem =>
                    elem.CreatorId == userId &&
                    elem.IsCreateCompleted == false));
        idList.AddRange(eventList.Select(elem => elem.EventId).ToList());
        db.RemoveRange(eventList);
        db.SaveChanges();
        return idList;
    }

    /// <summary>
    /// Returns a list of all active users from the database, excluding the creator, to be invited to the event.
    /// </summary>
    /// <param name="entity">Event</param>
    /// <returns>List of usersID</returns>
    public static List<long> GetInviteList(EventEntity entity)
    {
        using TgBotDbContext db = new();
        return db.Users
            .Where(user => user.UserStatus != UserStatus.Stop) // &&
            //user.UserId != entity.CreatorId) //TODO Debug (отправляю себе же приглашение для отладки)
            .Select(user => user.UserId)
            .ToList();
    }
}