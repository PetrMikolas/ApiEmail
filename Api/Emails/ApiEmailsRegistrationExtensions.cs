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
        app.MapPost("", async ([FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)] EmailDto emailDto, [FromServices] IEmailService emailService, CancellationToken cancellationToken) =>
        {
            if (!IsValidEmailDto(emailDto, out string error))
            {
                return Results.BadRequest(error);
            }

            try
            {
                await emailService.SendEmailAsync(emailDto.Message,
                                                  emailDto.Address,
                                                  $"{emailDto.FirstName} {emailDto.LastName}",
                                                  emailDto.Subject,
                                                  MimeKit.Text.TextFormat.Plain,
                                                  cancellationToken);

                return Results.NoContent();
            }
            catch (Exception)
            {
                return Results.StatusCode(StatusCodes.Status500InternalServerError);
            }
        })
        .WithTags("Email")
        .WithName("SendEmail")
        .WithOpenApi(operation => new(operation) { Summary = "Send an email" })
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status400BadRequest);

        return app;
    }

    /// <summary>
    /// Validates an EmailDto object.
    /// </summary>
    /// <param name="emailDto">The EmailDto object to validate.</param>
    /// <param name="error">An error message indicating why the validation failed, if any.</param>
    /// <returns>True if the EmailDto object is valid, otherwise false.</returns>
    internal static bool IsValidEmailDto(EmailDto? emailDto, out string error)
    {
        error = string.Empty;

        if (emailDto is null)
        {
            error = $"HTTP Request Body musí obsahovat objekt {typeof(EmailDto).Name}.";
            return false;
        }

        if (string.IsNullOrEmpty(emailDto.FirstName))
        {
            error = $"Parametr <{nameof(emailDto.FirstName)}> nemůže být prázdný. ";
        }

        if (string.IsNullOrEmpty(emailDto.LastName))
        {
            error += $"Parametr <{nameof(emailDto.LastName)}> nemůže být prázdný. ";
        }

        if (string.IsNullOrEmpty(emailDto.Message))
        {
            error += $"Parametr <{nameof(emailDto.Message)}> nemůže být prázdný. ";
        }

        if (string.IsNullOrEmpty(emailDto.Address))
        {
            error += $"Parametr <{nameof(emailDto.Address)}> nemůže být prázdný.";
        }

        return error == string.Empty;
    }

    /// <summary>
    /// Represents a data transfer object (DTO) for an email.
    /// </summary>
    /// <param name="FirstName">The first name of the sender.</param>
    /// <param name="LastName">The last name of the sender.</param>
    /// <param name="Subject">The subject of the email.</param>
    /// <param name="Message">The message content of the email.</param>
    /// <param name="Address">The email address of the sender.</param>
    internal sealed record EmailDto
    (
        string FirstName,
        string LastName,
        string Subject,
        string Message,
        string Address
    );
}