using Com.Github.PatBatTB.GEBB.Domain.Enums;
using Telegram.Bot.Types;

namespace Com.Github.PatBatTB.GEBB.Domain;

public class AlterCbData
{
    [Flags]
    public enum Param
    {
        Button = 0b_001,
        Menu = 0b_010,
        EventId = 0b_100,
    }

    private const string Separator = "_";

    private static readonly Dictionary<Param, Func<string, bool>> _constraints = new()
    {
    };

    private Param _par;

    public AlterCbData(string callbackString)
    {
        if (string.IsNullOrEmpty(callbackString)) throw new ArgumentException("Argument cannot be empty");
        _parameters = new();
        string[] arr = callbackString.Split(Separator);
        try
        {
            _par = (Param)Convert.ToInt32(arr[0], 2);
        }
        catch (FormatException e)
        {
            throw new ArgumentException("First argument must be binary number", e);
        }

        if (!ValidateCount(arr)) throw new ArgumentException("Incorrect number of arguments");
        for (int i = 1; i < arr.Length; i++)
        {
            string value = arr[i];
            AddParam(value);
        }

        DataString = callbackString;
    }

    public AlterCbData(CallbackQuery? callbackQuery) : this(callbackQuery?.Data ?? string.Empty)
    {
        Id = callbackQuery!.Id;
    }

    public AlterCbData(Dictionary<Param, string> parameters)
    {
        _parameters = new();
        foreach (Param flag in Enum.GetValues<Param>())
        {
            if (parameters.TryGetValue(flag, out string? value))
            {
                if (_constraints.TryGetValue(flag, out var func) && !func.Invoke(value))
                    throw new ArgumentException($"Invalid value for {flag} param");
                _parameters.Add(flag, value);
            }
        }

        DataString = InitializeDataString();
    }

    private Dictionary<Param, string> _parameters { get; }

    public CallbackButton Button
    {
        get
        {
            if (!_parameters.TryGetValue(Param.Button, out string? buttonString))
                throw new KeyNotFoundException();

            return Enum.Parse<CallbackButton>(buttonString);
        }
    }

    public CallbackMenu Menu
    {
        get
        {
            if (!_parameters.TryGetValue(Param.Menu, out string? menuSting))
                throw new KeyNotFoundException();
            return Enum.Parse<CallbackMenu>(menuSting);
        }
    }

    public string EventId
    {
        get
        {
            if (!_parameters.TryGetValue(Param.EventId, out string? idString))
                throw new KeyNotFoundException();
            return idString;
        }
    }

    public string? Id { get; }

    public string DataString { get; }

    private string InitializeDataString()
    {
        Param myPar = 0;
        List<string> valueList = new();
        foreach (var param in Enum.GetValues<Param>())
        {
            if (_parameters.TryGetValue(param, out string? value))
            {
                myPar += (int)param;
                valueList.Add(value);
            }
        }

        string binaryCode = Convert.ToString((int)myPar, 2).PadLeft(Enum.GetNames<Param>().Length, '0');

        return binaryCode + Separator + string.Join(Separator, valueList);
    }

    private bool ValidateCount(string[] arr)
    {
        int count = 1;
        foreach (char c in arr[0])
        {
            if (c == '1') count++;
        }

        return count == arr.Length;
    }

    private static bool ValidateLongType(string value)
    {
        return long.TryParse(value, out _);
    }

    private void AddParam(string value)
    {
        foreach (Param flag in Enum.GetValues<Param>())
        {
            if (_par.HasFlag(flag))
            {
                if (_constraints.TryGetValue(flag, out var func) && !func.Invoke(value))
                    throw new ArgumentException($"Invalid value for {flag} param");
                _parameters.Add(flag, value);
                _par &= ~flag;
                return;
            }
        }
    }
}