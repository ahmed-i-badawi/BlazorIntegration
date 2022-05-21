using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using SharedLibrary;
using SharedLibrary.Models;

namespace BlazorServer.Services;

public interface IEmailService
{
    Task SendEmail(EmailMessageModel message);
}

public class EmailService : IEmailService
{
    private readonly EmailConfiguration _emailConfig;

    public EmailService(EmailConfiguration emailConfig)
    {
        _emailConfig = emailConfig;
    }

    public async Task SendEmail(EmailMessageModel message)
    {
        var emailMessage = CreateEmailMessage(message);
        await Send(emailMessage);
    }


    private MimeMessage CreateEmailMessage(EmailMessageModel message)
    {
        // create message
        var emailMessage = new MimeMessage();
        emailMessage.From.Add(MailboxAddress.Parse(message.From ?? _emailConfig.From));
        emailMessage.To.Add(MailboxAddress.Parse(message.To));
        emailMessage.Subject = message.Subject;
        emailMessage.Body = new TextPart(TextFormat.Text) { Text = message.Content };
        return emailMessage;
    }
    
    private async Task Send(MimeMessage mailMessage)
    {

        using var client = new SmtpClient();

        try
        {
            //smtp.Connect(smtpHost, smtpPort, SecureSocketOptions.StartTls);

            await client.ConnectAsync(_emailConfig.SmtpServer, _emailConfig.Port, SecureSocketOptions.StartTls);
            client.AuthenticationMechanisms.Remove("XOAUTH2");
            await client.AuthenticateAsync(_emailConfig.UserName, _emailConfig.Password);
            await client.SendAsync(mailMessage);
        }
        catch
        {
            //log an error message or throw an exception or both.
            throw;
        }
        finally
        {
            await client.DisconnectAsync(true);
            client.Dispose();
        }
    }
}
