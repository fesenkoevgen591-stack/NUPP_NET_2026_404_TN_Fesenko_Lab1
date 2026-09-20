using System.Text.Json;

namespace SmartHome.Common;

public interface ICrudService<T>
{
    void Create(T element);
    T Read(Guid id);
    IEnumerable<T> ReadAll();
    void Update(T element);
    void Remove(T element);

    void Save(string filePath);
    void Load(string filePath);
}

public class CrudService<T> : ICrudService<T> where T : IEntity
{
    private List<T> items = new();

    public void Create(T element)
    {
        items.Add(element);
    }

    public T Read(Guid id)
    {
        return items.First(x => x.Id == id);
    }

    public IEnumerable<T> ReadAll()
    {
        return items;
    }

    public void Update(T element)
    {
        int index = items.FindIndex(x => x.Id == element.Id);

        if (index != -1)
            items[index] = element;
    }

    public void Remove(T element)
    {
        items.Remove(element);
    }

    public void Save(string filePath)
    {
        string json = JsonSerializer.Serialize(
            items,
            new JsonSerializerOptions
            {
                WriteIndented = true
            });

        File.WriteAllText(filePath, json);
    }

    public void Load(string filePath)
    {
        if (!File.Exists(filePath))
            return;

        string json = File.ReadAllText(filePath);

        items = JsonSerializer.Deserialize<List<T>>(json)
                ?? new List<T>();
    }
}