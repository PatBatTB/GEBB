using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace Com.GitHub.PatBatTB.GEBB.Domain;

using Github.PatBatTB.GEBB.Services;

public static class AppSettings
{
    public static string DbConnString { get; private set; }

    static AppSettings()
    {
        var DBname = "tgbot";
        AppSettingReader reader = new();
        if (reader.GetConnectionString(DBname) is not { } connectionString)
        {
            throw new JsonException($"file appsettings.json must be contains ConnectionString: {DBname}");
        };
        DbConnString = connectionString;
    }
}