using LiteNetLib;
using LiteNetLib.Utils;
using NetworkedPlugins.API.Interfaces;
using NetworkedPlugins.API.Models;
using NetworkedPlugins.API.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkedPlugins.API
{
    public abstract class NPAddonClient<TConfig> : IAddon<TConfig>
        where TConfig : IConfig, new()
    {
        public NPManager Manager { get; set; }
        public NPLogger Logger { get; set; }
        public string addonId { get; set; }

        public string defaultPath { get; set; }
        public string addonPath { get; set; }

        public void SendData(NetDataWriter writer)
        {
            Manager._netPacketProcessor.Send<ReceiveAddonDataPacket>(Manager.networkListener, new ReceiveAddonDataPacket()
            {
                AddonID = addonId,
                Data = writer.Data
            }, DeliveryMethod.ReliableOrdered);
        }

        public virtual void OnMessageReceived(NPServer server, NetDataReader reader)
        {

        }

        public virtual void OnEnable()
        {

        }

        public virtual void OnReady(NPServer server)
        {

        }

        public void OnConsoleCommand(string cmd, List<string> arguments)
        {

        }

        public void OnConsoleResponse(NPServer server, string command, string response, bool isRa)
        {

        }

        public TConfig Config { get; } = new TConfig();
        public int CompareTo(IAddon<IConfig> other) => 0;
    }
}
