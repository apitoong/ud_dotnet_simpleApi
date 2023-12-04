using Confluent.Kafka;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using simpleApi.Basic;
using simpleApi.Models;

public class KafkaConsumerService
{
    private static KafkaConsumerService _instance;
    protected readonly BasicLogger _basicLogger;
    protected readonly BasicConfiguration _basicConfiguration;
    protected string _source;
    protected List<string> _topic;
    private readonly ConsumerConfig _consumerConfig;
    private readonly IConsumer<Ignore, string> _consumerService;
    private readonly CancellationTokenSource _cancellationTokenSource;


    public KafkaConsumerService(BasicLogger basicLogger, BasicConfiguration basicConfiguration)
    {
        _basicLogger = basicLogger;
        _basicConfiguration = basicConfiguration;
        _consumerConfig = setConsumerConfig();
        _topic = setTopic();
        _consumerService = setConsummerService();
        _consumerService.Subscribe(_topic);
        _cancellationTokenSource = setCancellation();
        Start();
    }

    private ConsumerConfig setConsumerConfig()
    {
        return new ConsumerConfig
        {
            BootstrapServers = _basicConfiguration.GetVariable("KAFKA_BROKER"),
            GroupId = _basicConfiguration.GetVariable("KAFKA_GROUP"),
            AutoOffsetReset = AutoOffsetReset.Earliest, // atau Latest, tergantung kebutuhan Anda
            EnableAutoCommit = false
            // EnableAutoOffsetStore = false,
            // EnableAutoCommit = true,
            // StatisticsIntervalMs = 5000,
            // SessionTimeoutMs = 6000,
            // AutoOffsetReset = AutoOffsetReset.Earliest,
            // EnablePartitionEof = true,
            // PartitionAssignmentStrategy = PartitionAssignmentStrategy.CooperativeSticky
        };
    }


    private List<string> setTopic()
    {
        // register topic yang akan di konsume
        return new List<string> { _basicConfiguration.GetVariable("KAFKA_TOPIC") };
    }

    private IConsumer<Ignore, string> setConsummerService()
    {
        var result = new ConsumerBuilder<Ignore, string>(_consumerConfig).Build();
        return result;
    }

    private CancellationTokenSource setCancellation()
    {
        return new CancellationTokenSource();
    }

    //
    // public async Task ConsumeMessages(CancellationToken stoppingToken)
    // {
    //     using var consumer = new ConsumerBuilder<Ignore, string>(_consumerConfig).Build();
    //     consumer.Subscribe(_topic);
    //     string messageLog;
    //     // _basicLogger.Log("Information", "Kafka Consumer", _source,
    //     //     $"Subscribed to topics: {string.Join(", ", _topic)}");
    //
    //     while (!stoppingToken.IsCancellationRequested)
    //         try
    //         {
    //             _basicLogger.Log("Information", "Kafka Consumer", _source,
    //                 $"Subscribed to topics: {string.Join(", ", _topic)}", !stoppingToken.IsCancellationRequested);
    //             var result = consumer.Consume(stoppingToken);
    //             if (result.Message != null)
    //             {
    //                 messageLog =
    //                     $"Consumed message '{result.Value}' from topic '{result.TopicPartition.Topic}' at: '{result.TopicPartitionOffset}'.";
    //                 _basicLogger.Log("Information", "Kafka Consummer", _source, messageLog);
    //                 // Handle messages based on topic
    //                 HandleMessage(result.TopicPartition.Topic, result.Value);
    //             }
    //         }
    //         catch (OperationCanceledException e)
    //         {
    //             // The cancellation token was triggered, exit the loop
    //             messageLog =
    //                 $"Failed to consume message. Error OperationCanceledException :\n{e.Message}";
    //             _basicLogger.Log("Error", "Kafka Consumer", _source, messageLog);
    //             break;
    //         }
    //         catch (ConsumeException e)
    //         {
    //             messageLog =
    //                 $"Failed to consume message. Error ConsumeException:\n{e.Message}";
    //             _basicLogger.Log("Error", "Kafka Consumer", _source, messageLog);
    //         }
    // }
    public void Start()
    {
        Task.Run(() => ConsumeMessages(_cancellationTokenSource.Token));
    }

    public void Stop()
    {
        _cancellationTokenSource.Cancel();
        _consumerService.Close();
    }

    private void ConsumeMessages(CancellationToken cancellationToken)
    {
        try
        {
            while (!cancellationToken.IsCancellationRequested)
                try
                {
                    var consumeResult = _consumerService.Consume(cancellationToken);

                    if (consumeResult != null)
                    {
                        // Proses pesan di sini
                        Console.WriteLine(
                            $"Consumed message from topic {consumeResult.Topic}: {consumeResult.Message.Value}");

                        // Setelah berhasil memproses, tentukan offset secara manual
                        _consumerService.Commit(consumeResult);
                    }
                }
                catch (ConsumeException ex)
                {
                    // Handle kesalahan konsumsi di sini
                    Console.WriteLine($"Error consuming message: {ex.Error.Reason}");
                }
        }
        catch (OperationCanceledException)
        {
            // Task dibatalkan, akan keluar dari loop
        }
        finally
        {
            _consumerService.Close();
        }
    }

    public void HandleMessage(string topic, string message)
    {
        // Implement your logic to handle messages based on the topic
        switch (topic)
        {
            case "TOPIC_KAFKA_DOTNET":
                HandleDotNetTopic(message);
                break;
            case "TOPIC_KAFKA_PAYMENT":
                HandlePaymentTopic(message);
                break;
                // Add more cases as needed
        }
    }

    private void HandleDotNetTopic(string message)
    {
        // Implement logic for handling messages from TOPIC_KAFKA_DOTNET
        Console.WriteLine($"Handling message from TOPIC_KAFKA_DOTNET: {message}");
        // Add your specific logic here
    }

    private void HandlePaymentTopic(string message)
    {
        // Implement logic for handling messages from TOPIC_KAFKA_PAYMENT
        Console.WriteLine($"Handling message from TOPIC_KAFKA_PAYMENT: {message}");
        // Add your specific logic here
    }
}