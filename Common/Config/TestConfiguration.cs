using Microsoft.Extensions.Configuration;
using Serilog;

namespace Common.Config;

public class TestConfiguration
{
    private static IConfiguration? _configuration;

    public static IConfiguration GetConfiguration()
    {
        if (_configuration != null)
            return _configuration;

        _configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development"}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        return _configuration;
    }

    public static string GetBaseUrl()
    {
        var config = GetConfiguration();
        return config["AppSettings:BaseUrl"] ?? "https://example.com";
    }

    public static string GetApiBaseUrl()
    {
        var config = GetConfiguration();
        return config["AppSettings:ApiBaseUrl"] ?? "https://api.example.com";
    }

    public static string GetBrowser()
    {
        var config = GetConfiguration();
        return config["AppSettings:Browser"] ?? "chromium";
    }

    public static bool IsHeadless()
    {
        var config = GetConfiguration();
        return bool.Parse(config["AppSettings:Headless"] ?? "true");
    }

    public static int GetTimeout()
    {
        var config = GetConfiguration();
        return int.Parse(config["AppSettings:Timeout"] ?? "30000");
    }
}
