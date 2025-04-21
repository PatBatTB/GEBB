namespace Com.Github.PatBatTB.GEBB.Domain.Enums;

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
    Registration,
    Cancel,
    Edit,
    ParticipantList,
    CancelRegistration,
    ToDescription,
}

public static class CallbackButtonExtension
{
    private const string DoneSymbol = " \u2705";

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
            CallbackButton.TitleDone => CallbackButton.Title.Text() + DoneSymbol,
            CallbackButton.DateTimeOf => "Дата и время",
            CallbackButton.DateTimeOfDone => CallbackButton.DateTimeOf.Text() + DoneSymbol,
            CallbackButton.Address => "Адрес проведения",
            CallbackButton.AddressDone => CallbackButton.Address.Text() + DoneSymbol,
            CallbackButton.Cost => "Планируемая стоимость",
            CallbackButton.CostDone => CallbackButton.Cost.Text() + DoneSymbol,
            CallbackButton.ParticipantLimit => "Количество гостей",
            CallbackButton.ParticipantLimitDone => CallbackButton.ParticipantLimit.Text() + DoneSymbol,
            CallbackButton.Description => "Описание (не обязательно)",
            CallbackButton.DescriptionDone => CallbackButton.Description.Text() + DoneSymbol,
            CallbackButton.FinishCreating => "Завершить и создать",
            CallbackButton.Yes => "Да",
            CallbackButton.No => "Нет",
            CallbackButton.Registration => "Зарегистрироваться",
            CallbackButton.Cancel => "Отменить",
            CallbackButton.Edit => "Изменить",
            CallbackButton.ParticipantList => "Список участников",
            CallbackButton.CancelRegistration => "Отменить регистрацию",
            CallbackButton.ToDescription => "К описанию",
            _ => throw new ArgumentException("Incorrect CallBackButton enum")
        };
    }
}