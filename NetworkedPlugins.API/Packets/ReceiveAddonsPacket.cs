using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkedPlugins.API.Packets
{
    public class ReceiveAddonsPacket
    {
        public string[] AddonIds { get; set; }
    }
}
