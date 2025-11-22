using System.Threading.Tasks;

namespace ReservasCanchas_Web.Servicios.Interfaces
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string to, string subject, string body);
    }
}