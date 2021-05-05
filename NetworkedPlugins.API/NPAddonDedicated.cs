using LiteNetLib;
using LiteNetLib.Utils;
using NetworkedPlugins.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkedPlugins.API
{
    public class NPAddonDedicated
    {
        public NPManager Manager;
        public NPLogger Logger;
        public string addonId;

        public string defaultPath;

        public void SendData(ushort serverPort, NetDataWriter writer)
        {
            NetDataWriter wr = new NetDataWriter();
            wr.Put((byte)3);
            wr.Put(addonId);
            wr.PutBytesWithLength(writer.Data, 0, writer.Length);
            foreach (var obj in Manager.servers)
                if (obj.Value.ServerPort == serverPort)
                    obj.Key.Send(wr, LiteNetLib.DeliveryMethod.ReliableOrdered);
        }

        public void SendData(NPServer server, NetDataWriter writer)
        {
            NetDataWriter wr = new NetDataWriter();
            wr.Put((byte)3);
            wr.Put(addonId);
            wr.PutBytesWithLength(writer.Data, 0, writer.Length);
            foreach (var obj in Manager.servers)
                if (obj.Value.FullAddress == server.FullAddress)
                    obj.Key.Send(wr, LiteNetLib.DeliveryMethod.ReliableOrdered);
        }

        public void SendData(string serverAddress, NetDataWriter writer)
        {
            NetDataWriter wr = new NetDataWriter();
            wr.Put((byte)3);
            wr.Put(addonId);
            wr.PutBytesWithLength(writer.Data, 0, writer.Length);
            foreach (var obj in Manager.servers)
                if (obj.Key.EndPoint.Address.ToString() == serverAddress)
                    obj.Key.Send(wr, LiteNetLib.DeliveryMethod.ReliableOrdered);
        }

        public void SendData(string serverAddress, ushort serverPort, NetDataWriter writer)
        {
            NetDataWriter wr = new NetDataWriter();
            wr.Put((byte)3);
            wr.Put(addonId);
            wr.PutBytesWithLength(writer.Data, 0, writer.Length);
            foreach (var obj in Manager.servers)
                if (obj.Key.EndPoint.Address.ToString() == serverAddress)
                    if (obj.Value.ServerPort == serverPort)
                        obj.Key.Send(wr, LiteNetLib.DeliveryMethod.ReliableOrdered);
        }

        public List<NPServer> GetServers()
        {
            return Manager.servers.Values.ToList();
        }

        public bool isServerOnline(ushort port)
        {
            return Manager.servers.Any(p => p.Value.ServerPort == port);
        }

        public void SendData(NetDataWriter writer)
        {
            NetDataWriter wr = new NetDataWriter();
            wr.Put((byte)3);
            wr.Put(addonId);
            wr.PutBytesWithLength(writer.Data, 0, writer.Length);
            foreach (var obj in Manager.servers)
                obj.Key.Send(wr, LiteNetLib.DeliveryMethod.ReliableOrdered);
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

        public virtual void OnConsoleCommand(string cmd, List<string> arguments)
        {

        }
    }
}
