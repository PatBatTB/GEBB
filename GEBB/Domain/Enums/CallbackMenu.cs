namespace Com.Github.PatBatTB.GEBB.Domain.Enums;

public enum CallbackMenu
{
    //TODO Для Callbackdata использовать технические названия, а не имя энама
    Main,
    EventsList,
    CreateEvent,
    EditEvent,
    EventTitleReplace,
    EventDateTimeOfAgain,
    EventDateTimeOfReplace,
    EventAddressReplace,
    EventCostReplace,
    EventPartLimitReplace,
    EventDescrReplace,
    RegisterToEvent,
    CreatedEvent,
    RegEventDescr,
    RegEventPart,
    CreEventPart,
}

public static class CallbackMenuExtension
{
    private const string MainText = "Меню пользователя";
    private const string EventListText = MainText + "\n \u21b3 Список мероприятий";
    private const string CreateEventText = MainText + "\n    \u21b3 Создание нового мероприятия";
    private const string EditEventText = "Редактирование мероприятия\n";
    private const string EventTitleReplaceText = "Название мероприятия уже задано. Изменить?";
    private const string EventDateTimeOfAgainText = "Неправильный формат. Ввести снова?";
    private const string EventDateTimeOfReplaceText = "Дата уже была указана. Изменить?";
    private const string EventAddressReplaceText = "Место проведения уже указано. Изменить?";
    private const string EventCostReplaceText = "Стоимость уже указана. Изменить?";
    private const string EventParticipantLimitReplaceText = "Количество участников уже указано. Изменить?";
    private const string EventDescriptionReplaceText = "Описание уже указано. Изменить?";

    public static string Text(this CallbackMenu callbackMenu)
    {
        return callbackMenu switch
        {
            CallbackMenu.Main => MainText,
            CallbackMenu.EventsList => EventListText,
            CallbackMenu.CreateEvent => CreateEventText,
            CallbackMenu.EditEvent => EditEventText,
            CallbackMenu.EventTitleReplace => EventTitleReplaceText,
            CallbackMenu.EventDateTimeOfAgain => EventDateTimeOfAgainText,
            CallbackMenu.EventDateTimeOfReplace => EventDateTimeOfReplaceText,
            CallbackMenu.EventAddressReplace => EventAddressReplaceText,
            CallbackMenu.EventCostReplace => EventCostReplaceText,
            CallbackMenu.EventPartLimitReplace => EventParticipantLimitReplaceText,
            CallbackMenu.EventDescrReplace => EventDescriptionReplaceText,
            CallbackMenu.RegisterToEvent => throw new ArgumentException("RegisterToEvent has no text"),
            CallbackMenu.CreatedEvent => throw new ArgumentException("CreatedEvent has no text"),
            CallbackMenu.RegEventDescr => throw new ArgumentException("RegisteredEventDescription has no text"),
            CallbackMenu.RegEventPart => throw new ArgumentException("RegisteredEventParticipants has no text"),
            CallbackMenu.CreEventPart => throw new ArgumentException("CreateEventParticipants has no text"),
            _ => throw new ArgumentException("Incorrect CallbackMenu enum")
        };
    }
}