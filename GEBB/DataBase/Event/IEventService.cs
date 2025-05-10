using Com.Github.PatBatTB.GEBB.DataBase.User;

namespace Com.Github.PatBatTB.GEBB.DataBase.Event;

public interface IEventService
{
    ICollection<AppEvent> GetInCreating(long creatorId);
    AppEvent? Get(string eventId);
    void Update(AppEvent appEvent);
    AppEvent? Create(long creatorId, int messageId);
    void Remove(string eventId);
    void Remove(ICollection<AppEvent> events);
    ICollection<int> RemoveInCreating(long creatorId);
    void FinishCreating(AppEvent appEvent);
    void RegisterUser(AppEvent appEvent, AppUser appUser);
    void CancelRegistration(AppEvent appEvent, AppUser appUser);
    ICollection<AppEvent> GetMyOwnEvents(long creatorId);
    ICollection<AppEvent> GetRegisterEvents(long userId);
}