using Confluent.Kafka;
using events.Config;
using Microsoft.Extensions.Options;

namespace events.Services
{
    public class KafkaConsumerService : BackgroundService
    {
        private readonly KafkaConfig _config;
        private readonly ILogger<KafkaConsumerService> _logger;

        public KafkaConsumerService(IOptions<KafkaConfig> config, ILogger<KafkaConsumerService> logger)
        {
            _config = config.Value;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumerConfig = new ConsumerConfig
            {
                BootstrapServers = _config.BootstrapServers,
                GroupId = "events-service-consumer",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            using var consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build();
            consumer.Subscribe(new[] { _config.TopicMovie, _config.TopicUser, _config.TopicPayment });

            _logger.LogInformation("Kafka consumer started. Listening topics: {Topics}", 
                $"{_config.TopicMovie}, {_config.TopicUser}, {_config.TopicPayment}");

            try
            {
                // Асинхронный цикл обработки сообщений
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        // Consume блокирует, поэтому оборачиваем в Task.Run чтобы не блокировать BackgroundService
                        var result = await Task.Run(() => consumer.Consume(stoppingToken), stoppingToken);

                        if (result != null)
                        {
                            _logger.LogInformation("Consumed message from {Topic}: {Message}", 
                                result.Topic, result.Message.Value);
                        }
                    }
                    catch (ConsumeException e)
                    {
                        _logger.LogError(e, "Error consuming Kafka message");
                    }

                    // Минимальная пауза, чтобы не перегружать поток
                    await Task.Delay(50, stoppingToken);
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Kafka consumer stopping...");
            }
            finally
            {
                consumer.Close();
            }
        }
    }
}
