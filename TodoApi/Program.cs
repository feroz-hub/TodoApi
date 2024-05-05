using Microsoft.EntityFrameworkCore;
using TodoApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TodoDb>(opt => opt.UseInMemoryDatabase("TodoDbList"));
// Add services to the container.
var app = builder.Build();


// Configure the HTTP request pipeline.
app.MapGet("/todoitems", async ( TodoDb db) => await db.TodoItems.ToListAsync());

app.MapGet("/todoitems/{id}", async (int id, TodoDb db) => await db.TodoItems.FindAsync(id));
app.MapPost("/todoitems", async (TodoItem todoItem, TodoDb db) =>
{
    db.TodoItems.Add(todoItem);
    await db.SaveChangesAsync();
    return Results.Created($"/todoitems/{todoItem.Id}", todoItem);
});
app.MapPut("/todoitems/{id}", async (int id, TodoItem todoItem, TodoDb db) =>
{
    var todo = await db.TodoItems.FindAsync(id);
    if (todo is null) return Results.NotFound();
    todo.Name = todoItem.Name;
    todo.IsComplete = todoItem.IsComplete;
    await db.SaveChangesAsync();
    return Results.NoContent();

});
app.MapDelete("/todoitems/{id}", async (int id, TodoDb db) =>
{
    if (await db.TodoItems.FindAsync(id) is not TodoItem todoItem) return Results.NotFound();
    db.TodoItems.Remove(todoItem);
    await db.SaveChangesAsync();
    return Results.NoContent();

});

app.Run();

