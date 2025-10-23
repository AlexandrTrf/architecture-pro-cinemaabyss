using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Ocelot.LoadBalancer.LoadBalancers;
using proxy;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();

// Чтение переменных окружения
var moviesHost = builder.Configuration["MOVIES_HOST"] ?? "movies-service";
var moviesPort = int.Parse(builder.Configuration["MOVIES_PORT"] ?? "8081");
var moviesWeight = int.Parse(builder.Configuration["MOVIES_WEIGHT"] ?? "50");

var monolithHost = builder.Configuration["MONOLITH_HOST"] ?? "monolith";
var monolithPort = int.Parse(builder.Configuration["MONOLITH_PORT"] ?? "8080");
var monolithWeight = int.Parse(builder.Configuration["MONOLITH_WEIGHT"] ?? "50");

var eventsHost = builder.Configuration["EVENTS_HOST"] ?? "events-service";
var eventsPort = int.Parse(builder.Configuration["EVENTS_PORT"] ?? "8082");

var weightedDownstreamHostAndPorts = new List<WeightedDownstreamHostAndPort>
{
    new WeightedDownstreamHostAndPort
    {
        Host = moviesHost, Port = moviesPort, Weight = moviesWeight
    },
    new WeightedDownstreamHostAndPort
    {
        Host = monolithHost, Port = monolithPort, Weight = monolithWeight
    }
};

// Формируем Ocelot конфигурацию с весами
var ocelotConfig = new
{
    Routes = new List<object>
    {
        new {
            RouteId = "movies_route",
            UpstreamPathTemplate = "/api/movies/{everything}",
            UpstreamHttpMethod = new[] { "GET", "PUT", "POST", "DELETE" },
            DownstreamPathTemplate = "/api/movies/{everything}",
            LoadBalancerOptions = new { Type = "WeightedRoundRobin" },
            DownstreamScheme = "http",
            DownstreamHostAndPorts = new object[]
            {
                new { Host = moviesHost, Port = moviesPort, Weight = moviesWeight },
                new { Host = monolithHost, Port = monolithPort, Weight = monolithWeight }
            }
        },
        new {
            RouteId = "events_route",
            UpstreamPathTemplate = "/api/events/{everything}",
            UpstreamHttpMethod = new[] { "GET", "PUT", "POST", "DELETE" },
            DownstreamPathTemplate = "/api/events/{everything}",
            DownstreamScheme = "http",
            DownstreamHostAndPorts = new object[]
            {
                new { Host = eventsHost, Port = eventsPort }
            }
        },
        new {
            RouteId = "default_route",
            UpstreamPathTemplate = "/{everything}",
            UpstreamHttpMethod = new[] { "GET", "POST", "PUT", "DELETE" },
            DownstreamPathTemplate = "/{everything}",
            DownstreamScheme = "http",
            DownstreamHostAndPorts = new object[]
            {
                new { Host = monolithHost, Port = monolithPort }
            },
            Priority = 0
        }
    },
    GlobalConfiguration = new
    {
        BaseUrl = "http://localhost:8000"
    }
};

// Сохраняем JSON и подключаем к конфигурации
var ocelotFilePath = Path.Combine(AppContext.BaseDirectory, "ocelot.generated.json");
var ocelotContent = JsonSerializer.Serialize(ocelotConfig, new JsonSerializerOptions {WriteIndented = true});
Console.WriteLine(ocelotContent);
await File.WriteAllTextAsync(ocelotFilePath, ocelotContent);
builder.Configuration.AddJsonFile(ocelotFilePath, optional: false, reloadOnChange: false);

builder.Services.AddSingleton(Options.Create(weightedDownstreamHostAndPorts));

builder.Services.AddSingleton<ILoadBalancerCreator, WeightedRoundRobinLoadBalancerCreator>();

builder.Services.AddOcelot(builder.Configuration);

builder.Services.AddControllers();
var app = builder.Build();
await app.UseOcelot();
app.Run();
