namespace Com.Github.PatBatTB.GEBB.Domain;

public enum Command
{
    Start,
    Menu,
    Stop,
    CreateCancel
}

public static class CommandExtension
{
    public static string Name(this Command com)
    {
        return com switch
        {
            Command.Start => "/start",
            Command.Stop => "/stop",
            Command.Menu => "/menu",
            Command.CreateCancel => "/cancel",
            _ => throw new ArgumentException("Incorrect command enum")
        };
    }

    public static string Description(this Command com)
    {
        return com switch
        {
            Command.Start => "\u27a1\ufe0f Запустить",
            Command.Stop => "\ud83d\udeab Остановить",
            Command.Menu => "\ud83d\udccb Открыть меню",
            Command.CreateCancel => "\u21a9\ufe0f Отменить создание",
            _ => throw new ArgumentException("Incorrect command enum")
        };
    }

    public static List<UserStatus> Scope(this Command com)
    {
        return com switch
        {
            Command.Start => [UserStatus.Newuser, UserStatus.Stop],
            Command.Stop => [UserStatus.Active, UserStatus.CreateEvent],
            Command.Menu => [UserStatus.Active],
            Command.CreateCancel => [UserStatus.CreateEvent],
            _ => throw new ArgumentException("Incorrect command enum")
        };
    }
}