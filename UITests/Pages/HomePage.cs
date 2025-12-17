using Microsoft.Playwright;

namespace UITests.Pages;

public class HomePage
{
    private readonly IPage _page;
    private readonly string _baseUrl = "https://the-internet.herokuapp.com/";

    // Selectors
    private readonly string _heading = "h1.heading";
    private readonly string _formAuthLink = "a[href='/login']";

    public HomePage(IPage page)
    {
        _page = page;
    }

    public async Task NavigateAsync()
    {
        await _page.GotoAsync(_baseUrl);
    }

    public async Task<string> GetHeadingAsync()
    {
        return await _page.TextContentAsync(_heading) ?? string.Empty;
    }

    public async Task ClickFormAuthenticationLinkAsync()
    {
        await _page.ClickAsync(_formAuthLink);
    }

    public async Task<bool> IsLoadedAsync()
    {
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        return await _page.IsVisibleAsync(_heading);
    }
}
