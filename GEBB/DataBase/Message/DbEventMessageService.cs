using Com.Github.PatBatTB.GEBB.DataBase.Event;
using Com.Github.PatBatTB.GEBB.DataBase.User;
using Com.GitHub.PatBatTB.GEBB.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Com.Github.PatBatTB.GEBB.DataBase.Message;

public class DbEventMessageService : IEventMessageService
{
    private readonly DbEventService _dbEventService = new DbEventService();
    private readonly DbUserService _dbUserService = new DbUserService();
    
    public AppEventMessage Get(long userId)
    {
        using TgBotDbContext db = new();
        EventMessageEntity? entity = db.EventMessages
            .Include(e => e.Event).ThenInclude(ev => ev.RegisteredUsers)
            .Include(e => e.User)
            .First(e => e.UserId == userId);
        if (entity is null)
        {
            throw new EntityNotFoundException("UserId not found");
        }

        return EntityToEventMessage(entity);
    }

    public void Update(AppEventMessage eventMessage)
    {
        using TgBotDbContext db = new();
        if (db.EventMessages.Find(eventMessage.User.UserId) is not { } entity)
        {
            db.Add(EventMessageToEntity(eventMessage));
        }
        else
        {
            (int eventId, long creatorId) = _dbEventService.ParseEventId(eventMessage.Event.Id);
            entity.EventId = eventId;
            entity.CreatorId = creatorId;
            db.Update(entity);
        }

        db.SaveChanges();
    }

    public AppEventMessage EntityToEventMessage(EventMessageEntity entity)
    {
        return new AppEventMessage
        {
            Event = _dbEventService.EntityToEvent(entity.Event),
            User = _dbUserService.EntityToUser(entity.User)
        };
    }

    public EventMessageEntity EventMessageToEntity(AppEventMessage eventMessage)
    {
        (int eventId, long creatorId) = _dbEventService.ParseEventId(eventMessage.Event.Id);
        return new EventMessageEntity
        {
            UserId = eventMessage.User.UserId,
            EventId = eventId,
            CreatorId = creatorId
        };
    }

}