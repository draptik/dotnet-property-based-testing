using System.Collections.Concurrent;
using System.Text.Json;

// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable NotAccessedPositionalProperty.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
  _ = app.MapOpenApi();
}

app.UseHttpsRedirection();

var todoItems = new ConcurrentDictionary<Guid, TodoItem>();

app.MapPut("/api/todo", (CreateTodoItemRequest request) =>
{
  if (string.IsNullOrWhiteSpace(request.Title))
  {
    return Results.BadRequest("Title is required.");
  }

  var todoItem = new TodoItem(request.Title, request.Details);
  todoItems[todoItem.Id.Value] = todoItem;

  return Results.Created($"/api/todo/{todoItem.Id.Value}", todoItem);
});

app.MapGet("/api/todo", () =>
{
  var jsonSerializerOptions = new JsonSerializerOptions { WriteIndented = true };
  return Results.Json(todoItems.Values, jsonSerializerOptions);
});

app.Run();


internal record CreateTodoItemRequest(string Title, string? Details);

internal readonly record struct TodoItemId(Guid Value);

internal readonly record struct Title(string Value);

internal readonly record struct Details(string Value);

internal readonly record struct TodoItem
{
  public TodoItem(string title, string? details)
  {
    Id = new TodoItemId(Guid.NewGuid());
    Title = new Title(title);
    if (details != null)
    {
      Details = new Details(details);
    }
  }

  public TodoItemId Id { get; }
  public Title Title { get; }
  public Details? Details { get; }
}