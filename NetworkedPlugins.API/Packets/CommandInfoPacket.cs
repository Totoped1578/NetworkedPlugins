using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkedPlugins.API.Packets
{
    public struct CommandInfoPacket : INetSerializable
    {
        public string AddonID;
        public string CommandName;
        public string Description;
        public string Permission;
        public bool isRaCommand;

        public void Deserialize(NetDataReader reader)
        {
            AddonID = reader.GetString();
            CommandName = reader.GetString();
            Description = reader.GetString();
            Permission = reader.GetString();
            isRaCommand = reader.GetBool();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(AddonID);
            writer.Put(CommandName);
            writer.Put(Description);
            writer.Put(Permission);
            writer.Put(isRaCommand);
        }
    }
}
