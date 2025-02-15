using Com.Github.PatBatTB.GEBB.Domain;

namespace Com.Github.PatBatTB.GEBB.Services.Handlers;

public interface IHandler
{
    void Process(UpdateContainer container);
}