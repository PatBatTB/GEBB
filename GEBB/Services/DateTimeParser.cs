using System.Text.RegularExpressions;

namespace Com.Github.PatBatTB.GEBB.Services;

public static class DateTimeParser
{
    public static bool TryParse(string dateTimeString, out DateTime? parsedDateTime)
    {
        parsedDateTime = null;
        Regex regex =
            new Regex(
                @"^ *(\d{2}|\d{1})[\.,: _-]?(\d{2})[\.,-: _-]?(\d{4}|\d{2})[ \._,]+(\d{2}|\d{1})[\.,-: _-]?(\d{2})");
        MatchCollection matches = regex.Matches(dateTimeString);
        if (matches.Count != 1) return false;
        GroupCollection groupColl = matches[0].Groups;
        if (groupColl.Count != 6) return false;
        List<int> list;
        try
        {
            list = groupColl.Cast<Group>().Where(e => e is not Match).Select(e => int.Parse(e.Value))
                .ToList();
            parsedDateTime = new DateTime(
                day: list[0],
                month: list[1],
                year: list[2],
                hour: list[3],
                minute: list[4],
                second: 0);
        }
        catch (Exception)
        {
            return false;
        }
        return true;
    }
}