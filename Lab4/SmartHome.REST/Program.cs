using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SmartHome.Common;
using SmartHome.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
builder.Services.AddProblemDetails();

var databasePath = Path.GetFullPath(
    Path.Combine(builder.Environment.ContentRootPath, "..", "SmartHome.Lab4.db"));
var connectionString = builder.Configuration.GetConnectionString("SmartHome")
    ?? new SqliteConnectionStringBuilder { DataSource = databasePath, ForeignKeys = true }.ToString();
builder.Services.AddDbContext<SmartHomeContext>(options => options.UseSqlite(connectionString));
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped(typeof(ICrudServiceAsync<>), typeof(EfCrudService<>));

var app = builder.Build();

await using (var scope = app.Services.CreateAsyncScope())
{
    var context = scope.ServiceProvider.GetRequiredService<SmartHomeContext>();
    await context.Database.MigrateAsync();
}

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();
