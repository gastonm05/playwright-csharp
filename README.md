# Playwright C# Test Automation Framework

A comprehensive test automation framework built with **Playwright**, **C#**, **NUnit**, and **RestSharp** for both **API** and **UI** testing with **GitHub Actions CI/CD** integration.

## 📋 Table of Contents

- [Overview](#overview)
- [Project Structure](#project-structure)
- [Prerequisites](#prerequisites)
- [Quick Start](#quick-start)
- [Running Tests](#running-tests)
- [Configuration](#configuration)
- [CI/CD](#cicd)
- [Best Practices](#best-practices)
- [Troubleshooting](#troubleshooting)

## 🎯 Overview

This framework demonstrates enterprise-level test automation with:

✅ **Page Object Model (POM)** pattern for maintainable UI tests  
✅ **Organized structure** with shared utilities and configuration  
✅ **Environment-specific configs** (Development, Production, etc.)  
✅ **Externalized test data** for easy management  
✅ **GitHub Actions CI/CD** for automated test execution  
✅ **Comprehensive logging** with Serilog  
✅ **Support for multiple browsers** (Chromium, Firefox, WebKit)  

## 📁 Project Structure

```
playwright-csharp/
├── APITests/                         # API test project
│   ├── Models/                       # Data models (User, Post, Album, Comment)
│   ├── Tests/                        # Test classes
│   │   ├── JSONPlaceholderUserTests.cs
│   │   ├── JSONPlaceholderPostTests.cs
│   │   ├── JSONPlaceholderAlbumTests.cs
│   │   ├── JSONPlaceholderCommentTests.cs
│   │   └── QuickAPITest.cs
│   └── APITests.csproj
├── UITests/                          # UI test project
│   ├── Pages/                        # Page Object Models
│   │   ├── HomePage.cs              # Home page interactions
│   │   ├── LoginPage.cs             # Login form interactions
│   │   └── SecurePage.cs            # Secure area interactions
│   ├── Tests/                        # Test classes
│   │   ├── LoginTests.cs            # Login workflow tests
│   │   └── SampleUITests.cs
│   └── UITests.csproj
├── Common/                           # Shared utilities
│   ├── Config/
│   │   └── TestConfiguration.cs     # Centralized configuration
│   ├── Utils/
│   │   ├── BrowserManager.cs        # Browser lifecycle management
│   │   ├── ApiClient.cs             # HTTP client for API tests
│   │   └── LoggerSetup.cs           # Logging configuration
│   └── Common.csproj
├── .github/workflows/
│   └── ci.yml                        # GitHub Actions CI/CD pipeline
├── appsettings.json                  # Base configuration
├── appsettings.Development.json      # Development environment config
├── appsettings.Production.json       # Production environment config
├── TESTING.md                        # Detailed testing guide
├── SETUP.md                          # Setup and installation guide
├── README.md                         # This file
└── PlaywrightTests.sln               # Visual Studio solution
```

## ✅ Prerequisites

- **.NET 8.0+** - [Download](https://dotnet.microsoft.com/download)
- **Git** - Version control system
- **PowerShell 7+** - For Playwright browser installation
- **Visual Studio Code** or **Visual Studio 2022** (optional but recommended)

## 🚀 Quick Start

### 1. Clone Repository
```bash
git clone <repository-url>
cd playwright-csharp
```

### 2. Restore Dependencies
```bash
dotnet restore
```

### 3. Install Playwright Browsers
```bash
dotnet playwright install
```

### 4. Build Solution
```bash
dotnet build
```

### 5. Run Tests
```bash
dotnet test
```

For more detailed setup instructions, see [SETUP.md](./SETUP.md).

## 🧪 Running Tests

### Run All Tests
```bash
dotnet test
```

### Run API Tests Only
```bash
dotnet test APITests/APITests.csproj
```

### Run UI Tests Only
```bash
dotnet test UITests/UITests.csproj
```

### Run Specific Test Class
```bash
# Login UI tests
dotnet test --filter "LoginTests"

# JSONPlaceholder API tests
dotnet test --filter "JSONPlaceholderUserTests"
```

### Run Specific Test Method
```bash
dotnet test --filter "Test_LoginWithValidCredentials_HappyPath"
```

### Run with Detailed Output
```bash
dotnet test --verbosity detailed
```

### Run with Code Coverage
```bash
dotnet test /p:CollectCoverage=true /p:CoverageFormat=opencover
```

### Run in Headed Mode (show browser)
Set in `appsettings.Development.json`:
```json
{
  "AppSettings": {
    "Headless": false
  }
}
```

## ⚙️ Configuration

### Configuration Files

Configuration is managed through JSON files with environment-specific overrides:

#### Base Configuration (`appsettings.json`)
```json
{
  "AppSettings": {
    "BaseUrl": "https://example.com",
    "ApiBaseUrl": "https://jsonplaceholder.typicode.com",
    "Browser": "chromium",
    "Headless": true,
    "Timeout": 30000
  },
  "TestData": {
    "ValidUsername": "tomsmith",
    "ValidPassword": "SuperSecretPassword!",
    "InternetHomeUrl": "https://the-internet.herokuapp.com/"
  }
}
```

#### Using Configuration in Code
```csharp
using Common.Config;

// Access configuration values
string username = TestConfiguration.GetValidUsername();          // "tomsmith"
string password = TestConfiguration.GetValidPassword();          // "SuperSecretPassword!"
string browser = TestConfiguration.GetBrowser();                 // "chromium"
bool headless = TestConfiguration.IsHeadless();                  // true
int timeout = TestConfiguration.GetTimeout();                    // 30000
string url = TestConfiguration.GetInternetHomeUrl();             // "https://the-internet.herokuapp.com/"
```

### Environment Variables

Override settings via environment variables:
```bash
set ASPNETCORE_ENVIRONMENT=Production
set "AppSettings:Browser=firefox"
set "AppSettings:Headless=false"

dotnet test
```

## 🔄 CI/CD

### GitHub Actions Workflow

Tests run automatically on:
- **Push** to `main` or `develop` branches
- **Pull requests** to `main` or `develop` branches

**Workflow file:** `.github/workflows/ci.yml`

**Pipeline Steps:**
1. Checkout code
2. Setup .NET 8.0
3. Restore dependencies
4. Build in Release mode
5. Install Playwright browsers
6. Run API tests
7. Run UI tests
8. Upload test results as artifacts

### Simulate CI Locally
```bash
# Build in Release mode
dotnet build --configuration Release

# Install browsers
dotnet playwright install

# Run all tests
dotnet test --configuration Release --logger "console;verbosity=detailed"
```

## 📝 Test Examples

### UI Test - Login Flow
```csharp
[Test]
public async Task Test_LoginWithValidCredentials_HappyPath()
{
    // Arrange
    await _loginPage.NavigateAsync();
    await _loginPage.IsLoadedAsync();

    // Act
    await _loginPage.LoginAsync("tomsmith", "SuperSecretPassword!");
    await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);

    // Assert
    var isMessageVisible = await _securePage.IsSuccessMessageVisibleAsync();
    var successMessage = await _securePage.GetSuccessMessageAsync();
    
    Assert.That(isMessageVisible, Is.True);
    Assert.That(successMessage, Does.Contain("You logged into a secure area"));
}
```

### API Test - Get User
```csharp
[Test]
public async Task GetUser_ReturnsValidUser()
{
    // Arrange
    var client = new ApiClient();

    // Act
    var user = await client.GetAsync<User>("/users/1");

    // Assert
    Assert.That(user, Is.Not.Null);
    Assert.That(user.Id, Is.EqualTo(1));
    Assert.That(user.Name, Is.Not.Null);
}
```

## 🛠️ Key Classes

### TestConfiguration
Centralized configuration management with environment-specific overrides.

**Location:** `Common/Config/TestConfiguration.cs`

**Key Methods:**
- `GetValidUsername()` - Get test username from config
- `GetValidPassword()` - Get test password from config
- `GetInternetHomeUrl()` - Get test application URL
- `GetBrowser()` - Get browser type (chromium, firefox, webkit)
- `IsHeadless()` - Get headless mode setting
- `GetTimeout()` - Get timeout in milliseconds

### BrowserManager
Manages Playwright browser and page lifecycle.

**Location:** `Common/Utils/BrowserManager.cs`

**Key Methods:**
- `GetPageAsync()` - Create and return a new browser page
- `DisposeAsync()` - Close browser and clean up resources

### ApiClient
HTTP client for API testing.

**Location:** `Common/Utils/ApiClient.cs`

**Key Methods:**
- `GetAsync<T>(url)` - Make GET request
- `PostAsync<T>(url, data)` - Make POST request
- `PutAsync<T>(url, data)` - Make PUT request
- `DeleteAsync(url)` - Make DELETE request

## ✨ Best Practices

1. **Page Object Model** - Encapsulate page interactions in POM classes
2. **Externalize Test Data** - Use configuration instead of hardcoding values
3. **Descriptive Test Names** - Use pattern `Test_Feature_ExpectedResult`
4. **Arrange-Act-Assert** - Keep clear test structure
5. **Explicit Waits** - Use Playwright's built-in wait mechanisms
6. **Logging** - Log meaningful information for debugging
7. **Resource Cleanup** - Always dispose resources in teardown
8. **No Test Dependencies** - Each test should be independent
9. **Environment Agnostic** - Use configuration for environment differences

## 🔧 Troubleshooting

### Playwright Browsers Not Installed
```bash
dotnet playwright install
```

### Configuration Not Loading
Ensure `appsettings.json` files are in the test output directory:
```bash
copy appsettings.json UITests\bin\Debug\net8.0\
copy appsettings.Development.json UITests\bin\Debug\net8.0\
```

### Tests Timing Out
Increase timeout in `appsettings.json`:
```json
{
  "AppSettings": {
    "Timeout": 60000
  }
}
```

### Port Already in Use
Update port numbers in `appsettings.Development.json`:
```json
{
  "AppSettings": {
    "BaseUrl": "https://localhost:7002"
  }
}
```

### Browser Fails to Launch
Ensure Playwright browsers are installed and system has required dependencies:
```bash
# Reinstall browsers
dotnet playwright install --with-deps
```

## 📚 Additional Resources

- [SETUP.md](./SETUP.md) - Detailed setup instructions
- [TESTING.md](./TESTING.md) - Comprehensive testing guide
- [Playwright Documentation](https://playwright.dev/dotnet/)
- [NUnit Documentation](https://nunit.org/documentation/)
- [RestSharp Documentation](https://restsharp.dev/)

## 📞 Support

For issues or questions:
1. Check the troubleshooting section above
2. Review [SETUP.md](./SETUP.md) and [TESTING.md](./TESTING.md)
3. Check test output and GitHub Actions logs
4. Review framework documentation

## 📄 License

[Your License Here]

## 👤 Authors

- **Gaston** - Project Maintainer

---

**Last Updated:** December 2025  
**Framework Version:** 1.0  
**.NET Version:** 8.0+
