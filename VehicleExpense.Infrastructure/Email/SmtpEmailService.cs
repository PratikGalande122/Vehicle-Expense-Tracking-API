using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using VehicleExpense.Application.Interfaces;

namespace VehicleExpense.Infrastructure.Email;

/// <summary>
/// Sends OTP emails via SMTP.
/// </summary>
public class SmtpEmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public SmtpEmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendOtpEmailAsync(string toEmail, string otp)
    {
        var smtpSection = _configuration.GetSection("Smtp");
        var host = smtpSection["Host"] ?? "smtp.gmail.com";
        var port = int.Parse(smtpSection["Port"] ?? "587");
        var fromEmail = smtpSection["FromEmail"] ?? string.Empty;
        var password = smtpSection["Password"] ?? string.Empty;
        var displayName = smtpSection["DisplayName"] ?? "VehicleExpense";

        using var client = new SmtpClient(host, port)
        {
            Credentials = new NetworkCredential(fromEmail, password),
            EnableSsl = true
        };

        var message = new MailMessage
        {
            From = new MailAddress(fromEmail, displayName),
            Subject = "Your OTP Code",
            Body = $"Your one-time password is: <strong>{otp}</strong>. It expires in 5 minutes.",
            IsBodyHtml = true
        };
        message.To.Add(toEmail);

        await client.SendMailAsync(message);
    }
}
