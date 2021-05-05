using LiteNetLib;
using LiteNetLib.Utils;
using System.Collections.Generic;

namespace NetworkedPlugins.API.Models
{
    public class NPServer
    {
        public NPServer(NetPeer server, string serverAddress, ushort port, int maxPlayers)
        {
            this.peer = server;
            this.ServerAddress = serverAddress;
            this.ServerPort = port;
            this.MaxPlayers = maxPlayers;
        }

        internal NetPeer peer;
        public string ServerAddress { get; } = "localhost";
        public ushort ServerPort { get; } = 7777;
        public int MaxPlayers { get; } = 25;
        public Dictionary<string, NPPlayer> Players { get; set; } = new Dictionary<string, NPPlayer>();
        public List<NPAddonItem> Addons { get; internal set; } = new List<NPAddonItem>();
        public string FullAddress => $"{ServerAddress}:{ServerPort}";

        public NPPlayer GetPlayer(string userId)
        {
            if (Players.ContainsKey(userId))
            {
                return Players[userId];
            }
            return null;
        }

        public void SendBroadcast(string message, ushort duration, bool isAdminOnly = false)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put((byte)1);
            writer.Put(isAdminOnly);
            writer.Put(message);
            writer.Put(duration);
            peer.Send(writer, LiteNetLib.DeliveryMethod.ReliableOrdered);
        }

        public void SendHint(string message, float duration, bool isAdminOnly = false)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put((byte)6);
            writer.Put(isAdminOnly);
            writer.Put(message);
            writer.Put(duration);
            peer.Send(writer, LiteNetLib.DeliveryMethod.ReliableOrdered);
        }

        public void ClearBroadcast()
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put((byte)2);
            peer.Send(writer, LiteNetLib.DeliveryMethod.ReliableOrdered);
        }

        public void RoundRestart(ushort port = 0)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put((byte)5);
            writer.Put(port);
            peer.Send(writer, LiteNetLib.DeliveryMethod.ReliableOrdered);
        }
    }
}
