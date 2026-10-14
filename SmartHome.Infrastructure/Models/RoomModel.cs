using SmartHome.Common;

namespace SmartHome.Infrastructure.Models;

public class RoomModel : IEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = "";
    public int Floor { get; set; }

    public List<DeviceModel> Devices { get; set; } = new();
}

