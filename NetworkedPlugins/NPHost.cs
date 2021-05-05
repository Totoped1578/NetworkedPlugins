using Exiled.API.Features;
using LiteNetLib;
using LiteNetLib.Utils;
using MEC;
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
    public class NPHost : NPManager
    {
        private MainClass plugin;

        public static NPHost singleton;

        public NPHost(MainClass plugin)
        {
            this.plugin = plugin;
            Logger = new PluginLogger();
            singleton = this;
            string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string pluginDir = Path.Combine(folderPath, "EXILED", "Plugins", "NetworkedPlugins");
            if (!Directory.Exists(pluginDir))
                Directory.CreateDirectory(pluginDir);
            if (!Directory.Exists(Path.Combine(pluginDir, "addons-" + Server.Port)))
                Directory.CreateDirectory(Path.Combine(pluginDir, "addons-" + Server.Port));
            string[] addonsFiles = Directory.GetFiles(Path.Combine(pluginDir, "addons-" + Server.Port), "*.dll");
            Logger.Info($"Loading {addonsFiles.Length} addons.");
            foreach (var file in addonsFiles)
            {
                Assembly a = Assembly.LoadFrom(file);
                try
                {
                    foreach (Type t in a.GetTypes())
                    {
                        if (t.IsSubclassOf(typeof(NPAddonHost)) && t != typeof(NPAddonHost))
                        {
                            NPAddonHost addon = (NPAddonHost)Activator.CreateInstance(t);
                            NPAddonInfo addonInfo = (NPAddonInfo)Attribute.GetCustomAttribute(t, typeof(NPAddonInfo));
                            addon.Manager = this;
                            addon.Logger = Logger;
                            addon.addonId = addonInfo.addonID;
                            addon.defaultPath = Path.Combine(pluginDir, "addons-" + Server.Port);
                            Logger.Info($"Loading addon {addonInfo.addonName}.");
                            if (addons.ContainsKey(addonInfo.addonID))
                            {
                                Log.Warn($"Addon {addons[addonInfo.addonID].info.addonName} already have id {addonInfo.addonName}.");
                                break;
                            }
                            addon.OnEnable();
                            Logger.Info($"Waiting to client connections..");
                            addons.Add(addonInfo.addonID, new NPAddonItem() { addon = addon, info = addonInfo });
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
            eventListener = new EventBasedNetListener();
            networkListener = new NetManager(eventListener);
            eventListener.PeerDisconnectedEvent += HostDisconnected;
            eventListener.PeerConnectedEvent += HostConnected;
            eventListener.NetworkReceiveEvent += HostReceive;
            eventListener.ConnectionRequestEvent += HostConnectionRequest;
            networkListener.Start(plugin.Config.hostPort);
            Timing.RunCoroutine(RefreshPolls());
        }

        private void HostConnectionRequest(ConnectionRequest request)
        {
            if (request.Data.TryGetString(out string key))
            {
                if (key == plugin.Config.hostConnectionKey)
                {
                    if (request.Data.TryGetUShort(out ushort port))
                    {
                        if (request.Data.TryGetInt(out int maxplayers))
                        {
                            var peer = request.Accept();
                            if (!servers.ContainsKey(peer))
                                servers.Add(peer, new NPServer(peer, peer.EndPoint.Address.ToString(), port, maxplayers));
                            else
                                servers[peer] = new NPServer(peer, peer.EndPoint.Address.ToString(), port, maxplayers);
                            return;
                        }

                    }

                }
            }
            request.Reject();
        }

        private void HostReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            byte type = reader.GetByte();
            switch (type)
            {
                //Receive info about addons info from client
                case 0:
                    if (!servers.ContainsKey(peer))
                        break;
                    NPAddonsInfo info = JsonConvert.DeserializeObject<NPAddonsInfo>(reader.GetString());
                    string adds = "";
                    foreach (var i in info.addonIds.Where(p => addons.ContainsKey(p)).Select(s => addons[s]))
                    {
                        servers[peer].Addons.Add(i);
                        adds += $"{i.info.addonName} - {i.info.addonVersion}v made by {i.info.addonAuthor}" + Environment.NewLine;
                        (i.addon as NPAddonHost).OnReady(servers[peer]);
                    }
                    Logger.Info($"Received addons from client {peer.EndPoint.Address.ToString()}. {Environment.NewLine} {adds}");
                    NetDataWriter writer = new NetDataWriter();
                    writer.Put((byte)0);
                    peer.Send(writer, DeliveryMethod.ReliableOrdered);
                    break;
                case 3:
                    if (!servers.ContainsKey(peer))
                        break;
                    string addonId = reader.GetString();
                    foreach (var addon in addons.Where(pp => pp.Key == addonId))
                    {
                        (addon.Value.addon as NPAddonHost).OnMessageReceived(servers[peer], new NetDataReader(reader.GetBytesWithLength()));
                    }
                    break;
                case 4:
                    if (!servers.ContainsKey(peer))
                        break;
                    List<NPPlayer> players = JsonConvert.DeserializeObject<List<NPPlayer>>(reader.GetString());
                    IEnumerable<string> userids = players.Select(p => p.UserID);
                    foreach (var plr2 in servers[peer].Players.Where(p => !userids.Contains(p.Key)))
                    {
                        servers[peer].Players.Remove(plr2.Key);
                    }
                    foreach (var plr in players)
                    {
                        servers[peer].Players[plr.UserID] = plr;
                    }
                    break;
            }
            reader.Recycle();
        }

        private void HostDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            Logger.Info($"Client {peer.EndPoint.Address.ToString()} disconnected from host. (Info: {disconnectInfo.Reason.ToString()})");
            if (servers.ContainsKey(peer))
                servers.Remove(peer);
        }

        private void HostConnected(NetPeer peer)
        {
            Logger.Info($"Client {peer.EndPoint.Address.ToString()} connected to host.");
        }

        public IEnumerator<float> RefreshPolls()
        {
            while (true)
            {
                yield return Timing.WaitForOneFrame;
                if (networkListener != null)
                    if (networkListener.IsRunning)
                        networkListener.PollEvents();
            }
        }
    }
}
