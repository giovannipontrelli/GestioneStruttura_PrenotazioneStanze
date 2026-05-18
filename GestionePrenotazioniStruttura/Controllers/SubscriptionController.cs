using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class SubscriptionController : ControllerBase
{
    private readonly ISubscriptionService _service;

    public SubscriptionController(ISubscriptionService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<List<ReadSubscriptionDto>>> GetAll()
        => Ok(await _service.GetAllSubscriptionsAsync());

    [HttpGet("{id}")]
    public async Task<ActionResult<ReadSubscriptionDto>> GetById(int id)
    {
        var subscription = await _service.GetSubscriptionByIdAsync(id);
        if (subscription == null) return NotFound();
        return Ok(subscription);
    }

    [HttpPost]
    public async Task<ActionResult<ReadSubscriptionDto>> Create(CreateSubscriptionDto dto)
    {
        var created = await _service.CreateSubscriptionAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, UpdateSubscriptionDto dto)
    {
        var updated = await _service.UpdateSubscriptionAsync(id, dto);
        if (updated == null) return NotFound();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var deleted = await _service.DeleteSubscriptionAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }
}
