using NetworkedPlugins.API.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteConsole
{
    public class RemoteConsoleConfig : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public string BotToken { get; set; } = "";
        public Dictionary<int, RC> RemoteConsoles { get; set; }
    }

    public class RC
    {
        public ushort ServerPort { get; set; }
        public ulong GuildID { get; set; }
        public ulong ChannelID { get; set; }
        public ulong MainMessageID { get; set; }
        public ulong PlayerInfoMessageID { get; set; }
    }
}
