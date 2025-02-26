using System.Text.Json;
using Com.Github.PatBatTB.GEBB.Services;
using Microsoft.Extensions.Configuration;

namespace Com.GitHub.PatBatTB.GEBB.Domain;

public static class AppSettings
{
    static AppSettings()
    {
        var DBname = "tgbot";
        AppSettingReader reader = new();
        if (reader.GetConnectionString(DBname) is not { } connectionString)
        {
            throw new JsonException($"file appsettings.json must be contains ConnectionString: {DBname}");
        }

        DbConnString = connectionString;
    }

    public static string DbConnString { get; private set; }
}