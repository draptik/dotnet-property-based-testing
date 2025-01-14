module TodoApiTests

open System
open System.Net
open System.Net.Http.Json
open Microsoft.AspNetCore.Mvc.Testing
open Xunit

type Request = {
  Title: string
  Details: string
}

[<Fact>]
let ``create todo item should return created`` () =
  let tryParseGuid (input: string) =
    match Guid.TryParse(input) with
    | true, parsedGui -> Some parsedGui
    | false, _ -> None

  task {
    // Arrange
    use factory = new WebApplicationFactory<Program>()
    use client = factory.CreateClient()

    let url = "/api/todo"
    let request = { Title = "title1"; Details = "details1" }

    // Act
    let! response = client.PutAsJsonAsync(url, request)

    // Assert
    Assert.True(response.IsSuccessStatusCode, $"Expected success status code but got %A{response.StatusCode}")
    Assert.Equal(HttpStatusCode.Created, response.StatusCode)

    let! responseBody = response.Content.ReadFromJsonAsync<TodoItem>()
    let unparsedGuid = responseBody.Id.Value.ToString()
    let maybeId = tryParseGuid unparsedGuid
    Assert.True(maybeId.IsSome, "Expected Some but got None.")
  }