using Exiled.Permissions.Extensions;
using LiteNetLib.Utils;
using NetworkedPlugins.API;
using NetworkedPlugins.API.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HubAddon
{
    [NPAddonInfo(
        addonID = "HQ0XCwohZbEa8Vl5",
        addonAuthor = "Killers0992",
        addonName = "HubAddon",
        addonVersion = "1.0.0")]
    public class HubClient : NPAddonClient
    {
        public HubConfig.ConfigClient config;
        public Dictionary<string, string> registeredCommands { get; set; } = new Dictionary<string, string>();


        public override void OnEnable()
        {
            if (!Directory.Exists(Path.Combine(defaultPath, "HubAddon")))
                Directory.CreateDirectory(Path.Combine(defaultPath, "HubAddon"));
            if (!File.Exists(Path.Combine(defaultPath, "HubAddon", "config.json")))
                File.WriteAllText(Path.Combine(defaultPath, "HubAddon", "config.json"), JsonConvert.SerializeObject(new HubConfig.ConfigClient(), Formatting.Indented));
            config = JsonConvert.DeserializeObject<HubConfig.ConfigClient>(File.ReadAllText(Path.Combine(defaultPath, "HubAddon", "config.json")));
            File.WriteAllText(Path.Combine(defaultPath, "HubAddon", "config.json"), JsonConvert.SerializeObject(config, Formatting.Indented));
            Logger.Info("Config loaded.");
            if (config.isLobby)
            {
                Exiled.Events.Handlers.Server.WaitingForPlayers += Server_WaitingForPlayers;
            }
            Exiled.Events.Handlers.Server.SendingConsoleCommand += Server_SendingConsoleCommand;
        }

        public override void OnReady()
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

        public override void OnMessageReceived(NetDataReader reader)
        {
            byte type = reader.GetByte();
            switch (type)
            {
                //Register commands
                case 0:
                    var data = JsonConvert.DeserializeObject<Dictionary<string, string>>(reader.GetString());
                    registeredCommands = data;
                    Logger.Info($"Received {data.Count} commands, {string.Join(",\n", data.Keys)}");
                    break;
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
