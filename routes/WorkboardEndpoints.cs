using Microsoft.EntityFrameworkCore;

namespace taskmanager.routes;

public static class WorkboardModule
{
    public static void AddWorkboardEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet(
            "/workboards",
            async (TaskmangerDb db) =>
            {
                List<WorkboardResponse> boards = await db
                    .Workboards.Select(w => new WorkboardResponse(
                        w.Id,
                        w.Name,
                        w.Tasks.Select(t => new TaskSummary(t.Id, t.Name, t.Description)).ToList()
                    ))
                    .ToListAsync();
                return boards;
            }
        );
    }
}
