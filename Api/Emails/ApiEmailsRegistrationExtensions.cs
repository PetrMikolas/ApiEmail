using ApiEmail.Services.Email;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ApiEmail.Api.Emails;

public static class ApiEmailsRegistrationExtensions
{
    /// <summary>
    /// Maps an endpoint for send email.
    /// </summary>
    /// <param name="app">The web application.</param>
    /// <returns>The web application with mapped endpoint for sending emails.</returns>
    public static WebApplication MapEndpointsEmails(this WebApplication app)
    {
        app.MapPost("", async ([FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)] EmailRequest request, [FromServices] IEmailService emailService, CancellationToken cancellationToken) =>
        {
            if (!IsValidEmailRequest(request, out string error))
            {
                return Results.BadRequest(error);
            }

            try
            {
                await emailService.SendEmailAsync(request.MessageBody,
                                                  request.SenderAddress,
                                                  $"{request.SenderFirstName} {request.SenderLastName}",
                                                  request.Subject,
                                                  request.FromName,
                                                  MimeKit.Text.TextFormat.Plain,
                                                  cancellationToken);

                return Results.NoContent();
            }
            catch (Exception ex)
            {
                app.Logger.LogError(ex.ToString());
                return Results.StatusCode(StatusCodes.Status500InternalServerError);
            }
        })
        .WithTags("Email")
        .WithName("SendEmail")
        .WithOpenApi(operation => new(operation) { Summary = "Send an email" })
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status500InternalServerError);

        return app;
    }

    /// <summary>
    /// Validates an EmailRequest object.
    /// </summary>
    /// <param name="request">The EmailRequest object to validate.</param>
    /// <param name="error">An error message indicating why the validation failed, if any.</param>
    /// <returns>True if the EmailDto object is valid, otherwise false.</returns>
    internal static bool IsValidEmailRequest(EmailRequest? request, out string error)
    {
        error = string.Empty;

        if (request is null)
        {
            error = $"HTTP Request Body musí obsahovat objekt {typeof(EmailRequest).Name}.";
            return false;
        }

        if (string.IsNullOrEmpty(request.SenderFirstName))
        {
            error = $"Parametr <{nameof(request.SenderFirstName)}> nemůže být prázdný. ";
        }

        if (string.IsNullOrEmpty(request.SenderLastName))
        {
            error += $"Parametr <{nameof(request.SenderLastName)}> nemůže být prázdný. ";
        }

        if (string.IsNullOrEmpty(request.MessageBody))
        {
            error += $"Parametr <{nameof(request.MessageBody)}> nemůže být prázdný. ";
        }

        if (string.IsNullOrEmpty(request.SenderAddress))
        {
            error += $"Parametr <{nameof(request.SenderAddress)}> nemůže být prázdný.";
        }

        return error == string.Empty;
    }

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
    internal sealed record EmailRequest
    (
        string SenderFirstName,
        string SenderLastName,
        string SenderAddress,
        string Subject,
        string MessageBody,
        string? FromName
    );
}