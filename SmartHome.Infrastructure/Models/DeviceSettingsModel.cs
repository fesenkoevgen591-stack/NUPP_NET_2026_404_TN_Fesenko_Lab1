using SmartHome.Common;

namespace SmartHome.Infrastructure.Models;

public class DeviceSettingsModel : IEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid DeviceId { get; set; }
    public DeviceModel Device { get; set; } = null!;

    public int Brightness { get; set; }
    public string Color { get; set; } = "";
    public double PowerWatts { get; set; }
}

