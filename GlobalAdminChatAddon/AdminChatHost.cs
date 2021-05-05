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
    public class AdminChatHost : NPAddonHost
    {
        public override void OnEnable()
        {
            Exiled.Events.Handlers.Server.SendingRemoteAdminCommand += Server_SendingRemoteAdminCommand;
        }

        private void Server_SendingRemoteAdminCommand(Exiled.Events.EventArgs.SendingRemoteAdminCommandEventArgs ev)
        {
            if (ev.Name.StartsWith("@"))
            {
                string message = ev.Name + " " + string.Join(" ", ev.Arguments);
                string outMessage = message + " ~" + ev.Sender.Nickname;
                NetDataWriter writer = new NetDataWriter();
                writer.Put(outMessage);
                SendData(writer);
            }
        }

        public override void OnMessageReceived(NPServer server, NetDataReader reader)
        {
            var message = reader.GetString();
            foreach (var h in ReferenceHub.GetAllHubs())
            {
                if ((h.Value.serverRoles.AdminChatPerms || h.Value.serverRoles.RaEverywhere) && !h.Value.isDedicatedServer)
                {
                    h.Value.queryProcessor.TargetReply(h.Value.queryProcessor.connectionToClient, message, true, false, string.Empty);
                }
            }
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
