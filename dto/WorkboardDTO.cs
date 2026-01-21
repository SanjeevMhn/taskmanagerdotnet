namespace taskmanager;

public record TaskSummary(long Id, string Name, string Description);

public record WorkboardResponse(long Id, string Name, List<TaskSummary> Tasks);
