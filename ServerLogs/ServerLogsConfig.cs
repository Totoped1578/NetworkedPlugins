using NetworkedPlugins.API.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerLogs
{
    public class ServerLogsConfig : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public string DBConnection { get; set; } = "";
    }
}
