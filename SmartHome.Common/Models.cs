namespace SmartHome.Common;

public interface IEntity
{
    Guid Id { get; set; }
}

public delegate void DeviceStatusChangedHandler(Device device, bool status);

public class Device : IEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public bool IsOnline { get; set; }

    public static int DeviceCount;

    static Device()
    {
        DeviceCount = 0;
    }

    public Device()
    {
        Id = Guid.NewGuid();
        Name = "";
        DeviceCount++;
    }

    public Device(string name)
    {
        Id = Guid.NewGuid();
        Name = name;
        IsOnline = false;
        DeviceCount++;
    }

    public event DeviceStatusChangedHandler? StatusChanged;

    public void SetOnline(bool status)
    {
        IsOnline = status;
        StatusChanged?.Invoke(this, status);
    }

    public virtual string GetInfo()
    {
        return $"{Name}, Online: {IsOnline}";
    }

    public static int GetDeviceCount()
    {
        return DeviceCount;
    }
}

public class Lamp : Device
{
    public int Brightness { get; set; }
    public string Color { get; set; }
    public double PowerWatts { get; set; }

    public Lamp() { }

    public Lamp(string name, int brightness, string color, double power)
        : base(name)
    {
        Brightness = brightness;
        Color = color;
        PowerWatts = power;
    }

    public override string GetInfo()
    {
        return $"{Name}: яскравість {Brightness}%, колір {Color}";
    }
}

public class Camera : Device
{
    public string Resolution { get; set; } = "";
    public bool NightVision { get; set; }
    public int StorageGb { get; set; }

    public Camera() { }

    public Camera(string name, string resolution, bool nightVision, int storage)
        : base(name)
    {
        Resolution = resolution;
        NightVision = nightVision;
        StorageGb = storage;
    }

    public override string GetInfo()
    {
        return $"{Name}: {Resolution}, пам'ять {StorageGb} GB";
    }
}

public class Thermostat : Device
{
    public double CurrentTemperature { get; set; }
    public double TargetTemperature { get; set; }
    public string Mode { get; set; } = "";

    public Thermostat() { }

    public Thermostat(string name, double currentTemperature,
        double targetTemperature, string mode) : base(name)
    {
        CurrentTemperature = currentTemperature;
        TargetTemperature = targetTemperature;
        Mode = mode;
    }

    public override string GetInfo()
    {
        return $"{Name}: {CurrentTemperature}°C -> {TargetTemperature}°C";
    }
}

public class Room : IEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public int Floor { get; set; }
    public List<Device> Devices { get; set; }

    public Room(string name, int floor)
    {
        Id = Guid.NewGuid();
        Name = name;
        Floor = floor;
        Devices = new List<Device>();
    }

    public void AddDevice(Device device)
    {
        Devices.Add(device);
    }
}