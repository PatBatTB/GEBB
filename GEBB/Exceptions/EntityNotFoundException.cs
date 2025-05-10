namespace Com.GitHub.PatBatTB.GEBB.Exceptions;

public class EntityNotFoundException : Exception
{
    public EntityNotFoundException() : this("Entity not found in DB.")
    {
         
    }

    public EntityNotFoundException(string message) : base(message)
    {
        
    }
}