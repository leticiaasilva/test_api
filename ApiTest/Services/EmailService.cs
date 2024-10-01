using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

public class EmailService : IEmailService
{
    private readonly SmtpClient _smtpClient;

    public EmailService(SmtpClient smtpClient)
    {
        _smtpClient = smtpClient;
    }

    public async Task SendReminder(string email)
    {
        var mailMessage = new MailMessage("leticia.asilva@icloud.com", email)
        {
            Subject = "Reminder",
            Body = "This is a reminder email.",
            IsBodyHtml = true
        };

        await _smtpClient.SendMailAsync(mailMessage);
    }
}
