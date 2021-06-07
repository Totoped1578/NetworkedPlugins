using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkedPlugins.API.Packets
{
    public class ConsoleResponsePacket
    {
        public bool isRemoteAdmin { get; set; }
        public string Command { get; set; }
        public string Response { get; set; }
    }
}
