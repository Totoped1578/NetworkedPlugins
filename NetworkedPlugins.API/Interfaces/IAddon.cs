using LiteNetLib.Utils;
using NetworkedPlugins.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkedPlugins.API.Interfaces
{
    public interface IAddon<out TConfig> : IComparable<IAddon<IConfig>>
        where TConfig : IConfig
    {
        NPManager Manager { get; set; }

        NPLogger Logger { get; set; }

        void OnEnable();

        void OnMessageReceived(NPServer server, NetDataReader reader);

        void OnReady(NPServer server);

        void OnConsoleCommand(string cmd, List<string> arguments);

        void OnConsoleResponse(NPServer server, string command, string response, bool isRa);

        string addonId { get; set; }

        string defaultPath { get; set; }

        string addonPath { get; set; }

        TConfig Config { get; }
    }
}
