
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.EntityFrameworkCore;

var todos = new List<Todo>();
int nextId = 1;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TodoDb>(options => 
    options.UseSqlite("Data source=todos.db"));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();


app.MapGet("/", () => "Hello World!");
app.MapGet("/todo", () =>
{
    return todos;
});

app.MapPost("/todo", (TodoDto dto) =>
{
    if (string.IsNullOrWhiteSpace(dto.Name))
    {
        return Results.BadRequest("Todo name is required");
    }

    var todo = new Todo
    {
        Id = nextId,
        Name = dto.Name,
        IsComplete = dto.IsComplete
    };

    nextId++;
    todos.Add(todo);
    
    return Results.Ok(todo);
});

app.MapPut("/todo/{id}", (int id, TodoDto inputDto) =>
{
    if (string.IsNullOrWhiteSpace(inputDto.Name))
    {
        return Results.BadRequest("Todo name is required");
    }

    var todo = todos.FirstOrDefault(t => t.Id == id);

    if (todo == null) return Results.NotFound();

    todo.Name = inputDto.Name;
    todo.IsComplete = inputDto.IsComplete;

    return Results.Ok(todo);
});

app.MapDelete("/todo/{id}", (int id) =>
{
    var todo = todos.FirstOrDefault(t => t.Id == id);
    if (todo == null)
        return Results.NotFound();

    todos.Remove(todo);
    return Results.Ok();
});


app.Run();
