using simpleApi.Models;

namespace simpleApi.Service;

using Confluent.Kafka;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

public class KafkaProducerService
{
    private readonly IOptions<KafkaConfig> _kafkaConfig;
    private readonly ProducerConfig _producerConfig;
    protected readonly ILogger<KafkaProducerService> logger;

    public KafkaProducerService(IOptions<KafkaConfig> kafkaConfig, ILogger<KafkaProducerService> logger)
    {
        this.logger = logger;
        _kafkaConfig = kafkaConfig;

        _producerConfig = new ProducerConfig
        {
            BootstrapServers = _kafkaConfig.Value.Broker,
            MessageTimeoutMs = 5000,
            RequestTimeoutMs = 5000,
            EnableDeliveryReports = true
        };
    }

    public async Task ProduceMessageAsync(string message)
    {
        try
        {
            using var producer = new ProducerBuilder<Null, string>(_producerConfig)
                .SetErrorHandler((_, e) =>
                {
                    // Handle errors here (optional)
                    Console.WriteLine($"Error: {e.Reason}");
                })
                .Build();

            var deliveryResult = await producer.ProduceAsync(_kafkaConfig.Value.Topic,
                new Message<Null, string> { Value = message });

            if (deliveryResult.Status != PersistenceStatus.Persisted)
            {
                // Handle delivery error
                Console.WriteLine(
                    $"Failed to produce message. Error: {deliveryResult.Status == PersistenceStatus.Persisted}");

                // Set the response message when an error occurs
                // You can customize this response based on your requirements
                throw new Exception($"Failed to produce message. Error: {deliveryResult.Status}");
            }
            else
            {
                // Message successfully produced
                Console.WriteLine($"Produced message to {deliveryResult.TopicPartitionOffset}");
            }
        }
        catch (Exception ex)
        {
            // Handle exceptions here
            Console.WriteLine($"Exception: {ex.Message}");

            // Set the response message when an exception occurs
            // You can customize this response based on your requirements
            throw new Exception("An error occurred while producing the message.");
        }
    }
}