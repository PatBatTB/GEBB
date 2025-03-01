namespace Com.Github.PatBatTB.GEBB.Domain;

public enum Command
{
    Start,
    Menu,
    CreateCancel,
    Stop,
}

public static class CommandExtension
{
    public static string Name(this Command com)
    {
        return com switch
        {
            Command.Start => "/start",
            Command.Menu => "/menu",
            Command.CreateCancel => "/cancel",
            Command.Stop => "/stop",
            _ => throw new ArgumentException("Incorrect command enum")
        };
    }

    public static string Description(this Command com)
    {
        return com switch
        {
            Command.Start => "\u27a1\ufe0f Запустить",
            Command.Menu => "\ud83d\udccb Открыть меню",
            Command.CreateCancel => "\u21a9\ufe0f Отменить создание",
            Command.Stop => "\ud83d\udeab Остановить",
            _ => throw new ArgumentException("Incorrect command enum")
        };
    }

    public static List<UserStatus> Scope(this Command com)
    {
        return com switch
        {
            Command.Start => [UserStatus.Newuser, UserStatus.Stop],
            Command.Menu => [UserStatus.Active],
            Command.CreateCancel => [UserStatus.CreatingEvent],
            Command.Stop => [UserStatus.Active, UserStatus.OpenedMenu, UserStatus.CreatingEvent],
            _ => throw new ArgumentException("Incorrect command enum")
        };
    }
}