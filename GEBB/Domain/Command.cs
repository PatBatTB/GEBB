namespace Com.Github.PatBatTB.GEBB.Domain;

public enum Command
{
    Start,
    Menu,
    Stop
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
            _ => throw new ArgumentException("Incorrect command enum")
        };
    }

    public static string Description(this Command com)
    {
        return com switch
        {
            Command.Start => "Запустить",
            Command.Stop => "Остановить",
            Command.Menu => "Открыть меню",
            _ => throw new ArgumentException("Incorrect command enum")
        };
    }

    public static List<UserStatus> Scope(this Command com)
    {
        return com switch
        {
            Command.Start => [UserStatus.Newuser, UserStatus.Stop],
            Command.Stop => [UserStatus.Active],
            Command.Menu => [UserStatus.Active],
            _ => throw new ArgumentException("Incorrect command enum")
        };
    }
}