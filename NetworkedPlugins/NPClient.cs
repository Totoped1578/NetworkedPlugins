using Exiled.API.Features;
using LiteNetLib;
using LiteNetLib.Utils;
using MEC;
using Mirror;
using NetworkedPlugins.API;
using NetworkedPlugins.API.Attributes;
using NetworkedPlugins.API.Interfaces;
using NetworkedPlugins.API.Models;
using NetworkedPlugins.API.Packets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using Utf8Json;

namespace NetworkedPlugins
{
    public class NPClient : NPManager, INetEventListener
    {
        private MainClass plugin;

        private NetDataWriter defaultdata;

        public NPClient(MainClass plugin)
        {
            this.plugin = plugin;
            defaultdata = new NetDataWriter();
            defaultdata.Put(plugin.Config.hostConnectionKey);
            defaultdata.Put(Server.Port);
            defaultdata.Put(CustomNetworkManager.slots);
            Logger = new PluginLogger();
            string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string pluginDir = Path.Combine(folderPath, "EXILED", "Plugins", "NetworkedPlugins");
            if (!Directory.Exists(pluginDir))
                Directory.CreateDirectory(pluginDir);
            if (!Directory.Exists(Path.Combine(pluginDir, "addons-" + Server.Port)))
                Directory.CreateDirectory(Path.Combine(pluginDir, "addons-" + Server.Port));
            string[] addonsFiles = Directory.GetFiles(Path.Combine(pluginDir, "addons-" + Server.Port), "*.dll");
            Log.Info($"Loading {addonsFiles.Length} addons.");
            foreach (var file in addonsFiles)
            {
                Assembly a = Assembly.LoadFrom(file);
                try
                {
                    foreach (Type t in a.GetTypes().Where(type => !type.IsAbstract && !type.IsInterface))
                    {
                        if (!t.BaseType.IsGenericType || t.BaseType.GetGenericTypeDefinition() != typeof(NPAddonClient<>))
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
                        addon.defaultPath = Path.Combine(pluginDir, $"addons-{Server.Port}");
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
                    }
                }
                catch(Exception ex)
                {
                    Logger.Error($"Failed loading addon {Path.GetFileNameWithoutExtension(file)}. {ex.ToString()}");
                }
            }
            Logger.Info($"Starting CLIENT network...");
            Exiled.Events.Handlers.Server.SendingRemoteAdminCommand += Server_SendingRemoteAdminCommand;
            Exiled.Events.Handlers.Server.SendingConsoleCommand += Server_SendingConsoleCommand;
            Exiled.Events.Handlers.Player.Destroying += Player_Destroying;
            Exiled.Events.Handlers.Player.Verified += Player_Verified;
            Exiled.Events.Handlers.Server.WaitingForPlayers += Server_WaitingForPlayers;
            StartNetworkClient();
        }

        private void Server_WaitingForPlayers()
        {
            UpdatePlayers();
        }

        public Dictionary<string, NPPlayer> players = new Dictionary<string, NPPlayer>();

        private void Player_Verified(Exiled.Events.EventArgs.VerifiedEventArgs ev)
        {
            if (!players.ContainsKey(ev.Player.UserId))
            {
                players.Add(ev.Player.UserId, new NPPlayer(null, ev.Player.UserId));
            }
        }

        private void Player_Destroying(Exiled.Events.EventArgs.DestroyingEventArgs ev)
        {
            if (players.ContainsKey(ev.Player.UserId))
            {
                players.Remove(ev.Player.UserId);
            }
        }

