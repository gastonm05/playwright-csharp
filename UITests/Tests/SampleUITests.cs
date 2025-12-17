using NUnit.Framework;
using Microsoft.Playwright;
using Common.Utils;

namespace UITests.Tests;

[TestFixture]
public class SampleUITests
{
    private BrowserManager? _browserManager;
    private IPage? _page;

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
    public async Task SampleTest_NavigateToWebsite()
    {
        // Example test - navigate to a website and check title
        Assert.IsNotNull(_page);
        
        await _page.GotoAsync("https://example.com");
        var title = await _page.GetTitleAsync();
        
        Assert.That(title, Is.Not.Empty);
    }

    [Test]
    public async Task SampleTest_CheckPageContent()
    {
        Assert.IsNotNull(_page);
        
        await _page.GotoAsync("https://example.com");
        var content = await _page.GetTextContentAsync("body");
        
        Assert.That(content, Is.Not.Empty);
    }
}
