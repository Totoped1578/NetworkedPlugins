using Exiled.API.Features;
using LiteNetLib.Utils;
using NetworkedPlugins.API.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkedPlugins.API.Models
{
    public class NPPlayer : PlayerFuncs
    {
        public NPPlayer(NPServer server, string UserName, string UserID, int role)
        {
            this.server = server;
            this._username = UserName;
            this._userid = UserID;
            this._role = role;
        }
        private string _username;
        private string _userid;
        private int _role;
        public NPServer server { get; set; }

        public override string UserName => _username;

        public override string UserID => _userid;

        public override int Role => _role;

        public override void Kill()
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put((byte)4);
            writer.Put(UserID);
            writer.Put((byte)0);
            server.peer.Send(writer, LiteNetLib.DeliveryMethod.ReliableOrdered);
        }

        public override void SendReportMessage(string message)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put((byte)4);
            writer.Put(UserID);
            writer.Put((byte)1);
            writer.Put(message);
            server.peer.Send(writer, LiteNetLib.DeliveryMethod.ReliableOrdered);
        }

        public override void SendRAMessage(string message)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put((byte)4);
            writer.Put(UserID);
            writer.Put((byte)2);
            writer.Put(message);
            server.peer.Send(writer, LiteNetLib.DeliveryMethod.ReliableOrdered);
        }

        public override void SendConsoleMessage(string message, string color = "GREEN")
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put((byte)4);
            writer.Put(UserID);
            writer.Put((byte)3);
            writer.Put(message);
            writer.Put(color.ToUpper());
            server.peer.Send(writer, LiteNetLib.DeliveryMethod.ReliableOrdered);
        }

        public override void Redirect(ushort port)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put((byte)4);
            writer.Put(UserID);
            writer.Put((byte)4);
            writer.Put(port);
            server.peer.Send(writer, LiteNetLib.DeliveryMethod.ReliableOrdered);
        }

        public override void Disconnect(string reason)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put((byte)4);
            writer.Put(UserID);
            writer.Put((byte)5);
            writer.Put(reason);
            server.peer.Send(writer, LiteNetLib.DeliveryMethod.ReliableOrdered);
        }
    }
}
