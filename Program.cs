
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.EntityFrameworkCore;



var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TodoDb>(options => 
    options.UseSqlite("Data source=todos.db"));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();


app.MapGet("/", () => "Hello World!");
app.MapGet("/todo", async(TodoDb db) =>
{
    return await db.Todos.ToListAsync();
});

app.MapPost("/todo", async (TodoDto dto, TodoDb db) =>
{
    if (string.IsNullOrWhiteSpace(dto.Name))
    {
        return Results.BadRequest("Todo name is required");
    }

    var todo = new Todo
    {       
        Name = dto.Name,
        IsComplete = dto.IsComplete
    };

    db.Todos.Add(todo);
    await db.SaveChangesAsync();
    
    return Results.Ok(todo);
});

app.MapPut("/todo/{id}", async (int id, TodoDto inputDto, TodoDb db) =>
{
    if (string.IsNullOrWhiteSpace(inputDto.Name))
    {
        return Results.BadRequest("Todo name is required");
    }

    var todo = await db.Todos.FindAsync(id);

    if (todo == null) return Results.NotFound();

    todo.Name = inputDto.Name;
    todo.IsComplete = inputDto.IsComplete;

    await db.SaveChangesAsync();
    return Results.Ok(todo);
});

app.MapDelete("/todo/{id}", async (int id, TodoDb db) =>
{
    var todo = await db.Todos.FindAsync(id);
    if (todo == null)
        return Results.NotFound();

    db.Todos.Remove(todo);
    await db.SaveChangesAsync();

    return Results.Ok();
});

using(var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TodoDb>();
    db.Database.EnsureCreated();
}
app.Run();
