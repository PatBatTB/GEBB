using Com.Github.PatBatTB.GEBB.Domain.Enums;
using Telegram.Bot.Types;

namespace Com.Github.PatBatTB.GEBB.Domain;

public class CallbackData
{
    private const string Separator = "_";
    private readonly CallbackButton? _button;
    private readonly string? _eventId;

    private readonly Dictionary<Prop, object?> _mappingDict = new();
    private readonly CallbackMenu? _menu;

    public CallbackData()
    {
    }

    public CallbackData(CallbackQuery callbackQuery)
    {
        if (string.IsNullOrEmpty(callbackQuery?.Data)) return;
        CallbackId = callbackQuery.Id;
        string[] propsArr = callbackQuery.Data.Split(Separator);
        Prop cbProp;
        try
        {
            cbProp = (Prop)Convert.ToInt32(propsArr[0], 2);
        }
        catch (FormatException e)
        {
            throw new ArgumentException("First argument must be binary number", e);
        }

        if (!ValidateCount(propsArr)) throw new ArgumentException("Incorrect number of arguments");
        for (int i = 1; i < propsArr.Length; i++)
        {
            foreach (Prop prop in Enum.GetValues<Prop>())
            {
                if (cbProp.HasFlag(prop))
                {
                    switch (prop)
                    {
                        case Prop.Menu:
                            Menu = Enum.Parse<CallbackMenu>(propsArr[i]);
                            cbProp -= (int)prop;
                            break;
                        case Prop.Button:
                            Button = Enum.Parse<CallbackButton>(propsArr[i]);
                            cbProp -= (int)prop;
                            break;
                        case Prop.EventId:
                            EventId = propsArr[i];
                            cbProp -= (int)prop;
                            break;
                        default:
                            throw new KeyNotFoundException("unknown prop");
                    }

                    break;
                }
            }
        }
    }

    public CallbackButton? Button
    {
        get => _button;
        init
        {
            _button = value;
            _mappingDict[Prop.Button] = _button;
        }
    }

    public CallbackMenu? Menu
    {
        get => _menu;
        init
        {
            _menu = value;
            _mappingDict[Prop.Menu] = _menu;
        }
    }

    public string? EventId
    {
        get => _eventId;
        init
        {
            _eventId = value;
            _mappingDict[Prop.EventId] = _eventId;
        }
    }

    public string? CallbackId { get; private set; }

    public string GetDataString()
    {
        Prop flagProp = 0;
        List<string> objList = new();
        foreach (Prop prop in Enum.GetValues<Prop>())
        {
            if (_mappingDict.TryGetValue(prop, out object? obj))
            {
                flagProp += (int)prop;
                objList.Add(obj?.ToString()!);
            }
        }

        string binaryCode = Convert.ToString((int)flagProp, 2).PadLeft(Enum.GetValues<Prop>().Length, '0');
        return binaryCode + Separator + string.Join(Separator, objList);
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

    [Flags]
    private enum Prop
    {
        Menu = 0b1,
        Button = 0b10,
        EventId = 0b100,
    }
}