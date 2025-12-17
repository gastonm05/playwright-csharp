using Common.Utils;
using Common.Config;

// Test RestSharp connectivity
var apiClient = new ApiClient();

Console.WriteLine($"Base URL: {TestConfiguration.GetApiBaseUrl()}");

try
{
    var response = await apiClient.GetAsync("/posts/1");
    Console.WriteLine($"Status Code: {response.StatusCode}");
    Console.WriteLine($"Status Code Int: {(int)response.StatusCode}");
    Console.WriteLine($"Is Successful: {response.IsSuccessful}");
    Console.WriteLine($"Content: {response.Content?.Substring(0, 100)}");
    Console.WriteLine($"Error Message: {response.ErrorMessage}");
    Console.WriteLine($"Error Exception: {response.ErrorException?.Message}");
}
catch (Exception ex)
{
    Console.WriteLine($"Exception: {ex.Message}");
    Console.WriteLine($"Stack: {ex.StackTrace}");
}
