using NUnit.Framework;
using System.Text.Json;
using Common.Utils;
using APITests.Models;

namespace APITests.Tests;

[TestFixture]
public class JSONPlaceholderPostTests
{
    private ApiClient? _apiClient;

    [SetUp]
    public void Setup()
    {
        _apiClient = new ApiClient();
    }

    [Test]
    public async Task GetSinglePost_ReturnsPost_WithStatusOk()
    {
        Assert.That(_apiClient, Is.Not.Null);
        
        var response = await _apiClient!.GetAsync("/posts/1");
        
        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));
        
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var content = JsonSerializer.Deserialize<Post>(response.Content!, options);
        Assert.That(content, Is.Not.Null);
        Assert.That(content!.Id, Is.EqualTo(1));
        Assert.That(content.Title, Is.Not.Empty);
        Assert.That(content.Body, Is.Not.Empty);
    }

    [Test]
    public async Task GetAllPosts_ReturnsPostsList_WithStatusOk()
    {
        Assert.That(_apiClient, Is.Not.Null);
        
        var response = await _apiClient!.GetAsync("/posts");
        
        Assert.That((int)response.StatusCode, Is.EqualTo((int)System.Net.HttpStatusCode.OK));
        
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var content = JsonSerializer.Deserialize<List<Post>>(response.Content!, options);
        Assert.That(content, Is.Not.Null);
        Assert.That(content!.Count, Is.GreaterThan(0));
    }

    [Test]
    public async Task GetPostsByUserId_FilteredResults_WithStatusOk()
    {
        Assert.That(_apiClient, Is.Not.Null);
        
        var response = await _apiClient!.GetAsync("/posts?userId=1");
        
        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));
        
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var content = JsonSerializer.Deserialize<List<Post>>(response.Content!, options);
        Assert.That(content, Is.Not.Null);
        Assert.That(content!.Count, Is.GreaterThan(0));
        Assert.That(content.All(p => p.UserId == 1), Is.True);
    }

    [Test]
    public async Task CreatePost_WithValidData_ReturnsCreatedPost()
    {
        Assert.That(_apiClient, Is.Not.Null);
        
        var newPost = new Post
        {
            Title = "Test Post",
            Body = "This is a test post body",
            UserId = 1
        };

        var response = await _apiClient!.PostAsync("/posts", newPost);
        
        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.Created));
        
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var content = JsonSerializer.Deserialize<Post>(response.Content!, options);
        Assert.That(content, Is.Not.Null);
        Assert.That(content!.Title, Is.EqualTo("Test Post"));
        Assert.That(content.Body, Is.EqualTo("This is a test post body"));
    }

    [Test]
    public async Task UpdatePost_WithValidData_ReturnsUpdatedPost()
    {
        Assert.That(_apiClient, Is.Not.Null);
        
        var updatedPost = new Post
        {
            Id = 1,
            Title = "Updated Title",
            Body = "Updated body content",
            UserId = 1
        };

        var response = await _apiClient!.PutAsync("/posts/1", updatedPost);
        
        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));
        
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var content = JsonSerializer.Deserialize<Post>(response.Content!, options);
        Assert.That(content, Is.Not.Null);
        Assert.That(content!.Title, Is.EqualTo("Updated Title"));
    }

    [Test]
    public async Task DeletePost_WithValidId_ReturnsOk()
    {
        Assert.That(_apiClient, Is.Not.Null);
        
        var response = await _apiClient!.DeleteAsync("/posts/1");
        
        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));
    }

    [Test]
    public async Task GetPostComments_NestedResource_ReturnsCommentsList()
    {
        Assert.That(_apiClient, Is.Not.Null);
        
        var response = await _apiClient!.GetAsync("/posts/1/comments");
        
        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));
        
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var content = JsonSerializer.Deserialize<List<Comment>>(response.Content!, options);
        Assert.That(content, Is.Not.Null);
        Assert.That(content!.Count, Is.GreaterThan(0));
        Assert.That(content.All(c => c.PostId == 1), Is.True);
    }
}
