using ReservasCanchas_Web.Models;

namespace ReservasCanchas_Web.Servicios.Interfaces
{
    public interface IEmailService
    {
        Task SendReservaConfirmationEmailAsync(Reserva reserva);
    }
}