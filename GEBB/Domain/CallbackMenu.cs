namespace Com.Github.PatBatTB.GEBB.Domain;

public enum CallbackMenu
{
    Main,
    MyEvents
}

public static class CallbackMenuExtension
{
    private const string MainMessage = "Меню пользователя";
    private const string MyEvents = MainMessage + "\n \u21b3 Мои мероприятия";

    public static string Message(this CallbackMenu callbackMenu)
    {
        return callbackMenu switch
        {
            CallbackMenu.Main => MainMessage,
            CallbackMenu.MyEvents => MyEvents,
            _ => throw new ArgumentException("Incorrect CallbackMenu enum")
        };
    }
}