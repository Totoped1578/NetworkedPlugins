using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkedPlugins.API.Packets
{
    public class ExecuteConsoleCommandPacket
    {
        public string AddonID { get; set; }
        public string Command { get; set; }
    }
}
