namespace Com.Github.PatBatTB.GEBB.Domain.Enums;

public enum CallbackButton
{
    //TODO добавить технический код кнопок для callback, вместо использования имени энама.
    MyEvents,
    MyRegs,
    AvailEvents,
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
    PartLimit,
    PartLimitDone,
    Descr,
    DescrDone,
    FinishBuilding,
    Yes,
    No,
    Reg,
    Cancel,
    Edit,
    PartList,
    CancelReg,
    ToDescr,
    Settings,
    Alarm,
    ThreeDays,
    OneDay,
    Hours,
    Disable,
    One,
    Two,
    Three,
    Four,
    Five,
}

public static class CallbackButtonExtension
{
    private const string DoneSymbol = " \u2705";

    public static string Text(this CallbackButton callbackButton)
    {
        return callbackButton switch
        {
            CallbackButton.MyEvents => "Собственные мероприятия",
            CallbackButton.MyRegs => "Куда зарегистрирован?",
            CallbackButton.AvailEvents => "Открытые для регистрации",
            CallbackButton.Close => "Закрыть",
            CallbackButton.Create => "Создать новое мероприятие",
            CallbackButton.List => "Список мероприятий",
            CallbackButton.Back => "Назад",
            CallbackButton.Title => "Название мероприятия",
            CallbackButton.TitleDone => CallbackButton.Title.Text() + DoneSymbol,
            CallbackButton.DateTimeOf => "Дата и время",
            CallbackButton.DateTimeOfDone => CallbackButton.DateTimeOf.Text() + DoneSymbol,
            CallbackButton.Address => "Адрес проведения",
            CallbackButton.AddressDone => CallbackButton.Address.Text() + DoneSymbol,
            CallbackButton.Cost => "Планируемая стоимость",
            CallbackButton.CostDone => CallbackButton.Cost.Text() + DoneSymbol,
            CallbackButton.PartLimit => "Количество гостей",
            CallbackButton.PartLimitDone => CallbackButton.PartLimit.Text() + DoneSymbol,
            CallbackButton.Descr => "Описание (не обязательно)",
            CallbackButton.DescrDone => CallbackButton.Descr.Text() + DoneSymbol,
            CallbackButton.FinishBuilding => "Завершить и создать",
            CallbackButton.Yes => "Да",
            CallbackButton.No => "Нет",
            CallbackButton.Reg => "Зарегистрироваться",
            CallbackButton.Cancel => "Отменить",
            CallbackButton.Edit => "Изменить",
            CallbackButton.PartList => "Список участников",
            CallbackButton.CancelReg => "Отменить регистрацию",
            CallbackButton.ToDescr => "К описанию",
            CallbackButton.Settings => "Настройки",
            CallbackButton.Alarm => "Оповещения",
            CallbackButton.ThreeDays => "За 3 дня",
            CallbackButton.OneDay => "За 1 день",
            CallbackButton.Hours => "За несколько часов",
            CallbackButton.Disable => "Выключить",
            CallbackButton.One => "1",
            CallbackButton.Two => "2",
            CallbackButton.Three => "3",
            CallbackButton.Four => "4",
            CallbackButton.Five => "5",
            _ => throw new ArgumentException("Incorrect CallBackButton enum")
        };
    }
}