using Com.Github.PatBatTB.GEBB.DataBase.User;
using Com.Github.PatBatTB.GEBB.Domain.Enums;
using Com.GitHub.PatBatTB.GEBB.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Com.Github.PatBatTB.GEBB.DataBase.Event;

public class DbEventService : IEventService
{
    private const string Separator = "x";
    private readonly DbUserService _dbUserService = new();


    public ICollection<AppEvent> GetBuildEvents(long creatorId, EventStatus status)
    {
        using TgBotDbContext db = new();
        ICollection<BuildEventEntity> entities = db.TempEvents
            .Where(elem => elem.CreatorId == creatorId && elem.Status == status)
            .ToList();
        return entities.Select(TempEntityToEvent).ToList();
    }

    public AppEvent Get(string eventId)
    {
        (int messageId, long creatorId) = ParseEventId(eventId);
        using TgBotDbContext db = new();
        EventEntity eventEntity = db.Events
            .Include(elem => elem.RegisteredUsers)
            .First(elem => elem.EventId == messageId && elem.CreatorId == creatorId);
        return (eventEntity is not { } entity) ? throw new EntityNotFoundException() : EntityToEvent(entity);
    }

    public ICollection<AppEvent> GetMyOwnEvents(long creatorId)
    {
        using TgBotDbContext db = new();
        ICollection<EventEntity> entities = db.Events
            .Include(elem => elem.RegisteredUsers)
            .Where(elem => elem.CreatorId == creatorId && elem.DateTimeOf > DateTime.Now && elem.Status == EventStatus.Active)
            .ToList();
        return entities.Select(EntityToEvent).ToList();
    }

    public void Update(AppEvent appEvent)
    {
        using TgBotDbContext db = new();
        switch (appEvent.Status)
        {
            case EventStatus.Creating: case EventStatus.Editing:
                db.Update(EventToBuildEntity(appEvent));
                break;
            default:
                db.Update(EventToEntity(appEvent));
                break;
        }
        db.SaveChanges();
    }

    public AppEvent Create(long creatorId, int messageId)
    {
        using TgBotDbContext db = new();
        BuildEventEntity entity = new()
        {
            EventId = GetNextEventId(creatorId, db),
            MessageId = messageId,
            CreatorId = creatorId,
            CreatedAt = DateTime.Now,
            Status = EventStatus.Creating,
        };
        db.Add(entity);
        db.SaveChanges();
        return TempEntityToEvent(entity);
    }

    public AppEvent Edit(AppEvent appEvent)
    {
        using TgBotDbContext db = new();
        appEvent.Status = EventStatus.Editing;
        db.Add(EventToBuildEntity(appEvent));
        db.SaveChanges();
        return appEvent;
    }

    public void Remove(string eventId)
    {
        (int messageId, long creatorId) = ParseEventId(eventId);
        using TgBotDbContext db = new();
        if (db.Find<EventEntity>(messageId, creatorId) is not { } entity)
        {
            throw new Exception("Event not found in DB");
        }

        entity.Status = EventStatus.Deleted;
        db.Update(entity);
        db.SaveChanges();
    }

    public void Remove(ICollection<AppEvent> events)
    {
        using TgBotDbContext db = new();
        events.ToList().ForEach(elem => elem.Status = EventStatus.Deleted);
        ICollection<EventEntity> eventEntities = events.Select(EventToEntity).ToList();
        db.UpdateRange(eventEntities);
        db.SaveChanges();
    }

    /// <summary>
    /// Delete all events in building statuses (Creating, Editing)
    /// Returned ID list equals ID of messages with creating menus.
    /// </summary>
    /// <param name="creatorId">ID of event creator.</param>
    /// <param name="statusList">List of statuses to delete.</param>
    /// <returns>ID list of deleting events.</returns>
    public ICollection<int> RemoveInBuilding(long creatorId, List<EventStatus> statusList)
    {
        List<BuildEventEntity> eventList = [];
        List<int> messageIdList = [];
        using TgBotDbContext db = new();
        eventList.AddRange(
            db.TempEvents.AsEnumerable()
                .Where(elem =>
                    elem.CreatorId == creatorId && statusList.Contains(elem.Status)));
        messageIdList.AddRange(eventList.Select(elem => elem.EventId).ToList());
        db.RemoveRange(eventList);
        db.SaveChanges();
        return messageIdList;
    }

