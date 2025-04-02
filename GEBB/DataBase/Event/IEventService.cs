namespace Com.Github.PatBatTB.GEBB.DataBase.Event;

public interface IEventService
{
    ICollection<EventDto> GetInCreating(long creatorId);
    EventDto? Get(string eventId);
    void Merge(EventDto eventDto);
    void Add(int messageId, long creatorId);
    void Remove(string eventId);
    void Remove(ICollection<EventDto> events);
    ICollection<int> RemoveInCreating(long creatorId);
    void FinishCreating(EventDto eventDto);
}