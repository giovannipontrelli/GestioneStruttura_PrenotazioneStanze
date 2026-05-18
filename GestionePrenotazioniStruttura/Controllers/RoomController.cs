using GestionePrenotazioniStruttura.Models;
using GestionePrenotazioniStruttura.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class RoomsController : ControllerBase
{
    private readonly IRoomService _roomService;
    private readonly IAuthorizationService _auth;

    public RoomsController(IRoomService roomService, IAuthorizationService auth)
    {
        _roomService = roomService;
        _auth = auth;
    }

    [HttpGet]
    public async Task<ActionResult<List<RoomDto>>> GetAll()
    {
        var rooms = await _roomService.GetAllAsync();
        return Ok(rooms.Select(r => new RoomDto
        {
            Id = r.Id,
            Name = r.Name,
            Capacity = r.Capacity,
            StructureId = r.StructureId
        }).ToList());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<RoomDto>> GetById(int id)
    {
        var r = await _roomService.GetByIdAsync(id);
        if (r == null) return NotFound();
        return Ok(new RoomDto
        {
            Id = r.Id,
            Name = r.Name,
            Capacity = r.Capacity,
            StructureId = r.StructureId
        });
    }

    [HttpGet("structure/{structureId}")]
    public async Task<ActionResult<List<RoomDto>>> GetByStructure(int structureId)
    {
        var rooms = await _roomService.GetByStructureAsync(structureId);
        return Ok(rooms.Select(r => new RoomDto
        {
            Id = r.Id,
            Name = r.Name,
            Capacity = r.Capacity,
            StructureId = r.StructureId
        }).ToList());
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Revisor")]
    public async Task<ActionResult<RoomDto>> Create(CreateRoomDto dto)
    {
        var auth = await _auth.AuthorizeAsync(User, dto.StructureId, "StrutturaScope");
        if (!auth.Succeeded) return Forbid();

        var room = new Room
        {
            Name = dto.Name,
            Capacity = dto.Capacity,
            StructureId = dto.StructureId
        };
        var created = await _roomService.CreateAsync(room);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, new RoomDto
        {
            Id = created.Id,
            Name = created.Name,
            Capacity = created.Capacity,
            StructureId = created.StructureId
        });
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Revisor")]
    public async Task<ActionResult> Update(int id, UpdateRoomDto dto)
    {
        var auth = await _auth.AuthorizeAsync(User, id, "StrutturaScope");
        if (!auth.Succeeded) return Forbid();

        var room = new Room
        {
            Id = id,
            Name = dto.Name,
            Capacity = dto.Capacity,
            StructureId = dto.StructureId
        };
        var updated = await _roomService.UpdateAsync(room);
        if (!updated) return NotFound();
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,Revisor")]
    public async Task<ActionResult> Delete(int id)
    {
        var auth = await _auth.AuthorizeAsync(User, id, "StrutturaScope");
        if (!auth.Succeeded) return Forbid();

        var deleted = await _roomService.DeleteAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }
}
