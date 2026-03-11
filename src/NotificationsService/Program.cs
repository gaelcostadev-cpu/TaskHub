using NotificationsService.Messaging;
using NotificationsService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<NotificationService>();

builder.Services.AddHostedService<EventConsumer>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGet("/", () => "NotificationsService running");

app.Run();