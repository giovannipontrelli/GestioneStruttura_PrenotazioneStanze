using GestionePrenotazioniStruttura.Controllers;
using GestionePrenotazioniStruttura.DTOs;
using GestionePrenotazioniStruttura.Models;
using GestionePrenotazioniStruttura.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace GestionePrenotazioniStruttura.Tests.Controllers
{
    public class BookingsControllerTests
    {
        private readonly Mock<IBookingService> _bookingServiceMock;
        private readonly Mock<IAuthorizationService> _authMock;
        private readonly BookingsController _controller;

        public BookingsControllerTests()
        {
            _bookingServiceMock = new Mock<IBookingService>();
            _authMock = new Mock<IAuthorizationService>();
            _controller = new BookingsController(_bookingServiceMock.Object, _authMock.Object);
        }

        [Fact]
        public async Task GetAll_ReturnsListOfBookingDto()
        {
            _bookingServiceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<Booking>
            {
                new Booking { Id = 1, RoomId = 1, UserId = 1, StartTime = DateTime.UtcNow, EndTime = DateTime.UtcNow.AddHours(1) }
            });

            var result = await _controller.GetAll();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var bookings = Assert.IsType<List<BookingDto>>(okResult.Value);
            Assert.Single(bookings);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenBookingNull()
        {
            _bookingServiceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync((Booking?)null);

            var result = await _controller.GetById(1);

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task Create_ReturnsBadRequest_WhenOverlap()
        {
            _bookingServiceMock.Setup(s => s.CreateAsync(It.IsAny<Booking>())).ReturnsAsync((Booking?)null);

            var dto = new CreateBookingDto
            {
                RoomId = 1,
                UserId = 1,
                StartTime = DateTime.UtcNow,
                EndTime = DateTime.UtcNow.AddHours(1)
            };

            var result = await _controller.Create(dto);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Prenotazione sovrapposta.", badRequest.Value);
        }

        [Fact]
        public async Task Create_ReturnsCreated_WhenValid()
        {
            var booking = new Booking { Id = 1, RoomId = 1, UserId = 1, StartTime = DateTime.UtcNow, EndTime = DateTime.UtcNow.AddHours(1) };
            _bookingServiceMock.Setup(s => s.CreateAsync(It.IsAny<Booking>())).ReturnsAsync(booking);

            var dto = new CreateBookingDto
            {
                RoomId = 1,
                UserId = 1,
                StartTime = DateTime.UtcNow,
                EndTime = DateTime.UtcNow.AddHours(1)
            };

            var result = await _controller.Create(dto);
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returned = Assert.IsType<BookingDto>(createdResult.Value);
            Assert.Equal(booking.Id, returned.Id);
        }
    }
}
