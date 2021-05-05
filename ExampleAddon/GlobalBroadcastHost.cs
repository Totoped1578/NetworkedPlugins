using LiteNetLib;
using LiteNetLib.Utils;
using NetworkedPlugins.API;
using NetworkedPlugins.API.Attributes;
using NetworkedPlugins.API.Models;

namespace ExampleAddon
{
    [NPAddonInfo(
        addonID = "0dewadopsdap32",
        addonAuthor = "Killers0992",
        addonName = "GlobalBroadcast",
        addonVersion = "0.0.1")]
    public class GlobalBroadcastHost : NPAddonHost
    {
        public override void OnEnable()
        {
            Logger.Info("Addon enabled on HOST.");
        }

        public override void OnReady(NPServer server)
        {
            Logger.Info("Addon is ready");
        }

        public override void OnMessageReceived(NPServer server, NetDataReader reader)
        {
            Logger.Info($"Received ( \"{reader.GetString()}\" ) from server {server.ServerAddress}:{server.ServerPort}");
            NetDataWriter writer = new NetDataWriter();
            writer.Put("Response");
            SendData(server.ServerAddress, server.ServerPort, writer);
        }
    }
}
