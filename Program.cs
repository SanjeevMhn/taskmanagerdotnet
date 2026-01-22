using Microsoft.EntityFrameworkCore;
using taskmanager;
using taskmanager.routes;

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
    options.AddPolicy(
        "AllowSpecificOrigins",
        policy =>
        {
            policy
                .WithOrigins("http://localhost:4200", "http://localhost:4200/")
                .AllowAnyHeader()
                .AllowAnyMethod();
        }
    );
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
        config.DocExpansion = "list";
    });
}

app.AddWorkboardEndpoints();
app.AddTaskEndpoints();

app.Run();
