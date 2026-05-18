using GestionePrenotazioniStruttura.DTOs;
using GestionePrenotazioniStruttura.Models;

public interface IAuthService
{
    Task<AuthResponseDto> LoginAsync(UserLoginDto loginDto);
    Task<User> RegisterAsync(UserRegisterDto registerDto);
    Task LogoutByUserIdAsync(int userId);
    Task ConfermaUtenti(int id, string validation);
}