        public IEnumerator<float> DataCheckers()
        {
            players = new Dictionary<string, NPPlayer>();
            foreach(var plr in Player.List)
            {
                players.Add(plr.UserId, new NPPlayer(null, plr.UserId));
            }
            UpdatePlayers();
            while (true)
            {
                yield return Timing.WaitForSeconds(0.1f);

                try
                {
                    foreach (var plr in players)
                    {
                        if (!canSendData)
                            continue;
                        var realPlayer = Player.Get(plr.Key);
                        var player = plr.Value;
                        if (realPlayer != null)
                        {
                            NetDataWriter writer = new NetDataWriter();
                            if (player.UserName != realPlayer.Nickname)
                            {
                                player.UserName = realPlayer.Nickname;
                                writer = new NetDataWriter();
                                writer.Put(player.UserName);
                                _netPacketProcessor.Send<UpdatePlayerInfoPacket>(networkListener, new UpdatePlayerInfoPacket()
                                {
                                    UserID = player.UserID,
                                    Type = (byte)0,
                                    Data = writer.Data
                                }, DeliveryMethod.ReliableOrdered);
                            }
                            if (player.Role != (int)realPlayer.Role)
                            {
                                player.Role = (int)realPlayer.Role;
                                writer = new NetDataWriter();
                                writer.Put(player.Role);
                                _netPacketProcessor.Send<UpdatePlayerInfoPacket>(networkListener, new UpdatePlayerInfoPacket()
                                {
                                    UserID = player.UserID,
                                    Type = (byte)1,
                                    Data = writer.Data
                                }, DeliveryMethod.ReliableOrdered);
                            }
                            if (player.DoNotTrack != realPlayer.DoNotTrack)
                            {
                                player.DoNotTrack = realPlayer.DoNotTrack;
                                writer = new NetDataWriter();
                                writer.Put(player.DoNotTrack);
                                _netPacketProcessor.Send<UpdatePlayerInfoPacket>(networkListener, new UpdatePlayerInfoPacket()
                                {
                                    UserID = player.UserID,
                                    Type = (byte)2,
                                    Data = writer.Data
                                }, DeliveryMethod.ReliableOrdered);
                            }
                            if (player.RemoteAdminAccess != realPlayer.RemoteAdminAccess)
                            {
                                player.RemoteAdminAccess = realPlayer.RemoteAdminAccess;
                                writer = new NetDataWriter();
                                writer.Put(player.RemoteAdminAccess);
                                _netPacketProcessor.Send<UpdatePlayerInfoPacket>(networkListener, new UpdatePlayerInfoPacket()
                                {
                                    UserID = player.UserID,
                                    Type = (byte)3,
                                    Data = writer.Data
                                }, DeliveryMethod.ReliableOrdered);
                            }
                            if (player.IsOverwatchEnabled != realPlayer.IsOverwatchEnabled)
                            {
                                player.IsOverwatchEnabled = realPlayer.IsOverwatchEnabled;
                                writer = new NetDataWriter();
                                writer.Put(player.IsOverwatchEnabled);
                                _netPacketProcessor.Send<UpdatePlayerInfoPacket>(networkListener, new UpdatePlayerInfoPacket()
                                {
                                    UserID = player.UserID,
                                    Type = (byte)4,
                                    Data = writer.Data
                                }, DeliveryMethod.ReliableOrdered);
                            }
                            if (player.IPAddress != realPlayer.IPAddress)
                            {
                                player.IPAddress = realPlayer.IPAddress;
                                writer = new NetDataWriter();
                                writer.Put(player.IPAddress);
                                _netPacketProcessor.Send<UpdatePlayerInfoPacket>(networkListener, new UpdatePlayerInfoPacket()
                                {
                                    UserID = player.UserID,
                                    Type = (byte)5,
                                    Data = writer.Data
                                }, DeliveryMethod.ReliableOrdered);
                            }
                            if (player.IsMuted != realPlayer.IsMuted)
                            {
                                player.IsMuted = realPlayer.IsMuted;
                                writer = new NetDataWriter();
                                writer.Put(player.IsMuted);
                                _netPacketProcessor.Send<UpdatePlayerInfoPacket>(networkListener, new UpdatePlayerInfoPacket()
                                {
                                    UserID = player.UserID,
                                    Type = (byte)6,
                                    Data = writer.Data
                                }, DeliveryMethod.ReliableOrdered);
                            }
                            if (player.IsIntercomMuted != realPlayer.IsIntercomMuted)
                            {
                                player.IsIntercomMuted = realPlayer.IsIntercomMuted;
                                writer = new NetDataWriter();
                                writer.Put(player.IsIntercomMuted);
                                _netPacketProcessor.Send<UpdatePlayerInfoPacket>(networkListener, new UpdatePlayerInfoPacket()
                                {
                                    UserID = player.UserID,
                                    Type = (byte)7,
                                    Data = writer.Data
                                }, DeliveryMethod.ReliableOrdered);
                            }
                            if (player.IsGodModeEnabled != realPlayer.IsGodModeEnabled)
                            {
                                player.IsGodModeEnabled = realPlayer.IsGodModeEnabled;
                                writer = new NetDataWriter();
                                writer.Put(player.IsGodModeEnabled);
                                _netPacketProcessor.Send<UpdatePlayerInfoPacket>(networkListener, new UpdatePlayerInfoPacket()
                                {
                                    UserID = player.UserID,
                                    Type = (byte)8,
                                    Data = writer.Data
                                }, DeliveryMethod.ReliableOrdered);
                            }
                            if (player.Health != realPlayer.Health)
                            {
                                player.Health = realPlayer.Health;
                                writer = new NetDataWriter();
                                writer.Put(player.Health);
                                _netPacketProcessor.Send<UpdatePlayerInfoPacket>(networkListener, new UpdatePlayerInfoPacket()
                                {
                                    UserID = player.UserID,
                                    Type = (byte)9,
                                    Data = writer.Data
                                }, DeliveryMethod.ReliableOrdered);
                            }
                            if (player.MaxHealth != realPlayer.MaxHealth)
                            {
                                player.MaxHealth = realPlayer.MaxHealth;
                                writer = new NetDataWriter();
                                writer.Put(player.MaxHealth);
                                _netPacketProcessor.Send<UpdatePlayerInfoPacket>(networkListener, new UpdatePlayerInfoPacket()
                                {
                                    UserID = player.UserID,
                                    Type = (byte)10,
                                    Data = writer.Data
                                }, DeliveryMethod.ReliableOrdered);
                            }
                            if (player.GroupName != realPlayer.GroupName)
                            {
                                player.GroupName = realPlayer.GroupName;
                                writer = new NetDataWriter();
                                writer.Put(player.GroupName);
                                _netPacketProcessor.Send<UpdatePlayerInfoPacket>(networkListener, new UpdatePlayerInfoPacket()
                                {
                                    UserID = player.UserID,
                                    Type = (byte)11,
                                    Data = writer.Data
                                }, DeliveryMethod.ReliableOrdered);
                            }
                            if (player.RankColor != realPlayer.RankColor)
                            {
                                player.RankColor = realPlayer.RankColor;
                                writer = new NetDataWriter();
                                writer.Put(player.RankColor);
                                _netPacketProcessor.Send<UpdatePlayerInfoPacket>(networkListener, new UpdatePlayerInfoPacket()
                                {
                                    UserID = player.UserID,
                                    Type = (byte)12,
                                    Data = writer.Data
                                }, DeliveryMethod.ReliableOrdered);
                            }
                            if (player.RankName != realPlayer.RankName)
                            {
                                player.RankName = realPlayer.RankName;
                                writer = new NetDataWriter();
                                writer.Put(player.RankName);
                                _netPacketProcessor.Send<UpdatePlayerInfoPacket>(networkListener, new UpdatePlayerInfoPacket()
                                {
                                    UserID = player.UserID,
                                    Type = (byte)13,
                                    Data = writer.Data
                                }, DeliveryMethod.ReliableOrdered);
                            }
                            if (realPlayer.SessionVariables.TryGetValue("SP", out object sendP))
                            {
                                if ((bool)sendP && (player.Position.x != realPlayer.Position.x || player.Position.y != realPlayer.Position.y || player.Position.z != realPlayer.Position.z))
                                {
                                    writer = new NetDataWriter();
                                    player.Position = new Position()
                                    {
                                        x = realPlayer.Position.x,
                                        y = realPlayer.Position.y,
                                        z = realPlayer.Position.z
                                    };
                                    writer.Put<Position>(player.Position);
                                    _netPacketProcessor.Send<UpdatePlayerInfoPacket>(networkListener, new UpdatePlayerInfoPacket()
                                    {
                                        UserID = player.UserID,
                                        Type = (byte)14,
                                        Data = writer.Data
                                    }, DeliveryMethod.ReliableOrdered);
                                }
                            }
                            if (realPlayer.SessionVariables.TryGetValue("SP", out object sendR))
                            {
                                if ((bool)sendR && (player.Rotation.x != realPlayer.Rotation.x || player.Rotation.y != realPlayer.Rotation.y || player.Rotation.z != realPlayer.Rotation.z))
                                {
                                    writer = new NetDataWriter();
                                    player.Rotation = new Rotation()
                                    {
                                        x = realPlayer.Rotation.x,
                                        y = realPlayer.Rotation.y,
                                        z = realPlayer.Rotation.z
                                    };
                                    writer.Put<Rotation>(player.Rotation);
                                    _netPacketProcessor.Send<UpdatePlayerInfoPacket>(networkListener, new UpdatePlayerInfoPacket()
                                    {
                                        UserID = player.UserID,
                                        Type = (byte)15,
                                        Data = writer.Data
                                    }, DeliveryMethod.ReliableOrdered);
                                }
                            }


                            if (player.PlayerID != realPlayer.Id)
                            {
                                player.PlayerID = realPlayer.Id;
                                writer = new NetDataWriter();
                                writer.Put(player.PlayerID);
                                _netPacketProcessor.Send<UpdatePlayerInfoPacket>(networkListener, new UpdatePlayerInfoPacket()
                                {
                                    UserID = player.UserID,
                                    Type = (byte)16,
                                    Data = writer.Data
                                }, DeliveryMethod.ReliableOrdered);
                            }

                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.ToString());
                }
            }
        }

        public bool canSendData = false;

        private void Server_SendingConsoleCommand(Exiled.Events.EventArgs.SendingConsoleCommandEventArgs ev)
        {
            foreach(var command in registerdCommands)
            {
                if (!string.IsNullOrEmpty(command.CommandName))
                {
                    if (!command.isRaCommand && command.CommandName == ev.Name.ToUpper())
                    {
                        ev.IsAllowed = false;
                        _netPacketProcessor.Send<ExecuteCommandPacket>(networkListener, new ExecuteCommandPacket()
                        {
                            UserID = ev.Player.UserId,
                            AddonID = command.AddonID,
                            CommandName = command.CommandName,
                            Arguments = ev.Arguments.ToArray()
                        }, DeliveryMethod.ReliableOrdered);
                    }
                }
            }
        }

        private void Server_SendingRemoteAdminCommand(Exiled.Events.EventArgs.SendingRemoteAdminCommandEventArgs ev)
        {
            foreach (var command in registerdCommands)
            {
                if (!string.IsNullOrEmpty(command.CommandName))
                {
                    if (command.isRaCommand && command.CommandName == ev.Name.ToUpper())
                    {
                        ev.IsAllowed = false;
                        _netPacketProcessor.Send<ExecuteCommandPacket>(networkListener, new ExecuteCommandPacket()
                        {
                            UserID = ev.Sender.UserId,
                            AddonID = command.AddonID,
                            CommandName = command.CommandName,
                            Arguments = ev.Arguments.ToArray()
                        }, DeliveryMethod.ReliableOrdered);
                    }
                }
            }
        }

        public void StartNetworkClient()
        {
            _netPacketProcessor.RegisterNestedType<CommandInfoPacket>();
            _netPacketProcessor.RegisterNestedType<PlayerInfoPacket>();
            _netPacketProcessor.RegisterNestedType<Position>();
            _netPacketProcessor.RegisterNestedType<Rotation>();
            _netPacketProcessor.SubscribeReusable<ReceiveAddonDataPacket, NetPeer>(OnReceiveAddonsData);
            _netPacketProcessor.SubscribeReusable<ReceiveAddonsPacket, NetPeer>(OnReceiveAddons);
            _netPacketProcessor.SubscribeReusable<PlayerInteractPacket, NetPeer>(OnPlayerInteract);
            _netPacketProcessor.SubscribeReusable<RoundRestartPacket, NetPeer>(OnRoundRestart);
            _netPacketProcessor.SubscribeReusable<SendBroadcastPacket, NetPeer>(OnSendBroadcast);
            _netPacketProcessor.SubscribeReusable<SendHintPacket, NetPeer>(OnSendHint);
            _netPacketProcessor.SubscribeReusable<ClearBroadcastsPacket, NetPeer>(OnClearBroadcast);
            _netPacketProcessor.SubscribeReusable<ReceiveCommandsPacket, NetPeer>(OnReceiveCommandsData);
            _netPacketProcessor.SubscribeReusable<ExecuteConsoleCommandPacket, NetPeer>(OnExecuteConsoleCommand);
            networkListener = new NetManager(this);
            networkListener.Start();
            networkListener.Connect(plugin.Config.hostAddress, plugin.Config.hostPort, defaultdata);
            Timing.RunCoroutine(RefreshPolls());
            Timing.RunCoroutine(SendPlayersInfo());
        }

        private void OnExecuteConsoleCommand(ExecuteConsoleCommandPacket packet, NetPeer peer)
        {
            var sender = new CustomConsoleExecutor(this, packet.Command);
            GameCore.Console.singleton.TypeCommand(packet.Command, sender);
        }

        private void OnSendHint(SendHintPacket packet, NetPeer peer)
        {
            foreach (var plr in Player.List)
                if (plr.ReferenceHub.serverRoles.LocalRemoteAdmin || !packet.isAdminOnly)
                    plr.ShowHint(packet.Message, packet.Duration);
        }

        private void OnRoundRestart(RoundRestartPacket packet, NetPeer peer)
        {
            if (packet.Port != 0)
                ReferenceHub.HostHub.playerStats.RpcRoundrestartRedirect(0f, packet.Port);
            else
                ReferenceHub.HostHub.playerStats.Roundrestart();
        }

        private void OnReceiveAddonsData(ReceiveAddonDataPacket packet, NetPeer peer)
        {
            var reader = new NetDataReader(packet.Data);
            foreach (var addon in addons.Where(p5 => p5.Key == packet.AddonID))
            {
                try
                {
                    addon.Value.addon.OnMessageReceived(null, reader);
                }
                catch (Exception ex)
                {
                    Logger.Error($"Error while invoking OnMessageReceived in addon {addon.Value.info.addonName} {ex.ToString()}");
                }
            }
        }

        private void OnClearBroadcast(ClearBroadcastsPacket packet, NetPeer peer)
        {
            Server.Broadcast.RpcClearElements();
        }

        private void OnSendBroadcast(SendBroadcastPacket packet, NetPeer peer)
        {
            if (packet.isAdminOnly)
                foreach (var plr in Player.List)
                    if (plr.ReferenceHub.serverRoles.LocalRemoteAdmin)
                        plr.Broadcast(packet.Duration, packet.Message, Broadcast.BroadcastFlags.Normal);
            else
                Server.Broadcast.RpcAddElement(packet.Message, packet.Duration, Broadcast.BroadcastFlags.Normal);
        }

        private void OnPlayerInteract(PlayerInteractPacket packet, NetPeer peer)
        {
            NetDataReader reader = new NetDataReader(packet.Data);

            Player p = (packet.UserID == "SERVER CONSOLE" || packet.UserID == "GAME CONSOLE") ? Player.Get(PlayerManager.localPlayer) : Player.Get(packet.UserID);
            if (p == null)
            {
                Logger.Info($"Player not found {packet.UserID}, action: {packet.Type}.");
                return;
            }
            switch (packet.Type)
            {
                //Kill player
                case 0:
                    p.Kill();
                    break;
                //Report message
                case 1:
                    p.SendConsoleMessage("[REPORTING] " + reader.GetString(), "GREEN");
                    break;
                //Remoteadmin message
                case 2:
                    p.RemoteAdminMessage(reader.GetString(), true, "NP");
                    break;
                //Console message
                case 3:
                    p.SendConsoleMessage(reader.GetString(), reader.GetString());
                    break;
                //Redirect
                case 4:
                    SendClientToServer(p, reader.GetUShort());
                    break;
                //Disconnect
                case 5:
                    ServerConsole.Disconnect(p.GameObject, reader.GetString());
                    break;
                //Hint
                case 6:
                    p.ShowHint(reader.GetString(), reader.GetFloat());
                    break;
                //Send position to network
                case 7:
                    bool sendPosition = reader.GetBool();
                    if (!p.SessionVariables.ContainsKey("SP"))
                        p.SessionVariables.Add("SP", sendPosition);
                    p.SessionVariables["SP"] = sendPosition;
                    break;
                //Send rotation to network
                case 8:
                    bool sendRotation = reader.GetBool();
                    if (!p.SessionVariables.ContainsKey("SR"))
                        p.SessionVariables.Add("SR", sendRotation);
                    p.SessionVariables["SR"] = sendRotation;
                    break;
                //Teleport
                case 9:
                    p.Position = new UnityEngine.Vector3(reader.GetFloat(), reader.GetFloat(), reader.GetFloat());
                    break;
                //Godmode
                case 10:
                    p.IsGodModeEnabled = reader.GetBool();
                    break;
                //Noclip
                case 11:
                    p.NoClipEnabled = reader.GetBool();
                    break;
                //Clear Inv
                case 12:
                    p.ClearInventory();
                    break;
            }
        }

        private void OnReceiveAddons(ReceiveAddonsPacket packet, NetPeer peer)
        {
            foreach (var addon in addons.Where(p => packet.AddonIds.Contains(p.Key)))
            {
                try
                {
                    addon.Value.addon.OnReady(null);
                }
                catch (Exception ex)
                {
                    Log.Error($"Error while invoking OnReady in addon {addon.Value.info.addonName} {ex.ToString()}");
                }
            }
            canSendData = true;
        }

        private void OnReceiveCommandsData(ReceiveCommandsPacket packet, NetPeer peer)
        {
            registerdCommands = packet.Commands;
            foreach(var command in registerdCommands)
            {
                Logger.Info($"Command {command.CommandName} registered from addon {command.AddonID}, isRa?: {command.isRaCommand}");
            }
        }

        public List<CommandInfoPacket> registerdCommands = new List<CommandInfoPacket>();

        public void SendClientToServer(Player hub, ushort port)
        {
            var serverPS = hub.ReferenceHub.playerStats;
            NetworkWriter writer = NetworkWriterPool.GetWriter();
            writer.WriteSingle(1f);
            writer.WriteUInt16(port);
            RpcMessage msg = new RpcMessage
            {
                netId = serverPS.netId,
                componentIndex = serverPS.ComponentIndex,
                functionHash = GetMethodHash(typeof(PlayerStats), "RpcRoundrestartRedirect"),
                payload = writer.ToArraySegment()
            };
            hub.Connection.Send<RpcMessage>(msg, 0);
            NetworkWriterPool.Recycle(writer);
        }

        private static int GetMethodHash(Type invokeClass, string methodName)
        {
            return invokeClass.FullName.GetStableHashCode() * 503 + methodName.GetStableHashCode();
        }

        public IEnumerator<float> RefreshPolls()
        {
            while(true)
            {
                yield return Timing.WaitForOneFrame;
                if (networkListener != null)
                    if (networkListener.IsRunning)
                        networkListener.PollEvents();
            }
        }

        public IEnumerator<float> SendPlayersInfo()
        {
            while(true)
            {
                yield return Timing.WaitForSeconds(5f);
                try
                {
                    UpdatePlayers();
                }
                catch (Exception) { }
            }
        }

        public void UpdatePlayers()
        {
            List<PlayerInfoPacket> players = new List<PlayerInfoPacket>();
            foreach (var plr in Player.List)
            {
                players.Add(new PlayerInfoPacket()
                {
                    UserID = plr.UserId
                });
            }
            _netPacketProcessor.Send<ReceivePlayersDataPacket>(networkListener, new ReceivePlayersDataPacket()
            {
                Players = players
            }, DeliveryMethod.ReliableOrdered);
        }

        public CoroutineHandle dataChecker;

        public IEnumerator<float> Reconnect()
        {
            yield return Timing.WaitForSeconds(5f);
            Logger.Info($"Reconnecting to {plugin.Config.hostAddress}:{plugin.Config.hostPort}...");
            networkListener.Connect(plugin.Config.hostAddress, plugin.Config.hostPort, defaultdata);
        }

        public void OnPeerConnected(NetPeer peer)
        {
            Logger.Info("Client connected to host.");
            List<string> addon = new List<string>();
            foreach (var addon2 in addons)
                addon.Add(addon2.Key);
            _netPacketProcessor.Send<ReceiveAddonsPacket>(peer, new ReceiveAddonsPacket()
            {
                AddonIds = addon.ToArray()
            }, DeliveryMethod.ReliableOrdered);
            Logger.Info("Addons info sended to host, waiting to response...");
            dataChecker = Timing.RunCoroutine(DataCheckers());
        }

        public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            Logger.Info($"Client disconnected from host. (Info: {disconnectInfo.Reason.ToString()})");
            Timing.RunCoroutine(Reconnect());
            registerdCommands.Clear();
            canSendData = false;
            if (dataChecker != null)
                Timing.KillCoroutines(dataChecker);
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
            }
            catch (Exception ex)
            {
                Logger.Error($"Error while receiving data from server {ex}");
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
        }
    }
}
