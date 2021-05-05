using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HubAddon
{
    public class HubConfig
    {
        public class ConfigDedicated
        {
            public ushort LobbyPort { get; set; } = 7777;
            public Dictionary<string, ServerInfo> Servers { get; set; } = new Dictionary<string, ServerInfo>() { { "localhost:7777", new ServerInfo() { ServerName = "Test server" } } };
            public string GetServerName(ushort port)
            {
                foreach (var server in Servers)
                {
                    string[] sm = server.Key.Split(':');
                    if (ushort.TryParse(sm[1], out ushort sport))
                        if (sport == port)
                            return server.Value.ServerName;
                }
                return "";
            }

            public Tuple<string, ushort, ServerInfo> GetServerByName(string name)
            {
                foreach (var server in Servers)
                {
                    if (server.Key.ToUpper() == name.ToUpper() || server.Key.ToUpper().Contains(name.ToUpper()))
                    {
                        string[] sm = server.Key.Split(':');
                        ushort port = 0;
                        ushort.TryParse(sm[1], out port);
                        return new Tuple<string, ushort, ServerInfo>(server.Key, port, server.Value);
                    }
                }
                return null;
            }
        }

        public class ConfigHost
        {
            public bool isLobby { get; set; } = false;
            public ushort LobbyPort { get; set; } = 7777;
            public Dictionary<string, ServerInfo> Servers { get; set; } = new Dictionary<string, ServerInfo>() { { "localhost:7777", new ServerInfo() { ServerName = "Test server" } } };
            public string GetServerName(ushort port)
            {
                foreach (var server in Servers)
                {
                    string[] sm = server.Key.Split(':');
                    if (ushort.TryParse(sm[1], out ushort sport))
                        if (sport == port)
                            return server.Value.ServerName;
                }
                return "";
            }

            public Tuple<string, ushort, ServerInfo> GetServerByName(string name)
            {
                foreach (var server in Servers)
                {
                    if (server.Key.ToUpper() == name.ToUpper() || server.Key.ToUpper().Contains(name.ToUpper()))
                    {
                        string[] sm = server.Key.Split(':');
                        ushort port = 0;
                        ushort.TryParse(sm[1], out port);
                        return new Tuple<string, ushort, ServerInfo>(server.Key, port, server.Value);
                    }
                }
                return null;
            }
        
        }

        public class ServerInfo
        {
            public string ServerName { get; set; } = "DefaultName";
            public bool Restricted { get; set; } = false;
        }

        public class ConfigClient
        {
            public bool isLobby { get; set; } = false;
            public ushort LobbyPort { get; set; } = 7777;
        }
    }
}
