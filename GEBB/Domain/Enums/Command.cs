namespace Com.Github.PatBatTB.GEBB.Domain.Enums;

public enum Command
{
    Start,
    Menu,
    Cancel,
    Report,
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
            Command.Cancel => "/cancel",
            Command.Report => "/report",
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
            Command.Cancel => "\u21a9\ufe0f Отменить",
            Command.Report => "\uD83D\uDCDD Сообщить об ошибке",
            Command.Stop => "\ud83d\udeab Остановить",
            _ => throw new ArgumentException("Incorrect command enum")
        };
    }

    public static List<UserStatus> Scope(this Command com)
    {
        return com switch
        {
            Command.Start => [UserStatus.Newuser, UserStatus.Stop],
            Command.Menu => [UserStatus.Active, UserStatus.OpenedMenu],
            Command.Cancel => [UserStatus.CreatingEvent, UserStatus.EditingEvent, UserStatus.SendingMessage],
            Command.Report => [UserStatus.Active, UserStatus.CreatingEvent, UserStatus.EditingEvent, UserStatus.OpenedMenu],
            Command.Stop => [UserStatus.Active, UserStatus.OpenedMenu],
            _ => throw new ArgumentException("Incorrect command enum")
        };
    }
}