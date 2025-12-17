using Microsoft.Playwright;
using Common.Config;
using Serilog;

namespace Common.Utils;

public class BrowserManager : IAsyncDisposable
{
    private IBrowser? _browser;
    private IBrowserContext? _context;
    private IPage? _page;

    public async Task<IPage> GetPageAsync()
    {
        if (_page != null)
            return _page;

        LoggerSetup.ConfigureLogger();
        Log.Information("Initializing browser...");

        var playwright = await Playwright.CreateAsync();
        var browserType = TestConfiguration.GetBrowser().ToLower();

        _browser = browserType switch
        {
            "firefox" => await playwright.Firefox.LaunchAsync(new BrowserTypeLaunchOptions { Headless = TestConfiguration.IsHeadless() }),
            "chromium" => await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = TestConfiguration.IsHeadless() }),
            _ => await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = TestConfiguration.IsHeadless() })
        };

        _context = await _browser.NewContextAsync();
        _page = await _context.NewPageAsync();
        _page.SetDefaultTimeout(TestConfiguration.GetTimeout());

        Log.Information($"Browser initialized with {browserType}");
        return _page;
    }

    public IPage GetPage()
    {
        if (_page == null)
            throw new InvalidOperationException("Page not initialized. Call GetPageAsync first.");
        return _page;
    }

    public async ValueTask DisposeAsync()
    {
        if (_page != null)
            await _page.CloseAsync();

        if (_context != null)
            await _context.CloseAsync();

        if (_browser != null)
            await _browser.CloseAsync();

        Log.Information("Browser closed");
    }
}
