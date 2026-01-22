using Microsoft.EntityFrameworkCore;

namespace taskmanager.routes;

public static class TaskModule
{
    public static void AddTaskEndpoints(this IEndpointRouteBuilder app)
    {
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
    }
}
