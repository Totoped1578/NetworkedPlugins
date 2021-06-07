using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkedPlugins.API.Packets
{
    public class ReceivePlayersDataPacket
    {
        public List<PlayerInfoPacket> Players { get; set; }
    }
}
