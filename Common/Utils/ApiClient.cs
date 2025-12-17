using Common.Config;
using System.Net;
using System.Text;
using System.Text.Json;
using Serilog;

namespace Common.Utils;

public class ApiResponse
{
    public HttpStatusCode StatusCode { get; set; }
    public string? Content { get; set; }
    public bool IsSuccessful { get; set; }
    public Exception? ErrorException { get; set; }
    public string? ErrorMessage { get; set; }
    
    public T? DeserializeContent<T>()
    {
        if (string.IsNullOrWhiteSpace(Content))
            return default;
            
        try
        {
            return JsonSerializer.Deserialize<T>(Content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
        catch
        {
            return default;
        }
    }
}

public class ApiClient : IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;
    private readonly JsonSerializerOptions _jsonOptions;

    public ApiClient()
    {
        _baseUrl = TestConfiguration.GetApiBaseUrl();
        
        // Configure HttpClient
        _httpClient = new HttpClient(new HttpClientHandler
        {
            UseProxy = false,
            ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
        });
        
        // Set default headers
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "ApiClient/1.0");
        
        // Set timeout
        _httpClient.Timeout = TimeSpan.FromSeconds(30);
        
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = false
        };
        
        Log.Information($"API Client initialized with base URL: {_baseUrl}");
    }

    public async Task<ApiResponse> GetAsync(string endpoint)
    {
        return await ExecuteRequestAsync(HttpMethod.Get, endpoint, null);
    }

    public async Task<ApiResponse> PostAsync(string endpoint, object? body = null)
    {
        return await ExecuteRequestAsync(HttpMethod.Post, endpoint, body);
    }

    public async Task<ApiResponse> PutAsync(string endpoint, object? body = null)
    {
        return await ExecuteRequestAsync(HttpMethod.Put, endpoint, body);
    }

    public async Task<ApiResponse> DeleteAsync(string endpoint)
    {
        return await ExecuteRequestAsync(HttpMethod.Delete, endpoint, null);
    }

    private async Task<ApiResponse> ExecuteRequestAsync(HttpMethod method, string endpoint, object? body)
    {
        string fullUrl = $"{_baseUrl.TrimEnd('/')}/{endpoint.TrimStart('/')}";
        Log.Information($"{method} request to: {fullUrl}");
        
        try
        {
            using var request = new HttpRequestMessage(method, fullUrl);

            if (body != null)
            {
                var json = JsonSerializer.Serialize(body, _jsonOptions);
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");
                Log.Debug($"Request body: {json}");
            }

            using var response = await _httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();
            
            Log.Debug($"Response status: {response.StatusCode}");
            Log.Debug($"Response content length: {content.Length} characters");
            
            if (!response.IsSuccessStatusCode)
            {
                Log.Warning($"Request failed with status: {response.StatusCode}");
                Log.Warning($"Response content: {content}");
            }

            return new ApiResponse
            {
                StatusCode = response.StatusCode,
                Content = content,
                IsSuccessful = response.IsSuccessStatusCode
            };
        }
        catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
        {
            Log.Error($"Request timeout: {fullUrl}");
            return new ApiResponse 
            { 
                StatusCode = HttpStatusCode.RequestTimeout,
                IsSuccessful = false,
                ErrorException = ex,
                ErrorMessage = $"Request timeout: {ex.Message}"
            };
        }
        catch (HttpRequestException ex)
        {
            Log.Error($"HTTP request failed: {ex.Message}");
            return new ApiResponse 
            { 
                StatusCode = ex.StatusCode ?? HttpStatusCode.BadGateway,
                IsSuccessful = false,
                ErrorException = ex,
                ErrorMessage = $"HTTP error: {ex.Message}"
            };
        }
        catch (Exception ex)
        {
            Log.Error($"Request failed: {ex.Message}");
            return new ApiResponse 
            { 
                StatusCode = HttpStatusCode.InternalServerError,
                IsSuccessful = false,
                ErrorException = ex,
                ErrorMessage = ex.Message
            };
        }
    }

    public void Dispose()
    {
        _httpClient?.Dispose();
        GC.SuppressFinalize(this);
    }
}