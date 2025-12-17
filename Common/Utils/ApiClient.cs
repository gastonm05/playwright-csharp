using RestSharp;
using Common.Config;
using Serilog;

namespace Common.Utils;

public class ApiClient
{
    private readonly RestClient _client;

    public ApiClient()
    {
        var baseUrl = TestConfiguration.GetApiBaseUrl();
        _client = new RestClient(baseUrl);
        Log.Information($"API Client initialized with base URL: {baseUrl}");
    }

    public async Task<RestResponse> GetAsync(string endpoint)
    {
        Log.Information($"GET request to: {endpoint}");
        var request = new RestRequest(endpoint, Method.Get);
        var response = await _client.ExecuteAsync(request);
        Log.Information($"Response status: {response.StatusCode}");
        return response;
    }

    public async Task<RestResponse> PostAsync(string endpoint, object? body = null)
    {
        Log.Information($"POST request to: {endpoint}");
        var request = new RestRequest(endpoint, Method.Post);
        if (body != null)
        {
            request.AddJsonBody(body);
        }
        var response = await _client.ExecuteAsync(request);
        Log.Information($"Response status: {response.StatusCode}");
        return response;
    }

    public async Task<RestResponse> PutAsync(string endpoint, object? body = null)
    {
        Log.Information($"PUT request to: {endpoint}");
        var request = new RestRequest(endpoint, Method.Put);
        if (body != null)
        {
            request.AddJsonBody(body);
        }
        var response = await _client.ExecuteAsync(request);
        Log.Information($"Response status: {response.StatusCode}");
        return response;
    }

    public async Task<RestResponse> DeleteAsync(string endpoint)
    {
        Log.Information($"DELETE request to: {endpoint}");
        var request = new RestRequest(endpoint, Method.Delete);
        var response = await _client.ExecuteAsync(request);
        Log.Information($"Response status: {response.StatusCode}");
        return response;
    }
}
