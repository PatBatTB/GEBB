namespace Com.GitHub.PatBatTB.GEBB.Exceptions;

public class IncorrectDateException : Exception
{
    public IncorrectDateException()
    {
    }

    public IncorrectDateException(string message) : base(message)
    {
    }
}