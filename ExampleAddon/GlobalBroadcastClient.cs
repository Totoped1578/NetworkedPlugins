using LiteNetLib;
using LiteNetLib.Utils;
using MEC;
using NetworkedPlugins.API;
using NetworkedPlugins.API.Attributes;
using System.Collections;
using System.Collections.Generic;

namespace ExampleAddon
{
    [NPAddonInfo(
        addonID = "0dewadopsdap32",
        addonAuthor = "Killers0992", 
        addonName = "GlobalBroadcast", 
        addonVersion = "0.0.1")]
    public class GlobalBroadcastClient : NPAddonClient
    {
        public override void OnEnable()
        {
            Logger.Info("Addon enabled on CLIENT.");
            Timing.RunCoroutine(SendDatas());
        }

        public override void OnReady()
        {
            Logger.Info("Addon is ready");
        }

        public override void OnMessageReceived(NetDataReader reader)
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
