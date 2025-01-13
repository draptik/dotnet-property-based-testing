using System.Net;
using System.Net.Http.Json;

using Microsoft.AspNetCore.Mvc.Testing;

namespace WebAppDemoCsharp.Tests;

public class TodoApiTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
  private readonly HttpClient _client = factory.CreateClient();

  [Fact]
  public async Task CreateTodoItem_ShouldReturnCreated()
  {
    // Arrange
    var request = new { Title = "Test Todo", Details = "Test Details" };

    // Act
    var response = await _client.PutAsJsonAsync("/api/todo", request);

    // Assert
    Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    var responseBody = await response.Content.ReadFromJsonAsync<TodoItem>();
    Assert.True(Guid.TryParse(responseBody.Id.Value.ToString(), out _),
      "Response does not contain a valid GUID.");
  }
}