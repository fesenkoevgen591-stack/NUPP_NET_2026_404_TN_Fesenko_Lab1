using SmartHome.Common;


namespace SmartHome.Infrastructure.Models;

public class DeviceModel : IEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = "";
    public bool IsOnline { get; set; }

    public Guid RoomId { get; set; }
    public RoomModel Room { get; set; } = null!;

    public DeviceSettingsModel? Settings { get; set; }

    public List<TagModel> Tags { get; set; } = new();
}
