using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SmartHome.Common;
using SmartHome.Infrastructure.Models;
using SmartHome.REST.Models;

namespace SmartHome.REST.Controllers;

[ApiController]
[Route("api/devices")]
public sealed class DevicesController(
    ICrudServiceAsync<DeviceModel> devices,
    ICrudServiceAsync<RoomModel> rooms) : ControllerBase
{
    private const int SqliteForeignKeyConstraint = 787;

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<DeviceResponse>>> GetAll([FromQuery] PaginationQuery query)
    {
        var items = query.Page is int page && query.Amount is int amount
            ? await devices.ReadAllAsync(page, amount)
            : await devices.ReadAllAsync();

        return Ok(items.Select(ToResponse));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DeviceResponse>> GetById(Guid id)
    {
        var device = await FindDeviceAsync(id);
        return device is null ? NotFound() : Ok(ToResponse(device));
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<DeviceResponse>> Create(DeviceRequest request)
    {
        if (!await RoomExistsAsync(request.RoomId))
        {
            return InvalidRoom();
        }

        var device = new DeviceModel
        {
            Name = request.Name.Trim(),
            IsOnline = request.IsOnline,
            RoomId = request.RoomId
        };

        try
        {
            if (!await devices.CreateAsync(device))
            {
                return Conflict();
            }
        }
        catch (DbUpdateException exception) when (IsForeignKeyViolation(exception))
        {
            return InvalidRoom();
        }

        return CreatedAtAction(nameof(GetById), new { id = device.Id }, ToResponse(device));
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, DeviceRequest request)
    {
        var device = await FindDeviceAsync(id);
        if (device is null)
        {
            return NotFound();
        }

        if (!await RoomExistsAsync(request.RoomId))
        {
            return InvalidRoom();
        }

        device.Name = request.Name.Trim();
        device.IsOnline = request.IsOnline;
        device.RoomId = request.RoomId;

        try
        {
            return await devices.UpdateAsync(device) ? NoContent() : NotFound();
        }
        catch (DbUpdateConcurrencyException)
        {
            return NotFound();
        }
        catch (DbUpdateException exception) when (IsForeignKeyViolation(exception))
        {
            return InvalidRoom();
        }
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var device = await FindDeviceAsync(id);
        if (device is null)
        {
            return NotFound();
        }

        try
        {
            return await devices.RemoveAsync(device) ? NoContent() : NotFound();
        }
        catch (DbUpdateConcurrencyException)
        {
            return NotFound();
        }
    }

    private async Task<DeviceModel?> FindDeviceAsync(Guid id)
    {
        try
        {
            return await devices.ReadAsync(id);
        }
        catch (KeyNotFoundException)
        {
            return null;
        }
    }

    private async Task<bool> RoomExistsAsync(Guid id)
    {
        try
        {
            await rooms.ReadAsync(id);
            return true;
        }
        catch (KeyNotFoundException)
        {
            return false;
        }
    }

    private ActionResult InvalidRoom()
    {
        ModelState.AddModelError("roomId", "The room does not exist.");
        return ValidationProblem(ModelState);
    }

    private static bool IsForeignKeyViolation(DbUpdateException exception) =>
        exception.InnerException is SqliteException { SqliteExtendedErrorCode: SqliteForeignKeyConstraint };

    private static DeviceResponse ToResponse(DeviceModel device) =>
        new(device.Id, device.Name, device.IsOnline, device.RoomId);
}
