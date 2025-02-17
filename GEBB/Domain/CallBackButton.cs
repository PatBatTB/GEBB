namespace Com.Github.PatBatTB.GEBB.Domain;

public enum CallBackButton
{
    MyEvents,
    MyRegistrations,
    AvailableEvents,
    Close,
    Create,
    List,
    Back
}

public static class CallbackButtonExtension
{
    public static string Text(this CallBackButton callBackButton)
    {
        return callBackButton switch
        {
            CallBackButton.MyEvents => "Мои мероприятия",
            CallBackButton.MyRegistrations => "Мои регистрации",
            CallBackButton.AvailableEvents => "Доступные мероприятия",
            CallBackButton.Close => "Закрыть",
            CallBackButton.Create => "Создать",
            CallBackButton.List => "Список",
            CallBackButton.Back => "Назад",
            _ => throw new ArgumentException("Incorrect CallBackButton enum")
        };
    }
}