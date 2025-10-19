using Microsoft.AspNetCore.Mvc;
using events.Models;
using Microsoft.Extensions.Options;
using events.Config;
using System.Text.Json;
using events.Services;

namespace events.Controllers
{
    [ApiController]
    [Route("api/events")]
    public class EventsController : ControllerBase
    {
        private readonly KafkaProducerService _producer;
        private readonly KafkaConfig _config;

        public EventsController(KafkaProducerService producer, IOptions<KafkaConfig> config)
        {
            _producer = producer;
            _config = config.Value;
        }

        [HttpPost("movie")]
        public async Task<IActionResult> CreateMovieEvent([FromBody] MovieEvent movie)
        {
            var message = JsonSerializer.Serialize(movie);
            var (result, ok) = await _producer.SendMessageAsync(_config.TopicMovie, message);

            if (!ok) return StatusCode(500, new Error { ErrorMessage = "Kafka send failed" });

            return Created("", new EventResponse
            {
                Status = "success",
                Partition = result!.Partition.Value,
                Offset = result.Offset.Value,
                Event = new Event
                {
                    Payload = message
                }
            });
        }

        [HttpPost("user")]
        public async Task<IActionResult> CreateUserEvent([FromBody] UserEvent user)
        {
            var message = JsonSerializer.Serialize(user);
            var (result, ok) = await _producer.SendMessageAsync(_config.TopicUser, message);

            if (!ok) return StatusCode(500, new Error { ErrorMessage = "Kafka send failed" });

            return Created("", new EventResponse
            {
                Status = "success",
                Partition = result!.Partition.Value,
                Offset = result.Offset.Value,
                Event = new Event
                {
                    Payload = message
                }
            });
        }

        [HttpPost("payment")]
        public async Task<IActionResult> CreatePaymentEvent([FromBody] PaymentEvent payment)
        {
            var message = JsonSerializer.Serialize(payment);
            var (result, ok) = await _producer.SendMessageAsync(_config.TopicPayment, message);

            if (!ok) return StatusCode(500, new Error { ErrorMessage = "Kafka send failed" });

            return Created("", new EventResponse
            {
                Status = "success",
                Partition = result!.Partition.Value,
                Offset = result.Offset.Value,
                Event = new Event
                {
                    Payload = message
                }
            });
        }
    }
}
