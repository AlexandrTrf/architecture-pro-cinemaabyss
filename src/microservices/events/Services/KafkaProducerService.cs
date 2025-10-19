using Confluent.Kafka;
using events.Config;
using Microsoft.Extensions.Options;

namespace events.Services
{
    public class KafkaProducerService
    {
        private readonly IProducer<Null, string> _producer;
        private readonly KafkaConfig _config;
        private readonly ILogger<KafkaProducerService> _logger;

        public KafkaProducerService(IOptions<KafkaConfig> config, ILogger<KafkaProducerService> logger)
        {
            _config = config.Value;
            _logger = logger;

            var producerConfig = new ProducerConfig
            {
                BootstrapServers = _config.BootstrapServers
            };

            _producer = new ProducerBuilder<Null, string>(producerConfig).Build();
        }

        public async Task<(DeliveryResult<Null, string>?, bool)> SendMessageAsync(string topic, string message)
        {
            try
            {
                var result = await _producer.ProduceAsync(topic, new Message<Null, string> { Value = message });
                _logger.LogInformation("Message delivered to {TopicPartitionOffset}", result.TopicPartitionOffset);
                return (result, true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error producing message to Kafka topic {Topic}", topic);
                return (null, false);
            }
        }
    }
}