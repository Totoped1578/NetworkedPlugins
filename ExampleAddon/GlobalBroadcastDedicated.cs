using LiteNetLib;
using LiteNetLib.Utils;
using NetworkedPlugins.API;
using NetworkedPlugins.API.Attributes;
using NetworkedPlugins.API.Interfaces;
using NetworkedPlugins.API.Models;

namespace ExampleAddon
{
    [NPAddonInfo(
        addonID = "0dewadopsdap32",
        addonAuthor = "Killers0992",
        addonName = "GlobalBroadcast",
        addonVersion = "0.0.1")]
    public class GlobalBroadcastDedicated : NPAddonDedicated<AddonConfig>
    {
        public override void OnEnable()
        {
            Logger.Info("Addon enabled on DEDICATED HOST.");
        }

        public override void OnReady(NPServer server)
        {
            Logger.Info("Addon is ready");
        }

        public override void OnMessageReceived(NPServer server, NetDataReader reader)
        {
            Logger.Info($"Received message from server {server.ServerAddress}:{server.ServerPort}");
            foreach(var plr in server.Players.Values)
            {
                Logger.Info($"Player {plr.UserName} {plr.UserID}"); 
            }
            NetDataWriter writer = new NetDataWriter();
            writer.Put("Response");
            SendData(server.ServerAddress, server.ServerPort, writer);
        }
    }
}
