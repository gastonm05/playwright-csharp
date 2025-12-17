using NUnit.Framework;
using Common.Utils;

namespace APITests.Tests;

[TestFixture]
public class SampleAPITests
{
    private ApiClient? _apiClient;

    [SetUp]
    public void Setup()
    {
        _apiClient = new ApiClient();
    }

    [Test]
    public async Task SampleTest_GetRequest()
    {
        Assert.IsNotNull(_apiClient);
        
        var response = await _apiClient.GetAsync("/users");
        
        Assert.That((int)response.StatusCode, Is.EqualTo(200).Or.GreaterThanOrEqualTo(200).And.LessThan(300));
    }

    [Test]
    public async Task SampleTest_PostRequest()
    {
        Assert.IsNotNull(_apiClient);
        
        var body = new { name = "Test User", email = "test@example.com" };
        var response = await _apiClient.PostAsync("/users", body);
        
        Assert.That((int)response.StatusCode, Is.GreaterThanOrEqualTo(200).And.LessThan(300)
            .Or.EqualTo(400).Or.EqualTo(401).Or.EqualTo(403));
    }
}
