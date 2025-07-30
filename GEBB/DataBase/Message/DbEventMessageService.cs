using Com.Github.PatBatTB.GEBB.DataBase.Event;
using Com.GitHub.PatBatTB.GEBB.Exceptions;

namespace Com.Github.PatBatTB.GEBB.DataBase.Message;

public class DbEventMessageService : IEventMessageService
{
    private DbEventService _dbEventService = new DbEventService();
    
    public AppEventMessage Get(long userId)
    {
        using TgBotDbContext db = new();
        EventMessageEntity? entity = db.EventMessages.Find(userId);
        if (entity is null)
        {
            throw new EntityNotFoundException("UserId not found");
        }

        return EntityToEventMessage(entity);
    }

    public void Update(AppEventMessage eventMessage)
    {
        using TgBotDbContext db = new();
        if (db.EventMessages.Find(eventMessage.UserId) is not { } entity)
        {
            db.Add(EventMessageToEntity(eventMessage));
        }
        else
        {
            (int eventId, long creatorId) = _dbEventService.ParseEventId(eventMessage.EventId);
            entity.EventId = eventId;
            entity.CreatorId = creatorId;
            db.Update(entity);
        }

        db.SaveChanges();
    }

    private AppEventMessage EntityToEventMessage(EventMessageEntity entity)
    {
        return new AppEventMessage
        {
            EventId = _dbEventService.CreateEventId(entity.EventId, entity.CreatorId),
            UserId = entity.UserId
        };
    }

    private EventMessageEntity EventMessageToEntity(AppEventMessage eventMessage)
    {
        (int eventId, long creatorId) = _dbEventService.ParseEventId(eventMessage.EventId);
        return new EventMessageEntity
        {
            UserId = eventMessage.UserId,
            EventId = eventId,
            CreatorId = creatorId
        };
    }

}