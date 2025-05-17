using System.Text.Json;
using Com.Github.PatBatTB.GEBB.Services;
using log4net;
using Microsoft.Extensions.Configuration;

namespace Com.GitHub.PatBatTB.GEBB.Domain;

public static class AppSettings
{
    private static readonly ILog Log = LogManager.GetLogger(typeof(AppSettings));
    
    static AppSettings()
    {
        var DBname = "tgbot";
        AppSettingReader reader = new();
        if (reader.GetConnectionString(DBname) is not { } connectionString)
        {
            Log.Fatal($"file appsettings.json must be contains ConnectionString: {DBname}");
            throw new JsonException();
        }

        DbConnString = connectionString;
    }

    public static string DbConnString { get; private set; }
}