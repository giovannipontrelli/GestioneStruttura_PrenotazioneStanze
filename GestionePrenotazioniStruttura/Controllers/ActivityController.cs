using GestionePrenotazioniStruttura.Models;
using GestionePrenotazioniStruttura.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ActivityController : ControllerBase
{
    private readonly IActivityService _service;
    private readonly IAuthorizationService _auth;

    public ActivityController(IActivityService service, IAuthorizationService auth)
    {
        _service = service;
        _auth = auth;
    }

    [HttpGet]
    public async Task<ActionResult<List<ActivityReadDto>>> GetAll()
    {
        var activities = await _service.GetAllAsync();
        return Ok(activities);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ActivityReadDto>> GetById(int id)
    {
        var activity = await _service.GetByIdAsync(id);
        if (activity == null) return NotFound();
        return Ok(activity);
    }

    [HttpPost]
    public async Task<ActionResult<ActivityReadDto>> Create(ActivityCreateDto dto)
    {
        
        var created = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, ActivityUpdateDto dto)
    {
        var authorized = await _auth.AuthorizeAsync(User, id, "StrutturaScope");
        if (!authorized.Succeeded) return Forbid();

        var updated = await _service.UpdateAsync(dto, id);
        if (!updated) return NotFound();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var authorized = await _auth.AuthorizeAsync(User, id, "StrutturaScope");
        if (!authorized.Succeeded) return Forbid();

        var deleted = await _service.DeleteAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }
}
