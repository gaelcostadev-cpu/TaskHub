namespace Shared.Infrastructure.Messaging;

public interface IMessageBus
{
    Task PublishAsync<T>(T message, string routingKey);
}