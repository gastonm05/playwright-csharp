# Testing Guide

Complete guide for running and creating tests in the Playwright C# framework.

## 📖 Table of Contents

- [Running Tests](#running-tests)
- [API Tests](#api-tests)
- [UI Tests](#ui-tests)
- [Creating New Tests](#creating-new-tests)
- [Test Patterns](#test-patterns)
- [Debugging Tests](#debugging-tests)

---

## 🧪 Running Tests

### Basic Commands

```bash
# Run all tests
dotnet test

# Run specific project
dotnet test APITests/APITests.csproj
dotnet test UITests/UITests.csproj

# Run specific test class
dotnet test --filter "LoginTests"

# Run specific test method
dotnet test --filter "Test_LoginWithValidCredentials_HappyPath"

# Run with verbosity
dotnet test --verbosity detailed
dotnet test --verbosity quiet

# Run multiple test classes
dotnet test --filter "LoginTests|HomePage"
```

### Advanced Options

```bash
# Run with code coverage
dotnet test /p:CollectCoverage=true /p:CoverageFormat=opencover

# Run in parallel
dotnet test --parallel

# Run with specific configuration
dotnet test --configuration Release

# Run and save results
dotnet test --logger "trx;LogFileName=test_results.trx"
dotnet test --logger "console;verbosity=detailed"

# Run with custom environment
set ASPNETCORE_ENVIRONMENT=Production
dotnet test

# Stop on first failure
dotnet test --no-restore --configuration Release -- RunConfiguration.StopOnFirstFailure=true
```

---

## 🌐 API Tests

### Overview

API tests use `RestSharp` to test HTTP endpoints. Current tests target **JSONPlaceholder** (https://jsonplaceholder.typicode.com/).

### Test Classes

#### JSONPlaceholderUserTests
Tests user endpoints (`/users`)

```csharp
[TestFixture]
public class JSONPlaceholderUserTests
{
    private ApiClient _apiClient;

    [SetUp]
    public void Setup()
    {
        _apiClient = new ApiClient();
    }

    [Test]
    public async Task GetAllUsers_ReturnsUserList()
    {
        // Arrange & Act
        var response = await _apiClient.GetAsync<List<User>>("/users");

        // Assert
        Assert.That(response, Is.Not.Empty);
        Assert.That(response.First().Id, Is.GreaterThan(0));
    }

    [Test]
    public async Task GetUserById_ReturnsCorrectUser()
    {
        // Arrange & Act
        var response = await _apiClient.GetAsync<User>("/users/1");

        // Assert
        Assert.That(response.Id, Is.EqualTo(1));
        Assert.That(response.Name, Is.Not.Null);
    }
}
```

#### JSONPlaceholderPostTests
Tests post endpoints (`/posts`)

```csharp
[TestFixture]
public class JSONPlaceholderPostTests
{
    private ApiClient _apiClient;

    [SetUp]
    public void Setup()
    {
        _apiClient = new ApiClient();
    }

    [Test]
    public async Task GetAllPosts_ReturnsList()
    {
        var response = await _apiClient.GetAsync<List<Post>>("/posts");
        Assert.That(response, Is.Not.Empty);
    }

    [Test]
    public async Task GetPostById_ReturnsCorrectPost()
    {
        var response = await _apiClient.GetAsync<Post>("/posts/1");
        Assert.That(response.Id, Is.EqualTo(1));
    }

    [Test]
    public async Task GetPostsByUserId_ReturnsUserPosts()
    {
        var response = await _apiClient.GetAsync<List<Post>>("/posts?userId=1");
        Assert.That(response, Is.Not.Empty);
        Assert.That(response.All(p => p.UserId == 1), Is.True);
    }
}
```

#### JSONPlaceholderAlbumTests
Tests album endpoints (`/albums`)

```csharp
[TestFixture]
public class JSONPlaceholderAlbumTests
{
    private ApiClient _apiClient;

    [SetUp]
    public void Setup()
    {
        _apiClient = new ApiClient();
    }

    [Test]
    public async Task GetAlbumById_ReturnsCorrectAlbum()
    {
        var response = await _apiClient.GetAsync<Album>("/albums/1");
        Assert.That(response.Id, Is.EqualTo(1));
    }
}
```

#### JSONPlaceholderCommentTests
Tests comment endpoints (`/comments`)

```csharp
[TestFixture]
public class JSONPlaceholderCommentTests
{
    private ApiClient _apiClient;

    [SetUp]
    public void Setup()
    {
        _apiClient = new ApiClient();
    }

    [Test]
    public async Task GetCommentsByPostId_ReturnsPostComments()
    {
        var response = await _apiClient.GetAsync<List<Comment>>("/comments?postId=1");
        Assert.That(response, Is.Not.Empty);
    }
}
```

### Running API Tests

```bash
# Run all API tests
dotnet test APITests/APITests.csproj

# Run specific API test class
dotnet test --filter "JSONPlaceholderUserTests"

# Run specific API test method
dotnet test --filter "GetAllUsers_ReturnsUserList"

# Run with output
dotnet test APITests/APITests.csproj --verbosity detailed
```

---

## 🖥️ UI Tests

### Overview

UI tests use **Playwright** with the **Page Object Model (POM)** pattern. Tests target https://the-internet.herokuapp.com/.

### Test Pages (Page Object Models)

#### HomePage
Represents the home page with available examples.

**File:** `UITests/Pages/HomePage.cs`

```csharp
public class HomePage
{
    private readonly IPage _page;

    public HomePage(IPage page) => _page = page;

    public async Task NavigateAsync() => 
        await _page.GotoAsync("https://the-internet.herokuapp.com/");

    public async Task<string> GetHeadingAsync() => 
        await _page.TextContentAsync("h1.heading") ?? string.Empty;

    public async Task ClickFormAuthenticationLinkAsync() => 
        await _page.ClickAsync("a[href='/login']");

    public async Task<bool> IsLoadedAsync()
    {
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        return await _page.IsVisibleAsync("h1.heading");
    }
}
```

#### LoginPage
Represents the login page with username/password fields.

**File:** `UITests/Pages/LoginPage.cs`

```csharp
public class LoginPage
{
    private readonly IPage _page;

    public LoginPage(IPage page) => _page = page;

    public async Task NavigateAsync() => 
        await _page.GotoAsync("https://the-internet.herokuapp.com/login");

    public async Task EnterUsernameAsync(string username) => 
        await _page.FillAsync("#username", username);

    public async Task EnterPasswordAsync(string password) => 
        await _page.FillAsync("#password", password);

    public async Task ClickLoginButtonAsync() => 
        await _page.ClickAsync("button[type='submit']");

    public async Task LoginAsync(string username, string password)
    {
        await EnterUsernameAsync(username);
        await EnterPasswordAsync(password);
        await ClickLoginButtonAsync();
    }

    public async Task<string> GetErrorMessageAsync()
    {
        var errorElement = _page.Locator("#flash");
        if (await errorElement.IsVisibleAsync())
        {
            return await errorElement.TextContentAsync() ?? string.Empty;
        }
        return string.Empty;
    }
}
```

#### SecurePage
Represents the secure area shown after successful login.

**File:** `UITests/Pages/SecurePage.cs`

```csharp
public class SecurePage
{
    private readonly IPage _page;

    public SecurePage(IPage page) => _page = page;

    public async Task<bool> IsSuccessMessageVisibleAsync()
    {
        try
        {
            await _page.WaitForSelectorAsync("#flash", 
                new PageWaitForSelectorOptions { Timeout = 5000 });
            return await _page.Locator("#flash").IsVisibleAsync();
        }
        catch { return false; }
    }

    public async Task<string> GetSuccessMessageAsync()
    {
        await _page.WaitForSelectorAsync("#flash", 
            new PageWaitForSelectorOptions { Timeout = 5000 });
        var text = await _page.Locator("#flash").TextContentAsync() ?? string.Empty;
        return text.Replace("×", "").Trim();
    }

    public async Task ClickLogoutButtonAsync() => 
        await _page.ClickAsync("a[href='/logout']");
}
```

### Test Class

#### LoginTests
Comprehensive login workflow tests.

**File:** `UITests/Tests/LoginTests.cs`

```csharp
[TestFixture]
public class LoginTests
{
    private BrowserManager _browserManager;
    private IPage _page;
    private HomePage _homePage;
    private LoginPage _loginPage;
    private SecurePage _securePage;

    private string _validUsername;
    private string _validPassword;

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
        await _homePage.NavigateAsync();
        var isLoaded = await _homePage.IsLoadedAsync();
        var heading = await _homePage.GetHeadingAsync();

        // Assert
        Assert.That(isLoaded, Is.True);
        Assert.That(heading, Does.Contain("Welcome to the-internet"));
    }

    [Test]
    public async Task Test_LoginWithValidCredentials_HappyPath()
    {
        // Arrange
        await _loginPage.NavigateAsync();
        await _loginPage.IsLoadedAsync();

        // Act
        await _loginPage.LoginAsync(_validUsername, _validPassword);
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Assert
        Assert.That(await _securePage.IsSuccessMessageVisibleAsync(), Is.True);
        Assert.That(await _securePage.GetSuccessMessageAsync(), 
            Does.Contain("You logged into a secure area"));
    }

    [Test]
    public async Task Test_LoginWithInvalidPassword_ShowsError()
    {
        // Arrange
        await _loginPage.NavigateAsync();

        // Act
        await _loginPage.LoginAsync(_validUsername, "WrongPassword");
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        var errorMessage = await _loginPage.GetErrorMessageAsync();

        // Assert
        Assert.That(errorMessage, Is.Not.Empty);
        Assert.That(errorMessage, Does.Contain("Your password is invalid!"));
    }

    [Test]
    public async Task Test_LogoutRedirectsToLoginPage()
    {
        // Arrange
        await _loginPage.NavigateAsync();
        await _loginPage.LoginAsync(_validUsername, _validPassword);
        await _securePage.IsLoadedAsync();

        // Act
        await _securePage.ClickLogoutButtonAsync();
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        var loginPageHeading = await _loginPage.GetHeadingAsync();

        // Assert
        Assert.That(loginPageHeading, Does.Contain("Login Page"));
    }
}
```

### Running UI Tests

```bash
# Run all UI tests
dotnet test UITests/UITests.csproj

# Run LoginTests only
dotnet test --filter "LoginTests"

# Run specific test
dotnet test --filter "Test_LoginWithValidCredentials_HappyPath"

# Run in headed mode (browser visible)
# Set "Headless": false in appsettings.Development.json
dotnet test

# Run with custom browser
set "AppSettings:Browser=firefox"
dotnet test
```

---

## ➕ Creating New Tests

### Create a New Test Class

```csharp
using NUnit.Framework;
using Microsoft.Playwright;
using Common.Utils;

namespace UITests.Tests;

[TestFixture]
public class MyNewTests
{
    private BrowserManager _browserManager;
    private IPage _page;

    [SetUp]
    public async Task Setup()
    {
        _browserManager = new BrowserManager();
        _page = await _browserManager.GetPageAsync();
    }

    [TearDown]
    public async Task Teardown()
    {
        if (_browserManager != null)
            await _browserManager.DisposeAsync();
    }

    [Test]
    public async Task Test_DescribeWhatYouTest_ExpectedResult()
    {
        // Arrange
        var page = new YourPage(_page);
        await page.NavigateAsync();

        // Act
        await page.PerformAction();

        // Assert
        Assert.That(await page.IsActionCompleted(), Is.True);
    }
}
```

### Create a New Page Object

```csharp
using Microsoft.Playwright;

namespace UITests.Pages;

public class YourPage
{
    private readonly IPage _page;
    private readonly string _pageUrl = "https://example.com/your-page";

    // Define selectors
    private readonly string _submitButton = "button#submit";
    private readonly string _errorMessage = ".error-message";

    public YourPage(IPage page) => _page = page;

    public async Task NavigateAsync() => 
        await _page.GotoAsync(_pageUrl);

    public async Task ClickSubmitAsync() => 
        await _page.ClickAsync(_submitButton);

    public async Task<string> GetErrorMessageAsync() => 
        await _page.TextContentAsync(_errorMessage) ?? string.Empty;

    public async Task<bool> IsLoadedAsync()
    {
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        return await _page.IsVisibleAsync(_submitButton);
    }
}
```

---

## 🎯 Test Patterns

### Arrange-Act-Assert Pattern

```csharp
[Test]
public async Task Test_Feature_ExpectedResult()
{
    // ARRANGE - Set up test data and state
    var loginPage = new LoginPage(_page);
    await loginPage.NavigateAsync();

    // ACT - Perform the action being tested
    await loginPage.LoginAsync("user", "password");

    // ASSERT - Verify the results
    Assert.That(await securePage.IsSuccessMessageVisibleAsync(), Is.True);
}
```

### Page Object Model Pattern

```csharp
// Define interactions in Page class
public class LoginPage
{
    public async Task LoginAsync(string username, string password)
    {
        await _page.FillAsync("#username", username);
        await _page.FillAsync("#password", password);
        await _page.ClickAsync("button[type='submit']");
    }
}

// Use in tests
[Test]
public async Task Test_Login()
{
    var page = new LoginPage(_page);
    await page.LoginAsync("user", "password");
    // assertions...
}
```

### Reusable Setup/Teardown

```csharp
[TestFixture]
public class MyTests
{
    protected BrowserManager BrowserManager { get; set; }
    protected IPage Page { get; set; }

    [SetUp]
    public virtual async Task Setup()
    {
        BrowserManager = new BrowserManager();
        Page = await BrowserManager.GetPageAsync();
    }

    [TearDown]
    public virtual async Task Teardown()
    {
        if (BrowserManager != null)
            await BrowserManager.DisposeAsync();
    }
}

[TestFixture]
public class SpecificTests : MyTests
{
    [Test]
    public async Task Test_Something()
    {
        await Page.GotoAsync("https://example.com");
        // test code...
    }
}
```

---

## 🐛 Debugging Tests

### Use Playwright Inspector for Locator Selection

The **Playwright Inspector** allows you to interactively inspect and select elements while tests are running. This is useful for:
- Finding element locators visually
- Debugging element visibility issues
- Recording test interactions

#### How to Enable Playwright Inspector

Add `await _page!.PauseAsync();` to any test where you want to inspect elements:

```csharp
[Test]
public async Task Test_NavigateToHomePage()
{
    // Arrange & Act
    await _homePage!.NavigateAsync();
    var isLoaded = await _homePage.IsLoadedAsync();
    var heading = await _homePage.GetHeadingAsync();
    
    // Open Playwright Inspector - test will pause here
    Console.WriteLine("\n✅ Browser Opened with Inspector...");
    await _page!.PauseAsync(); // Inspector is opened here
    
    // After resuming from the Inspector, the test continues...
    // Assert
    Assert.That(isLoaded, Is.True, "Home page should be loaded");
    Assert.That(heading, Does.Contain("Welcome to the-internet"), "Home page heading should be displayed");
}
```

#### Running Tests with Inspector

Run your test normally - the browser will launch and pause at the `PauseAsync()` call:

```bash
# Run the test with the inspector
dotnet test --filter "Test_NavigateToHomePage"

# Or from VS Code Test Explorer
# Right-click the test → Run Test
```

#### Using the Inspector

When the test pauses:
1. **The Playwright Inspector window opens** with the page displayed
2. **Click the "Pick Locator" button** (or use the shortcut) to select elements
3. **Click elements on the page** to get their locator strings
4. **The locator is copied to your clipboard** - paste it in your Page Object Model
5. **Click "Resume" button** to continue the test

#### Example: Finding a Login Button Locator

```csharp
[Test]
public async Task Test_LoginWithInspector()
{
    // Arrange
    await _loginPage!.NavigateAsync();
    await _loginPage.IsLoadedAsync();

    // PAUSE HERE - Use Inspector to find the login button selector
    await _page!.PauseAsync();

    // Act - Once you've found the selector, click it
    await _page.ClickAsync("button[type='submit']");  // Locator from Inspector
    
    // Assert
    var errorMessage = await _loginPage.GetErrorMessageAsync();
    Assert.That(errorMessage, Is.Not.Empty);
}
```

#### Best Practices

- Use `PauseAsync()` **temporarily** during development to find locators
- **Remove or comment out** `PauseAsync()` before committing to version control
- Inspect **headless mode disabled** for better visibility: set `"Headless": false` in `appsettings.Development.json`
- Use the Inspector **in Development environment** for interactive debugging

---

### Enable Detailed Logging

```bash
# Run with detailed output
dotnet test --verbosity detailed

# Run with verbose logging
set SERILOG_LEVEL=Verbose
dotnet test
```

### Debug in Headed Mode

```bash
# Set in appsettings.Development.json
{
  "AppSettings": {
    "Headless": false,
    "Timeout": 60000
  }
}
```

### Use Playwright Inspector

```bash
# Set environment variable before running tests
set PWDEBUG=1
dotnet test UITests/UITests.csproj
```

### Add Console Output to Tests

```csharp
[Test]
public async Task Test_WithDebugOutput()
{
    Console.WriteLine("Test started at: " + DateTime.Now);
    
    await _page.GotoAsync("https://example.com");
    
    Console.WriteLine("Page loaded successfully");
    Console.WriteLine("Current URL: " + _page.Url);
}
```

### Common Debugging Commands

```bash
# List all available tests
dotnet test --list-tests

# Run and show stack traces
dotnet test --verbosity detailed

# Stop on first failure
dotnet test -- RunConfiguration.StopOnFirstFailure=true

# Run with trace
set TRACE=1
dotnet playwright install --with-deps
dotnet test
```

---

## 📊 Test Results

### View Test Results

```bash
# Console output (default)
dotnet test

# Generate TRX report
dotnet test --logger "trx;LogFileName=test_results.trx"

# Generate JSON report
dotnet test --logger "json;LogFileName=test_results.json"

# Generate HTML report
dotnet test --logger "console;verbosity=detailed" > test_output.html
```

---

**Last Updated:** December 2025
