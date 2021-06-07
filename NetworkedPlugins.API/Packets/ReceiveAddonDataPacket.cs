using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkedPlugins.API.Packets
{
    public class ReceiveAddonDataPacket
    {
        public string AddonID { get; set; }
        public byte[] Data { get; set; }
    }
}
