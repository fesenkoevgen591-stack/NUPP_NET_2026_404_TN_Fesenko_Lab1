
using SmartHome.Common;

public static class SmartHomeDemo
{
    public static async Task RunAsync()
    {
        Console.WriteLine("LAB 2: SmartHome Async CRUD");

        var service = new AsyncCrudService<Lamp>(
            Path.Combine(AppContext.BaseDirectory, "lamps_lab2.json"));

        // Create 1000 lamps in parallel
        Parallel.For(0, 1000, _ =>
        {
            Lamp lamp = LampFactory.CreateNew();
            service.CreateAsync(lamp).GetAwaiter().GetResult();
        });

        var lamps = (await service.ReadAllAsync()).ToList();

        Console.WriteLine($"Created: {lamps.Count}");

        // LINQ statistics
        Console.WriteLine(
            $"Brightness: min={lamps.Min(x => x.Brightness)}, " +
            $"max={lamps.Max(x => x.Brightness)}, " +
            $"avg={lamps.Average(x => x.Brightness):F2}");

        Console.WriteLine(
            $"Power: min={lamps.Min(x => x.PowerWatts):F2}, " +
            $"max={lamps.Max(x => x.PowerWatts):F2}, " +
            $"avg={lamps.Average(x => x.PowerWatts):F2}");

        // Pagination
        var page = await service.ReadAllAsync(2, 20);
        Console.WriteLine($"Page 2: {page.Count()}");

        // Read
        var first = lamps[0];
        var found = await service.ReadAsync(first.Id);
        Console.WriteLine($"Read: {found.Id == first.Id}");

        // Update
        var updated = new Lamp(
            first.Name, 75, "Green", 25.5)
        {
            Id = first.Id
        };

        Console.WriteLine(
            $"Update: {await service.UpdateAsync(updated)}");

        // Remove
        Console.WriteLine(
            $"Remove: {await service.RemoveAsync(updated)}");

        // Save to JSON
        Console.WriteLine(
            $"Save: {await service.SaveAsync()}");

        Console.WriteLine(
            $"Remaining objects: {service.Count()}");
    }
}
