using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.WebJobs.ServiceBus;
using Products.Interfaces;
using Azure.Messaging.ServiceBus;
using System.Text.Json;
using Products.Models;
using System.Text;

namespace PDV.ElectronicBill.Function;

public class ElectronicBillQueueTrigger(ILogger<ElectronicBillQueueTrigger> logger, IElectronicBill electronicBill, IHttpClientFactory httpClientFactory)
{
    private readonly ILogger<ElectronicBillQueueTrigger> _logger = logger;
    private readonly IElectronicBill _electronicBill = electronicBill;
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
    private string _baseURL = string.Empty;

    [FunctionName(nameof(ElectronicBillQueueTrigger))]
    public async Task Run([ServiceBusTrigger("product.sendelectronicbill", Connection = "PDVSBprod_SERVICEBUS")] ServiceBusReceivedMessage message,
        ServiceBusMessageActions messageActions)
    {
        _logger.LogInformation("Message ID: {id}", message.MessageId);
        _logger.LogInformation("Message Body: {body}", message.Body);
        _logger.LogInformation("Message Content-Type: {contentType}", message.ContentType);

        _baseURL = Environment.GetEnvironmentVariable("apiBaseURL") ?? throw new ArgumentException("apiBaseURL not found");

        Bill? bill = JsonSerializer.Deserialize<Bill>(message.Body);

        if (bill == null)
        {
            _logger.LogError("Error deserializing bill");
            return;
        }

        byte[]? certificate = await getCertificate(bill, CancellationToken.None);

        if (certificate == null)
        {
            _logger.LogError("Error getting certificate");
            return;
        }

        var comercio = await _httpClientFactory.CreateClient().GetAsync($"{_baseURL}/GetCommerceInformation", CancellationToken.None);

        var commersInfo = JsonSerializer.Deserialize<CommerceInformation>(await comercio.Content.ReadAsStringAsync());

        if (commersInfo == null)
        {
            _logger.LogError("Error deserializing commerce information");
            return;
        }

        var billFull = await _electronicBill.SendElectronicBill(bill, certificate, commersInfo.pinCertificado, CancellationToken.None);

        var serializedBillFull = JsonSerializer.Serialize(billFull);
        var content = new StringContent(serializedBillFull, Encoding.UTF8, "application/json");

        await _httpClientFactory.CreateClient().PutAsync($"{_baseURL}/AddElectronicBill", content);

        await new AzureServiceBus.AzureServicesBusHandler().PublishToMessageQueue(serializedBillFull, AzureServiceBus.Queue.SendEmail, _logger);

        Console.WriteLine($"Bill sent: {serializedBillFull}");

        // Complete the message
        await messageActions.CompleteMessageAsync(message);
    }

    private async Task<byte[]?> getCertificate(Bill bill, CancellationToken none)
    {
        var response = await _httpClientFactory.CreateClient().GetAsync($"{_baseURL}/GetCommerceInformation", none);

        var commersInfo = JsonSerializer.Deserialize<CommerceInformation>(await response.Content.ReadAsStringAsync());

        if (commersInfo == null)
        {
            _logger.LogError("Error deserializing commerce information");
            return null;
        }

        var certificate = await _httpClientFactory.CreateClient().GetAsync($"{_baseURL}/GetCertificate?commersName={commersInfo.name}", none);

        var stream = await certificate.Content.ReadAsStreamAsync();

        using var memoryStream = new MemoryStream();

        await stream.CopyToAsync(memoryStream);

        return memoryStream.ToArray();
    }
}
