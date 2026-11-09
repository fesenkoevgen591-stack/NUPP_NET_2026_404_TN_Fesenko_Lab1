using System.ComponentModel.DataAnnotations;

namespace SmartHome.REST.Models;

public sealed class RoomRequest
{
    [Required]
    public required string Name { get; init; }

    public required int Floor { get; init; }
}

public sealed record RoomResponse(Guid Id, string Name, int Floor);
