using Microsoft.EntityFrameworkCore;

namespace taskmanager;

class TaskmangerDb(DbContextOptions<TaskmangerDb> options) : DbContext(options)
{
    public DbSet<Workboard> Workboards => Set<Workboard>();
    public DbSet<Task> Tasks => Set<Task>();
}
