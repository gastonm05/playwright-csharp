using NUnit.Framework;
using Microsoft.Playwright;
using Common.Utils;
using Common.Config;
using UITests.Pages;

namespace UITests.Tests;

[TestFixture]
public class LoginTests
{
    private BrowserManager? _browserManager;
    private IPage? _page;
    private HomePage? _homePage;
    private LoginPage? _loginPage;
    private SecurePage? _securePage;

    // Test credentials
    private string? _validUsername;
    private string? _validPassword;
    private const string InvalidPassword = "WrongPassword";

    [SetUp]
    public async Task Setup()
    {
        _validUsername = TestConfiguration.GetValidUsername();
        _validPassword = TestConfiguration.GetValidPassword();

        _browserManager = new BrowserManager();
        _page = await _browserManager.GetPageAsync();
        
        _homePage = new HomePage(_page);
        _loginPage = new LoginPage(_page);
        _securePage = new SecurePage(_page);
    }

    [TearDown]
    public async Task Teardown()
    {
        if (_browserManager != null)
            await _browserManager.DisposeAsync();
    }

    [Test]
    public async Task Test_NavigateToHomePage()
    {
        // Arrange & Act
        await _homePage!.NavigateAsync();
        var isLoaded = await _homePage.IsLoadedAsync();
        var heading = await _homePage.GetHeadingAsync();
        
        // Console.WriteLine("\n✅ Browser Opened with Inspector...");
        // await _page!.PauseAsync(); // Inspector is Opened here

        // Assert
        Assert.That(isLoaded, Is.True, "Home page should be loaded");
        Assert.That(heading, Does.Contain("Welcome to the-internet"), "Home page heading should be displayed");
    }

    [Test]
    public async Task Test_NavigateToLoginPageFromHome()
    {
        // Arrange
        await _homePage!.NavigateAsync();
        await _homePage.IsLoadedAsync();

        // Act
        await _homePage.ClickFormAuthenticationLinkAsync();
        var isLoaded = await _loginPage!.IsLoadedAsync();
        var heading = await _loginPage.GetHeadingAsync();

        // Assert
        Assert.That(isLoaded, Is.True, "Login page should be loaded");
        Assert.That(heading, Does.Contain("Login Page"), "Login page heading should be displayed");
    }

    [Test]
    public async Task Test_LoginWithValidCredentials_HappyPath()
    {
        // Arrange
        await _loginPage!.NavigateAsync();
        await _loginPage.IsLoadedAsync();

        // Act
        await _loginPage.LoginAsync(_validUsername!, _validPassword!);
        
        // Wait for navigation to complete
        await _page!.WaitForLoadStateAsync(LoadState.NetworkIdle);
        var isSecurePageLoaded = await _securePage!.IsLoadedAsync();
        var isMessageVisible = await _securePage.IsSuccessMessageVisibleAsync();
        var successMessage = await _securePage.GetSuccessMessageAsync();

        // Assert
        Assert.That(isSecurePageLoaded, Is.True, "Secure page should be loaded after successful login");
        Assert.That(isMessageVisible, Is.True, "Success message should be visible");
        Assert.That(successMessage, Does.Contain("You logged into a secure area"), "Success message should confirm login");
    }

    [Test]
    public async Task Test_LoginWithInvalidPassword_ShowsError()
    {
        // Arrange
        await _loginPage!.NavigateAsync();
        await _loginPage.IsLoadedAsync();

        // Act
        await _loginPage.LoginAsync(_validUsername!, InvalidPassword);
        
        // Wait a moment for error message to appear
        await _page!.WaitForLoadStateAsync(LoadState.NetworkIdle);
        var errorMessage = await _loginPage.GetErrorMessageAsync();

        // Assert
        Assert.That(errorMessage, Is.Not.Empty, "Error message should be displayed");
        Assert.That(errorMessage, Does.Contain("Your password is invalid!"), "Error message should indicate invalid credentials");
    }

    [Test]
    public async Task Test_LogoutRedirectsToLoginPage()
    {
        // Arrange - Login first with valid credentials
        await _loginPage!.NavigateAsync();
        await _loginPage.IsLoadedAsync();
        await _loginPage.LoginAsync(_validUsername!, _validPassword!);
        await _securePage!.IsLoadedAsync();

        // Act
        await _securePage.ClickLogoutButtonAsync();
        await _page!.WaitForLoadStateAsync(LoadState.NetworkIdle);
        var loginPageHeading = await _loginPage.GetHeadingAsync();

        // Assert
        Assert.That(loginPageHeading, Does.Contain("Login Page"), "Should redirect to login page after logout");
    }

    [Test]
    public async Task Test_LogoutShowsSuccessMessage()
    {
        // Arrange - Login first with valid credentials
        await _loginPage!.NavigateAsync();
        await _loginPage.IsLoadedAsync();
        await _loginPage.LoginAsync(_validUsername!, _validPassword!);
        await _securePage!.IsLoadedAsync();

        // Act
        await _securePage.ClickLogoutButtonAsync();
        await _page!.WaitForLoadStateAsync(LoadState.NetworkIdle);
        var logoutMessage = await _loginPage!.GetErrorMessageAsync();

        // Assert
        Assert.That(logoutMessage, Does.Contain("You logged out of the secure area"), "Logout message should confirm successful logout");
    }
}
