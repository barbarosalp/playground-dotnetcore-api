namespace Barb.Core.Api.Configuration
{
    public class ApplicationConfiguration
    {
        public string Name { get; set; }
        public string Version { get; set; }
        public int Port { get; set; }
        public string RedisHost { get; set; }
        public int RedisPort { get; set; }
        public string KafkaHost { get; set; }
        public int KafkaPort { get; set; }
    }
}