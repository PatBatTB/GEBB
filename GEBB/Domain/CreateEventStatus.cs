namespace Com.Github.PatBatTB.GEBB.Domain;

public enum CreateEventStatus
{
    Title,
    DateTimeOf,
    Address,
    ParticipantLimit,
    Cost,
    Description
}

public static class CreateEventStatusExtension
{
    public static string Message(this CreateEventStatus status)
    {
        return status switch
        {
            //TODO Заполнить для остальных полей
            CreateEventStatus.Title => "Введите название мероприятия",
            _ => throw new ArgumentException("Unknown CreateEventStatus")
        };
    }
}