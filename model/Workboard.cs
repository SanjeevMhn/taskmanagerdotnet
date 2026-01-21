using System.ComponentModel.DataAnnotations;

namespace taskmanager;

public class Workboard
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public List<Task> Tasks { get; set; } = [];
}
