namespace Com.Github.PatBatTB.GEBB.Domain;

public enum CallbackButton
{
    MyEvents,
    MyRegistrations,
    AvailableEvents,
    Close,
    Create,
    List,
    Back,
    Title,
    TitleDone,
    DateTimeOf,
    DateTimeOfDone,
    Address,
    AddressDone,
    Cost,
    CostDone,
    ParticipantLimit,
    ParticipantLimitDone,
    Description,
    DescriptionDone,
    FinishCreating,
    Yes,
    No,
}

public static class CallbackButtonExtension
{
    public static string Text(this CallbackButton callbackButton)
    {
        return callbackButton switch
        {
            CallbackButton.MyEvents => "Мои мероприятия",
            CallbackButton.MyRegistrations => "Мои регистрации",
            CallbackButton.AvailableEvents => "Доступные мероприятия",
            CallbackButton.Close => "Закрыть",
            CallbackButton.Create => "Создать",
            CallbackButton.List => "Список",
            CallbackButton.Back => "Назад",
            CallbackButton.Title => "Название мероприятия",
            CallbackButton.TitleDone => CallbackButton.Title.Text() + " \u2705",
            CallbackButton.DateTimeOf => "Дата и время",
            CallbackButton.DateTimeOfDone => CallbackButton.DateTimeOf.Text() + " \u2705",
            CallbackButton.Address => "Адрес проведения",
            CallbackButton.AddressDone => CallbackButton.Address.Text() + " \u2705",
            CallbackButton.Cost => "Планируемая стоимость",
            CallbackButton.CostDone => CallbackButton.Cost.Text() + " \u2705",
            CallbackButton.ParticipantLimit => "Максимум человек",
            CallbackButton.ParticipantLimitDone => CallbackButton.ParticipantLimit.Text() + " \u2705",
            CallbackButton.Description => "Описание",
            CallbackButton.DescriptionDone => CallbackButton.Description.Text() + " \u2705",
            CallbackButton.FinishCreating => "Завершить и создать",
            CallbackButton.Yes => "Да",
            CallbackButton.No => "Нет",
            _ => throw new ArgumentException("Incorrect CallBackButton enum")
        };
    }
}