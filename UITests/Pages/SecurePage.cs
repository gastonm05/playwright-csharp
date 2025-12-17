using Microsoft.Playwright;

namespace UITests.Pages;

public class SecurePage
{
    private readonly IPage _page;

    // Selectors
    private readonly string _successMessage = "#flash";
    private readonly string _logoutButton = "a[href='/logout']";
    private readonly string _pageHeading = "h2";

    public SecurePage(IPage page)
    {
        _page = page;
    }

    public async Task<string> GetSuccessMessageAsync()
    {
        try
        {
            // Wait for the message to be visible
            await _page.WaitForSelectorAsync(_successMessage, new PageWaitForSelectorOptions { Timeout = 5000 });
            
            var messageElement = _page.Locator(_successMessage);
            var text = await messageElement.TextContentAsync() ?? string.Empty;
            // Clean up the message by removing the close button symbol and extra whitespace
            return text.Replace("×", "").Trim();
        }
        catch
        {
            return string.Empty;
        }
    }

    public async Task ClickLogoutButtonAsync()
    {
        await _page.ClickAsync(_logoutButton);
    }

    public async Task<bool> IsLoadedAsync()
    {
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        return await _page.IsVisibleAsync(_pageHeading);
    }

    public async Task<string> GetHeadingAsync()
    {
        return await _page.TextContentAsync(_pageHeading) ?? string.Empty;
    }

    public async Task<bool> IsSuccessMessageVisibleAsync()
    {
        try
        {
            await _page.WaitForSelectorAsync(_successMessage, new PageWaitForSelectorOptions { Timeout = 5000 });
            return await _page.Locator(_successMessage).IsVisibleAsync();
        }
        catch
        {
            return false;
        }
    }
}
