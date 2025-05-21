namespace Com.GitHub.PatBatTB.GEBB.Exceptions;

public class EventNotValidException : Exception
{
    public EventNotValidException() : base()
    {
    }

    public EventNotValidException(string message) : base(message)
    {
    }
}