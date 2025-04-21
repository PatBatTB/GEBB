using Com.Github.PatBatTB.GEBB.DataBase.Event;

namespace Com.Github.PatBatTB.GEBB.DataBase.User;

public interface IUserService
{
    UserDto Update(Telegram.Bot.Types.User tgUser);
    void Update(UserDto user);
    void Remove(UserDto user);
    ICollection<UserDto> GetInviteList(EventDto eventDto);
}