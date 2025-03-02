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

    public static void Update(EventEntity eventEntity)
    {
        using TgBotDBContext db = new();
        db.Update(eventEntity);
        db.SaveChanges();
    }

    public static void Remove<TEntity>(IEnumerable<TEntity> entities)
    {
        using TgBotDBContext db = new();
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
        using TgBotDBContext db = new();
        eventList.AddRange(
            db.Events.AsEnumerable()
                .Where(elem =>
                    elem.CreatorId == userId &&
                    elem.IsCreateCompleted == false));
        //записать ID
        idList.AddRange(eventList.Select(elem => elem.EventId).ToList());
        //удалить все мероприятия в базе в режиме создания.
        db.RemoveRange(eventList);
        db.SaveChanges();
        return idList;
    }
}