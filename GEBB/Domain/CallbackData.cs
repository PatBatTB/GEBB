namespace Com.Github.PatBatTB.GEBB.Domain;

public class CallbackData
{
    [Flags]
    public enum Param
    {
        UserId = 0b_0001,
        EventName = 0b_0010,
        MenuId = 0b_0100,
        ButtonName = 0b_1000,
    }

    private const string Separator = "_";

    private static readonly Dictionary<Param, Func<string, bool>> _constraints = new()
    {
        [Param.UserId] = ValidateLongType,
        [Param.MenuId] = ValidateLongType,
    };

    private Param _par;

    public CallbackData(string str)
    {
        Parameters = new();
        string[] arr = str.Split(Separator);
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

        DataString = str;
    }

    public CallbackData(Dictionary<Param, string> parameters)
    {
        Parameters = new();
        foreach (Param flag in Enum.GetValues<Param>())
        {
            if (parameters.TryGetValue(flag, out string? value))
            {
                if (_constraints.TryGetValue(flag, out var func) && !func.Invoke(value))
                    throw new ArgumentException($"Invalid value for {flag} param");
                Parameters.Add(flag, value);
            }
        }

        DataString = InitializeDataString();
    }

    public Dictionary<Param, string> Parameters { get; }
    public string DataString { get; }

    private string InitializeDataString()
    {
        Param myPar = 0;
        List<string> valueList = new();
        foreach (var param in Enum.GetValues<Param>())
        {
            if (Parameters.TryGetValue(param, out string? value))
            {
                myPar += (int)param;
                valueList.Add(value);
            }
        }

        string binaryCode = Convert.ToString((int)myPar, 2).PadLeft(4, '0');

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
                Parameters.Add(flag, value);
                _par &= ~flag;
                return;
            }
        }
    }
}