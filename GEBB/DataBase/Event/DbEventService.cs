using Com.Github.PatBatTB.GEBB.DataBase.User;

namespace Com.Github.PatBatTB.GEBB.DataBase.Event;

public class DbEventService : IEventService
{
    private const string Separator = "x";
    private readonly DbUserService _dbUserService = new();


    public ICollection<EventDto> GetInCreating(long creatorId)
    {
        using TgBotDbContext db = new();
        ICollection<EventEntity> entities = db.Events
            .Where(elem => elem.CreatorId == creatorId && elem.IsCreateCompleted == false)
            .ToList();
        return entities.Select(EntityToDto).ToList();
    }

    public EventDto? Get(string eventId)
    {
        (int messageId, long creatorId) = ParseEventId(eventId);
        using TgBotDbContext db = new();
        return (db.Find<EventEntity>(messageId, creatorId) is not { } entity) ? null : EntityToDto(entity);
    }

    public void Merge(EventDto eventDto)
    {
        using TgBotDbContext db = new();
        db.Update(DtoToEntity(eventDto));
        db.SaveChanges();
    }

    public void Add(int messageId, long creatorId)
    {
        EventEntity entity = new()
        {
            EventId = messageId,
            CreatorId = creatorId,
            CreatedAt = DateTime.Now,
            IsActive = false,
            IsCreateCompleted = false
        };
        using TgBotDbContext db = new();
        db.Add(entity);
        db.SaveChanges();
    }

    public void Remove(string eventId)
    {
        using TgBotDbContext db = new();
        db.Find<EventEntity>();
    }

    public void Remove(ICollection<EventDto> events)
    {
        using TgBotDbContext db = new();
        ICollection<EventEntity> eventEntities = events.Select(DtoToEntity).ToList();
        db.RemoveRange(eventEntities);
        db.SaveChanges();
    }

    /// <summary>
    /// Delete all events in status "creating" (IsCreateComplete = false) for specify user.
    /// Returned ID list equals ID of messages with creating menus.
    /// </summary>
    /// <param name="creatorId">ID of event creator.</param>
    /// <returns>ID list of deleting events.</returns>
    public ICollection<int> RemoveInCreating(long creatorId)
    {
        List<EventEntity> eventList = [];
        List<int> messageIdList = [];
        using TgBotDbContext db = new();
        eventList.AddRange(
            db.Events.AsEnumerable()
                .Where(elem =>
                    elem.CreatorId == creatorId &&
                    elem.IsCreateCompleted == false));
        messageIdList.AddRange(eventList.Select(elem => elem.EventId).ToList());
        db.RemoveRange(eventList);
        db.SaveChanges();
        return messageIdList;
    }

    public void FinishCreating(EventDto eventDto)
    {
        EventEntity entity = DtoToEntity(eventDto);
        entity.IsCreateCompleted = true;
        using TgBotDbContext db = new();
        db.Update(entity);
        db.SaveChanges();
    }

    private EventEntity DtoToEntity(EventDto dto)
    {
        (int messageId, long creatorId) = ParseEventId(dto.EventId);

        return new()
        {
            EventId = messageId,
            CreatorId = creatorId,
            Title = dto.Title,
            DateTimeOf = dto.DateTimeOf,
            CreatedAt = dto.CreatedAt,
            Address = dto.Address,
            ParticipantLimit = dto.ParticipantLimit,
            Cost = dto.Cost,
            Description = dto.Description,
            IsCreateCompleted = dto.IsCreateCompleted,
            IsActive = dto.IsActive,
        };
    }

    private EventDto EntityToDto(EventEntity entity)
    {
        using TgBotDbContext db = new();
        UserEntity userEntity = db.Find<UserEntity>(entity.CreatorId) ?? throw new KeyNotFoundException();
        UserDto[] userDtos = entity.Users.Select(user => _dbUserService.EntityToDto(user)).ToArray();
        return new()
        {
            EventId = CreateEventId(entity.EventId, entity.CreatorId),
            Creator = _dbUserService.EntityToDto(userEntity),
            Title = entity.Title,
            Address = entity.Address,
            Cost = entity.Cost,
            DateTimeOf = entity.DateTimeOf,
            CreatedAt = entity.CreatedAt,
            Description = entity.Description,
            MessageId = entity.EventId,
            ParticipantLimit = entity.ParticipantLimit,
            RegisteredUsers = userDtos,
            IsCreateCompleted = entity.IsCreateCompleted,
            IsActive = entity.IsActive,
        };
    }

    private (int messageId, long creatorId) ParseEventId(string eventId)
    {
        string[] ids = eventId.Split(Separator);
        if (ids.Length != 2 || !int.TryParse(ids[0], out int messageId) || !long.TryParse(ids[1], out long creatorId))
        {
            throw new ArgumentException("eventId doesn't match mask \"messageId + x + creatorId\"");
        }

        return (messageId, creatorId);
    }

    private string CreateEventId(int messageId, long creatorId)
    {
        return messageId + Separator + creatorId;
    }
}