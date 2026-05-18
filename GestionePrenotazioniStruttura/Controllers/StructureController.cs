using GestionePrenotazioniStruttura.Models;
using GestionePrenotazioniStruttura.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class StructuresController : ControllerBase
{
    private readonly IStructureService _structureService;

    public StructuresController(IStructureService structureService)
    {
        _structureService = structureService;
    }

    [HttpGet]
    public async Task<ActionResult<List<StructureDto>>> GetAll()
    {
        var structures = await _structureService.GetAllAsync();
        return Ok(structures.Select(s => new StructureDto
        {
            Id = s.Id,
            Name = s.Name,
            Address = s.Address,
            Description = s.Description
        }).ToList());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<StructureDto>> GetById(int id)
    {
        var s = await _structureService.GetByIdAsync(id);
        if (s == null) return NotFound();
        return Ok(new StructureDto
        {
            Id = s.Id,
            Name = s.Name,
            Address = s.Address,
            Description = s.Description
        });
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<StructureDto>> Create(CreateStructureDto dto)
    {
        var s = new Structure
        {
            Name = dto.Name,
            Address = dto.Address,
            Description = dto.Description
        };
        var created = await _structureService.CreateAsync(s);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, new StructureDto
        {
            Id = created.Id,
            Name = created.Name,
            Address = created.Address,
            Description = created.Description
        });
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> Update(int id, UpdateStructureDto dto)
    {
        var s = new Structure
        {
            Id = id,
            Name = dto.Name,
            Address = dto.Address,
            Description = dto.Description
        };
        var updated = await _structureService.UpdateAsync(s);
        if (!updated) return NotFound();
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> Delete(int id)
    {
        var deleted = await _structureService.DeleteAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }
}
