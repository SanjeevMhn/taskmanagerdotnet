using Microsoft.EntityFrameworkCore;
using taskmanager;
using Task = taskmanager.Task;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<TaskmangerDb>(options => options.UseNpgsql(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument(config =>
{
    config.DocumentName = "TaskManagerAPI";
    config.Title = "TaskManager API v1";
    config.Version = "v1";
});

builder.Services.AddCors(options => 
{
    options.AddPolicy("AllowSpecificOrigins", policy => 
    {
        policy.WithOrigins("http://localhost:4200", "http://localhost:4200/")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseCors("AllowSpecificOrigins");

if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi(config =>
    {
        config.DocumentTitle = "TaskManagerAPI";
        config.Path = "/swagger";
        // config.DocumentPath = "/swagger/{documentPath}/swagger.json";
        config.DocExpansion = "list";
    });
}

app.MapGet(
    "/workboards",
    async (TaskmangerDb db) =>
    {
        var boards = await db
            .Workboards.Select(w => new WorkboardResponse(
                w.Id,
                w.Name,
                w.Tasks.Select(t => new TaskSummary(t.Id, t.Name, t.Description)).ToList()
            ))
            .ToListAsync();
        return boards;
    }
);

app.MapGet(
    "/tasks",
    async (TaskmangerDb db) =>
    {
        var tasks = await db
            .Tasks.Select(p => new
            {
                p.Id,
                p.Name,
                p.Description,
                WorkboardId = p.Workboard.Id,
            })
            .ToListAsync();
        return tasks;
    }
);

app.MapGet(
    "/task/{id}",
    async (long id, TaskmangerDb db) =>
    {
        var task = await db
            .Tasks.Where(t => t.Id == id)
            .Select(p => new
            {
                p.Id,
                p.Name,
                p.Description,
                WorkboardId = p.Workboard.Id,
            })
            .ToListAsync();

        return task;
    }
);

app.MapPut(
    "/task/{id}",
    async (long id, TaskDTO updatedTask, TaskmangerDb db) =>
    {
        var task = await db.Tasks.FindAsync(id);
        if (task is not null)
        {
            task.Name = updatedTask.Name;
            task.Description = updatedTask.Description;
            task.WorkboardId = updatedTask.WorkboardId;

            await db.SaveChangesAsync();
            return Results.NoContent();
        }

        return Results.NotFound();
    }
);

app.MapDelete(
    "task/{id}",
    async (long id, TaskmangerDb db) =>
    {
        var task = await db.Tasks.FindAsync(id);
        if (task is not null)
        {
            db.Tasks.Remove(task);
            await db.SaveChangesAsync();
            return Results.NoContent();
        }
        return Results.NotFound();
    }
);

app.MapPost(
    "/add-task",
    async (TaskDTO task, TaskmangerDb db) =>
    {
        var newTask = new Task
        {
            Name = task.Name,
            Description = task.Description,
            WorkboardId = task.WorkboardId,
        };
        db.Tasks.Add(newTask);
        await db.SaveChangesAsync();

        return Results.Created($"/task/{newTask.Id}", task);
    }
);

app.Run();
