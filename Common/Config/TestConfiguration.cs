using System.Configuration;
using Microsoft.Extensions.Configuration;

namespace Common.Config;

public static class TestConfiguration
{
    private static IConfiguration? _configuration;

    public static IConfiguration GetConfiguration()
    {
        if (_configuration != null)
            return _configuration;

        // Get the root directory (where the solution is)
        var rootPath = Path.Combine(Directory.GetCurrentDirectory(), "..");
        if (!Directory.Exists(Path.Combine(rootPath, "appsettings.json")))
        {
            rootPath = Directory.GetCurrentDirectory();
        }

        _configuration = new ConfigurationBuilder()
            .SetBasePath(rootPath)
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
        // Try to get from config first, then default to JSONPlaceholder
        try
        {
            var config = GetConfiguration();
            var url = config["AppSettings:ApiBaseUrl"];
            if (!string.IsNullOrEmpty(url))
                return url;
        }
        catch
        {
            // If config loading fails, use default
        }
        
        return "https://jsonplaceholder.typicode.com";
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

    public static string GetValidUsername()
    {
        var config = GetConfiguration();
        return config["TestData:ValidUsername"] ?? "testuser";
    }

    public static string GetValidPassword()
    {
        var config = GetConfiguration();
        return config["TestData:ValidPassword"] ?? "testpassword";
    }

    public static string GetInternetHomeUrl()
    {
        var config = GetConfiguration();
        return config["TestData:InternetHomeUrl"] ?? "https://the-internet.herokuapp.com/";
    }
}