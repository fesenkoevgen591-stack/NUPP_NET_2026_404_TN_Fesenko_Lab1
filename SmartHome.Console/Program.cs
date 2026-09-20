using SmartHome.Common;

Console.WriteLine("=== SMART HOME ===");

var lamp1 = new Lamp(
    "Лампа кухня",
    80,
    "Теплий",
    12);

var lamp2 = new Lamp(
    "Лампа спальня",
    60,
    "Білий",
    10);

var camera = new Camera(
    "Камера",
    "1920x1080",
    true,
    128);

var thermostat = new Thermostat(
    "Термостат",
    21,
    23,
    "Auto");

var room = new Room("Кухня", 1);

room.AddDevice(lamp1);
room.AddDevice(camera);
room.AddDevice(thermostat);

// Подія
lamp1.StatusChanged += (device, status) =>
{
    Console.WriteLine($"Подія: {device.Name} змінив стан на {status}");
};

lamp1.SetOnline(true);

// Метод розширення
Console.WriteLine(lamp1.GetStatusText());

// CRUD
CrudService<Lamp> service = new();

service.Create(lamp1);
service.Create(lamp2);

Console.WriteLine("\n--- Усі лампи ---");

foreach (var lamp in service.ReadAll())
{
    Console.WriteLine(lamp.GetInfo());
}

// READ
var found = service.Read(lamp1.Id);

Console.WriteLine("\nЗнайдено:");
Console.WriteLine(found.GetInfo());

// UPDATE
lamp1.Brightness = 100;
service.Update(lamp1);

// REMOVE
service.Remove(lamp2);

// SAVE
service.Save("lamps.json");

Console.WriteLine("\n--- Після змін ---");

foreach (var lamp in service.ReadAll())
{
    Console.WriteLine(lamp.GetInfo());
}

Console.WriteLine(
    $"\nСтворено пристроїв: {Device.GetDeviceCount()}");