using System.Runtime.CompilerServices;
using simpleApi.Basic;

namespace simpleApi.Service;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

public class BackgroundProcessService : BackgroundService
{
    private readonly ILogger<BackgroundService> _logger;
    protected readonly KafkaConsumerService _kafkaConsumerService;

    public BackgroundProcessService(ILogger<BackgroundProcessService> logger, KafkaConsumerService kafkaConsumerService)
    {
        _logger = logger;
        _kafkaConsumerService = kafkaConsumerService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
            _logger.LogInformation("Proses di belakang sedang berjalan pada: {time} -=>", DateTimeOffset.Now);
        // await _kafkaConsumerService.ConsumeMessages(stoppingToken);
        // Tambahkan logika atau proses Anda di sini
        // await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
    }
}