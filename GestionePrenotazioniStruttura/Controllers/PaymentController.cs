using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class PaymentController : ControllerBase
{
    private readonly IPaymentService _service;
    public PaymentController(IPaymentService service, IAuthorizationService @object) => _service = service;

    [HttpGet]
    public async Task<ActionResult<List<ReadPaymentDto>>> GetAll() =>
        Ok(await _service.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<ActionResult<ReadPaymentDto>> GetById(int id)
    {
        var payment = await _service.GetByIdAsync(id);
        if (payment == null) return NotFound();
        return Ok(payment);
    }

    [HttpPost]
    public async Task<ActionResult<ReadPaymentDto>> Create(CreatePaymentDto dto)
    {
        var created = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> Update(int id, UpdatePaymentDto dto)
    {
        var result = await _service.UpdateAsync(dto, id);
        if (!result) return NotFound();
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> Delete(int id)
    {
        var result = await _service.DeleteAsync(id);
        if (!result) return NotFound();
        return NoContent();
    }
}
