using LiteNetLib.Utils;
using NetworkedPlugins.API;
using NetworkedPlugins.API.Attributes;
using NetworkedPlugins.API.Interfaces;
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
    public class HubDedicated : NPAddonDedicated<HubConfig.ConfigDedicated>
    {
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
            Logger.Info("Config loaded.");
            Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    await Task.Delay(3000);
                    NetDataWriter writer = new NetDataWriter();
                    writer.Put((byte)1);
                    hubservers.Clear();
                    SendData(writer);
                }
            });
        }

        public override void OnReady(NPServer server)
        {

        }

        public void ProcessHostCommand(PlayerFuncs plr, string commandName, List<string> arguments)
        {
            switch (commandName.ToUpper())
            {
                case "HUB":
                case "LOBBY":
                    if (isServerOnline(Config.LobbyPort))
                    {
                        plr.Redirect(Config.LobbyPort);
                        plr.SendConsoleMessage($"Connecting to server <color=yellow>{Config.GetServerName(Config.LobbyPort)}</color>...", "green");
                    }
                    else
                    {
                        plr.SendConsoleMessage($"Server <color=yellow>{Config.GetServerName(Config.LobbyPort)}</color> is offline.", "red");
                    }
                    break;
                case "SERVERS":
                    string retstr = "Servers: \n";
                    foreach (var server in hubservers.Values)
                    {
                        retstr += $" - <color=yellow>{Config.GetServerName(server.ServerPort)}</color> {server.Players.Count}/{server.MaxPlayers}";
                    }
                    plr.SendConsoleMessage(retstr);
                    break;
                case "CHANGESERVER":
                    if (arguments.Count != 0)
                    {
                        var server = Config.GetServerByName(arguments[0]);
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
                    if (arguments.Count != 0)
                    {
                        foreach (var server in GetServers())
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
                        try
                        {
                            ProcessHostCommand(plr, commandName, arguments);
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(ex.ToString());
                        }
                        Logger.Info($"Player {executorUserid} used command {commandName}, server: {server.FullAddress}");
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
