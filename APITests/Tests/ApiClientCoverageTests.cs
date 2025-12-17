using NUnit.Framework;
using System.Text.Json;
using Common.Utils;
using APITests.Models;

namespace APITests.Tests;

/// <summary>
/// Tests for ApiClient error handling and edge cases to improve code coverage.
/// Focuses on exception paths and the DeserializeContent helper method.
/// </summary>
[TestFixture]
public class ApiClientCoverageTests
{
    private ApiClient? _apiClient;

    [SetUp]
    public void Setup()
    {
        _apiClient = new ApiClient();
    }

    [TearDown]
    public void Teardown()
    {
        _apiClient?.Dispose();
    }

    #region DeserializeContent Tests

    [Test]
    public async Task DeserializeContent_WithValidJson_ReturnsDeserializedObject()
    {
        Assert.That(_apiClient, Is.Not.Null);
        
        var response = await _apiClient!.GetAsync("/users/1");
        
        Assert.That(response.Content, Is.Not.Null);
        Assert.That(response.IsSuccessful, Is.True);
        
        // Test the DeserializeContent helper method
        var user = response.DeserializeContent<User>();
        
        Assert.That(user, Is.Not.Null);
        Assert.That(user!.Id, Is.EqualTo(1));
        Assert.That(user.Name, Is.Not.Empty);
    }

    [Test]
    public async Task DeserializeContent_WithListJson_ReturnsDeserializedList()
    {
        Assert.That(_apiClient, Is.Not.Null);
        
        var response = await _apiClient!.GetAsync("/posts");
        
        Assert.That(response.Content, Is.Not.Null);
        Assert.That(response.IsSuccessful, Is.True);
        
        // Test deserialization of list
        var posts = response.DeserializeContent<List<Post>>();
        
        Assert.That(posts, Is.Not.Null);
        Assert.That(posts!.Count, Is.GreaterThan(0));
        Assert.That(posts[0].Id, Is.GreaterThan(0));
    }

    [Test]
    public void DeserializeContent_WithNullContent_ReturnsDefault()
    {
        Assert.That(_apiClient, Is.Not.Null);
        
        var response = new ApiResponse
        {
            Content = null,
            IsSuccessful = true,
            StatusCode = System.Net.HttpStatusCode.OK
        };
        
        var result = response.DeserializeContent<User>();
        
        Assert.That(result, Is.Null);
    }

    [Test]
    public void DeserializeContent_WithEmptyContent_ReturnsDefault()
    {
        Assert.That(_apiClient, Is.Not.Null);
        
        var response = new ApiResponse
        {
            Content = string.Empty,
            IsSuccessful = true,
            StatusCode = System.Net.HttpStatusCode.OK
        };
        
        var result = response.DeserializeContent<User>();
        
        Assert.That(result, Is.Null);
    }

    [Test]
    public void DeserializeContent_WithInvalidJson_ReturnsDefault()
    {
        Assert.That(_apiClient, Is.Not.Null);
        
        var response = new ApiResponse
        {
            Content = "{ invalid json }",
            IsSuccessful = false,
            StatusCode = System.Net.HttpStatusCode.BadRequest
        };
        
        var result = response.DeserializeContent<User>();
        
        // Should return default when deserialization fails
        Assert.That(result, Is.Null);
    }

    [Test]
    public void DeserializeContent_WithWhitespaceContent_ReturnsDefault()
    {
        Assert.That(_apiClient, Is.Not.Null);
        
        var response = new ApiResponse
        {
            Content = "   ",
            IsSuccessful = false,
            StatusCode = System.Net.HttpStatusCode.BadRequest
        };
        
        var result = response.DeserializeContent<User>();
        
        Assert.That(result, Is.Null);
    }

    #endregion

    #region Error Response Tests

