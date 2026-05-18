using GestionePrenotazioniStruttura.Controllers;
using GestionePrenotazioniStruttura.DTOs;
using GestionePrenotazioniStruttura.Models;
using GestionePrenotazioniStruttura.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using Xunit;

public class ActivityControllerTests
{
    private readonly Mock<IActivityService> _serviceMock;
    private readonly Mock<IAuthorizationService> _authMock;
    private readonly ActivityController _controller;

    public ActivityControllerTests()
    {
        _serviceMock = new Mock<IActivityService>();
        _authMock = new Mock<IAuthorizationService>();
        _controller = new ActivityController(_serviceMock.Object, _authMock.Object);
    }

    [Fact]
    public async Task GetAll_ShouldReturnOk()
    {
        _serviceMock.Setup(s => s.GetAllAsync())
            .ReturnsAsync(new List<ActivityReadDto> { new ActivityReadDto { Id = 1, Title = "Yoga" } });

        var result = await _controller.GetAll();
        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var list = Assert.IsType<List<ActivityReadDto>>(ok.Value);
        Assert.Single(list);
    }

    [Fact]
    public async Task GetById_ShouldReturnOk_WhenFound()
    {
        _serviceMock.Setup(s => s.GetByIdAsync(1))
            .ReturnsAsync(new ActivityReadDto { Id = 1, Title = "Yoga" });

        var result = await _controller.GetById(1);
        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal("Yoga", ((ActivityReadDto)ok.Value).Title);
    }

    [Fact]
    public async Task GetById_ShouldReturnNotFound_WhenNull()
    {
        _serviceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync((ActivityReadDto)null);
        var result = await _controller.GetById(1);
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task Create_ShouldReturnCreatedAtAction()
    {
        var dto = new ActivityCreateDto { Title = "Yoga" };
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim("StructureId", "1") }));

        _serviceMock.Setup(s => s.CreateAsync(dto))
            .ReturnsAsync(new ActivityReadDto { Id = 1, Title = "Yoga" });

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext { User = user }
        };

        var result = await _controller.Create(dto);
        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returned = Assert.IsType<ActivityReadDto>(created.Value);
        Assert.Equal("Yoga", returned.Title);
    }

    [Fact]
    public async Task Update_ShouldReturnNoContent_WhenAuthorizedAndUpdated()
    {
        _authMock.Setup(a => a.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), 1, "StrutturaScope"))
            .ReturnsAsync(AuthorizationResult.Success());

        _serviceMock.Setup(s => s.UpdateAsync(It.IsAny<ActivityUpdateDto>(), 1)).ReturnsAsync(true);

        var result = await _controller.Update(1, new ActivityUpdateDto { Title = "Pilates" });
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Update_ShouldReturnForbid_WhenNotAuthorized()
    {
        _authMock.Setup(a => a.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), 1, "StrutturaScope"))
            .ReturnsAsync(AuthorizationResult.Failed());

        var result = await _controller.Update(1, new ActivityUpdateDto { Title = "Pilates" });
        Assert.IsType<ForbidResult>(result);
    }

    [Fact]
    public async Task Update_ShouldReturnNotFound_WhenUpdateFails()
    {
        _authMock.Setup(a => a.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), 1, "StrutturaScope"))
            .ReturnsAsync(AuthorizationResult.Success());
        _serviceMock.Setup(s => s.UpdateAsync(It.IsAny<ActivityUpdateDto>(), 1)).ReturnsAsync(false);

        var result = await _controller.Update(1, new ActivityUpdateDto { Title = "Pilates" });
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Delete_ShouldReturnNoContent_WhenAuthorizedAndDeleted()
    {
        _authMock.Setup(a => a.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), 1, "StrutturaScope"))
            .ReturnsAsync(AuthorizationResult.Success());
        _serviceMock.Setup(s => s.DeleteAsync(1)).ReturnsAsync(true);

        var result = await _controller.Delete(1);
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Delete_ShouldReturnForbid_WhenNotAuthorized()
    {
        _authMock.Setup(a => a.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), 1, "StrutturaScope"))
            .ReturnsAsync(AuthorizationResult.Failed());

        var result = await _controller.Delete(1);
        Assert.IsType<ForbidResult>(result);
    }

    [Fact]
    public async Task Delete_ShouldReturnNotFound_WhenDeleteFails()
    {
        _authMock.Setup(a => a.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), 1, "StrutturaScope"))
            .ReturnsAsync(AuthorizationResult.Success());
        _serviceMock.Setup(s => s.DeleteAsync(1)).ReturnsAsync(false);

        var result = await _controller.Delete(1);
        Assert.IsType<NotFoundResult>(result);
    }
}
