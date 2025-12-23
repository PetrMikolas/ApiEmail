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
    /// <param name="messageBody">The body content of the email message.</param>
    /// <param name="senderAddress">The email address of the sender.</param>
    /// <param name="senderName">The name of the sender.</param>
    /// <param name="subject">The subject of the email (optional). Defaults to an empty string.</param>
    /// <param name="fromName">An optional display name used in the email's <c>From</c> header.<br/>
    /// When specified, this value represents the application or system name from which the email is sent.<br/> 
    /// If not provided, a default sender name configured in the application configuration is used.</param>
    /// <param name="textFormat">The format of the email content (optional). Defaults to <see cref="TextFormat.Plain"/>.</param>
    /// <param name="cancellationToken">The cancellation token (optional). Defaults to <see cref="CancellationToken.None"/>.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SendEmailAsync(
        string messageBody, 
        string senderAddress, 
        string senderName, 
        string subject = "", 
        string? fromName = null, 
        TextFormat textFormat = TextFormat.Plain, 
        CancellationToken cancellationToken = default);
}