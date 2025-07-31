namespace Com.Github.PatBatTB.GEBB.Domain.Enums;

/// <summary>
///     These statuses are serializable as INT. Don't change these order!
/// </summary>
public enum UserStatus
{
    Stop,
    Newuser,
    Active,
    OpenedMenu,
    CreatingEvent,
    EditingEvent,
    SendingMessage,
}