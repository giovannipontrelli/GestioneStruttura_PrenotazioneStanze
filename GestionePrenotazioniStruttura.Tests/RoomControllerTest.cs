using GestionePrenotazioniStruttura.Controllers;
using GestionePrenotazioniStruttura.DTOs;
using GestionePrenotazioniStruttura.Models;
using GestionePrenotazioniStruttura.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

public class RoomsControllerTests
{
    private readonly Mock<IRoomService> _serviceMock;
    private readonly Mock<IAuthorizationService> _authMock;
    private readonly RoomsController _controller;

    public RoomsControllerTests()
    {
        _serviceMock = new Mock<IRoomService>();
        _authMock = new Mock<IAuthorizationService>();
        _controller = new RoomsController(_serviceMock.Object, _authMock.Object);
    }

    [Fact]
    public async Task GetAll_ShouldReturnOk()
    {
        _serviceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<Room> { new Room { Id = 1 } });
        var result = await _controller.GetAll();
        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var list = Assert.IsType<List<RoomDto>>(ok.Value);
        Assert.Single(list);
    }

    [Fact]
    public async Task GetById_ShouldReturnOk_WhenFound()
    {
        _serviceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(new Room { Id = 1 });
        var result = await _controller.GetById(1);
        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(1, ((RoomDto)ok.Value).Id);
    }

    [Fact]
    public async Task GetById_ShouldReturnNotFound_WhenMissing()
    {
        _serviceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync((Room)null);
        var result = await _controller.GetById(1);
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task Create_ShouldReturnForbid_WhenNotAuthorized()
    {
        _authMock.Setup(a => a.AuthorizeAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>(), 1, "StrutturaScope"))
            .ReturnsAsync(AuthorizationResult.Failed());

        var dto = new CreateRoomDto { StructureId = 1 };
        var result = await _controller.Create(dto);
        Assert.IsType<ForbidResult>(result.Result);
    }

    [Fact]
    public async Task Create_ShouldReturnCreated_WhenAuthorized()
    {
        _authMock.Setup(a => a.AuthorizeAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>(), 1, "StrutturaScope"))
            .ReturnsAsync(AuthorizationResult.Success());
        _serviceMock.Setup(s => s.CreateAsync(It.IsAny<Room>())).ReturnsAsync(new Room { Id = 1, StructureId = 1 });

        var dto = new CreateRoomDto { StructureId = 1 };
        var result = await _controller.Create(dto);
        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(1, ((RoomDto)created.Value).Id);
    }

    [Fact]
    public async Task Update_ShouldReturnForbid_WhenNotAuthorized()
    {
        _authMock.Setup(a => a.AuthorizeAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>(), 1, "StrutturaScope"))
            .ReturnsAsync(AuthorizationResult.Failed());

        var dto = new UpdateRoomDto { StructureId = 1 };
        var result = await _controller.Update(1, dto);
        Assert.IsType<ForbidResult>(result);
    }

    [Fact]
    public async Task Update_ShouldReturnNoContent_WhenSuccess()
    {
        _authMock.Setup(a => a.AuthorizeAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>(), 1, "StrutturaScope"))
            .ReturnsAsync(AuthorizationResult.Success());
        _serviceMock.Setup(s => s.UpdateAsync(It.IsAny<Room>())).ReturnsAsync(true);

        var dto = new UpdateRoomDto { StructureId = 1 };
        var result = await _controller.Update(1, dto);
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Update_ShouldReturnNotFound_WhenMissing()
    {
        _authMock.Setup(a => a.AuthorizeAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>(), 1, "StrutturaScope"))
            .ReturnsAsync(AuthorizationResult.Success());
        _serviceMock.Setup(s => s.UpdateAsync(It.IsAny<Room>())).ReturnsAsync(false);

        var dto = new UpdateRoomDto { StructureId = 1 };
        var result = await _controller.Update(1, dto);
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Delete_ShouldReturnNoContent_WhenDeleted()
    {
        _authMock.Setup(a => a.AuthorizeAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>(), 1, "StrutturaScope"))
            .ReturnsAsync(AuthorizationResult.Success());
        _serviceMock.Setup(s => s.DeleteAsync(1)).ReturnsAsync(true);

        var result = await _controller.Delete(1);
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Delete_ShouldReturnNotFound_WhenMissing()
    {
        _authMock.Setup(a => a.AuthorizeAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>(), 1, "StrutturaScope"))
            .ReturnsAsync(AuthorizationResult.Success());
        _serviceMock.Setup(s => s.DeleteAsync(1)).ReturnsAsync(false);

        var result = await _controller.Delete(1);
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GetByStructure_ShouldReturnFilteredRooms()
    {
        _serviceMock.Setup(s => s.GetByStructureAsync(1)).ReturnsAsync(new List<Room> { new Room { Id = 1, StructureId = 1 } });
        var result = await _controller.GetByStructure(1);
        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var list = Assert.IsType<List<RoomDto>>(ok.Value);
        Assert.Single(list);
        Assert.Equal(1, list[0].StructureId);
    }
}
