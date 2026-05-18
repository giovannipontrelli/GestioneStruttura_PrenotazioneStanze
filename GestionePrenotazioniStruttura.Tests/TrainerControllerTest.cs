using GestionePrenotazioniStruttura.Controllers;
using GestionePrenotazioniStruttura.DTOs;
using GestionePrenotazioniStruttura.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

public class TrainerControllerTests
{
    private readonly Mock<ITrainerService> _serviceMock;
    private readonly TrainerController _controller;

    public TrainerControllerTests()
    {
        _serviceMock = new Mock<ITrainerService>();
        _controller = new TrainerController(_serviceMock.Object);
    }

    [Fact]
    public async Task GetAll_ShouldReturnOk()
    {
        _serviceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<TrainerReadDto> { new TrainerReadDto { Id = 1 } });
        var result = await _controller.GetAll();
        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var list = Assert.IsType<List<TrainerReadDto>>(ok.Value);
        Assert.Single(list);
    }

    [Fact]
    public async Task GetById_ShouldReturnOk_WhenFound()
    {
        _serviceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(new TrainerReadDto { Id = 1 });
        var result = await _controller.GetById(1);
        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(1, ((TrainerReadDto)ok.Value).Id);
    }

    [Fact]
    public async Task GetById_ShouldReturnNotFound_WhenMissing()
    {
        _serviceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync((TrainerReadDto)null);
        var result = await _controller.GetById(1);
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task Create_ShouldReturnCreated()
    {
        var dto = new TrainerCreateDto { Name = "T1" };
        _serviceMock.Setup(s => s.CreateAsync(dto)).ReturnsAsync(new TrainerReadDto { Id = 1, Name = "T1" });

        var result = await _controller.Create(dto);
        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(1, ((TrainerReadDto)created.Value).Id);
    }

    [Fact]
    public async Task Update_ShouldReturnNoContent_WhenUpdated()
    {
        _serviceMock.Setup(s => s.UpdateAsync(It.IsAny<TrainerUpdateDto>(), 1)).ReturnsAsync(true);
        var result = await _controller.Update(1, new TrainerUpdateDto { Name = "New" });
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Update_ShouldReturnNotFound_WhenMissing()
    {
        _serviceMock.Setup(s => s.UpdateAsync(It.IsAny<TrainerUpdateDto>(), 1)).ReturnsAsync(false);
        var result = await _controller.Update(1, new TrainerUpdateDto { Name = "X" });
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
