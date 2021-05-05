using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using LiteNetLib;
using LiteNetLib.Utils;
using MEC;
using NetworkedPlugins.API;
using NetworkedPlugins.API.Attributes;
using NetworkedPlugins.API.Interfaces;
using NetworkedPlugins.API.Models;
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
    public class HubHost : NPAddonHost
    {
        public HubConfig.ConfigHost config;
        public Dictionary<string, string> registeredCommands { get; set; } = new Dictionary<string, string>()
        {
            { "lobby", "hub.lobby" },
            { "hub", "hub.lobby" },
            { "servers", "hub.servers" },
            { "changeserver", "hub.changeserver" },
            { "alert", "hub.alert" },
            { "sendall", "hub.sendall" }
        };
        public Dictionary<string, NPServer> hubservers = new Dictionary<string, NPServer>();


        public override void OnEnable()
        {
            if (!Directory.Exists(Path.Combine(defaultPath, "HubAddon")))
                Directory.CreateDirectory(Path.Combine(defaultPath, "HubAddon"));
            if (!File.Exists(Path.Combine(defaultPath, "HubAddon", "config.json")))
                File.WriteAllText(Path.Combine(defaultPath, "HubAddon", "config.json"), JsonConvert.SerializeObject(new HubConfig.ConfigHost(), Formatting.Indented));
            config = JsonConvert.DeserializeObject<HubConfig.ConfigHost>(File.ReadAllText(Path.Combine(defaultPath, "HubAddon", "config.json")));
            File.WriteAllText(Path.Combine(defaultPath, "HubAddon", "config.json"), JsonConvert.SerializeObject(config, Formatting.Indented));
            Logger.Info("Config loaded.");
            if (config.isLobby)
            {
                Exiled.Events.Handlers.Server.WaitingForPlayers += Server_WaitingForPlayers;
            }
            Timing.RunCoroutine(RefreshHubServers());
            Exiled.Events.Handlers.Server.SendingConsoleCommand += Server_SendingConsoleCommand;
        }

        public override void OnReady(NPServer server)
        {

        }

        private IEnumerator<float> RefreshHubServers()
        {
            while (true)
            {
                yield return Timing.WaitForSeconds(3f);
                NetDataWriter writer = new NetDataWriter();
                writer.Put((byte)1);
                hubservers.Clear();
                SendData(writer);
            }
        }

        private void Server_SendingConsoleCommand(Exiled.Events.EventArgs.SendingConsoleCommandEventArgs ev)
        {
            if (registeredCommands.TryGetValue(ev.Name.ToLower(), out string perm))
            {
                if (ev.Player.CheckPermission(perm))
                {
                    ev.IsAllowed = false;
                    ProcessHostCommand(new NormalPlayer(ev.Player), ev.Name, ev.Arguments);
                }
            }

        }

        public void ProcessHostCommand(PlayerFuncs plr, string commandName, List<string> arguments)
        {
            switch (commandName.ToUpper())
            {
                case "HUB":
                case "LOBBY":
                    if (isServerOnline(config.LobbyPort))
                    {
                        plr.Redirect(config.LobbyPort);
                        plr.SendConsoleMessage($"Connecting to server <color=yellow>{config.GetServerName(config.LobbyPort)}</color>...", "green");
                    }
                    else
                    {
                        plr.SendConsoleMessage($"Server <color=yellow>{config.GetServerName(config.LobbyPort)}</color> is offline.", "red");
                    }
                    break;
                case "SERVERS":
                    string retstr = "Servers: \n";
                    foreach(var server in hubservers.Values)
                    {
                        retstr += $" - <color=yellow>{config.GetServerName(server.ServerPort)}</color> {server.Players.Count}/{server.MaxPlayers}";
                    }
                    plr.SendConsoleMessage(retstr);
                    break;
                case "CHANGESERVER":
                    if (arguments.Count != 0)
                    {
                        var server = config.GetServerByName(arguments[0]);
                        if (server != null)
                        {
                            if (isServerOnline(server.Item2))
                            {
                                plr.Redirect(server.Item2);
                                plr.SendConsoleMessage($"Connecting to server <color=yellow>{arguments[0]}</color>...", "green");
                            }
                            else
                            {
                                plr.SendConsoleMessage($"Server <color=yellow>{arguments[0]}</color> is offline.");
                            }
                        }
                        else
                        {
                            plr.SendConsoleMessage($"Server <color=yellow>{arguments[0]}</color> not found.");
                        }
                    }
                    else
                    {
                        plr.SendConsoleMessage("Missing arguments: CHANGESERVER <servername>");
                    }
                    break;
                case "ALERT":
                    if (arguments.Count != 0) {
                        foreach(var server in GetServers())
                        {
                            server.SendBroadcast(string.Join(" ", arguments), 3);
                        }
                        plr.SendRAMessage("Done");
                    }
                    else
                    {
                        plr.SendRAMessage("Missing arguments: ALERT <message>");
                    }
                    break;
                case "SENDALL":

                    break;
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
                //Get registered command
                case 0:
                    NetDataWriter writer = new NetDataWriter();
                    writer.Put((byte)0);
                    writer.Put(JsonConvert.SerializeObject(registeredCommands));
                    SendData(server, writer);
                    Logger.Info($"Server {server.FullAddress} requested command sync.");
                    break;
                //Execute command
                case 1:
                    string executorUserid = reader.GetString();
                    string commandName = reader.GetString();
                    List<string> arguments = reader.GetStringArray().ToList();
                    var plr = server.GetPlayer(executorUserid);
                    if (plr != null)
                    {
                        Logger.Info($"Player {executorUserid} used command {commandName}, server: {server.FullAddress}");
                        ProcessHostCommand(plr, commandName, arguments);
                    }
                    else
                    {
                        Logger.Info($"Player {executorUserid} not found, command: {commandName}.");
                    }
                    break;
                //Update hub server
                case 2:
                    hubservers[server.FullAddress] = server;
                    break;
            }
        }
    }
}
