using Exiled.Permissions.Extensions;
using LiteNetLib.Utils;
using NetworkedPlugins.API;
using NetworkedPlugins.API.Attributes;
using NetworkedPlugins.API.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utf8Json;

namespace HubAddon
{
    [NPAddonInfo(
        addonID = "HQ0XCwohZbEa8Vl5",
        addonAuthor = "Killers0992",
        addonName = "HubAddon",
        addonVersion = "1.0.0")]
    public class HubClient : NPAddonClient<HubConfig.ConfigClient>
    {
        public Dictionary<string, string> registeredCommands { get; set; } = new Dictionary<string, string>();


        public override void OnEnable()
        {
            if (Config.isLobby)
            {
                Exiled.Events.Handlers.Server.WaitingForPlayers += Server_WaitingForPlayers;
            }
            Exiled.Events.Handlers.Server.SendingConsoleCommand += Server_SendingConsoleCommand;
        }

        public override void OnReady(NPServer server)
        {
            Logger.Info("Request command sync");
            NetDataWriter writer = new NetDataWriter();
            writer.Put((byte)0);
            SendData(writer);
        }

        private void Server_SendingConsoleCommand(Exiled.Events.EventArgs.SendingConsoleCommandEventArgs ev)
        {
            if (registeredCommands.TryGetValue(ev.Name.ToLower(), out string perm))
            {
                if (ev.Player.CheckPermission(perm))
                {
                    ev.IsAllowed = false;
                    NetDataWriter writer = new NetDataWriter();
                    writer.Put((byte)1);
                    writer.Put(ev.Player.UserId);
                    writer.Put(ev.Name);
                    writer.PutArray(ev.Arguments.ToArray());
                    SendData(writer);
                }
            }
        }


        private void Server_WaitingForPlayers()
        {
            HubGlobals.InitLobby();
        }

        public override void OnMessageReceived(NPServer server, NetDataReader reader)
        {
            byte type = reader.GetByte();
            switch (type)
            {
                //Update hub
                case 1:
                    NetDataWriter writer = new NetDataWriter();
                    writer.Put((byte)2);
                    SendData(writer);
                    break;
            }
        }
    }
}
