using Telegram.Bot.Requests;

namespace Com.Github.PatBatTB.GEBB.DataBase.Message;

public interface IEventMessageService
{
    AppEventMessage Get(long userId);
    void Update(AppEventMessage eventMessage);
}