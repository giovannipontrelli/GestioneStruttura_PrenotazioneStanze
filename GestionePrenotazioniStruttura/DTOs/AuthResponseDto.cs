namespace GestionePrenotazioniStruttura.DTOs
{
    public class AuthResponseDto
    {
        public string Token { get; set; } = null!;
        public DateTime Expiration { get; set; }
        public string Role { get; set; } = null!;
    }
    public class ConfirmUserRequest
    {
        public int Id { get; set; }
        public string Validation { get; set; } = null!; 
    }
}
