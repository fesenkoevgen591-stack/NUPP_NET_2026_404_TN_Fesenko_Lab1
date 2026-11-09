using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SmartHome.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
var databasePath = Path.GetFullPath(Path.Combine(builder.Environment.ContentRootPath, "..", "SmartHome.Lab4.db"));
var connectionString = builder.Configuration.GetConnectionString("SmartHome")
    ?? new SqliteConnectionStringBuilder
    {
        DataSource = databasePath,
        ForeignKeys = true
    }.ToString();

builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<SmartHomeContext>(options => options.UseSqlite(connectionString));

var app = builder.Build();

await using (var scope = app.Services.CreateAsyncScope())
{
    var context = scope.ServiceProvider.GetRequiredService<SmartHomeContext>();
    await context.Database.MigrateAsync();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Rooms}/{action=Index}/{id?}");

app.Run();
