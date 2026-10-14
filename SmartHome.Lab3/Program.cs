
using Microsoft.EntityFrameworkCore;
using SmartHome.Infrastructure;
using SmartHome.Infrastructure.Models;

Console.WriteLine("=== LAB 3: ENTITY FRAMEWORK CORE ===");

string dbPath = Path.GetFullPath(
    "SmartHome.Infrastructure/SmartHome.db");

var options = new DbContextOptionsBuilder<SmartHomeContext>()
    .UseSqlite($"Data Source={dbPath}")
    .Options;

await using var context = new SmartHomeContext(options);

// Перевірка та створення таблиць за міграціями
await context.Database.MigrateAsync();

var roomService = new EfCrudService<RoomModel>(
    new Repository<RoomModel>(context));

var deviceService = new EfCrudService<DeviceModel>(
    new Repository<DeviceModel>(context));

// CREATE
var room = new RoomModel
{
    Name = "Living Room",
    Floor = 1
};

await roomService.CreateAsync(room);
Console.WriteLine("CREATE Room: OK");

var device = new DeviceModel
{
    Name = "Smart Lamp",
    IsOnline = true,
    RoomId = room.Id
};

await deviceService.CreateAsync(device);
Console.WriteLine("CREATE Device: OK");

// READ
var found = await deviceService.ReadAsync(device.Id);
Console.WriteLine(
    $"READ Device: {found?.Name}, Online: {found?.IsOnline}");

// UPDATE
device.Name = "Updated Smart Lamp";
await deviceService.UpdateAsync(device);
Console.WriteLine("UPDATE Device: OK");

// READ ALL
var devices = await deviceService.ReadAllAsync();
Console.WriteLine($"DEVICES IN DATABASE: {devices.Count()}");

// DELETE
await deviceService.RemoveAsync(device);
Console.WriteLine("DELETE Device: OK");

// Перевірка видалення

bool isDeleted = false;

try
{
    await deviceService.ReadAsync(device.Id);
}
catch (KeyNotFoundException)
{
    isDeleted = true;
}

Console.WriteLine($"DEVICE DELETED: {isDeleted}");


// Додаємо пристрій для перегляду в SQLite
var savedDevice = new DeviceModel
{
    Name = "Camera",
    IsOnline = true,
    RoomId = room.Id
};

await deviceService.CreateAsync(savedDevice);
Console.WriteLine("SAVED Device: Camera");

Console.WriteLine($"DATABASE: {dbPath}");
Console.WriteLine("LAB 3 COMPLETED");


Console.WriteLine("\n=== DATABASE RELATIONSHIPS ===");

// Окремий контекст для роботи зі зв'язками
await using var relationContext = new SmartHomeContext(options);

// Знаходимо камеру в базі даних
var camera = await relationContext.Set<DeviceModel>()
    .Include(d => d.Settings)
    .Include(d => d.Tags)
    .FirstAsync(d => d.Id == savedDevice.Id);

// Зв'язок 1:1
if (camera.Settings == null)
{
    relationContext.Set<DeviceSettingsModel>().Add(
        new DeviceSettingsModel
        {
            DeviceId = camera.Id,
            Brightness = 80,
            Color = "White",
            PowerWatts = 12
        });
}

// Зв'язок N:M
foreach (var tagName in new[] { "Security", "Smart Home" })
{
    var tag = await relationContext.Set<TagModel>()
        .FirstOrDefaultAsync(t => t.Name == tagName);

    if (tag == null)
    {
        tag = new TagModel { Name = tagName };
        relationContext.Set<TagModel>().Add(tag);
    }

    if (!camera.Tags.Any(t => t.Name == tagName))
    {
        camera.Tags.Add(tag);
    }
}

await relationContext.SaveChangesAsync();

Console.WriteLine("1:1 Device -> DeviceSettings: OK");
Console.WriteLine("N:M Devices <-> Tags: OK");
Console.WriteLine("DATABASE RELATIONSHIPS SAVED");
