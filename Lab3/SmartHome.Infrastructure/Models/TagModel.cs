using SmartHome.Common;

namespace SmartHome.Infrastructure.Models;

public class TagModel : IEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = "";

    public List<DeviceModel> Devices { get; set; } = new();
}

