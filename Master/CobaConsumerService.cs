using Confluent.Kafka;

namespace simpleApi.Basic;

public class CobaConsumerService : BackgroundService
{
    private readonly ConsumerConfig _consumerConfig;
    private readonly ILogger<CobaConsumerService> _logger;

    public CobaConsumerService(ConsumerConfig consumerConfig, ILogger<CobaConsumerService> logger)
    {
        _consumerConfig = consumerConfig ?? throw new ArgumentNullException(nameof(consumerConfig));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var consumer = new ConsumerBuilder<Ignore, string>(_consumerConfig).Build();

        consumer.Subscribe("your_kafka_topic");

        try
        {
            while (!stoppingToken.IsCancellationRequested)
                try
                {
                    var consumeResult = consumer.Consume(stoppingToken);

                    if (consumeResult != null && consumeResult.Message != null)
                    {
                        var message = consumeResult.Message.Value;
                        _logger.LogInformation($"Received message: {message}");
                        // Add your message processing logic here
                        // For example, you can pass the message to another service or repository
                        // var result = _someService.ProcessMessage(message);
                    }
                }
                catch (ConsumeException ex)
                {
                    _logger.LogError($"Error consuming message: {ex.Error.Reason}");
                }
                catch (OperationCanceledException)
                {
                    // Handle cancellation
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error processing message: {ex.Message}");
                }
        }
        finally
        {
            consumer.Close();
        }
    }
}