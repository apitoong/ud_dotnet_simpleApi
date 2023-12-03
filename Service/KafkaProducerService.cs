using AutoMapper;
using Newtonsoft.Json;
using simpleApi.Basic;
using simpleApi.Helpers;
using simpleApi.Models;

namespace simpleApi.Service;

using Confluent.Kafka;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

public class KafkaProducerService
{
    protected readonly BasicLogger _basicLogger;
    protected readonly BasicConfiguration _basicConfiguration;
    protected string _source;
    protected readonly IMapper _mapper;

    private readonly ProducerConfig _producerConfig;
    private static IProducer<Null, string> _producerInstance;


    public KafkaProducerService(BasicLogger basicLogger, BasicConfiguration basicConfiguration, IMapper mapper)
    {
        _basicLogger = basicLogger;
        _basicConfiguration = basicConfiguration;
        _source = GetType().Name;
        _producerConfig = setProducerConfig();
        _producerInstance = createProducer();
    }

    private ProducerConfig setProducerConfig()
    {
        return new ProducerConfig
        {
            BootstrapServers = _basicConfiguration.GetVariable("KAFKA_BROKER"),
            MessageTimeoutMs = Helper.StringToInteger(_basicConfiguration.GetVariable("KAFKA_MESSAGE_TIMEOUT")),
            RequestTimeoutMs = Helper.StringToInteger(_basicConfiguration.GetVariable("KAFKA_REQUEST_TIMEOUT")),
            EnableDeliveryReports = true
        };
    }

    private IProducer<Null, string> createProducer()
    {
        return new ProducerBuilder<Null, string>(_producerConfig)
            .SetErrorHandler((_, e) =>
            {
                // Handle errors here (optional)
                Console.WriteLine($"Error: {e.Reason}");
            })
            .Build();
    }

    public async Task ProduceMessageAsync(string topic, KafkaMessage message)
    {
        try
        {
            var jsonkafkaMessage = JsonConvert.SerializeObject(message);
            var deliveryResult = await _producerInstance.ProduceAsync(topic,
                new Message<Null, string> { Value = jsonkafkaMessage });
            string messageLog;
            if (deliveryResult.Status != PersistenceStatus.Persisted)
            {
                // Handle delivery error
                messageLog =
                    $"Failed to produce message. Error: {deliveryResult.Status == PersistenceStatus.Persisted}";
                _basicLogger.Log("Error", "Kafka Producer", _source, messageLog, message);
                throw new Exception($"Failed to produce message. Error: {deliveryResult.Status}");
            }

            // Message successfully produced
            messageLog = $"Produced message to {deliveryResult.TopicPartitionOffset}";
            _basicLogger.Log("Information", "Kafka Producer", _source, messageLog, message);
        }
        catch (Exception ex)
        {
            // Handle exceptions here
            var messageLog =
                $"Failed to produce message. Error Exception";
            _basicLogger.Log("Error", "Kafka Producer", _source, messageLog, ex.Message);
            // Set the response message when an exception occurs
            // You can customize this response based on your requirements
            throw new Exception("An error occurred while producing the message.");
        }
    }
}