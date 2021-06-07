using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkedPlugins.API.Packets
{
    public class PlayerInteractPacket
    {
        public string UserID { get; set; }
        public byte Type { get; set; }
        public byte[] Data { get; set; }
    }
}
