using ApiEmail.Models;
using ApiEmail.Services.Email;
using Microsoft.AspNetCore.Mvc;

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
        app.MapPost("", async ([FromBody] EmailDto emailDto, [FromServices] IEmailService emailService, CancellationToken cancellationToken) =>
        {
            if (emailDto is null)
            {
                return Results.BadRequest($"HTTP Request Body musí obsahovat objekt {typeof(EmailDto)}.");
            }

            await emailService.SendEmailAsync(emailDto.Message, emailDto.Address, $"{emailDto.FirstName} {emailDto.LastName}", emailDto.Subject, MimeKit.Text.TextFormat.Plain, cancellationToken);

            return Results.NoContent();
        })
        .WithTags("Email")
        .WithName("SendEmail")
        .WithOpenApi(operation => new(operation) { Summary = "Send an email" })
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status400BadRequest);

        return app;
    }
}