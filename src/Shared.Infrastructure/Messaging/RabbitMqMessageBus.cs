using RabbitMQ.Client;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Text.Json;

namespace Shared.Infrastructure.Messaging;

public class RabbitMqMessageBus : IMessageBus, IDisposable
{
    private readonly IConnection _connection;
    private readonly IChannel _channel;

    public RabbitMqMessageBus(IConfiguration configuration)
    {
        var factory = new ConnectionFactory
        {
            HostName = configuration["RabbitMQ:Host"] ?? "localhost",
            UserName = configuration["RabbitMQ:Username"] ?? "guest",
            Password = configuration["RabbitMQ:Password"] ?? "guest"
        };

        _connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
        _channel = _connection.CreateChannelAsync().GetAwaiter().GetResult();

        _channel.ExchangeDeclareAsync(
            exchange: "taskhub.events",
            type: ExchangeType.Topic,
            durable: true
        ).GetAwaiter().GetResult();
    }

    public async Task PublishAsync<T>(T message, string routingKey)
    {
        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

        var props = new BasicProperties
        {
            ContentType = "application/json",
            DeliveryMode = DeliveryModes.Persistent
        };

        await _channel.BasicPublishAsync(
            exchange: "taskhub.events",
            routingKey: routingKey,
            mandatory: false,
            basicProperties: props,
            body: body
        );
    }

    public void Dispose()
    {
        _channel.CloseAsync().GetAwaiter().GetResult();
        _connection.CloseAsync().GetAwaiter().GetResult();
    }
}