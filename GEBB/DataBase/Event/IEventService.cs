using Com.Github.PatBatTB.GEBB.DataBase.User;
using Com.Github.PatBatTB.GEBB.Domain.Enums;

namespace Com.Github.PatBatTB.GEBB.DataBase.Event;

public interface IEventService
{
    ICollection<AppEvent> GetBuildEvents(long creatorId, EventStatus status);
    AppEvent Get(string eventId);
    void Update(AppEvent appEvent);
    AppEvent Create(long creatorId, int messageId);
    AppEvent Edit(AppEvent appEvent);
    void Remove(string eventId);
    void Remove(ICollection<AppEvent> events);
    ICollection<int> RemoveInBuilding(long creatorId);
    ICollection<int> RemoveInBuilding(long creatorId, EventStatus status);
    ICollection<int> RemoveInBuilding(long creatorId, List<EventStatus> statusList);
    void FinishCreating(AppEvent appEvent);
    void FinishEditing(AppEvent appEvent, out AppEvent oldEvent, out AppEvent newEvent);
    void RegisterUser(AppEvent appEvent, AppUser appUser);
    void CancelRegistration(AppEvent appEvent, AppUser appUser);
    ICollection<AppEvent> GetMyOwnEvents(long creatorId);
    ICollection<AppEvent> GetRegisterEvents(long userId);
    ICollection<AppEvent> GetAvailableEvents(long userId);
}