    public ICollection<int> RemoveInBuilding(long creatorId)
    {
        return RemoveInBuilding(creatorId, Enum.GetValues<EventStatus>().ToList());
    }

    public ICollection<int> RemoveInBuilding(long creatorId, EventStatus status)
    {
        return RemoveInBuilding(creatorId, [status]);
    }

    public void FinishCreating(AppEvent appEvent)
    {
        BuildEventEntity buildEntity = EventToBuildEntity(appEvent);
        EventEntity entity = EventToEntity(appEvent);
        entity.Status = EventStatus.Active;
        using TgBotDbContext db = new();
        db.Add(entity);
        db.Remove(buildEntity);
        db.SaveChanges();
    }

    public void FinishEditing(AppEvent appEvent, out AppEvent oldEvent, out AppEvent newEvent)
    {
        (int eventId, long creatorId) = ParseEventId(appEvent.Id);
        BuildEventEntity buildEntity = EventToBuildEntity(appEvent);
        using TgBotDbContext db = new();
        List<EventEntity> entities = db.Events.Where(elem => elem.EventId == eventId && elem.CreatorId == creatorId)
            .Include(elem => elem.RegisteredUsers)
            .ToList();
        if (entities.Count == 0) throw new EntityNotFoundException();
        EventEntity eventEntity = entities[0];
        oldEvent = EntityToEvent(eventEntity);

        eventEntity.Title = appEvent.Title;
        eventEntity.Address = appEvent.Address;
        eventEntity.DateTimeOf = appEvent.DateTimeOf;
        eventEntity.Cost = appEvent.Cost;
        eventEntity.ParticipantLimit = appEvent.ParticipantLimit;
        eventEntity.Description = appEvent.Description;
        newEvent = EntityToEvent(eventEntity);
        db.Update(eventEntity);
        db.Remove(buildEntity);
        db.SaveChanges();
    }

    public void RegisterUser(AppEvent appEvent, AppUser appUser)
    {
        using TgBotDbContext db = new();
        (int eventId, long creatorId) = ParseEventId(appEvent.Id);
        db.Database.ExecuteSqlRaw(
            """
            INSERT INTO Registrations VALUES 
            ({0}, {1}, {2})
            """, appUser.UserId, eventId, creatorId);
    }

    public void CancelRegistration(AppEvent appEvent, AppUser appUser)
    {
        (int messageId, long creatorId) = ParseEventId(appEvent.Id);
        long userId = appUser.UserId;
        using TgBotDbContext db = new();
        db.Database.ExecuteSqlRaw(
            """
            DELETE FROM Registrations
            WHERE UserId = {0} AND EventId = {1} AND CreatorId = {2}
            """, userId, messageId, creatorId);
    }

    public ICollection<AppEvent> GetRegisterEvents(long userId)
    {
        using TgBotDbContext db = new();
        if (db.Users.Find(userId) is not { } user)
        {
            throw new Exception("User not found in DB.");
        }
        List<EventEntity> eventEntities = db.Events
            .Include(elem => elem.RegisteredUsers)
            .Where(elem => elem.RegisteredUsers.Contains(user) && 
                           elem.DateTimeOf > DateTime.Now && 
                           elem.Status == EventStatus.Active)
            .ToList();
        return eventEntities.Select(EntityToEvent).ToList();
    }

    public ICollection<AppEvent> GetAvailableEvents(long userId)
    {
        using TgBotDbContext db = new();
        if (db.Users.Find(userId) is not { } user)
        {
            throw new Exception("User not found in DB.");
        }

        List<EventEntity> eventEntities = db.Events
            .Include(elem => elem.RegisteredUsers)
            .Where(elem => !elem.RegisteredUsers.Contains(user) &&
                           elem.CreatorId != userId &&
                           (elem.ParticipantLimit == 0 || elem.ParticipantLimit > elem.RegisteredUsers.Count) &&
                           elem.DateTimeOf > DateTime.Now && 
                           elem.Status == EventStatus.Active)
            .ToList();
        return eventEntities.Select(EntityToEvent).ToList();
    }

