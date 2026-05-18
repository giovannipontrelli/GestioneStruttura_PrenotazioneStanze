using GestionePrenotazioniStruttura.Controllers;
using GestionePrenotazioniStruttura.DTOs;
using GestionePrenotazioniStruttura.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

public class PaymentControllerTests
{
    private readonly Mock<IPaymentService> _serviceMock;
    private readonly PaymentController _controller;

    public PaymentControllerTests()
    {
        _serviceMock = new Mock<IPaymentService>();
        _controller = new PaymentController(_serviceMock.Object, null);
    }

    [Fact]
    public async Task GetAll_ShouldReturnOkWithPayments()
    {
        _serviceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<ReadPaymentDto> { new ReadPaymentDto { Id = 1 } });
        var result = await _controller.GetAll();
        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var list = Assert.IsType<List<ReadPaymentDto>>(ok.Value);
        Assert.Single(list);
    }

    [Fact]
    public async Task GetById_ShouldReturnOk_WhenFound()
    {
        _serviceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(new ReadPaymentDto { Id = 1 });
        var result = await _controller.GetById(1);
        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(1, ((ReadPaymentDto)ok.Value).Id);
    }

    [Fact]
    public async Task GetById_ShouldReturnNotFound_WhenNull()
    {
        _serviceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync((ReadPaymentDto)null);
        var result = await _controller.GetById(1);
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task Create_ShouldReturnCreated()
    {
        var dto = new CreatePaymentDto { Amount = 10 };
        _serviceMock.Setup(s => s.CreateAsync(dto)).ReturnsAsync(new ReadPaymentDto { Id = 1, Amount = 10 });

        var result = await _controller.Create(dto);
        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        var payment = Assert.IsType<ReadPaymentDto>(created.Value);
        Assert.Equal(1, payment.Id);
    }

    [Fact]
    public async Task Update_ShouldReturnNoContent_WhenSuccessful()
    {
        _serviceMock.Setup(s => s.UpdateAsync(It.IsAny<UpdatePaymentDto>(), 1)).ReturnsAsync(true);
        var result = await _controller.Update(1, new UpdatePaymentDto());
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Update_ShouldReturnNotFound_WhenFailed()
    {
        _serviceMock.Setup(s => s.UpdateAsync(It.IsAny<UpdatePaymentDto>(), 1)).ReturnsAsync(false);
        var result = await _controller.Update(1, new UpdatePaymentDto());
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Delete_ShouldReturnNoContent_WhenDeleted()
    {
        _serviceMock.Setup(s => s.DeleteAsync(1)).ReturnsAsync(true);
        var result = await _controller.Delete(1);
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Delete_ShouldReturnNotFound_WhenMissing()
    {
        _serviceMock.Setup(s => s.DeleteAsync(1)).ReturnsAsync(false);
        var result = await _controller.Delete(1);
        Assert.IsType<NotFoundResult>(result);
    }
}
