using NUnit.Framework;
using System.Text.Json;
using Common.Utils;
using APITests.Models;

namespace APITests.Tests;

[TestFixture]
public class JSONPlaceholderAlbumTests
{
    private ApiClient? _apiClient;

    [SetUp]
    public void Setup()
    {
        _apiClient = new ApiClient();
    }

    [Test]
    public async Task GetSingleAlbum_ReturnsAlbum_WithStatusOk()
    {
        Assert.That(_apiClient, Is.Not.Null);
        
        var response = await _apiClient!.GetAsync("/albums/1");
        
        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));
        
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var content = JsonSerializer.Deserialize<Album>(response.Content!, options);
        Assert.That(content, Is.Not.Null);
        Assert.That(content!.Id, Is.EqualTo(1));
        Assert.That(content.Title, Is.Not.Empty);
    }

    [Test]
    public async Task GetAllAlbums_ReturnsAlbumsList_WithStatusOk()
    {
        Assert.That(_apiClient, Is.Not.Null);
        
        var response = await _apiClient!.GetAsync("/albums");
        
        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));
        
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var content = JsonSerializer.Deserialize<List<Album>>(response.Content!, options);
        Assert.That(content, Is.Not.Null);
        Assert.That(content!.Count, Is.GreaterThan(0));
    }

    [Test]
    public async Task GetAlbumsByUserId_FilteredResults_WithStatusOk()
    {
        Assert.That(_apiClient, Is.Not.Null);
        
        var response = await _apiClient!.GetAsync("/albums?userId=1");
        
        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));
        
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var content = JsonSerializer.Deserialize<List<Album>>(response.Content!, options);
        Assert.That(content, Is.Not.Null);
        Assert.That(content!.Count, Is.GreaterThan(0));
        Assert.That(content.All(a => a.UserId == 1), Is.True);
    }

    [Test]
    public async Task GetAlbumPhotos_NestedResource_ReturnsPhotosList()
    {
        Assert.That(_apiClient, Is.Not.Null);
        
        var response = await _apiClient!.GetAsync("/albums/1/photos");
        
        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));
        
        var content = response.Content;
        Assert.That(content, Is.Not.Empty);
        Assert.That(content, Does.Contain("albumId"));
    }
}
