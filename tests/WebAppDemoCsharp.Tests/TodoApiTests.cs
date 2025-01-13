using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http.Json;

using FsCheck;
using FsCheck.Xunit;

using Microsoft.AspNetCore.Mvc.Testing;

namespace WebAppDemoCsharp.Tests;

[SuppressMessage("Usage", "xUnit1004:Test methods should not be skipped")]
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

  [Property(Skip = "Does not work", Replay = "750343325,297429063")]
  public void PropertyBased_CreateTodoItem_ShouldHandleValidInputs(NonWhiteSpaceString title,
    NonWhiteSpaceString details)
  {
    var request = new { Title = title.Get, Details = details.Get };

    // Act
    var responseTask = Task.Run(async () => await _client.PutAsJsonAsync("/api/todo", request));
    responseTask.Wait();
    var response = responseTask.Result;

    // Assert
    Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    var responseBodyTask = Task.Run(async () => await response.Content.ReadFromJsonAsync<TodoItem>());
    responseBodyTask.Wait();
    var responseBody = responseBodyTask.Result;

    Assert.Equal(title.Get, responseBody.Title.Value);
    Assert.Equal(details.Get, responseBody.Details?.Value);
    Assert.True(Guid.TryParse(responseBody.Id.Value.ToString(), out _),
      "Response does not contain a valid GUID.");
  }

  [Fact(Skip = "Does not work")]
  public void PropertyBased_CreateTodoItem_ShouldHandleValidInputs2()
  {
    Prop.ForAll<NonWhiteSpaceString, NonWhiteSpaceString>((t, d) =>
    {
      var title = t.Get;
      var details = d.Get;
      var request = new { Title = title, Details = details };
      var response = Task.Run(() => _client.PutAsJsonAsync("/api/todo", request)).Result;
      var responseBody = Task.Run(() => response.Content.ReadFromJsonAsync<TodoItem>()).Result;
      return responseBody.Title.Value == title;
    }).QuickCheckThrowOnFailure();
  }


  public async Task<int> AddAsync(int a, int b)
  {
    await Task.Delay(10); // Simulate asynchronous work
    return a + b;
  }

  [Fact]
  public void AsyncPropertyTest()
  {
    // Property to check the AddAsync method
    Prop.ForAll<int, int>((a, b) =>
    {
      // Wrap the async code and run synchronously
      var result = AddAsync(a, b).GetAwaiter().GetResult();
      return result == a + b;
    }).QuickCheckThrowOnFailure();
  }
}