using System.ComponentModel.DataAnnotations;

namespace SmartHome.REST.Models;

public sealed class DeviceRequest
{
    [Required]
    public required string Name { get; init; }

    public required bool IsOnline { get; init; }

    public required Guid RoomId { get; init; }
}

public sealed record DeviceResponse(Guid Id, string Name, bool IsOnline, Guid RoomId);
