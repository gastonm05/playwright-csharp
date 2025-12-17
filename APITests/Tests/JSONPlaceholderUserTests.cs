using NUnit.Framework;
using System.Text.Json;
using Common.Utils;
using APITests.Models;

namespace APITests.Tests;

[TestFixture]
public class JSONPlaceholderUserTests
{
    private ApiClient? _apiClient;

    [SetUp]
    public void Setup()
    {
        _apiClient = new ApiClient();
    }

    [Test]
    public async Task GetSingleUser_ReturnsUser_WithStatusOk()
    {
        Assert.That(_apiClient, Is.Not.Null);
        
        var response = await _apiClient!.GetAsync("/users/1");
        
        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));
        
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var content = JsonSerializer.Deserialize<User>(response.Content!, options);
        Assert.That(content, Is.Not.Null);
        Assert.That(content!.Id, Is.EqualTo(1));
        Assert.That(content.Name, Is.Not.Empty);
        Assert.That(content.Email, Is.Not.Empty);
    }

    [Test]
    public async Task GetAllUsers_ReturnsUsersList_WithStatusOk()
    {
        Assert.That(_apiClient, Is.Not.Null);
        
        var response = await _apiClient!.GetAsync("/users");
        
        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));
        
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var content = JsonSerializer.Deserialize<List<User>>(response.Content!, options);
        Assert.That(content, Is.Not.Null);
        Assert.That(content!.Count, Is.GreaterThan(0));
    }

    [Test]
    public async Task GetUserPosts_NestedResource_ReturnsPostsList()
    {
        Assert.That(_apiClient, Is.Not.Null);
        
        var response = await _apiClient!.GetAsync("/users/1/posts");
        
        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));
        
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var content = JsonSerializer.Deserialize<List<Post>>(response.Content!, options);
        Assert.That(content, Is.Not.Null);
        Assert.That(content!.Count, Is.GreaterThan(0));
        Assert.That(content.All(p => p.UserId == 1), Is.True);
    }

    [Test]
    public async Task GetUserAlbums_NestedResource_ReturnsAlbumsList()
    {
        Assert.That(_apiClient, Is.Not.Null);
        
        var response = await _apiClient!.GetAsync("/users/1/albums");
        
        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));
        
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var content = JsonSerializer.Deserialize<List<Album>>(response.Content!, options);
        Assert.That(content, Is.Not.Null);
        Assert.That(content!.Count, Is.GreaterThan(0));
        Assert.That(content.All(a => a.UserId == 1), Is.True);
    }

    [Test]
    public async Task GetUserTodos_NestedResource_ReturnsTodosList()
    {
        Assert.That(_apiClient, Is.Not.Null);
        
        var response = await _apiClient!.GetAsync("/users/1/todos");
        
        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));
        
        var content = response.Content;
        Assert.That(content, Is.Not.Empty);
        Assert.That(content, Does.Contain("id"));
    }
}
