using ApiEmail.Api.Emails;
using ApiEmail.Services.Email;
using FluentValidation;
using System.Reflection;
using static ApiEmail.Services.Email.EmailService;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseSentry(o =>
{
    o.Dsn = builder.Configuration["SentryDsn"]!;
    o.Debug = false;
    o.TracesSampleRate = 1.0;
});

// Add services to the container.
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services
    .AddOptions<EmailOptions>()
    .Bind(builder.Configuration.GetSection(EmailOptions.Key))
    .ValidateDataAnnotations();

builder.Services.AddScoped<IEmailService, EmailService>();

// Add CORS policy to allow specified origins
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        policy =>
        {
            policy.WithOrigins(builder.Configuration["CorsOrigins"]!)
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Not used in Docker – HTTPS is handled by the proxy (avoids warning or redirect loop)
//app.UseHttpsRedirection();

app.UseCors("CorsPolicy");

app.MapEndpointsEmails();

app.Run();