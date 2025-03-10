namespace Com.Github.PatBatTB.GEBB.Domain.Enums;

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
            CreateEventStatus.Title => "Введите название мероприятия",
            CreateEventStatus.DateTimeOf =>
                "Когда состоится мероприятие?\nУкажите дату и время в формате \"20.02.2025 14.30\"",
            CreateEventStatus.Address => "Где состоится мероприятие? Укажите место проведения или адрес",
            CreateEventStatus.ParticipantLimit =>
                "Сколько человек хотите пригласить? Введите ноль, если нет ограничения на количество",
            CreateEventStatus.Cost =>
                "Сколько денег надо с собой взять? Введите стоимость билета или планируемые затраты на человека)",
            CreateEventStatus.Description =>
                "Расскажите подробнее о мероприятии.\nМожно добавить описание или необходимую дополнительную информацию",
            _ => throw new ArgumentException("Unknown CreateEventStatus")
        };
    }
}