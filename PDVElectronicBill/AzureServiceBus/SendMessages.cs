using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Logging;

namespace AzureServiceBus;

public class AzureServicesBusHandler : IAzureServicesBusHandler
{
    
    public async Task PublishToMessageQueue(string message, Queue queue, ILogger? logger)
    {
        // The Service Bus client types are safe to cache and use as a singleton for the lifetime
        // of the application, which is best practice when messages are being published or read
        // regularly.
        //
        // Set the transport type to AmqpWebSockets so that the ServiceBusClient uses the port 443. 
        // If you use the default AmqpTcp, ensure that ports 5671 and 5672 are open.
        var clientOptions = new ServiceBusClientOptions
        {
            TransportType = ServiceBusTransportType.AmqpWebSockets
        };

        await using var client = new ServiceBusClient(
                Environment.GetEnvironmentVariable("PDVSBprod_SERVICEBUS"),
                clientOptions);

        await using var sender = client.CreateSender(queue.Value);

        try
        {
            // Use the producer client to send the batch of messages to the Service Bus queue
            await sender.SendMessageAsync(new ServiceBusMessage(message));
            Console.WriteLine($"Sent message: {message}");
        }
        catch (Exception ex)
        {
            logger?.LogError("Error sending message: {ex}", ex);
        }
    }
}

public interface IAzureServicesBusHandler
{
    Task PublishToMessageQueue(string message, Queue queue, ILogger? logger);
}

public class Queue
{
    private Queue(string value) { Value = value; }

    public string Value { get; private set; }

    public static Queue SendElectronicBill   { get { return new Queue("product.sendelectronicbill"); } }
    public static Queue SendEmail   { get { return new Queue("product-sendemail"); } }

    public override string ToString()
    {
        return Value;
    }
}