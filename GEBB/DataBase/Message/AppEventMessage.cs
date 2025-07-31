using Com.Github.PatBatTB.GEBB.DataBase.Event;
using Com.Github.PatBatTB.GEBB.DataBase.User;

namespace Com.Github.PatBatTB.GEBB.DataBase.Message;

public class AppEventMessage
{
    public AppUser User { get; set; }
    public AppEvent Event { get; set; }
}