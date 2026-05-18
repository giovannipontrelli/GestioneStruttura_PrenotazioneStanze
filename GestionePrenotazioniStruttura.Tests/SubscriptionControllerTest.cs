using GestionePrenotazioniStruttura.Controllers;
using GestionePrenotazioniStruttura.DTOs;
using GestionePrenotazioniStruttura.Models;
using GestionePrenotazioniStruttura.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

public class StructuresControllerTests
{
    private readonly Mock<IStructureService> _serviceMock;
    private readonly StructuresController _controller;

    public StructuresControllerTests()
    {
        _serviceMock = new Mock<IStructureService>();
        _controller = new StructuresController(_serviceMock.Object);
    }

    [Fact]
    public async Task GetAll_ShouldReturnOk()
    {
        _serviceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<Structure> { new Structure { Id = 1, Name = "S1" } });
        var result = await _controller.GetAll();
        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var list = Assert.IsType<List<StructureDto>>(ok.Value);
        Assert.Single(list);
        Assert.Equal("S1", list[0].Name);
    }

    [Fact]
    public async Task GetById_ShouldReturnOk_WhenFound()
    {
        _serviceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(new Structure { Id = 1, Name = "S1" });
        var result = await _controller.GetById(1);
        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(1, ((StructureDto)ok.Value).Id);
    }

    [Fact]
    public async Task GetById_ShouldReturnNotFound_WhenMissing()
    {
        _serviceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync((Structure)null);
        var result = await _controller.GetById(1);
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task Create_ShouldReturnCreated()
    {
        var dto = new CreateStructureDto { Name = "S1" };
        _serviceMock.Setup(s => s.CreateAsync(It.IsAny<Structure>())).ReturnsAsync(new Structure { Id = 1, Name = "S1" });

        var result = await _controller.Create(dto);
        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(1, ((StructureDto)created.Value).Id);
    }

    [Fact]
    public async Task Update_ShouldReturnNoContent_WhenSuccess()
    {
        _serviceMock.Setup(s => s.UpdateAsync(It.IsAny<Structure>())).ReturnsAsync(true);
        var result = await _controller.Update(1, new UpdateStructureDto { Name = "S1" });
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Update_ShouldReturnNotFound_WhenMissing()
    {
        _serviceMock.Setup(s => s.UpdateAsync(It.IsAny<Structure>())).ReturnsAsync(false);
        var result = await _controller.Update(1, new UpdateStructureDto { Name = "S1" });
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
