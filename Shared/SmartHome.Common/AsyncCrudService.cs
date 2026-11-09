
using System.Collections;
using System.Collections.Concurrent;
using System.Text.Json;

namespace SmartHome.Common;

public interface ICrudServiceAsync<T> : IEnumerable<T>
{
    Task<bool> CreateAsync(T element);
    Task<T> ReadAsync(Guid id);
    Task<IEnumerable<T>> ReadAllAsync();
    Task<IEnumerable<T>> ReadAllAsync(int page, int amount);
    Task<bool> UpdateAsync(T element);
    Task<bool> RemoveAsync(T element);
    Task<bool> SaveAsync();
}

public class AsyncCrudService<T> : ICrudServiceAsync<T>
    where T : IEntity
{
    private readonly ConcurrentDictionary<Guid, T> _items = new();
    private readonly SemaphoreSlim _fileLock = new(1, 1);

    public string FilePath { get; }

    public AsyncCrudService(string filePath)
    {
        FilePath = filePath;
    }

    public Task<bool> CreateAsync(T element)
    {
        return Task.FromResult(
            _items.TryAdd(element.Id, element));
    }

    public Task<T> ReadAsync(Guid id)
    {
        if (_items.TryGetValue(id, out var element))
            return Task.FromResult(element);

        throw new KeyNotFoundException(
            $"Element with ID {id} not found.");
    }

    public Task<IEnumerable<T>> ReadAllAsync()
    {
        IEnumerable<T> result = _items.Values.ToArray();
        return Task.FromResult(result);
    }

    public Task<IEnumerable<T>> ReadAllAsync(int page, int amount)
    {
        if (page < 1 || amount < 1)
            throw new ArgumentOutOfRangeException(
                nameof(page), "Page and amount must be positive.");

        IEnumerable<T> result = _items.Values
            .OrderBy(x => x.Id)
            .Skip((page - 1) * amount)
            .Take(amount)
            .ToArray();

        return Task.FromResult(result);
    }

    public Task<bool> UpdateAsync(T element)
    {
        while (_items.TryGetValue(element.Id, out var old))
        {
            if (_items.TryUpdate(element.Id, element, old))
                return Task.FromResult(true);
        }

        return Task.FromResult(false);
    }

    public Task<bool> RemoveAsync(T element)
    {
        return Task.FromResult(
            _items.TryRemove(element.Id, out _));
    }

    public async Task<bool> SaveAsync()
    {
        await _fileLock.WaitAsync();

        try
        {
            string? directory = Path.GetDirectoryName(
                Path.GetFullPath(FilePath));

            if (directory != null)
                Directory.CreateDirectory(directory);

            await using var stream = new FileStream(
                FilePath,
                FileMode.Create,
                FileAccess.Write,
                FileShare.None,
                4096,
                useAsync: true);

            await JsonSerializer.SerializeAsync(
                stream, _items.Values.ToArray(),
                new JsonSerializerOptions
                {
                    WriteIndented = true
                });

            return true;
        }
        catch (IOException)
        {
            return false;
        }
        finally
        {
            _fileLock.Release();
        }
    }

    public IEnumerator<T> GetEnumerator()
    {
        return _items.Values.ToArray()
            .AsEnumerable().GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
