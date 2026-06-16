namespace PortSafe.API.Interfaces
{
    public interface IEmailService
    {
        Task SendPasswordResetCodeAsync(string toEmail, string code);
    }
}
