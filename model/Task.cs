namespace taskmanager;

public class Task
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public long WorkboardId { get; set; }
    public Workboard Workboard { get; set; } = null!;
}
