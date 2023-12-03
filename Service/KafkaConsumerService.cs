using Confluent.Kafka;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using simpleApi.Models;

public class KafkaConsumerService
{
    private readonly IOptions<KafkaConfig> _kafkaConfig;
    private readonly ConsumerConfig _consumerConfig;

    public KafkaConsumerService(IOptions<KafkaConfig> kafkaConfig)
    {
        _kafkaConfig = kafkaConfig;

        _consumerConfig = new ConsumerConfig
        {
            BootstrapServers = _kafkaConfig.Value.Broker,
            GroupId = _kafkaConfig.Value.Group,
            AutoOffsetReset = AutoOffsetReset.Earliest
        };
    }

    public async Task ConsumeMessages(CancellationToken cancellationToken)
    {
        using var consumer = new ConsumerBuilder<Ignore, string>(_consumerConfig).Build();
        consumer.Subscribe(_kafkaConfig.Value.Topic);

        while (!cancellationToken.IsCancellationRequested)
            try
            {
                var result = consumer.Consume(cancellationToken);

                if (result.Message != null)
                {
                    Console.WriteLine(
                        $"Consumed message '{result.Value}' from topic '{result.TopicPartition.Topic}' at: '{result.TopicPartitionOffset}'.");

                    // Assuming the message is in JSON format, you can deserialize it to your model
                    var album = JsonConvert.DeserializeObject<Album>(result.Value);
                    // Perform any necessary processing with the deserialized object
                    // ...
                }
            }
            catch (OperationCanceledException)
            {
                // The cancellation token was triggered, exit the loop
                break;
            }
            catch (ConsumeException e)
            {
                Console.WriteLine($"Error while consuming message: {e.Error.Reason}");
            }
    }
}