using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ApiEmail.Services.Email;

/// <summary>
/// Service for managing email-related operations, based on the <seealso cref="IEmailService"/> interface.
/// </summary>
internal sealed class EmailService : IEmailService
{
    private readonly EmailOptions _options;
    private readonly ILogger<EmailService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="EmailService"/> class.
    /// </summary>
    /// <param name="options">The email options.</param>
    /// <param name="logger">The logger instance for logging errors and information.</param>
    public EmailService(IOptions<EmailOptions> options, ILogger<EmailService> logger)
    {
        _logger = logger;
        _options = InitializeOptions(options);
    }

    /// <summary>
    /// Initializes the email options from the provided configuration, handling any validation exceptions.
    /// </summary>
    /// <param name="options">The email configuration options.</param>
    /// <returns>The initialized email options.</returns>
    private EmailOptions InitializeOptions(IOptions<EmailOptions> options)
    {
        try
        {
            return options.Value;
        }
        catch (OptionsValidationException ex)
        {
            _logger.LogError(ex, $"Chyba při validaci {nameof(EmailOptions)} pro EmailService");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Chyba při inicializaci {nameof(EmailOptions)} pro EmailService");
        }

        return new EmailOptions();
    }

    public async Task SendEmailAsync(string message, string address, string name, string subject = "", TextFormat textFormat = TextFormat.Plain, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(message);
        ArgumentException.ThrowIfNullOrEmpty(address);
        ArgumentException.ThrowIfNullOrEmpty(name);

        var mimeMessage = new MimeMessage();
        mimeMessage.From.Add(new MailboxAddress(_options.FromName, _options.FromEmailAddress));
        mimeMessage.To.Add(new MailboxAddress(_options.FromName, _options.FromEmailAddress));
        mimeMessage.Subject = !string.IsNullOrEmpty(subject) ? subject : _options.DefaultSubject;
        mimeMessage.Body = new TextPart(textFormat) { Text = GetMessageBody(message, name, address) };

        if (!string.IsNullOrEmpty(_options.BccEmailAddress))
        {
            mimeMessage.Bcc.Add(new MailboxAddress(_options.BccName, _options.BccEmailAddress));
        }

        try
        {
            using var smtpClient = new SmtpClient();
            await smtpClient.ConnectAsync(_options.SmtpHost, _options.SmtpPort, _options.SmtpUseSsl, cancellationToken);
            await smtpClient.AuthenticateAsync(_options.SmtpUserName, _options.SmtpPassword, cancellationToken);
            await smtpClient.SendAsync(mimeMessage, cancellationToken);
            await smtpClient.DisconnectAsync(true, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            throw;
        }
    }

    /// <summary>
    /// Creates the body of the email message with the sender's information and message content.
    /// </summary>
    /// <param name="message">The main content of the email message.</param>
    /// <param name="name">The name of the sender.</param>
    /// <param name="address">The email address of the sender.</param>
    /// <returns>The formatted email message body.</returns>
    private string GetMessageBody(string message, string name, string address)
    {
        // Format the email message body with sender's information and message content
        var formattedMessage = new StringBuilder();
        formattedMessage.AppendLine("Odesílatel:");
        formattedMessage.AppendLine(name);
        formattedMessage.AppendLine(address);
        formattedMessage.AppendLine();
        formattedMessage.AppendLine(message);

        return formattedMessage.ToString();
    }

    /// <summary>
    /// Represents the configuration options for the email service.
    /// </summary>
    public sealed record EmailOptions
    {
        /// <summary>
        /// The key for accessing the email options.
        /// </summary>
        public const string Key = "EmailOptions";

        /// <summary>
        /// Gets or sets the SMTP host address.
        /// </summary>
        [Required]
        public string SmtpHost { get; init; } = string.Empty;

        /// <summary>
        /// Gets or sets the SMTP port number.
        /// </summary>
        [Required]
        public int SmtpPort { get; init; }

        /// <summary>
        /// Gets or sets a value indicating whether SSL/TLS should be used for SMTP.
        /// </summary>
        [Required]
        public bool SmtpUseSsl { get; init; }

        /// <summary>
        /// Gets or sets the SMTP username.
        /// </summary>
        [Required]
        public string SmtpUserName { get; init; } = string.Empty;

        /// <summary>
        /// Gets or sets the SMTP password.
        /// </summary>
        [Required]
        public string SmtpPassword { get; init; } = string.Empty;

        /// <summary>
        /// Gets or sets the name displayed as the sender of the email.
        /// </summary>
        [Required]
        public string FromName { get; init; } = string.Empty;

        /// <summary>
        /// Gets or sets the email address used as the sender.
        /// </summary>
        [Required]
        public string FromEmailAddress { get; init; } = string.Empty;

        /// <summary>
        /// Gets or sets the email address where error notifications are sent.
        /// </summary>        
        public string AdminEmailAddress { get; init; } = string.Empty;

        /// <summary>
        /// Gets or sets the default subject for emails.
        /// </summary>
        public string DefaultSubject { get; init; } = string.Empty;

        /// <summary>
        /// Gets or sets the name displayed as the BCC recipient of the email.
        /// </summary>
        public string BccName { get; init; } = string.Empty;

        /// <summary>
        /// Gets or sets the email address used for BCC recipients.
        /// </summary>
        public string BccEmailAddress { get; init; } = string.Empty;
    }
}