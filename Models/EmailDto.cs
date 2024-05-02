namespace ApiEmail.Models;


/// <summary>
/// Represents a data transfer object (DTO) for an email.
/// </summary>
public class EmailDto
{
    /// <summary>
    /// Gets or sets the first name of the sender.
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the last name of the sender.
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the subject of the email.
    /// </summary>
    public string Subject { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the message content of the email.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the email address of the sender.
    /// </summary>
    public string Address { get; set; } = string.Empty;
}