namespace simpleApi.Models;

public class KafkaConfig
{
    public string Network { get; set; }
    public string Broker { get; set; }
    public string Group { get; set; }
    public int Partition { get; set; }
    public string Topic { get; set; }
}