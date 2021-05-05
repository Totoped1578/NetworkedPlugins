using LiteNetLib.Utils;
using NetworkedPlugins.API;
using NetworkedPlugins.API.Attributes;
using NetworkedPlugins.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalAdminChatAddon
{
    [NPAddonInfo(
        addonID = "GXL8Wg0XrT9bpRzb",
        addonAuthor = "Killers0992",
        addonName = "GlobalAdminChatAddon",
        addonVersion = "1.0.0")]
    public class AdminChatDedicated : NPAddonDedicated
    {

        public override void OnMessageReceived(NPServer server, NetDataReader reader)
        {
            var message = reader.GetString();
            NetDataWriter writer = new NetDataWriter();
            writer.Put(message);
            foreach (var server2 in GetServers())
            {
                if (server2.FullAddress != server.FullAddress)
                {

                    SendData(server2, writer);
                }
            }
        }
    }
}
