# Playwright C# Test Automation Framework

A comprehensive test automation framework built with Playwright, C#, NUnit, and RestSharp for both API and UI testing with GitHub Actions CI/CD integration.

## Project Structure

```
playwright-csharp/
├── APITests/                 # API test project
│   ├── Tests/               # API test files
│   └── APITests.csproj
├── UITests/                 # UI test project
│   ├── Pages/               # Page Object Model classes
│   ├── Tests/               # UI test files
│   └── UITests.csproj
├── Common/                  # Shared utilities and configurations
│   ├── Config/              # Configuration management
│   │   └── TestConfiguration.cs
│   ├── Utils/               # Common utilities
│   │   ├── BrowserManager.cs
│   │   ├── ApiClient.cs
│   │   └── LoggerSetup.cs
│   └── Common.csproj
├── .github/workflows/       # GitHub Actions CI pipeline
│   └── ci.yml
├── appsettings.json         # Default configuration
├── appsettings.Development.json
├── appsettings.Production.json
└── PlaywrightTests.sln      # Solution file
```

## Prerequisites

- .NET 8.0 SDK or higher
- Visual Studio 2022 or Visual Studio Code
- Git

## Installation

1. Clone the repository:
```bash
git clone <repository-url>
cd playwright-csharp
```

2. Restore NuGet packages:
```bash
dotnet restore
```

3. Install Playwright browsers:
```bash
dotnet build
pwsh bin/Debug/net8.0/playwright.ps1 install
```

## Configuration

The framework uses `appsettings.json` for configuration. Modify the settings as needed:

- **BaseUrl**: The base URL for UI tests (default: https://example.com)
- **ApiBaseUrl**: The base URL for API tests (default: https://api.example.com)
- **Browser**: Browser type - chromium, firefox, or webkit (default: chromium)
- **Headless**: Run browser in headless mode (default: true)
- **Timeout**: Default timeout in milliseconds (default: 30000)

## Running Tests

### Run all tests:
```bash
dotnet test
```

### Run UI tests only:
```bash
dotnet test UITests/UITests.csproj
```

### Run API tests only:
```bash
dotnet test APITests/APITests.csproj
```

### Run with specific configuration:
```bash
dotnet test --configuration Release
```

### Run with detailed logging:
```bash
dotnet test --logger "console;verbosity=detailed"
```

## Project Features

### Common Features
- **TestConfiguration**: Centralized configuration management
- **BrowserManager**: Browser and page lifecycle management
- **ApiClient**: RESTful API client for API testing
- **LoggerSetup**: Serilog-based logging

### UI Testing
- Playwright browser automation
- Page Object Model pattern support
- Multi-browser support (Chromium, Firefox, WebKit)
- Headless and headed mode support

### API Testing
- RestSharp for HTTP requests
- Support for GET, POST, PUT, DELETE operations
- JSON body serialization
- Response status code assertions

## Dependencies

- **Microsoft.Playwright**: ^1.48.0 - Browser automation
- **RestSharp**: ^107.3.0 - HTTP client for API testing
- **NUnit**: ^4.1.0 - Unit testing framework
- **NUnit3TestAdapter**: ^4.6.1 - Test adapter for Visual Studio
- **Microsoft.NET.Test.Sdk**: ^17.11.1 - Test SDK
- **Serilog**: ^4.2.0 - Logging framework
- **Microsoft.Extensions.Configuration**: ^8.0.0 - Configuration management

## CI/CD Pipeline

The project includes a GitHub Actions workflow (`.github/workflows/ci.yml`) that:

1. Triggers on push and pull requests to main and develop branches
2. Sets up .NET 8.0
3. Restores dependencies
4. Builds the solution
5. Installs Playwright browsers
6. Runs API tests
7. Runs UI tests
8. Uploads test results as artifacts

### Triggering CI Pipeline

Push your changes to the repository:
```bash
git add .
git commit -m "Your commit message"
git push origin main
```

## Creating New Tests

### UI Test Example
```csharp
[TestFixture]
public class MyUITests
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
        await _browserManager.DisposeAsync();
    }

    [Test]
    public async Task MyTest()
    {
        await _page.GotoAsync("https://example.com");
        // Your test logic here
    }
}
```

### API Test Example
```csharp
[TestFixture]
public class MyAPITests
{
    private ApiClient _apiClient;

    [SetUp]
    public void Setup()
    {
        _apiClient = new ApiClient();
    }

    [Test]
    public async Task MyTest()
    {
        var response = await _apiClient.GetAsync("/endpoint");
        Assert.That(response.StatusCode, Is.EqualTo(200));
    }
}
```

## Best Practices

1. **Page Object Model**: Use the `Pages/` directory to create page objects for UI tests
2. **Configuration**: Store sensitive data in environment variables or secure configuration
3. **Logging**: Use the configured logger for debugging and monitoring
4. **Async/Await**: Always use async operations with Playwright and RestSharp
5. **Assertions**: Use NUnit assertions for consistent test validation
6. **Test Organization**: Group related tests in the same test class with [TestFixture]

## Troubleshooting

### Playwright browsers not found
```bash
pwsh bin/Debug/net8.0/playwright.ps1 install
```

### Configuration not loading
Ensure `appsettings.json` is in the root directory and the working directory is set correctly.

### Port already in use
Update the port numbers in `appsettings.Development.json` if ports are already in use.

## Contributing

1. Create a feature branch
2. Make your changes
3. Push to the repository
4. Create a pull request

## License

This project is licensed under the MIT License.

## Support

For issues and questions, please create an issue in the repository or contact the development team.