    public EventEntity EventToEntity(AppEvent appEvent)
    {
        (int eventId, long creatorId) = ParseEventId(appEvent.Id);
        ICollection<UserEntity> userEntities = appEvent.RegisteredUsers.Select(_dbUserService.UserToEntity).ToList();
        return new()
        {
            EventId = eventId,
            CreatorId = creatorId,
            Title = appEvent.Title,
            DateTimeOf = appEvent.DateTimeOf,
            CreatedAt = appEvent.CreatedAt,
            Address = appEvent.Address,
            ParticipantLimit = appEvent.ParticipantLimit,
            Cost = appEvent.Cost,
            Description = appEvent.Description,
            RegisteredUsers = userEntities,
            Status = appEvent.Status,
        };
    }

    public AppEvent EntityToEvent(EventEntity entity)
    {
        using TgBotDbContext db = new();
        UserEntity userEntity = db.Find<UserEntity>(entity.CreatorId) ?? throw new Exception("User not found in DB");
        AppUser[] appUsers = entity.RegisteredUsers.Select(user => _dbUserService.EntityToUser(user)).ToArray();
        return new()
        {
            Id = CreateEventId(entity.EventId, entity.CreatorId),
            Creator = _dbUserService.EntityToUser(userEntity),
            Title = entity.Title,
            Address = entity.Address,
            Cost = entity.Cost,
            DateTimeOf = entity.DateTimeOf,
            CreatedAt = entity.CreatedAt,
            Description = entity.Description,
            ParticipantLimit = entity.ParticipantLimit,
            RegisteredUsers = appUsers,
            Status = entity.Status,
        };
    }
    
    private AppEvent TempEntityToEvent(BuildEventEntity entity)
    {
        using TgBotDbContext db = new();
        UserEntity userEntity = db.Find<UserEntity>(entity.CreatorId) ?? throw new Exception("User not found in DB");
        return new()
        {
            Id = CreateEventId(entity.EventId, entity.CreatorId),
            Creator = _dbUserService.EntityToUser(userEntity),
            MessageId = entity.MessageId,
            Title = entity.Title,
            Address = entity.Address,
            Cost = entity.Cost,
            DateTimeOf = entity.DateTimeOf,
            CreatedAt = entity.CreatedAt,
            Description = entity.Description,
            ParticipantLimit = entity.ParticipantLimit,
            Status = entity.Status,
        };
    }

    private BuildEventEntity EventToBuildEntity(AppEvent appEvent)
    {
        (int eventId, long creatorId) = ParseEventId(appEvent.Id);
        return new()
        {
            EventId = eventId,
            CreatorId = creatorId,
            Title = appEvent.Title,
            MessageId = appEvent.MessageId,
            DateTimeOf = appEvent.DateTimeOf,
            CreatedAt = appEvent.CreatedAt,
            Address = appEvent.Address,
            ParticipantLimit = appEvent.ParticipantLimit,
            Cost = appEvent.Cost,
            Description = appEvent.Description,
            Status = appEvent.Status,
        };
    }

    public (int eventId, long creatorId) ParseEventId(string appEventId)
    {
        string[] ids = appEventId.Split(Separator);
        if (ids.Length != 2 || !int.TryParse(ids[0], out int eventId) || !long.TryParse(ids[1], out long creatorId))
        {
            throw new ArgumentException("eventId doesn't match mask \"eventId + x + creatorId\"");
        }

        return (eventId, creatorId);
    }

    public string CreateEventId(int eventId, long creatorId)
    {
        return eventId + Separator + creatorId;
    }

    private int GetNextEventId(long creatorId, TgBotDbContext db)
    {
        IQueryable<int> ids = db.Events.Where(elem => elem.CreatorId == creatorId).Select(elem => elem.EventId);
        return ids.Any() ? ids.Max() + 1 : 1;
    }
}