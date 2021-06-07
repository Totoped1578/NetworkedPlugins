using LiteNetLib;
using LiteNetLib.Utils;
using MEC;
using NetworkedPlugins.API;
using NetworkedPlugins.API.Attributes;
using NetworkedPlugins.API.Models;
using System.Collections;
using System.Collections.Generic;

namespace ExampleAddon
{
    [NPAddonInfo(
        addonID = "0dewadopsdap32",
        addonAuthor = "Killers0992", 
        addonName = "GlobalBroadcast", 
        addonVersion = "0.0.1")]
    public class GlobalBroadcastClient : NPAddonClient<AddonConfig>
    {
        public override void OnEnable()
        {
            Logger.Info("Addon enabled on CLIENT.");
            Timing.RunCoroutine(SendDatas());
        }

        public override void OnReady(NPServer server)
        {
            Logger.Info("Addon is ready");
        }

        public override void OnMessageReceived(NPServer server, NetDataReader reader)
        {
            Logger.Info($"Received ( \"{reader.GetString()}\" )");
        }

        public IEnumerator<float> SendDatas()
        {
            while(true)
            {
                yield return Timing.WaitForSeconds(3f);
                NetDataWriter writer = new NetDataWriter();
                writer.Put("Some string");
                SendData(writer);
            }
        }
    }
}
