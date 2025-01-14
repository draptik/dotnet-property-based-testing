module TodoApiTests

open System
open System.Net
open System.Net.Http.Json
open Microsoft.AspNetCore.Mvc.Testing
open Xunit
open FsCheck
open FsCheck.Xunit

type Request = { Title: string; Details: string }

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

    let request = {
      Title = "title1"
      Details = "details1"
    }

    // Act
    let! response = client.PutAsJsonAsync(url, request)

    // Assert
    Assert.True(response.IsSuccessStatusCode, $"Expected success status code but got %A{response.StatusCode}")
    Assert.Equal(HttpStatusCode.Created, response.StatusCode)

    let! responseBody = response.Content.ReadFromJsonAsync<TodoItem>()
    let unparsedId = responseBody.Id.Value.ToString()
    let maybeId = tryParseGuid unparsedId
    Assert.True(maybeId.IsSome, "Expected Some but got None.")
  }

[<Property(Skip = "Does not work")>]
let ``create todo item should handle valid input`` (t: NonWhiteSpaceString, d: NonWhiteSpaceString) =
  let title = t.Get
  let details = d.Get

  let tryParseGuid (input: string) =
    match Guid.TryParse(input) with
    | true, parsedGuid -> Some parsedGuid
    | false, _ -> None

  task {
    // Arrange
    use factory = new WebApplicationFactory<Program>()
    use client = factory.CreateClient()

    let url = "/api/todo"
    let request = { Title = title; Details = details }

    // Act
    let! response = client.PutAsJsonAsync(url, request)

    // Assert
    Assert.True(response.IsSuccessStatusCode, $"Expected success status code but got %A{response.StatusCode}")
    Assert.Equal(HttpStatusCode.Created, response.StatusCode)

    let! responseBody = response.Content.ReadFromJsonAsync<TodoItem>()
    let unparsedId = responseBody.Id.Value.ToString()
    let maybeId = tryParseGuid unparsedId
    Assert.True(maybeId.IsSome, "Expected Some but got None.")
  }