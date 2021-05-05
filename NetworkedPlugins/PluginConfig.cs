using Exiled.API.Interfaces;

namespace NetworkedPlugins
{
    public class PluginConfig : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool IsHost { get; set; } = false;
        public string hostConnectionKey { get; set; } = "UNKNOWN_KEY";
        public string hostAddress { get; set; } = "localhost";
        public ushort hostPort { get; set; } = 7777;
    }
}