    [Test]
    public async Task GetAsync_WithInvalidEndpoint_Returns404()
    {
        Assert.That(_apiClient, Is.Not.Null);
        
        var response = await _apiClient!.GetAsync("/invalid-endpoint-xyz-404");
        
        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.NotFound));
        Assert.That(response.IsSuccessful, Is.False);
        Assert.That(response.Content, Is.Not.Null);
    }

    [Test]
    public async Task GetAsync_FailedResponse_HasErrorInfo()
    {
        Assert.That(_apiClient, Is.Not.Null);
        
        var response = await _apiClient!.GetAsync("/invalid-endpoint-xyz-404");
        
        Assert.That(response.IsSuccessful, Is.False);
        // When response fails, Content is populated with error details
        Assert.That(response.Content, Is.Not.Null.Or.Empty);
    }

    [Test]
    public async Task PostAsync_WithInvalidData_StillReturnsResponse()
    {
        Assert.That(_apiClient, Is.Not.Null);
        
        // JSONPlaceholder accepts any POST, but we test the response handling
        var invalidData = new { };
        var response = await _apiClient!.PostAsync("/posts", invalidData);
        
        // JSONPlaceholder always accepts POST, but test ensures response is handled
        Assert.That(response.IsSuccessful, Is.True);
        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.Created));
    }

    [Test]
    public async Task DeleteAsync_WithInvalidId_StillReturnsResponse()
    {
        Assert.That(_apiClient, Is.Not.Null);
        
        // JSONPlaceholder accepts DELETE even for non-existent IDs
        var response = await _apiClient!.DeleteAsync("/posts/999999");
        
        // JSONPlaceholder always accepts DELETE
        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));
    }

    #endregion

    #region Response Properties Tests

    [Test]
    public async Task ApiResponse_WithSuccessfulRequest_HasCorrectProperties()
    {
        Assert.That(_apiClient, Is.Not.Null);
        
        var response = await _apiClient!.GetAsync("/users/1");
        
        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));
        Assert.That(response.IsSuccessful, Is.True);
        Assert.That(response.ErrorException, Is.Null);
        Assert.That(response.ErrorMessage, Is.Null);
        Assert.That(response.Content, Is.Not.Null);
    }

    [Test]
    public async Task ApiResponse_WithFailedRequest_HasErrorInfo()
    {
        Assert.That(_apiClient, Is.Not.Null);
        
        var response = await _apiClient!.GetAsync("/invalid-endpoint");
        
        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.NotFound));
        Assert.That(response.IsSuccessful, Is.False);
    }

    #endregion

    #region Edge Case Tests

    [Test]
    public async Task GetAsync_WithQueryParameters_ReturnsFilteredResults()
    {
        Assert.That(_apiClient, Is.Not.Null);
        
        var response = await _apiClient!.GetAsync("/posts?userId=2");
        
        Assert.That(response.IsSuccessful, Is.True);
        var posts = response.DeserializeContent<List<Post>>();
        
        Assert.That(posts, Is.Not.Null);
        Assert.That(posts!.All(p => p.UserId == 2), Is.True);
    }

    [Test]
    public async Task GetAsync_WithComplexEndpoint_ReturnsResults()
    {
        Assert.That(_apiClient, Is.Not.Null);
        
        // Test nested resource endpoint
        var response = await _apiClient!.GetAsync("/users/1/posts");
        
        Assert.That(response.IsSuccessful, Is.True);
        var posts = response.DeserializeContent<List<Post>>();
        
        Assert.That(posts, Is.Not.Null);
    }

    [Test]
    public async Task PostAsync_WithComplexObject_SerializesCorrectly()
    {
        Assert.That(_apiClient, Is.Not.Null);
        
        var newPost = new Post
        {
            Title = "Coverage Test Post",
            Body = "Testing body with special characters: é à ü",
            UserId = 5
        };

        var response = await _apiClient!.PostAsync("/posts", newPost);
        
        Assert.That(response.IsSuccessful, Is.True);
        var createdPost = response.DeserializeContent<Post>();
        
        Assert.That(createdPost, Is.Not.Null);
        Assert.That(createdPost!.Title, Is.EqualTo("Coverage Test Post"));
    }

    #endregion

    #region Client Disposal Tests

    [Test]
    public void ApiClient_Dispose_DoesNotThrow()
    {
        var client = new ApiClient();
        
        // Should not throw any exception
        Assert.DoesNotThrow(() => client.Dispose());
    }

    [Test]
    public void ApiClient_MultipleDispose_DoesNotThrow()
    {
        var client = new ApiClient();
        
        // Multiple dispose calls should not throw
        Assert.DoesNotThrow(() =>
        {
            client.Dispose();
            client.Dispose();
        });
    }

    #endregion
}
