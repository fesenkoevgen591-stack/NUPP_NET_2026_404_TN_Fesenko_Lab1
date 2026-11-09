using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartHome.Common;
using SmartHome.Infrastructure.Models;
using SmartHome.REST.Models;

namespace SmartHome.REST.Controllers;

[ApiController]
[Route("api/rooms")]
public sealed class RoomsController(ICrudServiceAsync<RoomModel> rooms) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<RoomResponse>>> GetAll([FromQuery] PaginationQuery query)
    {
        var items = query.Page is int page && query.Amount is int amount
            ? await rooms.ReadAllAsync(page, amount)
            : await rooms.ReadAllAsync();

        return Ok(items.Select(ToResponse));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RoomResponse>> GetById(Guid id)
    {
        var room = await FindRoomAsync(id);
        return room is null ? NotFound() : Ok(ToResponse(room));
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<RoomResponse>> Create(RoomRequest request)
    {
        var room = new RoomModel { Name = request.Name.Trim(), Floor = request.Floor };
        if (!await rooms.CreateAsync(room))
        {
            return Conflict();
        }

        return CreatedAtAction(nameof(GetById), new { id = room.Id }, ToResponse(room));
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, RoomRequest request)
    {
        var room = await FindRoomAsync(id);
        if (room is null)
        {
            return NotFound();
        }

        room.Name = request.Name.Trim();
        room.Floor = request.Floor;

        try
        {
            return await rooms.UpdateAsync(room) ? NoContent() : NotFound();
        }
        catch (DbUpdateConcurrencyException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var room = await FindRoomAsync(id);
        if (room is null)
        {
            return NotFound();
        }

        try
        {
            return await rooms.RemoveAsync(room) ? NoContent() : NotFound();
        }
        catch (DbUpdateConcurrencyException)
        {
            return NotFound();
        }
    }

    private async Task<RoomModel?> FindRoomAsync(Guid id)
    {
        try
        {
            return await rooms.ReadAsync(id);
        }
        catch (KeyNotFoundException)
        {
            return null;
        }
    }

    private static RoomResponse ToResponse(RoomModel room) =>
        new(room.Id, room.Name, room.Floor);
}
