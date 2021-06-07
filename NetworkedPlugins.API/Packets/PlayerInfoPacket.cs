using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkedPlugins.API.Packets
{
    public struct PlayerInfoPacket : INetSerializable
    {
        public string UserID;
        
        public void Deserialize(NetDataReader reader)
        {
            UserID = reader.GetString();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(UserID);
        }
    }
}
