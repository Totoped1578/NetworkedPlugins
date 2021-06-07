using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkedPlugins.API.Packets
{
    public class ExecuteCommandPacket
    {
        public string UserID { get; set; }
        public string AddonID { get; set; }
        public string CommandName { get; set; }
        public string[] Arguments { get; set; }
    }
}
