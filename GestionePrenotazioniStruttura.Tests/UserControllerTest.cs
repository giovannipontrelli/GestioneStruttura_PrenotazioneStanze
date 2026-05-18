using GestionePrenotazioniStruttura.Controllers;
using GestionePrenotazioniStruttura.DTOs;
using GestionePrenotazioniStruttura.Models;
using GestionePrenotazioniStruttura.Models.Enum;
using GestionePrenotazioniStruttura.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

public class UsersControllerTests
{
    private readonly Mock<IUserService> _serviceMock;
    private readonly UsersController _controller;

    public UsersControllerTests()
    {
        _serviceMock = new Mock<IUserService>();
        _controller = new UsersController(_serviceMock.Object);
    }

    [Fact]
    public async Task GetAll_ShouldReturnOk()
    {
        _serviceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<User> { new User { Id = 1, Name = "U1", Role = Role.Admin } });
        var result = await _controller.GetAll();
        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var list = Assert.IsType<List<UserDto>>(ok.Value);
        Assert.Single(list);
        Assert.Equal("U1", list[0].Name);
    }

    [Fact]
    public async Task GetById_ShouldReturnOk_WhenFound()
    {
        _serviceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(new User { Id = 1, Name = "U1", Role = Role.Admin });
        var result = await _controller.GetById(1);
        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(1, ((UserDto)ok.Value).Id);
    }

    [Fact]
    public async Task GetById_ShouldReturnNotFound_WhenMissing()
    {
        _serviceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync((User)null);
        var result = await _controller.GetById(1);
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task Create_ShouldReturnCreated()
    {
        var dto = new CreateUserDto { Name = "U1", Email = "u1@test.com", Role = "Customer" };
        _serviceMock.Setup(s => s.CreateAsync(It.IsAny<User>())).ReturnsAsync(new User { Id = 1, Name = "U1", Role = Role.Customer });

        var result = await _controller.Create(dto);
        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(1, ((UserDto)created.Value).Id);
    }

    [Fact]
    public async Task Update_ShouldReturnNoContent_WhenUpdated()
    {
        _serviceMock.Setup(s => s.UpdateAsync(It.IsAny<User>())).ReturnsAsync(true);
        var result = await _controller.Update(1, new UpdateUserDto { Name = "New", Role = "Admin" });
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Update_ShouldReturnNotFound_WhenMissing()
    {
        _serviceMock.Setup(s => s.UpdateAsync(It.IsAny<User>())).ReturnsAsync(false);
        var result = await _controller.Update(1, new UpdateUserDto { Name = "X", Role = "Customer" });
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
