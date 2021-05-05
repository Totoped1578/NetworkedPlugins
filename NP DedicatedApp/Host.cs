using LiteNetLib;
using LiteNetLib.Utils;
using NetworkedPlugins.API;
using NetworkedPlugins.API.Attributes;
using NetworkedPlugins.API.Interfaces;
using NetworkedPlugins.API.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NPDedicatedApp
{
    public class Host : NPManager
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
                var file = File.Create("./config.json");
                file.Write(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new PConfig(), Formatting.Indented)));
                file.Flush();
            }
            config = JsonConvert.DeserializeObject<PConfig>(File.ReadAllText("./config.json"));
            if (!Directory.Exists("./addons"))
                Directory.CreateDirectory("./addons");
            if (!Directory.Exists("./dependencies"))
                Directory.CreateDirectory("./dependencies");
            string[] depsFIles = Directory.GetFiles("./dependencies", "*.dll");

            foreach (var deps  in depsFIles)
            {
                Assembly a = Assembly.LoadFrom(deps);

            }
            string[] addonsFiles = Directory.GetFiles("./addons", "*.dll");
            Logger.Info($"Loading {addonsFiles.Length} addons.");
            foreach (var file in addonsFiles)
            {
                Assembly a = Assembly.LoadFrom(file);
                try
                {
                    string addonID = "";
                    foreach (Type t in a.GetTypes())
                    {
                        if (t.IsSubclassOf(typeof(NPAddonDedicated)) && t != typeof(NPAddonDedicated))
                        {
                            NPAddonDedicated addon = (NPAddonDedicated)Activator.CreateInstance(t);
                            NPAddonInfo addonInfo = (NPAddonInfo)Attribute.GetCustomAttribute(t, typeof(NPAddonInfo));
                            addon.Manager = this;
                            addon.Logger = Logger;
                            addon.addonId = addonInfo.addonID;
                            addonID = addonInfo.addonID;
                            addon.defaultPath = Path.Combine("addons");
                            Logger.Info($"Loading addon {addonInfo.addonName}.");
                            if (addons.ContainsKey(addonInfo.addonID))
                            {
                                Logger.Error($"Addon {addons[addonInfo.addonID].info.addonName} already have id {addonInfo.addonName}.");
                                break;
                            }
                            addon.OnEnable();
                            Logger.Info($"Waiting to client connections..");
                            addons.Add(addonInfo.addonID, new NPAddonItem() { addon = addon, info = addonInfo });
                        }
                    }
                    if (addonID != "")
                    {
                        foreach (Type t in a.GetTypes())
                        {
                            if (t.GetInterface("ICommand") == typeof(ICommand))
                            {
                                NPCommand cmd = (NPCommand)Attribute.GetCustomAttribute(t, typeof(NPCommand));
                                if (cmd != null)
                                {
                                    ICommand cmdFunc = (ICommand)Activator.CreateInstance(t);
                                    RegisterCommand(addonID, cmd.CommandName.ToUpper(), cmd.Permission, cmdFunc);
                                }
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
            eventListener = new EventBasedNetListener();
            networkListener = new NetManager(eventListener);
            eventListener.PeerDisconnectedEvent += HostDisconnected;
            eventListener.PeerConnectedEvent += HostConnected;
            eventListener.NetworkReceiveEvent += HostReceive;
            eventListener.ConnectionRequestEvent += HostConnectionRequest;
            Logger.Info($"IP: {config.hostAddress}");
            Logger.Info($"Port: {config.hostPort}");
            networkListener.Start(config.hostPort);
            Task.Factory.StartNew(async () =>
            {
                await RefreshPolls();
            });
        }

        private void HostConnectionRequest(ConnectionRequest request)
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
                                servers.Add(peer, new NPServer(peer, peer.EndPoint.Address.ToString(), port, maxplayers));
                            else
                                servers[peer] = new NPServer(peer, peer.EndPoint.Address.ToString(), port, maxplayers);
                            Logger.Info($"New server added {peer.EndPoint.Address.ToString()}, port: {port}");
                            return;
                        }
                    }

                }
            }
        }

        private void HostReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            byte type = reader.GetByte();
            try
            {
                switch (type)
                {
                    //Receive info about addons info from client
                    case 0:
                        if (!servers.ContainsKey(peer))
                            break;
                        NPAddonsInfo info = JsonConvert.DeserializeObject<NPAddonsInfo>(reader.GetString());
                        string adds = "";
                        Dictionary<string, string> cmds = new Dictionary<string, string>();
                        foreach (var i in info.addonIds.Where(p => addons.ContainsKey(p)).Select(s => addons[s]))
                        {
                            servers[peer].Addons.Add(i);
                            adds += $"{i.info.addonName} - {i.info.addonVersion}v made by {i.info.addonAuthor}" + Environment.NewLine;
                            (i.addon as NPAddonDedicated).OnReady(servers[peer]);
                            foreach (var cm in GetCommands(i.info.addonID))
                            {
                                if (!cmds.ContainsKey(cm.Key))
                                    cmds.Add(cm.Key, cm.Value);
                            }
                        }
                        Logger.Info($"Received addons from client {peer.EndPoint.Address.ToString()}. {Environment.NewLine} {adds}");
                        NetDataWriter writer = new NetDataWriter();
                        writer.Put((byte)0);
                        writer.Put(JsonConvert.SerializeObject(cmds));
                        peer.Send(writer, DeliveryMethod.ReliableOrdered);
                        break;
                    case 3:
                        if (!servers.ContainsKey(peer))
                            break;
                        string addonId = reader.GetString();
                        foreach (var addon in addons.Where(pp => pp.Key == addonId))
                        {
                            (addon.Value.addon as NPAddonDedicated).OnMessageReceived(servers[peer], new NetDataReader(reader.GetBytesWithLength()));
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
                            plr.server = servers[peer];
                            servers[peer].Players[plr.UserID] = plr;
                        }
                        break;
                }
            }catch(Exception ex)
            {
                Logger.Error(ex.ToString());
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
    }
}
