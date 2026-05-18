using GestionePrenotazioniStruttura.DTOs;
using GestionePrenotazioniStruttura.Models;
using GestionePrenotazioniStruttura.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestionePrenotazioniStruttura.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        private readonly IAuthorizationService _auth;

        public BookingsController(IBookingService bookingService, IAuthorizationService auth)
        {
            _bookingService = bookingService;
            _auth = auth;
        }

        [HttpGet]
        public async Task<ActionResult<List<BookingDto>>> GetAll()
        {
            var bookings = await _bookingService.GetAllAsync();
            return Ok(bookings.Select(b => new BookingDto
            {
                Id = b.Id,
                RoomId = b.RoomId,
                UserId = b.UserId,
                ActivityId = b.ActivityId,
                StartTime = b.StartTime,
                EndTime = b.EndTime
            }).ToList());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BookingDto>> GetById(int id)
        {
            var b = await _bookingService.GetByIdAsync(id);
            if (b == null) return NotFound();
            return Ok(new BookingDto
            {
                Id = b.Id,
                RoomId = b.RoomId,
                UserId = b.UserId,
                ActivityId = b.ActivityId,
                StartTime = b.StartTime,
                EndTime = b.EndTime
            });
        }

        [HttpGet("room/{roomId}")]
        public async Task<ActionResult<List<BookingDto>>> GetByRoom(int roomId)
        {
            var bookings = await _bookingService.GetByRoomAsync(roomId);
            return Ok(bookings.Select(b => new BookingDto
            {
                Id = b.Id,
                RoomId = b.RoomId,
                UserId = b.UserId,
                ActivityId = b.ActivityId,
                StartTime = b.StartTime,
                EndTime = b.EndTime
            }).ToList());
        }

        [HttpPost]
        public async Task<ActionResult<BookingDto>> Create(CreateBookingDto dto)
        {
            var booking = new Booking
            {
                RoomId = dto.RoomId,
                UserId = dto.UserId,
                ActivityId = dto.ActivityId,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime
            };

            var created = await _bookingService.CreateAsync(booking);
            if (created == null) return BadRequest("Prenotazione sovrapposta.");

            return CreatedAtAction(nameof(GetById), new { id = created.Id }, new BookingDto
            {
                Id = created.Id,
                RoomId = created.RoomId,
                UserId = created.UserId,
                ActivityId = created.ActivityId,
                StartTime = created.StartTime,
                EndTime = created.EndTime
            });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(int id)
        {
            var auth = await _auth.AuthorizeAsync(User, id, "StrutturaScope");
            if (!auth.Succeeded) return Forbid();

            var result = await _bookingService.DeleteAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }
        [HttpPut("{id}")]
        public async Task<ActionResult<BookingDto>> Update(int id, UpdateBookingDto dto)
        {
            var auth = await _auth.AuthorizeAsync(User, id, "StrutturaScope");
            if (!auth.Succeeded) return Forbid();

            var booking = new Booking
            {
                Id = id,
                RoomId = dto.RoomId,
                UserId = dto.UserId,
                ActivityId = dto.ActivityId,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime
            };

            var updated = await _bookingService.UpdateAsync(booking);
            if (updated == null) return BadRequest("Prenotazione sovrapposta.");

            return Ok(new BookingDto
            {
                Id = updated.Id,
                RoomId = updated.RoomId,
                UserId = updated.UserId,
                ActivityId = updated.ActivityId,
                StartTime = updated.StartTime,
                EndTime = updated.EndTime
            });
        }
    }
}
