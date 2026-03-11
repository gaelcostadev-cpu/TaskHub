using Microsoft.AspNetCore.Connections;
using NotificationsService.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading.Channels;

namespace NotificationsService.Messaging;

public class EventConsumer : BackgroundService
{
    private readonly IConfiguration _configuration;
    private readonly NotificationService _notificationService;
    private readonly ILogger<EventConsumer> _logger;

    private IConnection? _connection;
    private IChannel? _channel;

    private const int MaxRetries = 5;

    public EventConsumer(
        IConfiguration configuration,
        NotificationService notificationService,
        ILogger<EventConsumer> logger)
    {
        _configuration = configuration;
        _notificationService = notificationService;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = _configuration["RabbitMq:Host"] ?? "localhost",
            UserName = _configuration["RabbitMq:Username"] ?? "guest",
            Password = _configuration["RabbitMq:Password"] ?? "guest"
        };

        _connection = await factory.CreateConnectionAsync();
        _channel = await _connection.CreateChannelAsync();

        await QueueSetup.ConfigureAsync(_channel);

        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.ReceivedAsync += async (sender, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var routingKey = ea.RoutingKey;

            var retryCount = GetRetryCount(ea);

            try
            {
                await _notificationService.HandleAsync(routingKey, message);

                await _channel.BasicAckAsync(ea.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing event");

                if (retryCount < MaxRetries)
                {
                    await RetryMessage(ea, body, retryCount + 1);
                }
                else
                {
                    await SendToDeadLetter(ea, body);
                }

                await _channel.BasicAckAsync(ea.DeliveryTag, false);
            }
        };

        await _channel.BasicConsumeAsync(
            queue: QueueSetup.NotificationsQueue,
            autoAck: false,
            consumer: consumer
        );
    }

    private int GetRetryCount(BasicDeliverEventArgs ea)
    {
        if (ea.BasicProperties.Headers != null &&
            ea.BasicProperties.Headers.TryGetValue("x-retry-count", out var value))
        {
            return Convert.ToInt32(value);
        }

        return 0;
    }

    private async Task RetryMessage(BasicDeliverEventArgs ea, byte[] body, int retryCount)
    {
        var props = new BasicProperties
        {
            Headers = new Dictionary<string, object?>
            {
                ["x-retry-count"] = retryCount
            },
            ContentType = "application/json",
            DeliveryMode = DeliveryModes.Persistent
        };

        await _channel!.BasicPublishAsync(
            exchange: QueueSetup.RetryExchange,
            routingKey: ea.RoutingKey,
            mandatory: false,
            basicProperties: props,
            body: body
        );
    }

    private async Task SendToDeadLetter(BasicDeliverEventArgs ea, byte[] body)
    {
        var props = new BasicProperties
        {
            ContentType = ea.BasicProperties.ContentType,
            DeliveryMode = ea.BasicProperties.DeliveryMode,
            Headers = ea.BasicProperties.Headers
        };

        await _channel!.BasicPublishAsync(
            exchange: QueueSetup.DeadExchange,
            routingKey: ea.RoutingKey,
            mandatory: false,
            basicProperties: props,
            body: body
        );
    }

    public override void Dispose()
    {
        _channel?.CloseAsync().GetAwaiter().GetResult();
        _connection?.CloseAsync().GetAwaiter().GetResult();
        base.Dispose();
    }
}