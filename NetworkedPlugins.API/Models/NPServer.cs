using LiteNetLib;
using LiteNetLib.Utils;
using NetworkedPlugins.API.Packets;
using System.Collections.Generic;

namespace NetworkedPlugins.API.Models
{
    public class NPServer
    {
        public NPServer(NetPeer server, NetPacketProcessor processor, string serverAddress, ushort port, int maxPlayers)
        {
            this.processor = processor;
            this.peer = server;
            this.ServerAddress = serverAddress;
            this.ServerPort = port;
            this.MaxPlayers = maxPlayers;
        }

        internal NetPacketProcessor processor;
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

        public void ExecuteCommand(string command, List<string> arguments)
        {
            processor.Send<ExecuteConsoleCommandPacket>(peer, new ExecuteConsoleCommandPacket()
            {
                AddonID = "",
                Command = command + " " + string.Join(" ", arguments)
            }, DeliveryMethod.ReliableOrdered);
        }

        public void SendBroadcast(string message, ushort duration, bool isAdminOnly = false)
        {
            processor.Send<SendBroadcastPacket>(peer, new SendBroadcastPacket()
            {
                Message = message,
                isAdminOnly = isAdminOnly,
                Duration = duration
            }, DeliveryMethod.ReliableOrdered);
        }

        public void SendHint(string message, float duration, bool isAdminOnly = false)
        {
            processor.Send<SendHintPacket>(peer, new SendHintPacket()
            {
                Message = message,
                isAdminOnly = isAdminOnly,
                Duration = duration
            }, DeliveryMethod.ReliableOrdered);
        }

        public void ClearBroadcast()
        {
            processor.Send<ClearBroadcastsPacket>(peer, new ClearBroadcastsPacket(), DeliveryMethod.ReliableOrdered);
        }

        public void RoundRestart(ushort port = 0)
        {
            processor.Send<RoundRestartPacket>(peer, new RoundRestartPacket()
            {
                Port = port
            }, DeliveryMethod.ReliableOrdered);

        }
    }
}
