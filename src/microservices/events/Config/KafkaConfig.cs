namespace events.Config;

public class KafkaConfig
{
    public string BootstrapServers { get; set; } = string.Empty;
    public string TopicMovie { get; set; } = "movie-events";
    public string TopicUser { get; set; } = "user-events";
    public string TopicPayment { get; set; } = "payment-events";
}