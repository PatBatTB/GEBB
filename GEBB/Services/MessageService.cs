using System.Globalization;
using Com.Github.PatBatTB.GEBB.DataBase.Event;
using Com.Github.PatBatTB.GEBB.DataBase.User;

namespace Com.Github.PatBatTB.GEBB.Services;

public static class MessageService
{
    public static string GetUserInvitesToEventString(AppUser appUser, AppEvent appEvent)
    {
        string participantLimitString = GetParticipantLimitString(appEvent.ParticipantLimit);
        string costString = GetCostString(appEvent.Cost);
        return $"@{appUser.Username} приглашает на мероприятие!\n" +
                  $"Название: {appEvent.Title}\n" +
                  $"Дата: {appEvent.DateTimeOf!.Value.ToString("ddd dd MMMM yyyy", new CultureInfo("ru-RU"))}\n" +
                  $"Время: {appEvent.DateTimeOf!.Value:HH:mm}\n" +
                  $"Место: {appEvent.Address}\n" +
                  $"Максимум человек: {participantLimitString}\n" +
                  $"Планируемые затраты: {costString}\n" +
                  (string.IsNullOrEmpty(appEvent.Description)
                      ? ""
                      : $"Дополнительная информация: {appEvent.Description}");
    }

    public static string GetMyEventDescription(AppEvent appEvent)
    {
        string participantLimitString = GetParticipantLimitString(appEvent.ParticipantLimit);
        string costString = GetCostString(appEvent.Cost);
        return $"Название: {appEvent.Title}\n" +
                  $"Дата: {appEvent.DateTimeOf!.Value.ToString("ddd dd MMMM yyyy", new CultureInfo("ru-RU"))}\n" +
                  $"Время: {appEvent.DateTimeOf!.Value:HH:mm}\n" +
                  $"Место: {appEvent.Address}\n" +
                  $"Максимум человек: {participantLimitString}\n" +
                  $"Зарегистрировалось: {appEvent.RegisteredUsers.Count}\n" +
                  $"Планируемые затраты: {costString}\n" +
                  (string.IsNullOrEmpty(appEvent.Description)
                      ? ""
                      : $"Дополнительная информация: {appEvent.Description}");
    }

    public static string GetEventDescription(AppEvent appEvent)
    {
        string participantLimitString = GetParticipantLimitString(appEvent.ParticipantLimit);
        string costString = GetCostString(appEvent.Cost);
        return $"Название: {appEvent.Title}\n" +
                  $"Организатор: @{appEvent.Creator.Username}\n" +
                  $"Дата: {appEvent.DateTimeOf!.Value.ToString("ddd dd MMMM yyyy", new CultureInfo("ru-RU"))}\n" +
                  $"Время: {appEvent.DateTimeOf!.Value:HH:mm}\n" +
                  $"Место: {appEvent.Address}\n" +
                  $"Максимум человек: {participantLimitString}\n" +
                  $"Зарегистрировалось: {appEvent.RegisteredUsers.Count}\n" +
                  $"Планируемые затраты: {costString}\n" +
                  (string.IsNullOrEmpty(appEvent.Description)
                      ? ""
                      : $"Дополнительная информация: {appEvent.Description}");
    }

    private static string GetParticipantLimitString(int? participantLimit)
    {
        string? limitString = (participantLimit is null or 0)
            ? "Без ограничений"
            : participantLimit.ToString();
        return limitString ?? "Без ограничений";
    }

    private static string GetCostString(int? cost)
    {
        string? costString = (cost is null or 0) ? "Бесплатно" : cost.ToString();
        return costString ?? "Бесплатно";
    }
}