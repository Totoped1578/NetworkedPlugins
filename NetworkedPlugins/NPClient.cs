using Exiled.API.Features;
using LiteNetLib;
using LiteNetLib.Utils;
using MEC;
using Mirror;
using NetworkedPlugins.API;
using NetworkedPlugins.API.Attributes;
using NetworkedPlugins.API.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NetworkedPlugins
{
    public class NPClient : NPManager
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
                    foreach (Type t in a.GetTypes())
                    {
                        if (t.IsSubclassOf(typeof(NPAddonClient)) && t != typeof(NPAddonClient))
                        {
                            NPAddonClient addon = (NPAddonClient)Activator.CreateInstance(t);
                            NPAddonInfo addonInfo = (NPAddonInfo)Attribute.GetCustomAttribute(t, typeof(NPAddonInfo));
                            addon.Manager = this;
                            addon.Logger = Logger;
                            addon.addonId = addonInfo.addonID;
                            addon.defaultPath = Path.Combine(pluginDir, "addons-" + Server.Port);
                            Logger.Info($"Loading addon {addonInfo.addonName}.");
                            if (addons.ContainsKey(addonInfo.addonID))
                            {
                                Logger.Info($"Addon {addons[addonInfo.addonID].info.addonName} already have id {addonInfo.addonName}.");
                                break;
                            }
                            addon.OnEnable();
                            Logger.Info($"Waiting to client connections...");
                            addons.Add(addonInfo.addonID, new NPAddonItem() { addon = addon, info = addonInfo });
                        }
                    }
                }
                catch(Exception ex)
                {
                    Logger.Error($"Failed loading addon {Path.GetFileNameWithoutExtension(file)}. {ex.ToString()}");
                }
            }
            Logger.Info($"Starting CLIENT network...");
            Exiled.Events.Handlers.Server.SendingRemoteAdminCommand += Server_SendingRemoteAdminCommand;
            StartNetworkClient();
        }

        private void Server_SendingRemoteAdminCommand(Exiled.Events.EventArgs.SendingRemoteAdminCommandEventArgs ev)
        {
        }

        public void StartNetworkClient()
        {
            eventListener = new EventBasedNetListener();
            networkListener = new NetManager(eventListener);
            eventListener.PeerDisconnectedEvent += ClientDisconnected;
            eventListener.PeerConnectedEvent += ClientConnected;
            eventListener.NetworkReceiveEvent += ClientReceive;
            networkListener.Start();
            networkListener.Connect(plugin.Config.hostAddress, plugin.Config.hostPort, defaultdata);
            Timing.RunCoroutine(RefreshPolls());
            Timing.RunCoroutine(SendPlayersInfo());
        }

        private void ClientReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            byte type = reader.GetByte();
            switch(type)
            {
                //Enable event handlers for addons.
                case 0:

                    foreach(var addon in addons)
                    {
                        try
                        {
                            (addon.Value.addon as NPAddonClient).OnReady();
                        }
                        catch(Exception ex)
                        {
                            Log.Error($"Error while invoking OnConnected in addon {addon.Value.info.addonName} {ex.ToString()}");
                        }
                    }
                    var cmds = JsonConvert.DeserializeObject<Dictionary<string,string>>(reader.GetString());
                    break;
                //Send broadcast
                case 1:
                    bool isAdminOnly = reader.GetBool();
                    string message = reader.GetString();
                    ushort dur = reader.GetUShort();
                    if (isAdminOnly)
                        foreach (var plr in Player.List)
                            if (plr.ReferenceHub.serverRoles.LocalRemoteAdmin)
                                plr.Broadcast(dur, message, Broadcast.BroadcastFlags.Normal);
                    else
                        Server.Broadcast.RpcAddElement(message, dur, Broadcast.BroadcastFlags.Normal);
                    break;
                //Clear broadcasts
                case 2:
                    Server.Broadcast.RpcClearElements();
                    break;
                //Receive addon data
                case 3:
                    string addonId = reader.GetString();
                    foreach(var addon in addons.Where(p5 => p5.Key == addonId))
                    {
                        try
                        {
                            (addon.Value.addon as NPAddonClient).OnMessageReceived(new NetDataReader(reader.GetBytesWithLength()));
                        }
                        catch (Exception ex)
                        {
                            Logger.Error($"Error while invoking OnReceiveData in addon {addon.Value.info.addonName} {ex.ToString()}");
                        }
                    }
                    break;
                //Interact with player
                case 4:
                    string UserID = reader.GetString();
                    byte action = reader.GetByte();
                    Player p = (UserID == "SERVER CONSOLE" || UserID == "GAME CONSOLE") ? Player.Get(PlayerManager.localPlayer) : Player.Get(UserID);
                    if (p == null)
                    {
                        Logger.Info($"Player not found {UserID}, action: {action}.");
                    }
                    else
                    {
                        Logger.Info($"Execute action {action} on player {UserID}.");
                    }
                    switch (action)
                    {
                        //Kill player
                        case 0:
                            if (p != null)
                                p.Kill();
                            break;
                        //Report message
                        case 1:
                            if (p != null)
                                p.SendConsoleMessage("[REPORTING] " + reader.GetString(), "GREEN");
                            break;
                        //Remoteadmin message
                        case 2:
                            if (p != null)
                                p.RemoteAdminMessage(reader.GetString(), true, "NP");
                            break;
                        //Console message
                        case 3:
                            if (p != null)
                                p.SendConsoleMessage(reader.GetString(), reader.GetString());
                            break;
                        //Redirect
                        case 4:
                            if (p != null)
                                SendClientToServer(p, reader.GetUShort());
                            break;
                        //Disconnect
                        case 5:
                            if (p != null)
                                ServerConsole.Disconnect(p.GameObject, reader.GetString());
                            break;
                    }
                    break;
                //Roundrestart
                case 5:
                    ushort port = reader.GetUShort();
                    if (port != 0)
                        ReferenceHub.HostHub.playerStats.RpcRoundrestartRedirect(0f, port);
                    else
                        ReferenceHub.HostHub.playerStats.Roundrestart();
                    break;
                //Send broadcast
                case 6:
                    bool isAdminOnly2 = reader.GetBool();
                    string message2 = reader.GetString();
                    float dur2 = reader.GetFloat();
                        foreach (var plr in Player.List)
                            if (plr.ReferenceHub.serverRoles.LocalRemoteAdmin || isAdminOnly2)
                                plr.ShowHint(message2, dur2);
                    break;
            }
            reader.Recycle();
        }

        private void ClientDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            Logger.Info($"Client disconnected from host. (Info: {disconnectInfo.Reason.ToString()})");
            Timing.RunCoroutine(Reconnect());
        }

        private void ClientConnected(NetPeer peer)
        {
            Logger.Info("Client connected to host.");
            NetDataWriter writer = new NetDataWriter();
            writer.Put((byte)0);
            NPAddonsInfo info = new NPAddonsInfo();
            foreach (var aInfo in addons)
                info.addonIds.Add(aInfo.Key);
            writer.Put(JsonConvert.SerializeObject(info));
            peer.Send(writer, DeliveryMethod.ReliableOrdered);
            Logger.Info("Addons info sended to host, waiting to response...");
        }

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
            List<NPPlayer> players = new List<NPPlayer>();
            foreach (var plr in Player.List)
                players.Add(new NPPlayer(null, plr.Nickname, plr.UserId, (int)plr.Role));
            NetDataWriter writer = new NetDataWriter();
            writer.Put((byte)4);
            writer.Put(JsonConvert.SerializeObject(players));
            networkListener.SendToAll(writer, DeliveryMethod.ReliableOrdered);
        }

        public IEnumerator<float> Reconnect()
        {
            yield return Timing.WaitForSeconds(5f);
            Logger.Info($"Reconnecting to {plugin.Config.hostAddress}:{plugin.Config.hostPort}...");
            networkListener.Connect(plugin.Config.hostAddress, plugin.Config.hostPort, defaultdata);
        }
    }
}
