using MailKit.Net.Smtp;
using MimeKit;
using Notification.Api.Interfaces;

namespace Notification.Api.Mail;

public class EmailService(IConfiguration configuration): IEmailService
{
    public async Task SendEmailAsync(string toEmail, string subject, string message)
    {
        var email = new MimeMessage();
        email.From.Add(new MailboxAddress("E-Wallet Notification", "noreply@ewallet.com"));
        email.To.Add(new MailboxAddress("",  toEmail));
        email.Subject = subject;
        
        var bodyBuilder = new BodyBuilder{TextBody = message};
        email.Body = bodyBuilder.ToMessageBody();

        using var smtp = new SmtpClient();

        await smtp.ConnectAsync(configuration["SmtpSettings:Host"]!,
            int.Parse(configuration["SmtpSettings:Port"]!), false);
        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);

    }
}