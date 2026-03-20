using Microsoft.EntityFrameworkCore;
using NotificationsService.Endpoints;
using NotificationsService.Infrastructure;
using NotificationsService.Messaging;
using NotificationsService.Realtime;
using NotificationsService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<NotificationsDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres"))
);
builder.Services.AddSignalR();
builder.Services.AddScoped<NotificationRepository>();
builder.Services.AddScoped<NotificationService>();

builder.Services.AddHostedService<EventConsumer>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider
        .GetRequiredService<NotificationsDbContext>();

    db.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGet("/", () => "NotificationsService running");
app.MapHub<NotificationsHub>("/ws/notifications");
app.MapNotificationEndpoints();

app.Run();