using Exiled.API.Features;
using NetworkedPlugins.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkedPlugins
{
    public class MainClass : Plugin<PluginConfig>
    {
        public override string Name { get; } = "NetworkedPlugins";
        public override string Prefix { get; } = "networkedplugins";
        public override string Author { get; } = "Killers0992";

        private NPClient client;
        private NPHost host;

        public override void OnEnabled()
        {
            Log.Info("Enabling NetworkedPlugins...");
            if (Config.IsHost)
                host = new NPHost(this);
            else
                client = new NPClient(this);
        }
    }
}
