using LiteNetLib;
using LiteNetLib.Utils;
using NetworkedPlugins.API;
using NetworkedPlugins.API.Attributes;
using NetworkedPlugins.API.Interfaces;
using NetworkedPlugins.API.Models;
using NetworkedPlugins.API.Packets;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading.Tasks;

namespace NPDedicatedApp
{
    public class Host : NPManager, INetEventListener
    {
        public class PConfig
        {
            public string hostConnectionKey { get; set; } = "UNKNOWN_KEY";
            public string hostAddress { get; set; } = "localhost";
            public ushort hostPort { get; set; } = 7777;
        }

        public PConfig config;
        public static Host host;
        public Host()
        {
            host = this;
            Logger = new ConsoleLogger();

            if (!File.Exists("./config.json"))
            {
                File.WriteAllText("./config.json", JsonConvert.SerializeObject(new PConfig(), Formatting.Indented));
            }
            config = JsonConvert.DeserializeObject<PConfig>(File.ReadAllText("./config.json"));
            if (!Directory.Exists("./addons"))
                Directory.CreateDirectory("./addons");

            string[] addonsFiles = Directory.GetFiles("./addons", "*.dll");
            Logger.Info($"Loading {addonsFiles.Length} addons.");
            foreach (var file in addonsFiles)
            {
                Assembly a = Assembly.LoadFrom(file);
                try
                {
                    string addonID = "";
                    foreach (Type t in a.GetTypes().Where(type => !type.IsAbstract && !type.IsInterface))
                    {
                        if (!t.BaseType.IsGenericType || t.BaseType.GetGenericTypeDefinition() != typeof(NPAddonDedicated<>))
                        {
                            continue;
                        }

                        IAddon<IConfig> addon = null;

                        var constructor = t.GetConstructor(Type.EmptyTypes);
                        if (constructor != null)
                        {
                            addon = constructor.Invoke(null) as IAddon<IConfig>;
                        }
                        else
                        {
                            var value = Array.Find(t.GetProperties(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public), property => property.PropertyType == t)?.GetValue(null);

                            if (value != null)
                                addon = value as IAddon<IConfig>;
                        }

                        if (addon == null)
                        {
                            continue;
                        }

                        NPAddonInfo addonInfo = (NPAddonInfo)Attribute.GetCustomAttribute(t, typeof(NPAddonInfo));
                        addon.Manager = this;
                        addon.Logger = Logger;
                        addon.addonId = addonInfo.addonID;
                        addonID = addonInfo.addonID;
                        addon.defaultPath = Path.Combine("addons");
                        addon.addonPath = Path.Combine(addon.defaultPath, addonInfo.addonName);
                        if (addons.ContainsKey(addonInfo.addonID))
                        {
                            Logger.Error($"Addon {addons[addonInfo.addonID].info.addonName} already have id {addonInfo.addonName}.");
                            break;
                        }
                        addons.Add(addonInfo.addonID, new NPAddonItem() { addon = addon, info = addonInfo });
                        LoadAddonConfig(addon.addonId);
                        Logger.Info($"Loading addon {addonInfo.addonName}.");
                        addon.OnEnable();
                        Logger.Info($"Waiting to client connections..");
                        foreach (var type in a.GetTypes())
                        {
                            if (typeof(ICommand).IsAssignableFrom(type))
                            {
                                ICommand cmd = (ICommand)Activator.CreateInstance(type);
                                RegisterCommand(addon.addonId, cmd);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error($"Failed loading addon {Path.GetFileNameWithoutExtension(file)}. {ex.ToString()}");
                }
            }
            Logger.Info($"Starting HOST network...");
            StartNetworkHost();
        }

        public void StartNetworkHost()
        {
            _netPacketProcessor.RegisterNestedType<CommandInfoPacket>();
            _netPacketProcessor.RegisterNestedType<PlayerInfoPacket>();
            _netPacketProcessor.RegisterNestedType<Position>();
            _netPacketProcessor.RegisterNestedType<Rotation>();
            _netPacketProcessor.SubscribeReusable<ReceiveAddonsPacket, NetPeer>(OnReceiveAddons);
            _netPacketProcessor.SubscribeReusable<ReceiveAddonDataPacket, NetPeer>(OnReceiveAddonData);
            _netPacketProcessor.SubscribeReusable<ReceivePlayersDataPacket, NetPeer>(OnReceivePlayersData);
            _netPacketProcessor.SubscribeReusable<ExecuteCommandPacket, NetPeer>(OnExecuteCommand);
            _netPacketProcessor.SubscribeReusable<UpdatePlayerInfoPacket, NetPeer>(OnUpdatePlayerInfo);
            _netPacketProcessor.SubscribeReusable<ConsoleResponsePacket, NetPeer>(OnConsoleResponse);
            networkListener = new NetManager(this);
            Logger.Info($"IP: {config.hostAddress}");
            Logger.Info($"Port: {config.hostPort}");
            networkListener.Start(config.hostPort);
            Task.Factory.StartNew(async () =>
            {
                await RefreshPolls();
            });
        }

        private void OnConsoleResponse(ConsoleResponsePacket packet, NetPeer peer)
        {
            if (!servers.TryGetValue(peer, out NPServer server))
                return;

            foreach (var addon in addons)
            {
                addon.Value.addon.OnConsoleResponse(server, packet.Command, packet.Response, packet.isRemoteAdmin);
            }
        }

        private void OnUpdatePlayerInfo(UpdatePlayerInfoPacket packet, NetPeer peer)
        {
            if (!servers.TryGetValue(peer, out NPServer server))
                return;

            if (!server.Players.ContainsKey(packet.UserID))
                server.Players.Add(packet.UserID, new NPPlayer(server, packet.UserID));

            if (!server.Players.TryGetValue(packet.UserID, out NPPlayer player))
                return;

            NetDataReader reader = new NetDataReader(packet.Data);

            Logger.Info($"UpdatePlayerInfo, userid {packet.UserID}, type {packet.Type}");
            switch (packet.Type)
            {
                case 0:
                    player.UserName = reader.GetString();
                    break;
                case 1:
                    player.Role = reader.GetInt();
                    break;
                case 2:
                    player.DoNotTrack = reader.GetBool();
                    break;
                case 3:
                    player.RemoteAdminAccess = reader.GetBool();
                    break;
                case 4:
                    player.IsOverwatchEnabled = reader.GetBool();
                    break;
                case 5:
                    player.IPAddress = reader.GetString();
                    break;
                case 6:
                    player.IsMuted = reader.GetBool();
                    break;
                case 7:
                    player.IsIntercomMuted = reader.GetBool();
                    break;
                case 8:
                    player.IsGodModeEnabled = reader.GetBool();
                    break;
                case 9:
                    player.Health = reader.GetFloat();
                    break;
                case 10:
                    player.MaxHealth = reader.GetInt();
                    break;
                case 11:
                    player.GroupName = reader.GetString();
                    break;
                case 12:
                    player.RankColor = reader.GetString();
                    break;
                case 13:
                    player.RankName = reader.GetString();
                    break;
                case 14:
                    player.Position = reader.Get<Position>();
                    break;
                case 15:
                    player.Rotation = reader.Get<Rotation>();
                    break;
                case 16:
                    player.PlayerID = reader.GetInt();
                    break;

            }
        }

        private void OnExecuteCommand(ExecuteCommandPacket packet, NetPeer peer)
        {
            if (!servers.TryGetValue(peer, out NPServer server))
                return;
            if (!server.Players.TryGetValue(packet.UserID, out NPPlayer player))
                return;
            ExecuteCommand(player, packet.AddonID, packet.CommandName, packet.Arguments.ToList());
        }

        private void OnReceivePlayersData(ReceivePlayersDataPacket packet, NetPeer peer)
        {
            if (!servers.TryGetValue(peer, out NPServer server))
                return;

            List<string> onlinePlayers = new List<string>();
            foreach (var plr in packet.Players)
            {
                if (!server.Players.TryGetValue(plr.UserID, out NPPlayer player))
                {
                    server.Players.Add(plr.UserID, new NPPlayer(server, plr.UserID));
                }
                onlinePlayers.Add(plr.UserID);
            }
            foreach (var offlinePlayer in server.Players.Where(p => !onlinePlayers.Contains(p.Key)).Select(p => p.Key))
            {
                server.Players.Remove(offlinePlayer);
            }
        }

        private void OnReceiveAddonData(ReceiveAddonDataPacket packet, NetPeer peer)
        {
            if (!servers.TryGetValue(peer, out NPServer server))
                return;
            NetDataReader reader = new NetDataReader(packet.Data);
            foreach (var addon in addons.Where(pp => pp.Key == packet.AddonID))
            {
                addon.Value.addon.OnMessageReceived(server, reader);
            }
        }

        private void OnReceiveAddons(ReceiveAddonsPacket packet, NetPeer peer)
        {
            if (!servers.TryGetValue(peer, out NPServer server))
                return;

            string adds = "";
            List<CommandInfoPacket> cmds = new List<CommandInfoPacket>();
            List<string> addonsId = new List<string>();
            foreach (var i in packet.AddonIds.Where(p => addons.ContainsKey(p)).Select(s => addons[s]))
            {
                servers[peer].Addons.Add(i);
                adds += $"{i.info.addonName} - {i.info.addonVersion}v made by {i.info.addonAuthor}" + Environment.NewLine;
                i.addon.OnReady(servers[peer]);
                cmds.AddRange(GetCommands(i.info.addonID));
                addonsId.Add(i.info.addonID);
            }
            Logger.Info($"Received addons from server {server.FullAddress}, {adds}");
            _netPacketProcessor.Send<ReceiveCommandsPacket>(peer, new ReceiveCommandsPacket()
            {
                Commands = cmds
            }, DeliveryMethod.ReliableOrdered);
            _netPacketProcessor.Send<ReceiveAddonsPacket>(peer, new ReceiveAddonsPacket()
            {
                AddonIds = addonsId.ToArray()
            }, DeliveryMethod.ReliableOrdered);
        }

        public async Task RefreshPolls()
        {
            while (true)
            {
                await Task.Delay(15);
                if (host.networkListener != null)
                    if (host.networkListener.IsRunning)
                        host.networkListener.PollEvents();
            }
        }


        public void OnPeerConnected(NetPeer peer)
        {
            Logger.Info($"Client {peer.EndPoint.Address.ToString()} connected to host.");
        }

        public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            if (servers.TryGetValue(peer, out NPServer server))
            {
                Logger.Info($"Client {server.FullAddress} disconnected from host. (Info: {disconnectInfo.Reason.ToString()})");
                servers.Remove(peer);
            }
        }

        public void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
        {
            Logger.Error($"Network error from endpoint {endPoint.Address}, {socketError}");
        }

        public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            try
            {
                _netPacketProcessor.ReadAllPackets(reader, peer);
            }catch(Exception ex)
            {
                Logger.Error($"Failed in {peer.EndPoint.Address}, {ex}");
            }
        }

        public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
        {
        }

        public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
        {

        }

        public void OnConnectionRequest(ConnectionRequest request)
        {
            if (request.Data.TryGetString(out string key))
            {
                if (key == config.hostConnectionKey)
                {
                    if (request.Data.TryGetUShort(out ushort port))
                    {
                        if (request.Data.TryGetInt(out int maxplayers))
                        {
                            var peer = request.Accept();
                            if (!servers.ContainsKey(peer))
                                servers.Add(peer, new NPServer(peer, _netPacketProcessor, peer.EndPoint.Address.ToString(), port, maxplayers));
                            else
                                servers[peer] = new NPServer(peer, _netPacketProcessor, peer.EndPoint.Address.ToString(), port, maxplayers);
                            Logger.Info($"New server added {peer.EndPoint.Address.ToString()}, port: {port}");
                            return;
                        }
                    }
                }
            }
        }
    }
}
