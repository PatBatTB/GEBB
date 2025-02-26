namespace Com.Github.PatBatTB.GEBB.Domain;

public enum CallbackMenu
{
    Main,
    MyEvents,
    CreateEvent
}

public static class CallbackMenuExtension
{
    private const string MainTitle = "Меню пользователя";
    private const string MyEventsTitle = MainTitle + "\n \u21b3 Мои мероприятия";
    private const string CreateEventTitle = MyEventsTitle + "\n    \u21b3 Создание нового мероприятия";

    public static string Title(this CallbackMenu callbackMenu)
    {
        return callbackMenu switch
        {
            CallbackMenu.Main => MainTitle,
            CallbackMenu.MyEvents => MyEventsTitle,
            CallbackMenu.CreateEvent => CreateEventTitle,
            _ => throw new ArgumentException("Incorrect CallbackMenu enum")
        };
    }
}