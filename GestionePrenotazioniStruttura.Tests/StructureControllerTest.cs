using GestionePrenotazioniStruttura.Controllers;
using GestionePrenotazioniStruttura.DTOs;
using GestionePrenotazioniStruttura.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

public class SubscriptionControllerTests
{
    private readonly Mock<ISubscriptionService> _serviceMock;
    private readonly SubscriptionController _controller;

    public SubscriptionControllerTests()
    {
        _serviceMock = new Mock<ISubscriptionService>();
        _controller = new SubscriptionController(_serviceMock.Object);
    }

    [Fact]
    public async Task GetAll_ShouldReturnOk()
    {
        _serviceMock.Setup(s => s.GetAllSubscriptionsAsync()).ReturnsAsync(new List<ReadSubscriptionDto> { new ReadSubscriptionDto { Id = 1 } });
        var result = await _controller.GetAll();
        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var list = Assert.IsType<List<ReadSubscriptionDto>>(ok.Value);
        Assert.Single(list);
    }

    [Fact]
    public async Task GetById_ShouldReturnOk_WhenFound()
    {
        _serviceMock.Setup(s => s.GetSubscriptionByIdAsync(1)).ReturnsAsync(new ReadSubscriptionDto { Id = 1 });
        var result = await _controller.GetById(1);
        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(1, ((ReadSubscriptionDto)ok.Value).Id);
    }

    [Fact]
    public async Task GetById_ShouldReturnNotFound_WhenMissing()
    {
        _serviceMock.Setup(s => s.GetSubscriptionByIdAsync(1)).ReturnsAsync((ReadSubscriptionDto)null);
        var result = await _controller.GetById(1);
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task Create_ShouldReturnCreated()
    {
        var dto = new CreateSubscriptionDto { Name = "Sub1" };
        _serviceMock.Setup(s => s.CreateSubscriptionAsync(dto)).ReturnsAsync(new ReadSubscriptionDto { Id = 1, Name = "Sub1" });

        var result = await _controller.Create(dto);
        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(1, ((ReadSubscriptionDto)created.Value).Id);
    }

    [Fact]
    public async Task Update_ShouldReturnNoContent_WhenUpdated()
    {
        _serviceMock.Setup(s => s.UpdateSubscriptionAsync(1, It.IsAny<UpdateSubscriptionDto>())).ReturnsAsync(new ReadSubscriptionDto { Id = 1 });
        var result = await _controller.Update(1, new UpdateSubscriptionDto { Name = "New" });
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Update_ShouldReturnNotFound_WhenMissing()
    {
        _serviceMock.Setup(s => s.UpdateSubscriptionAsync(1, It.IsAny<UpdateSubscriptionDto>())).ReturnsAsync((ReadSubscriptionDto)null);
        var result = await _controller.Update(1, new UpdateSubscriptionDto { Name = "New" });
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Delete_ShouldReturnNoContent_WhenDeleted()
    {
        _serviceMock.Setup(s => s.DeleteSubscriptionAsync(1)).ReturnsAsync(true);
        var result = await _controller.Delete(1);
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Delete_ShouldReturnNotFound_WhenMissing()
    {
        _serviceMock.Setup(s => s.DeleteSubscriptionAsync(1)).ReturnsAsync(false);
        var result = await _controller.Delete(1);
        Assert.IsType<NotFoundResult>(result);
    }
}
