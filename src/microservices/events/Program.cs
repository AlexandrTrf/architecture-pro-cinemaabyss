using events.Config;
using events.Services;

var builder = WebApplication.CreateBuilder(args);

// Конфигурация Kafka из переменных окружения
builder.Services.Configure<KafkaConfig>(builder.Configuration.GetSection("Kafka"));

// Добавляем Kafka Producer и Consumer
builder.Services.AddSingleton<KafkaProducerService>();
builder.Services.AddHostedService<KafkaConsumerService>();

// Контроллеры
builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();
app.Run();