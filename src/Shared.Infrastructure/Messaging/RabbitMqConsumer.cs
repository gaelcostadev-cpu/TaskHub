using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using Shared.Contracts.Events;

namespace Shared.Infrastructure.Messaging;

public abstract class RabbitMqConsumer<T>
{
    private readonly IConnection _connection;
    private readonly IChannel _channel;

    protected RabbitMqConsumer(IConfiguration configuration)
    {
        var factory = new ConnectionFactory
        {
            HostName = configuration["RabbitMq:Host"],
            UserName = configuration["RabbitMq:Username"],
            Password = configuration["RabbitMq:Password"]
        };

        _connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
        _channel = _connection.CreateChannelAsync().GetAwaiter().GetResult();
    }

    protected async Task StartConsuming(string queue, string routingKey)
    {
        await _channel.QueueDeclareAsync(
            queue,
            durable: true,
            exclusive: false,
            autoDelete: false
        );

        await _channel.QueueBindAsync(
            queue,
            "taskhub.events",
            routingKey
        );

        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.ReceivedAsync += async (sender, args) =>
        {
            var body = Encoding.UTF8.GetString(args.Body.ToArray());

            var envelope = JsonSerializer.Deserialize<EventEnvelope<T>>(body);

            if (envelope != null)
                await HandleAsync(envelope.Data);

            await _channel.BasicAckAsync(args.DeliveryTag, false);
        };

        await _channel.BasicConsumeAsync(queue, false, consumer);
    }

    protected abstract Task HandleAsync(T message);
}