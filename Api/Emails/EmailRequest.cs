namespace ApiEmail.Api.Emails;

/// <summary>
/// Represents a data transfer object for sending an email via the email API.
/// </summary>
/// <param name="SenderFirstName">The first name of the person submitting the email (e.g., contact form sender).</param>
/// <param name="SenderLastName">The last name of the person submitting the email.</param>
/// <param name="SenderAddress">The email address of the sender.</param>
/// <param name="Subject">The subject of the email message.</param>
/// <param name="MessageBody">The body content of the email message.</param>
/// <param name="FromName">An optional display name used as the sender name in the email header.<br/>
/// Typically represents the application or website name from which the email is sent.</param>
public sealed record EmailRequest
(
    string SenderFirstName,
    string SenderLastName,
    string SenderAddress,
    string Subject,
    string MessageBody,
    string? FromName
);