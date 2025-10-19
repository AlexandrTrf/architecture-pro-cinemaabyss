namespace proxy;

public class WeightedDownstreamHostAndPort
{
    public string Host { get; set; }
    
    public int Port { get; set; }
    
    public int Weight { get; set; }
}