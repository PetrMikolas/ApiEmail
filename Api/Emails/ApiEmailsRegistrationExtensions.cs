using ApiEmail.Services.Email;
using FluentValidation;
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
        app.MapPost("", async (
            [FromBody] EmailRequest request,
            [FromServices] IEmailService emailService,
            [FromServices] IValidator<EmailRequest> validator,
            CancellationToken cancellationToken) =>
        {
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
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
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status500InternalServerError);

        return app;
    }
}