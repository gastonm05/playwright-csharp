using NUnit.Framework;
using System.Text.Json;
using Common.Utils;
using APITests.Models;

namespace APITests.Tests;

[TestFixture]
public class JSONPlaceholderCommentTests
{
    private ApiClient? _apiClient;

    [SetUp]
    public void Setup()
    {
        _apiClient = new ApiClient();
    }

    [Test]
    public async Task GetSingleComment_ReturnsComment_WithStatusOk()
    {
        Assert.That(_apiClient, Is.Not.Null);
        
        var response = await _apiClient!.GetAsync("/comments/1");
        
        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));
        
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var content = JsonSerializer.Deserialize<Comment>(response.Content!, options);
        Assert.That(content, Is.Not.Null);
        Assert.That(content!.Id, Is.EqualTo(1));
        Assert.That(content.Email, Is.Not.Empty);
        Assert.That(content.Body, Is.Not.Empty);
    }

    [Test]
    public async Task GetAllComments_ReturnsCommentsList_WithStatusOk()
    {
        Assert.That(_apiClient, Is.Not.Null);
        
        var response = await _apiClient!.GetAsync("/comments");
        
        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));
        
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var content = JsonSerializer.Deserialize<List<Comment>>(response.Content!, options);
        Assert.That(content, Is.Not.Null);
        Assert.That(content!.Count, Is.GreaterThan(0));
    }

    [Test]
    public async Task GetCommentsByPostId_FilteredResults_WithStatusOk()
    {
        Assert.That(_apiClient, Is.Not.Null);
        
        var response = await _apiClient!.GetAsync("/comments?postId=1");
        
        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));
        
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var content = JsonSerializer.Deserialize<List<Comment>>(response.Content!, options);
        Assert.That(content, Is.Not.Null);
        Assert.That(content!.Count, Is.GreaterThan(0));
        Assert.That(content.All(c => c.PostId == 1), Is.True);
    }

    [Test]
    public async Task CreateComment_WithValidData_ReturnsCreatedComment()
    {
        Assert.That(_apiClient, Is.Not.Null);
        
        var newComment = new Comment
        {
            PostId = 1,
            Name = "Test Comment",
            Email = "test@example.com",
            Body = "This is a test comment"
        };

        var response = await _apiClient!.PostAsync("/comments", newComment);
        
        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.Created));
        
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var content = JsonSerializer.Deserialize<Comment>(response.Content!, options);
        Assert.That(content, Is.Not.Null);
        Assert.That(content!.Name, Is.EqualTo("Test Comment"));
        Assert.That(content.Email, Is.EqualTo("test@example.com"));
    }
}
