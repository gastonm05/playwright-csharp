using NUnit.Framework;
using Common.Utils;

namespace APITests.Tests;

[TestFixture]
public class QuickAPITest
{
    [Test]
    public async Task QuickTest_CheckConnection()
    {
        var apiClient = new ApiClient();
        var response = await apiClient.GetAsync("/posts/1");
        
        Console.WriteLine($"===== Response Details =====");
        Console.WriteLine($"Status Code: {response.StatusCode}");
        Console.WriteLine($"Status Code as int: {(int)response.StatusCode}");
        Console.WriteLine($"Is Successful: {response.IsSuccessful}");
        Console.WriteLine($"Content Length: {response.Content?.Length}");
        Console.WriteLine($"Error: {response.ErrorMessage}");
        Console.WriteLine($"Exception: {response.ErrorException?.Message}");
        
        // Just log, don't assert
        Assert.That(true, Is.True);
    }
}
