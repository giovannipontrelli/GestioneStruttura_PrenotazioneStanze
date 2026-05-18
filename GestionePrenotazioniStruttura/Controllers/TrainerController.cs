using GestionePrenotazioniStruttura.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class TrainerController : ControllerBase
{
    private readonly ITrainerService _service;

    public TrainerController(ITrainerService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<List<TrainerReadDto>>> GetAll()
    {
        var trainers = await _service.GetAllAsync();
        return Ok(trainers);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TrainerReadDto>> GetById(int id)
    {
        var trainer = await _service.GetByIdAsync(id);
        if (trainer == null) return NotFound();
        return Ok(trainer);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<TrainerReadDto>> Create(TrainerCreateDto dto)
    {
        var created = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> Update(int id, TrainerUpdateDto dto)
    {
        var updated = await _service.UpdateAsync(dto, id);
        if (!updated) return NotFound();
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> Delete(int id)
    {
        var deleted = await _service.DeleteAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }
}
