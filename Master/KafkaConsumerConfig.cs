using Confluent.Kafka;

namespace simpleApi.Basic;

public static class KafkaConsumerConfig
{
    public static ConsumerConfig GetConfig()
    {
        return new ConsumerConfig
        {
            BootstrapServers = "localhost:29092", // Replace with your Kafka broker address
            GroupId = "GROUP_KAFKA_DOTNET",
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false
        };
    }
}