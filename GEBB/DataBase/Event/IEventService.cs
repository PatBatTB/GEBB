using Com.Github.PatBatTB.GEBB.DataBase.User;

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
    void RegisterUser(EventDto eventDto, UserDto userDto);
    ICollection<EventDto> GetMy(long creatorId);
}