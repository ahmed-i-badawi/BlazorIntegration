using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using SharedLibrary;
using SharedLibrary.Entities;
using SharedLibrary.Models;

namespace BlazorServer.Services;

public interface IEmailService
{
    Task SendEmail(EmailMessageModel message);
    Task SendMachineRegisterationMail(Machine dbMachine);
    Task SendSiteRegisterationMail(string email, string hash, string userName, string password);
    Task SendIntegratorRegisterationMail(string email, string hash, string userName, string password);

}

public class EmailService : IEmailService
{
    private readonly EmailConfiguration _emailConfig;

    public EmailService(EmailConfiguration emailConfig)
    {
        _emailConfig = emailConfig;
    }

    public async Task SendIntegratorRegisterationMail(string email, string hash, string userName, string password)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return;
        }
        EmailMessageModel mailMessage = new EmailMessageModel(
   email,
   $"Integrator registeration details",
   $"you have registered with hash: {hash}, User Name: {userName}, Password: {password}");

        await this.SendEmail(mailMessage);
    }


    public async Task SendSiteRegisterationMail(string email, string hash, string userName, string password)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return;
        }
        EmailMessageModel mailMessage = new EmailMessageModel(
   email,
   $"Site registeration details",
   $"you have registered with hash: {hash}, User Name: {userName}, Password: {password}");

        await this.SendEmail(mailMessage);
    }

    public async Task SendMachineRegisterationMail(Machine dbMachine)
    {
        if (string.IsNullOrWhiteSpace(dbMachine.Site?.ApplicationUser?.Email))
        {
            return;
        }

        EmailMessageModel mailMessage = new EmailMessageModel(
              dbMachine.Site.ApplicationUser.Email,
              $"machine registeration details",
              $"you have registered your machine: {dbMachine.Name} on your hash: {dbMachine.Site.Hash}");

        await this.SendEmail(mailMessage);
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
