using RabbitMQ.Client;
using System.Threading.Channels;

namespace NotificationsService.Messaging;

public static class QueueSetup
{
    public const string EventsExchange = "taskhub.events";
    public const string RetryExchange = "taskhub.retry";
    public const string DeadExchange = "taskhub.dead";

    public const string NotificationsQueue = "taskhub.notifications";
    public const string RetryQueue = "taskhub.notifications.retry";
    public const string DeadQueue = "taskhub.notifications.dead";

    public static async Task ConfigureAsync(IChannel channel)
    {
        // Exchanges
        await channel.ExchangeDeclareAsync(
            exchange: EventsExchange,
            type: ExchangeType.Topic,
            durable: true
        );

        await channel.ExchangeDeclareAsync(
            exchange: RetryExchange,
            type: ExchangeType.Topic,
            durable: true
        );

        await channel.ExchangeDeclareAsync(
            exchange: DeadExchange,
            type: ExchangeType.Topic,
            durable: true
        );

        // MAIN QUEUE
        var mainArgs = new Dictionary<string, object?>
        {
            { "x-dead-letter-exchange", RetryExchange }
        };

        await channel.QueueDeclareAsync(
            queue: NotificationsQueue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: mainArgs
        );

        // RETRY QUEUE
        var retryArgs = new Dictionary<string, object?>
        {
            { "x-dead-letter-exchange", EventsExchange },
            { "x-message-ttl", 10000 } // 10 segundos
        };

        await channel.QueueDeclareAsync(
            queue: RetryQueue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: retryArgs
        );

        // DEAD QUEUE
        await channel.QueueDeclareAsync(
            queue: DeadQueue,
            durable: true,
            exclusive: false,
            autoDelete: false
        );

        // Bindings
        await channel.QueueBindAsync(
            queue: NotificationsQueue,
            exchange: EventsExchange,
            routingKey: "task.*"
        );

        await channel.QueueBindAsync(
            queue: NotificationsQueue,
            exchange: EventsExchange,
            routingKey: "comment.*"
        );

        await channel.QueueBindAsync(
            queue: RetryQueue,
            exchange: RetryExchange,
            routingKey: "#"
        );

        await channel.QueueBindAsync(
            queue: DeadQueue,
            exchange: DeadExchange,
            routingKey: "#"
        );
    }
}