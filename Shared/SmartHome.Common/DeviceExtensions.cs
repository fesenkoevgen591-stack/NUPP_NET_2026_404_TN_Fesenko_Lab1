namespace SmartHome.Common;

public static class DeviceExtensions
{
    public static string GetStatusText(this Device device)
    {
        return device.IsOnline
            ? "Пристрій увімкнений"
            : "Пристрій вимкнений";
    }
}