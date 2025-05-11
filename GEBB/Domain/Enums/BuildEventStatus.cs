namespace Com.Github.PatBatTB.GEBB.Domain.Enums;

public enum BuildEventStatus
{
    CreateTitle,
    CreateDateTimeOf,
    CreateAddress,
    CreateParticipantLimit,
    CreateCost,
    CreateDescription,
    EditTitle,
    EditDateTimeOf,
    EditAddress,
    EditParticipantLimit,
    EditCost,
    EditDescription,
}

public static class CreateEventStatusExtension
{
    public static string Message(this BuildEventStatus status)
    {
        return status switch
        {
            BuildEventStatus.CreateTitle => 
                "Введите название мероприятия",
            BuildEventStatus.CreateDateTimeOf =>
                "Когда состоится мероприятие?\nУкажите дату и время в формате \"20.02.2025 14.30\"",
            BuildEventStatus.CreateAddress => 
                "Где состоится мероприятие?\nУкажите место проведения или адрес",
            BuildEventStatus.CreateParticipantLimit =>
                "Сколько человек хотите пригласить?\n(Введите ноль, если нет ограничения на количество)",
            BuildEventStatus.CreateCost =>
                "Сколько денег надо с собой взять?\n(Введите стоимость билета или планируемые затраты на человека)",
            BuildEventStatus.CreateDescription =>
                "Расскажите подробнее о мероприятии.\nМожно добавить описание или необходимую дополнительную информацию",
            BuildEventStatus.EditTitle =>
                "Введите новое название мероприятия",
            BuildEventStatus.EditDateTimeOf =>
                "Введите новую дату мероприятия в формате \"20.02.2025 14.30\"",
            BuildEventStatus.EditAddress =>
                "Укажите новое место проведения",
            BuildEventStatus.EditParticipantLimit =>
                "Укажите новое количество гостей.\n(Количество должно быть не меньше, чем уже зарегистрировалось людей)",
            BuildEventStatus.EditCost =>
                "Введите новую стоимость билета или планируемые затраты на человека.",
            BuildEventStatus.EditDescription =>
                "Введите новое описание мероприятия",
            _ => throw new ArgumentException("Unknown CreateEventStatus")
        };
    }
}