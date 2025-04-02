using Com.Github.PatBatTB.GEBB.DataBase.Event;

namespace Com.Github.PatBatTB.GEBB.DataBase.User;

public interface IUserService
{
    UserDto Merge(Telegram.Bot.Types.User tgUser);
    void Merge(UserDto user);
    void Remove(UserDto user);
    ICollection<UserDto> GetInviteList(EventDto eventDto);
}