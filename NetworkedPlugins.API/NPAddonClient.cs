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
    public class NPAddonClient
    {
        public NPManager Manager;
        public NPLogger Logger;
        public string addonId;

        public string defaultPath;

        public void SendData(NetDataWriter writer)
        {
            NetDataWriter wr = new NetDataWriter();
            wr.Put((byte)3);
            wr.Put(addonId);
            wr.PutBytesWithLength(writer.Data, 0, writer.Length);
            Manager.networkListener.SendToAll(wr, LiteNetLib.DeliveryMethod.ReliableOrdered);
        }

        public virtual void OnMessageReceived(NetDataReader reader)
        {

        }

        public virtual void OnEnable()
        {

        }

        public virtual void OnReady()
        {

        }
    }
}
