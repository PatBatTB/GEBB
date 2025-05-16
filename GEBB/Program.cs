using Com.Github.PatBatTB.GEBB.DataBase;
using log4net.Config;

[assembly: XmlConfigurator(ConfigFile = "log4net.config", Watch = true)]

namespace Com.GitHub.PatBatTB.GEBB;

public static class Program
{
    public static async Task Main(string[] args)
    {
        await using (TgBotDbContext db = new())
        {
            await db.Database.EnsureCreatedAsync();
        }

        App app = new();
        await app.Run();
    }
}