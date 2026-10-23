
using Microsoft.AspNetCore.Identity.UI.Services;

namespace HybridLibrarySystem.Web.Features.Email;

public class EmailSender(ILogger<EmailSender> logger) : IEmailSender
{
    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        logger.LogInformation("Sending email to {Email} with subject {Subject} and message {Message}", email, subject, htmlMessage);
        return Task.CompletedTask;
    }
}
