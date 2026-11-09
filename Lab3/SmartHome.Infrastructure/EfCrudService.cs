
using System.Collections;
using SmartHome.Common;

namespace SmartHome.Infrastructure;

public class EfCrudService<T> : ICrudServiceAsync<T>
    where T : class, IEntity
{
    private readonly IRepository<T> _repository;

    public EfCrudService(IRepository<T> repository)
    {
        _repository = repository;
    }

    public async Task<bool> CreateAsync(T element)
    {
        var existing = await _repository.GetByIdAsync(element.Id);

        if (existing != null)
            return false;

        await _repository.AddAsync(element);
        return true;
    }

    public async Task<T> ReadAsync(Guid id)
    {
        var element = await _repository.GetByIdAsync(id);

        return element ?? throw new KeyNotFoundException(
            $"Element with ID {id} not found.");
    }

    public async Task<IEnumerable<T>> ReadAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<IEnumerable<T>> ReadAllAsync(
        int page, int amount)
    {
        if (page < 1 || amount < 1)
            throw new ArgumentOutOfRangeException(
                nameof(page),
                "Page and amount must be positive.");

        var items = await _repository.GetAllAsync();

        return items
            .OrderBy(x => x.Id)
            .Skip((page - 1) * amount)
            .Take(amount)
            .ToArray();
    }

    public async Task<bool> UpdateAsync(T element)
    {
        var existing = await _repository.GetByIdAsync(element.Id);

        if (existing == null)
            return false;

        await _repository.UpdateAsync(element);
        return true;
    }

    public async Task<bool> RemoveAsync(T element)
    {
        var existing = await _repository.GetByIdAsync(element.Id);

        if (existing == null)
            return false;

        await _repository.DeleteAsync(element);
        return true;
    }

    public Task<bool> SaveAsync()
    {
        // Repository вже викликає SaveChangesAsync
        return Task.FromResult(true);
    }

    public IEnumerator<T> GetEnumerator()
    {
        return ReadAllAsync()
            .GetAwaiter()
            .GetResult()
            .GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
