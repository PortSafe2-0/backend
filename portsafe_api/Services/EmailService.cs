using System.Net;
using System.Net.Mail;
using PortSafe.API.Interfaces;

namespace PortSafe.API.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration config, ILogger<EmailService> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task SendPasswordResetCodeAsync(string toEmail, string code)
        {
            var smtp = _config.GetSection("Smtp");
            var host = smtp["Host"];

            if (string.IsNullOrEmpty(host))
            {
                _logger.LogWarning("SMTP não configurado. Código de redefinição para {Email}: {Code}", toEmail, code);
                return;
            }

            var port = int.Parse(smtp["Port"]!);
            var enableSsl = bool.Parse(smtp["EnableSsl"] ?? "true");
            var username = smtp["Username"]!;
            var password = smtp["Password"]!;
            var fromName = smtp["FromName"] ?? "PortSafe";

            var body = $@"
<div style='font-family:sans-serif;max-width:480px;margin:auto;padding:32px;background:#0A0F1E;color:#fff;border-radius:16px;'>
  <h2 style='color:#60A5FA;'>PortSafe — Redefinição de Senha</h2>
  <p>Seu código de verificação é:</p>
  <div style='font-size:36px;font-weight:900;letter-spacing:12px;color:#fff;background:#1e293b;padding:16px 24px;border-radius:12px;text-align:center;margin:24px 0;'>
    {code}
  </div>
  <p style='color:#6B7280;font-size:13px;'>Este código expira em <strong style='color:#fff;'>15 minutos</strong>. Se você não solicitou a redefinição, ignore este e-mail.</p>
</div>";

            using var client = new SmtpClient(host, port)
            {
                EnableSsl = enableSsl,
                Credentials = new NetworkCredential(username, password),
            };

            var message = new MailMessage
            {
                From = new MailAddress(username, fromName),
                Subject = "PortSafe — Código de Redefinição de Senha",
                Body = body,
                IsBodyHtml = true,
            };
            message.To.Add(toEmail);

            try
            {
                await client.SendMailAsync(message);
                _logger.LogInformation("E-mail de redefinição enviado para {Email}", toEmail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Falha ao enviar e-mail para {Email}", toEmail);
                throw;
            }
        }
    }
}
