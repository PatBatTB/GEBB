namespace Com.Github.PatBatTB.GEBB.Domain.Enums;

public enum CallbackMenu
{
    Main,
    MyEvents,
    CreateEvent,
    EventTitleReplace,
    EventDateTimeOfAgain,
    EventDateTimeOfReplace,
    EventAddressReplace,
    EventCostReplace,
    EventParticipantLimitReplace,
    EventDescriptionReplace,
    EventRegister,
    EventHandle,
}

public static class CallbackMenuExtension
{
    private const string MainText = "Меню пользователя";
    private const string MyEventsText = MainText + "\n \u21b3 Мои мероприятия";
    private const string CreateEventText = MyEventsText + "\n    \u21b3 Создание нового мероприятия";
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
            CallbackMenu.MyEvents => MyEventsText,
            CallbackMenu.CreateEvent => CreateEventText,
            CallbackMenu.EventTitleReplace => EventTitleReplaceText,
            CallbackMenu.EventDateTimeOfAgain => EventDateTimeOfAgainText,
            CallbackMenu.EventDateTimeOfReplace => EventDateTimeOfReplaceText,
            CallbackMenu.EventAddressReplace => EventAddressReplaceText,
            CallbackMenu.EventCostReplace => EventCostReplaceText,
            CallbackMenu.EventParticipantLimitReplace => EventParticipantLimitReplaceText,
            CallbackMenu.EventDescriptionReplace => EventDescriptionReplaceText,
            CallbackMenu.EventRegister => throw new ArgumentException("EventRegister has no text"),
            CallbackMenu.EventHandle => throw new ArgumentException("EventHandle has no text"),
            _ => throw new ArgumentException("Incorrect CallbackMenu enum")
        };
    }
}