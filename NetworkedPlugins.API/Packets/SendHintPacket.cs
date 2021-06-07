using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkedPlugins.API.Packets
{
    public class SendHintPacket
    {
        public bool isAdminOnly { get; set; }
        public string Message { get; set; }
        public float Duration { get; set; }
    }
}
