
using SmartHome.Common;
using Xunit;

namespace SmartHome.Tests;

public class AsyncCrudServiceTests
{
    private static AsyncCrudService<Lamp> CreateService()
    {
        string path = Path.Combine(
            Path.GetTempPath(),
            $"lamps_test_{Guid.NewGuid()}.json");

        return new AsyncCrudService<Lamp>(path);
    }

    [Fact]
    public async Task CreateAsync_AddsLamp()
    {
        var service = CreateService();
        var lamp = new Lamp("Test", 50, "White", 20);

        bool result = await service.CreateAsync(lamp);

        Assert.True(result);
        Assert.Single(service);
    }

    [Fact]
    public async Task CreateAsync_DuplicateId_ReturnsFalse()
    {
        var service = CreateService();
        var lamp = new Lamp("Test", 50, "White", 20);

        await service.CreateAsync(lamp);
        bool result = await service.CreateAsync(lamp);

        Assert.False(result);
        Assert.Single(service);
    }

    [Fact]
    public async Task ReadAsync_ReturnsCorrectLamp()
    {
        var service = CreateService();
        var lamp = new Lamp("Bedroom", 70, "Blue", 30);

        await service.CreateAsync(lamp);
        var result = await service.ReadAsync(lamp.Id);

        Assert.Equal(lamp.Id, result.Id);
        Assert.Equal("Bedroom", result.Name);
    }

    [Fact]
    public async Task UpdateAsync_ChangesLamp()
    {
        var service = CreateService();
        var lamp = new Lamp("Old", 30, "White", 10);

        await service.CreateAsync(lamp);

        var updated = new Lamp("New", 90, "Red", 50)
        {
            Id = lamp.Id
        };

        bool result = await service.UpdateAsync(updated);
        var found = await service.ReadAsync(lamp.Id);

        Assert.True(result);
        Assert.Equal(90, found.Brightness);
        Assert.Equal("New", found.Name);
    }

    [Fact]
    public async Task RemoveAsync_DeletesLamp()
    {
        var service = CreateService();
        var lamp = new Lamp("Test", 50, "White", 20);

        await service.CreateAsync(lamp);
        bool result = await service.RemoveAsync(lamp);

        Assert.True(result);
        Assert.Empty(service);
    }

    [Fact]
    public async Task Pagination_ReturnsCorrectAmount()
    {
        var service = CreateService();

        for (int i = 0; i < 50; i++)
        {
            await service.CreateAsync(
                new Lamp($"Lamp{i}", 50, "White", 20));
        }

        var page = await service.ReadAllAsync(2, 10);

        Assert.Equal(10, page.Count());
    }

    [Fact]
    public async Task SaveAsync_CreatesJsonFile()
    {
        var service = CreateService();

        await service.CreateAsync(
            new Lamp("Test", 50, "White", 20));

        try
        {
            bool result = await service.SaveAsync();

            Assert.True(result);
            Assert.True(File.Exists(service.FilePath));

            string json = await File.ReadAllTextAsync(
                service.FilePath);

            Assert.Contains("Test", json);
        }
        finally
        {
            if (File.Exists(service.FilePath))
                File.Delete(service.FilePath);
        }
    }
}
