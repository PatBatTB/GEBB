using Com.Github.PatBatTB.GEBB.DataBase.Event;

namespace Com.Github.PatBatTB.GEBB.DataBase.User;

public interface IUserService
{
    AppUser Get(long userId);
    AppUser Update(Telegram.Bot.Types.User tgUser);
    void Update(AppUser appUser);
    ICollection<AppUser> GetInviteList(AppEvent appEvent);
}