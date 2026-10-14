
using System.Collections;
using System.Collections.Concurrent;
using System.Text.Json;

await SmartHomeDemo.RunAsync();


var path = Path.Combine(AppContext.BaseDirectory, "buses.json");
var service = new CrudServiceAsync<Bus>(path);
Console.WriteLine("LAB 2: Async CRUD, Parallel, LINQ");
Parallel.For(0, 1000, _ => service.CreateAsync(Bus.CreateNew()).GetAwaiter().GetResult());
Console.WriteLine($"Created: {service.Count}");
var all = (await service.ReadAllAsync()).ToList();
Console.WriteLine($"Passengers: min={all.Min(x => x.Passengers)}, max={all.Max(x => x.Passengers)}, avg={all.Average(x => x.Passengers):F2}");
Console.WriteLine($"Year: min={all.Min(x => x.Year)}, max={all.Max(x => x.Year)}, avg={all.Average(x => x.Year):F2}");
Console.WriteLine($"Mileage: min={all.Min(x => x.Mileage)}, max={all.Max(x => x.Mileage)}, avg={all.Average(x => x.Mileage):F2}");
Console.WriteLine($"Page 2 (20 per page): {(await service.ReadAllAsync(2,20)).Count()}");
var first = all[0];
Console.WriteLine($"Read: {(await service.ReadAsync(first.Id)) is not null}");
first.Passengers = 55;
Console.WriteLine($"Update: {await service.UpdateAsync(first)}");
Console.WriteLine($"Remove: {await service.RemoveAsync(first)}");
Console.WriteLine($"Save: {await service.SaveAsync()} => {path}");
await SynchronizationExamples.RunAsync();

public interface IHasId { Guid Id { get; } }
public interface ICrudServiceAsync<T> : IEnumerable<T> where T : IHasId
{
    Task<bool> CreateAsync(T element);
    Task<T?> ReadAsync(Guid id);
    Task<IEnumerable<T>> ReadAllAsync();
    Task<IEnumerable<T>> ReadAllAsync(int page, int amount);
    Task<bool> UpdateAsync(T element);
    Task<bool> RemoveAsync(T element);
    Task<bool> SaveAsync();
}
public sealed class Bus : IHasId
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public int Passengers { get; set; }
    public int Year { get; set; }
    public double Mileage { get; set; }
    public static Bus CreateNew() => new()
    {
        Passengers = Random.Shared.Next(10, 81),
        Year = Random.Shared.Next(2000, 2027),
        Mileage = Math.Round(Random.Shared.NextDouble() * 900000, 2)
    };
}
public sealed class CrudServiceAsync<T> : ICrudServiceAsync<T> where T : class, IHasId
{
    private readonly ConcurrentDictionary<Guid,T> _items = new();
    private readonly SemaphoreSlim _saveGate = new(1,1);
    private readonly string _filePath;
    public int Count => _items.Count;
    public CrudServiceAsync(string filePath) => _filePath = filePath;
    public Task<bool> CreateAsync(T element) => Task.FromResult(_items.TryAdd(element.Id, element));
    public Task<T?> ReadAsync(Guid id) => Task.FromResult(_items.TryGetValue(id, out var value) ? value : null);
    public Task<IEnumerable<T>> ReadAllAsync() => Task.FromResult<IEnumerable<T>>(_items.Values.ToArray());
    public Task<IEnumerable<T>> ReadAllAsync(int page, int amount)
    {
        if(page < 1 || amount < 1) throw new ArgumentOutOfRangeException(nameof(page), "Page and amount must be positive");
        return Task.FromResult<IEnumerable<T>>(_items.Values.OrderBy(x => x.Id).Skip((page-1)*amount).Take(amount).ToArray());
    }
    public Task<bool> UpdateAsync(T element)
    {
        while (_items.TryGetValue(element.Id, out var old))
            if (_items.TryUpdate(element.Id, element, old)) return Task.FromResult(true);
        return Task.FromResult(false);
    }
    public Task<bool> RemoveAsync(T element) => Task.FromResult(_items.TryRemove(element.Id, out _));
    public async Task<bool> SaveAsync()
    {
        await _saveGate.WaitAsync();
        try
        {
            var folder = Path.GetDirectoryName(Path.GetFullPath(_filePath))!;
            Directory.CreateDirectory(folder);
            var temp = _filePath + ".tmp";
            await using(var stream = new FileStream(temp, FileMode.Create, FileAccess.Write, FileShare.None, 81920, true))
                await JsonSerializer.SerializeAsync(stream, _items.Values.ToArray(), new JsonSerializerOptions { WriteIndented = true });
            File.Move(temp, _filePath, true);
            return true;
        }
        catch (IOException ex) { Console.Error.WriteLine(ex.Message); return false; }
        catch (UnauthorizedAccessException ex) { Console.Error.WriteLine(ex.Message); return false; }
        finally { _saveGate.Release(); }
    }
    public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>)_items.Values.ToArray()).GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
public static class SynchronizationExamples
{
    public static async Task RunAsync()
    {
        int counter = 0;
        object gate = new();
        Parallel.For(0, 1000, _ => { lock(gate) counter++; });
        Console.WriteLine($"lock counter: {counter}");
        using var semaphore = new SemaphoreSlim(2,2);
        int active = 0, peak = 0;
        await Task.WhenAll(Enumerable.Range(0,10).Select(async _ => {
            await semaphore.WaitAsync();
            try { var current = Interlocked.Increment(ref active); if(current > peak) Interlocked.Exchange(ref peak,current); await Task.Delay(5); }
            finally { Interlocked.Decrement(ref active); semaphore.Release(); }
        }));
        Console.WriteLine($"SemaphoreSlim peak <= 2: {peak}");
        using var signal = new AutoResetEvent(false);
        var waiter = Task.Run(() => signal.WaitOne(TimeSpan.FromSeconds(5)));
        signal.Set();
        Console.WriteLine($"AutoResetEvent signaled: {await waiter}");
    }
}
