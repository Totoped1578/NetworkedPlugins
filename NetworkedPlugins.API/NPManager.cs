using LiteNetLib;
using NetworkedPlugins.API.Attributes;
using NetworkedPlugins.API.Interfaces;
using NetworkedPlugins.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkedPlugins.API
{
    public class NPManager
    {
        public Dictionary<string, NPAddonItem> addons = new Dictionary<string, NPAddonItem>();

        public Dictionary<NetPeer, NPServer> servers = new Dictionary<NetPeer, NPServer>();
        public Dictionary<string, Dictionary<string, Tuple<string, ICommand>>> commands { get; set; } = new Dictionary<string, Dictionary<string, Tuple<string, ICommand>>>();

        public void RegisterCommand(string addonID, string commandName, string permission, ICommand command)
        {
            if (!commands.ContainsKey(addonID))
                commands.Add(addonID, new Dictionary<string, Tuple<string, ICommand>>());
            if (!commands[addonID].ContainsKey(commandName))
            {
                commands[addonID].Add(commandName, new Tuple<string, ICommand>(permission, command));
            }
            Logger.Info($"Command: {commandName} registered for addon {addons[addonID].info.addonName}");
        }

        public void ExecuteCommand(PlayerFuncs plr, string addonId, string commandName, List<string> arguments)
        {
            if (commands[addonId].ContainsKey(commandName))
            {
                commands[addonId][commandName].Item2.Invoke(plr, arguments);
            }
        }

        public Dictionary<string, string> GetCommands(string addonId)
        {
            Dictionary<string, string> cmds2 = new Dictionary<string, string>();
            if (!commands.ContainsKey(addonId))
                commands.Add(addonId, new Dictionary<string, Tuple<string, ICommand>>());
            foreach(var cmd in commands[addonId])
            {
                cmds2.Add(cmd.Key, cmd.Value.Item1);
            }
            return cmds2;
        }


        public static NPLogger Logger;

        public EventBasedNetListener eventListener;
        public NetManager networkListener;
    }
}
