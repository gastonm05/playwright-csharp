using Microsoft.Playwright;

namespace UITests.Pages;

public class LoginPage
{
    private readonly IPage _page;
    private readonly string _baseUrl = "https://the-internet.herokuapp.com/login";

    // Selectors
    private readonly string _usernameInput = "#username";
    private readonly string _passwordInput = "#password";
    private readonly string _loginButton = "button[type='submit']";
    private readonly string _errorMessage = "#flash";
    private readonly string _pageHeading = "h2";

    public LoginPage(IPage page)
    {
        _page = page;
    }

    public async Task NavigateAsync()
    {
        await _page.GotoAsync(_baseUrl);
    }

    public async Task EnterUsernameAsync(string username)
    {
        await _page.FillAsync(_usernameInput, username);
    }

    public async Task EnterPasswordAsync(string password)
    {
        await _page.FillAsync(_passwordInput, password);
    }

    public async Task ClickLoginButtonAsync()
    {
        await _page.ClickAsync(_loginButton);
    }

    public async Task LoginAsync(string username, string password)
    {
        await EnterUsernameAsync(username);
        await EnterPasswordAsync(password);
        await ClickLoginButtonAsync();
    }

    public async Task<string> GetErrorMessageAsync()
    {
        var errorElement = _page.Locator(_errorMessage);
        if (await errorElement.IsVisibleAsync())
        {
            return await errorElement.TextContentAsync() ?? string.Empty;
        }
        return string.Empty;
    }

    public async Task<string> GetHeadingAsync()
    {
        return await _page.TextContentAsync(_pageHeading) ?? string.Empty;
    }

    public async Task<bool> IsLoadedAsync()
    {
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        return await _page.IsVisibleAsync(_pageHeading);
    }
}
