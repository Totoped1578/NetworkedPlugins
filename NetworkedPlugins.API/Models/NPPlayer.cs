using Exiled.API.Features;
using LiteNetLib.Utils;
using NetworkedPlugins.API.Interfaces;
using NetworkedPlugins.API.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkedPlugins.API.Models
{
    public class NPPlayer : PlayerFuncs
    {
        public NPPlayer(NPServer server, string UserID)
        {
            this.server = server;
            this.UserID = UserID;
        }

        public NPServer server { get; set; }

        public override void Kill()
        {
            server.processor.Send<PlayerInteractPacket>(server.peer, new PlayerInteractPacket()
            {
                UserID = UserID,
                Type = (byte)0
            }, LiteNetLib.DeliveryMethod.ReliableOrdered);
        }

        public override void SendReportMessage(string message)
        {
            var writer = new NetDataWriter();
            writer.Put(message);
            server.processor.Send<PlayerInteractPacket>(server.peer, new PlayerInteractPacket()
            {
                UserID = UserID,
                Type = (byte)1,
                Data = writer.Data
            }, LiteNetLib.DeliveryMethod.ReliableOrdered);
        }

        public override void SendRAMessage(string message)
        {
            var writer = new NetDataWriter();
            writer.Put(message);
            server.processor.Send<PlayerInteractPacket>(server.peer, new PlayerInteractPacket()
            {
                UserID = UserID,
                Type = (byte)2,
                Data = writer.Data
            }, LiteNetLib.DeliveryMethod.ReliableOrdered);
        }

        public override void SendConsoleMessage(string message, string color = "GREEN")
        {
            var writer = new NetDataWriter();
            writer.Put(message);
            writer.Put(color);
            server.processor.Send<PlayerInteractPacket>(server.peer, new PlayerInteractPacket()
            {
                UserID = UserID,
                Type = (byte)3,
                Data = writer.Data
            }, LiteNetLib.DeliveryMethod.ReliableOrdered);
        }

        public override void Redirect(ushort port)
        {
            var writer = new NetDataWriter();
            writer.Put(port);
            server.processor.Send<PlayerInteractPacket>(server.peer, new PlayerInteractPacket()
            {
                UserID = UserID,
                Type = (byte)4,
                Data = writer.Data
            }, LiteNetLib.DeliveryMethod.ReliableOrdered);
        }

        public override void Disconnect(string reason)
        {
            var writer = new NetDataWriter();
            writer.Put(reason);
            server.processor.Send<PlayerInteractPacket>(server.peer, new PlayerInteractPacket()
            {
                UserID = UserID,
                Type = (byte)5,
                Data = writer.Data
            }, LiteNetLib.DeliveryMethod.ReliableOrdered);
        }

        public override void SendHint(string message, float duration)
        {
            var writer = new NetDataWriter();
            writer.Put(message);
            writer.Put(duration);
            server.processor.Send<PlayerInteractPacket>(server.peer, new PlayerInteractPacket()
            {
                UserID = UserID,
                Type = (byte)6,
                Data = writer.Data
            }, LiteNetLib.DeliveryMethod.ReliableOrdered);
        }

        public override void SendPosition(bool state = false)
        {
            var writer = new NetDataWriter();
            writer.Put(state);
            server.processor.Send<PlayerInteractPacket>(server.peer, new PlayerInteractPacket()
            {
                UserID = UserID,
                Type = (byte)7,
                Data = writer.Data
            }, LiteNetLib.DeliveryMethod.ReliableOrdered);
        }

        public override void SendRotation(bool state = false)
        {
            var writer = new NetDataWriter();
            writer.Put(state);
            server.processor.Send<PlayerInteractPacket>(server.peer, new PlayerInteractPacket()
            {
                UserID = UserID,
                Type = (byte)8,
                Data = writer.Data
            }, LiteNetLib.DeliveryMethod.ReliableOrdered);
        }

        public override void Teleport(float x, float y, float z)
        {
            var writer = new NetDataWriter();
            writer.Put(x);
            writer.Put(y);
            writer.Put(z);
            server.processor.Send<PlayerInteractPacket>(server.peer, new PlayerInteractPacket()
            {
                UserID = UserID,
                Type = (byte)9,
                Data = writer.Data
            }, LiteNetLib.DeliveryMethod.ReliableOrdered);
        }

        public override void SetGodmode(bool state = false)
        {
            var writer = new NetDataWriter();
            writer.Put(state);
            server.processor.Send<PlayerInteractPacket>(server.peer, new PlayerInteractPacket()
            {
                UserID = UserID,
                Type = (byte)10,
                Data = writer.Data
            }, LiteNetLib.DeliveryMethod.ReliableOrdered);
        }

        public override void SetNoclip(bool state = false)
        {
            var writer = new NetDataWriter();
            writer.Put(state);
            server.processor.Send<PlayerInteractPacket>(server.peer, new PlayerInteractPacket()
            {
                UserID = UserID,
                Type = (byte)11,
                Data = writer.Data
            }, LiteNetLib.DeliveryMethod.ReliableOrdered);
        }

        public override void ClearInventory()
        {
            server.processor.Send<PlayerInteractPacket>(server.peer, new PlayerInteractPacket()
            {
                UserID = UserID,
                Type = (byte)12,
                Data = new byte[0]
            }, LiteNetLib.DeliveryMethod.ReliableOrdered);
        }
    }
}
