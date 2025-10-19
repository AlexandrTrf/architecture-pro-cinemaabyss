using Microsoft.Extensions.Options;
using Ocelot.Configuration;
using Ocelot.LoadBalancer.LoadBalancers;
using Ocelot.Responses;
using Ocelot.Values;
using proxy;

public class WeightedRoundRobinLoadBalancer : ILoadBalancer
{
    private readonly List<WeightedDownstreamHostAndPort> _options;
    private readonly List<(DownstreamHostAndPort Host, int Weight)> _hosts;
    private int _currentIndex = -1;
    private int _currentWeightCounter = 0;
    private int _maxWeight;
    private int _gcdWeight;

    public WeightedRoundRobinLoadBalancer(List<DownstreamHostAndPort> hosts, IOptions<List<WeightedDownstreamHostAndPort>>  options)
    {
        if (hosts == null || !hosts.Any())
            throw new ArgumentException("Hosts cannot be empty");
        _options = options.Value;

        // Читаем вес через рефлексию
        _hosts = hosts.Select(h =>
        {
            var opt = _options.FirstOrDefault(o => o.Host == h.Host);
            return (h, opt?.Weight ?? 1);
        }).ToList();
        
        _maxWeight = _hosts.Max(x => x.Weight);
        _gcdWeight = GCD(_hosts.Select(x => x.Weight).ToList());
    }
    
    public Task<Response<ServiceHostAndPort>> LeaseAsync(HttpContext httpContext)
    {
        
        ServiceHostAndPort host = null;

        while (true)
        {
            _currentIndex = (_currentIndex + 1) % _hosts.Count;
            if (_currentIndex == 0)
            {
                _currentWeightCounter -= _gcdWeight;
                if (_currentWeightCounter <= 0)
                    _currentWeightCounter = _maxWeight;
            }

            if (_hosts[_currentIndex].Weight >= _currentWeightCounter)
            {
                host = new ServiceHostAndPort(_hosts[_currentIndex].Host.Host, _hosts[_currentIndex].Host.Port);
                break;
            }
        }

        return Task.FromResult<Response<ServiceHostAndPort>>(new OkResponse<ServiceHostAndPort>(host));
    }

    public void Release(ServiceHostAndPort host)
    {
        // ничего не делаем
    }

    public string Type { get; } = "WeightedRoundRobin";

    private int GCD(List<int> numbers)
    {
        int gcd = numbers[0];
        for (int i = 1; i < numbers.Count; i++)
        {
            gcd = GCD(gcd, numbers[i]);
        }
        return gcd;
    }

    private int GCD(int a, int b)
    {
        while (b != 0)
        {
            int t = b;
            b = a % b;
            a = t;
        }
        return a;
    }
}
