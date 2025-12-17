# Setup Guide

Complete installation and configuration guide for the Playwright C# testing framework.

## 📋 Table of Contents

- [Prerequisites](#prerequisites)
- [Installation Steps](#installation-steps)
- [Playwright Setup](#playwright-setup)
- [Configuration](#configuration)
- [Verify Installation](#verify-installation)
- [Troubleshooting](#troubleshooting)
- [Working with Multiple Browsers](#working-with-multiple-browsers)

---

## ✅ Prerequisites

### System Requirements

- **Operating System**: Windows 10+, macOS 11+, or Linux
- **Hardware**: 4GB RAM minimum, 2GB disk space
- **Internet Connection**: Required for dependencies and browser downloads

### Required Software

#### 1. .NET 8.0 SDK

**Install .NET 8.0:**

```bash
# Check if .NET 8.0 is installed
dotnet --version

# Visit: https://dotnet.microsoft.com/download/dotnet/8.0
# Download and run the installer for your OS
```

**Verify Installation:**
```bash
dotnet --version
# Should output: 8.x.x or higher

dotnet --list-sdks
# Should show 8.0.x in the list
```

#### 2. Git

**Install Git:**
```bash
# Windows: https://git-scm.com/download/win
# macOS: brew install git
# Linux: apt-get install git (Debian/Ubuntu)

# Verify
git --version
```

#### 3. PowerShell 7+ (Windows)

**Install PowerShell Core:**
```bash
# Windows: https://learn.microsoft.com/en-us/powershell/scripting/install/installing-powershell-on-windows

# Alternative: Using WinGet
winget install Microsoft.PowerShell

# Verify
pwsh --version
```

#### 4. Visual Studio Code (Optional but Recommended)

```bash
# Download: https://code.visualstudio.com/
# Or: winget install Microsoft.VisualStudioCode
```

**Recommended Extensions:**
- C# DevKit (ms-dotnettools.csharp)
- Playwright Test for VSCode (ms-playwright.playwright)
- REST Client (humao.rest-client)

---

## 🚀 Installation Steps

### Step 1: Clone the Repository

```bash
# Clone the repository
git clone https://github.com/your-repo/playwright-csharp.git
cd playwright-csharp

# Or if already have the code locally
cd c:\Users\Gaston\source\repos\playwright-csharp
```

### Step 2: Restore NuGet Dependencies

```bash
# Restore all packages
dotnet restore

# This downloads all dependencies from NuGet:
# - Playwright 1.48.0
# - NUnit 4.1.0
# - RestSharp 113.0.0
# - Serilog 4.2.0
# - Microsoft.Extensions.Configuration 8.0.0
```

**What this installs:**
- Playwright browser automation library
- NUnit test framework
- RestSharp HTTP client
- Configuration management libraries
- Logging framework

### Step 3: Build the Solution

```bash
# Build in Debug mode
dotnet build

# Or build in Release mode
dotnet build --configuration Release

# Expected output:
# Build succeeded. ... warnings (optional)
```

### Step 4: Install Playwright Browsers

```bash
# Install browsers (required for UI tests)
dotnet playwright install

# Or install specific browser
dotnet playwright install chromium
dotnet playwright install firefox
dotnet playwright install webkit

# With dependencies (Linux)
dotnet playwright install --with-deps
```

**What gets installed:**
- Chromium (default)
- Firefox (optional)
- WebKit (optional)
- Codec dependencies
- Location: `~/.cache/ms-playwright/` (varies by OS)

**Time Required:** 2-5 minutes (first time)

**Verify Installation:**
```bash
# Check installed browsers
dir %USERPROFILE%\.cache\ms-playwright\

# Or on Linux/macOS
ls ~/.cache/ms-playwright/
```

---

## 🎯 Playwright Setup

### Browser Configuration

The framework supports multiple browsers. Configure in `appsettings.json`:

```json
{
  "AppSettings": {
    "Browser": "chromium",
    "Headless": true,
    "Timeout": 30000
  }
}
```

**Supported Browsers:**
- `chromium` - Chromium-based browser (default, fastest)
- `firefox` - Firefox browser (good compatibility)
- `webkit` - WebKit/Safari browser (for Safari testing)

### Headless vs Headed Mode

**Headless Mode (Default):**
```json
{
  "AppSettings": {
    "Headless": true
  }
}
```
- Browser runs without UI window
- Faster execution
- Used in CI/CD pipelines
- Better for automated testing

**Headed Mode (Development):**
```json
{
  "AppSettings": {
    "Headless": false
  }
}
```
- Browser window visible during test execution
- Good for debugging test failures
- Slower execution
- Can observe browser interactions

**Set Headed Mode for Development:**

In `appsettings.Development.json`:
```json
{
  "AppSettings": {
    "Headless": false
  }
}
```

Then run:
```bash
set ASPNETCORE_ENVIRONMENT=Development
dotnet test
```

### Timeout Configuration

```json
{
  "AppSettings": {
    "Timeout": 30000
  }
}
```

**Timeout Values:**
- `5000` - Aggressive (5 seconds)
- `10000` - Standard (10 seconds)
- `30000` - Normal (30 seconds) - **Recommended**
- `60000` - Patient (60 seconds)

---

## ⚙️ Configuration

### Configuration Files

The framework uses a hierarchical configuration system:

```
appsettings.json (base settings)
    ↓
appsettings.{ASPNETCORE_ENVIRONMENT}.json (overrides)
```

### Structure

**appsettings.json:**
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

**appsettings.Development.json:**
```json
{
  "AppSettings": {
    "Headless": false,
    "Browser": "chromium",
    "Timeout": 60000
  }
}
```

**appsettings.Production.json:**
```json
{
  "AppSettings": {
    "Headless": true,
    "Browser": "chromium"
  }
}
```

### Setting Environment

```bash
# Windows
set ASPNETCORE_ENVIRONMENT=Development
dotnet test

# PowerShell
$env:ASPNETCORE_ENVIRONMENT = "Development"
dotnet test

# Linux/macOS
export ASPNETCORE_ENVIRONMENT=Development
dotnet test
```

### Configuration Classes

**TestConfiguration.cs** - Centralized access to configuration:

```csharp
public class TestConfiguration
{
    public static string GetBrowser() => 
        GetConfiguration().GetSection("AppSettings:Browser").Value ?? "chromium";

    public static bool IsHeadless() => 
        bool.Parse(GetConfiguration().GetSection("AppSettings:Headless").Value ?? "true");

    public static int GetTimeout() => 
        int.Parse(GetConfiguration().GetSection("AppSettings:Timeout").Value ?? "30000");

    public static string GetValidUsername() => 
        GetConfiguration().GetSection("TestData:ValidUsername").Value ?? string.Empty;

    public static string GetValidPassword() => 
        GetConfiguration().GetSection("TestData:ValidPassword").Value ?? string.Empty;
}
```

---

## ✔️ Verify Installation

### Quick Verification

```bash
# 1. Check .NET installation
dotnet --version

# 2. Check dependencies restored
dotnet restore

# 3. Build the solution
dotnet build

# 4. Verify Playwright installed
dotnet playwright install --list

# 5. Run a simple test
dotnet test --filter "Test_NavigateToHomePage"
```

### Expected Output

**Successful .NET check:**
```
.NET SDK 8.0.x (or later)
```

**Successful restore:**
```
Restore completed in X.XXs
```

**Successful build:**
```
Build succeeded. 0 Warning(s)
```

**Successful Playwright install:**
```
Chromium 131.0.6772.0 (EXECUTABLE)
Firefox 123.0 (EXECUTABLE)
WebKit 17.4 (EXECUTABLE)
```

**Successful test run:**
```
[2025-12-17 16:46:05.232 -03:00] Browser initialized with chromium
Passed Test_NavigateToHomePage [2s]
Test Run Successful
```

---

## 🔧 Troubleshooting

### Issue 1: "dotnet: command not found"

**Problem:** .NET SDK not installed or not in PATH

**Solution:**
```bash
# Install .NET 8.0
# Download from: https://dotnet.microsoft.com/download/dotnet/8.0

# Verify after installation
dotnet --version

# If still not found, add to PATH manually
# Windows: Edit System Environment Variables > Add .NET location to PATH
```

### Issue 2: "Failed to install browser"

**Problem:** Playwright browsers failed to download

**Solution:**
```bash
# Clear cache and reinstall
rm -r %USERPROFILE%\.cache\ms-playwright\
dotnet playwright install

# Or with dependencies
dotnet playwright install --with-deps

# Try alternate download source (if behind proxy)
set PLAYWRIGHT_DOWNLOAD_HOST=https://npm.taobao.org/mirrors/playwright
dotnet playwright install
```

### Issue 3: "Timeout waiting for selector"

**Problem:** Element not found within timeout

**Solution:**
```bash
# 1. Increase timeout in appsettings.json
"Timeout": 60000

# 2. Run in headed mode to see what's happening
# Set Headless: false in appsettings.Development.json
set ASPNETCORE_ENVIRONMENT=Development
dotnet test

# 3. Check selector validity
# Use browser DevTools to verify CSS selectors
```

### Issue 4: "Configuration not loading"

**Problem:** Tests can't find appsettings.json

**Solution:**
```bash
# 1. Verify files exist
dir appsettings.json
dir UITests\bin\Debug\net8.0\appsettings.json

# 2. Copy config files to output directory
copy appsettings.json UITests\bin\Debug\net8.0\
copy appsettings.Development.json UITests\bin\Debug\net8.0\

# 3. Clean and rebuild
dotnet clean
dotnet build
```

### Issue 5: "Port already in use"

**Problem:** Another process using the same port

**Solution:**
```bash
# Find process using port
netstat -ano | findstr :5000

# Kill the process
taskkill /PID <PID> /F

# Or use different port in configuration
```

### Issue 6: Network Issues Behind Corporate Proxy

**Problem:** Can't download dependencies or browsers

**Solution:**
```bash
# Configure proxy for NuGet
dotnet nuget config get-all

dotnet nuget update source "NuGet official package source" \
  --username myUsername \
  --password myPassword \
  --store-password-in-clear-text

# Configure proxy for Playwright downloads
set HTTP_PROXY=http://proxy.company.com:8080
set HTTPS_PROXY=http://proxy.company.com:8080
dotnet playwright install
```

### Issue 7: Tests Pass Locally but Fail in CI/CD

**Problem:** Environment differences between local and CI

**Solution:**
```bash
# 1. Check .NET version in CI matches local
# Both should be 8.0+

# 2. Ensure Playwright installed in CI
# Add to workflow: dotnet playwright install

# 3. Match configurations
# CI should use Production environment

# 4. Check browser availability
# Ensure headless mode: true in production config
```

---

## 🌐 Working with Multiple Browsers

### Testing with Different Browsers

**Option 1: Run tests with each browser**
```bash
# Chromium
set AppSettings:Browser=chromium
dotnet test

# Firefox
set AppSettings:Browser=firefox
dotnet test

# WebKit
set AppSettings:Browser=webkit
dotnet test
```

**Option 2: Parameterized tests (multiple runs)**
```csharp
[TestFixture(Browser.Chromium)]
[TestFixture(Browser.Firefox)]
[TestFixture(Browser.WebKit)]
public class BrowserCompatibilityTests
{
    private readonly Browser _browser;

    public BrowserCompatibilityTests(Browser browser)
    {
        _browser = browser;
    }

    [Test]
    public async Task Test_LoginWorkingOnAllBrowsers()
    {
        // Test code using _browser...
    }
}

public enum Browser
{
    Chromium,
    Firefox,
    WebKit
}
```

### Browser-Specific Configuration

Create separate configuration files for each browser:

**appsettings.chromium.json:**
```json
{
  "AppSettings": {
    "Browser": "chromium"
  }
}
```

**appsettings.firefox.json:**
```json
{
  "AppSettings": {
    "Browser": "firefox"
  }
}
```

---

## 📝 Post-Installation Checklist

- [ ] .NET 8.0 SDK installed and verified
- [ ] Git installed and repository cloned
- [ ] PowerShell 7+ installed (Windows)
- [ ] Dependencies restored (`dotnet restore`)
- [ ] Solution builds successfully (`dotnet build`)
- [ ] Playwright browsers installed (`dotnet playwright install`)
- [ ] Configuration files in place (appsettings.json)
- [ ] Test files accessible
- [ ] Sample test runs successfully

---

## 🎓 Next Steps

1. **Read Testing Guide:** See [TESTING.md](TESTING.md)
2. **Review Project Structure:** See [README.md](README.md)
3. **Run Sample Tests:** `dotnet test --filter "LoginTests"`
4. **Create New Tests:** Follow patterns in [TESTING.md](TESTING.md)
5. **Configure CI/CD:** Push to GitHub to trigger workflows

---

**Last Updated:** December 2025
