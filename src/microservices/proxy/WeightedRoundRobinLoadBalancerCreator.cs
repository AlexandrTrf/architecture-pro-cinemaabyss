using Microsoft.Extensions.Options;
using Ocelot.Configuration;
using Ocelot.LoadBalancer.LoadBalancers;
using Ocelot.Responses;
using Ocelot.ServiceDiscovery.Providers;

namespace proxy;

public class WeightedRoundRobinLoadBalancerCreator : ILoadBalancerCreator
{
    private readonly IOptions<List<WeightedDownstreamHostAndPort>> _downstreamHostAndPorts;

    public WeightedRoundRobinLoadBalancerCreator(IOptions<List<WeightedDownstreamHostAndPort>> downstreamHostAndPorts)
    {
        _downstreamHostAndPorts = downstreamHostAndPorts;
    }
    
    public bool Supports(string type) => type.Equals("WeightedRoundRobin", StringComparison.OrdinalIgnoreCase);

    public Response<ILoadBalancer> Create(DownstreamRoute route, IServiceDiscoveryProvider serviceProvider)
    {
        return new OkResponse<ILoadBalancer>(new WeightedRoundRobinLoadBalancer(route.DownstreamAddresses,_downstreamHostAndPorts));
    }

    public string Type { get; }= "WeightedRoundRobin";
}