using MimeKit.Text;

namespace ApiEmail.Services.Email;

/// <summary>
/// Service for sending emails. 
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Sends an email asynchronously.
    /// </summary>
    /// <param name="message">The message content of the email.</param>
    /// <param name="address">The email address of the sender.</param>
    /// <param name="name">The name of the sender.</param>
    /// <param name="subject">The subject of the email (optional). Defaults to an empty string.</param>
    /// <param name="textFormat">The format of the email content (optional). Defaults to <see cref="TextFormat.Plain"/>.</param>
    /// <param name="cancellationToken">The cancellation token (optional). Defaults to <see cref="CancellationToken.None"/>.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SendEmailAsync(string message, string address, string name, string subject = "", TextFormat textFormat = TextFormat.Plain, CancellationToken cancellationToken = default);
}