using LiteNetLib;
using LiteNetLib.Utils;
using NetworkedPlugins.API.Attributes;
using NetworkedPlugins.API.Extensions;
using NetworkedPlugins.API.Interfaces;
using NetworkedPlugins.API.Models;
using NetworkedPlugins.API.Packets;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkedPlugins.API
{
    public class NPManager
    {
        public Dictionary<string, NPAddonItem> addons = new Dictionary<string, NPAddonItem>();

        public Dictionary<NetPeer, NPServer> servers = new Dictionary<NetPeer, NPServer>();

        public Dictionary<string, Dictionary<string, ICommand>> commands = new Dictionary<string, Dictionary<string, ICommand>>();

        public void RegisterCommand(string addonId, ICommand command)
        {

            if (!commands.ContainsKey(addonId))
                commands.Add(addonId, new Dictionary<string, ICommand>());
            if (!commands[addonId].ContainsKey(command.CommandName.ToUpper()))
            {
                commands[addonId].Add(command.CommandName.ToUpper(), command);
                Logger.Info($"Command {command.CommandName.ToUpper()} registered in addon {addonId}");
            }
        }

        public void ExecuteCommand(PlayerFuncs plr, string addonId, string commandName, List<string> arguments)
        {
            if (commands[addonId].ContainsKey(commandName.ToUpper()))
            {
                commands[addonId][commandName.ToUpper()].Invoke(plr, arguments);
            }
        }

        public void LoadAddonConfig(string addonId)
        {
            if (addons.TryGetValue(addonId, out NPAddonItem npdi))
            {
                if (!Directory.Exists(Path.Combine(npdi.addon.defaultPath, npdi.info.addonName)))
                {
                    Directory.CreateDirectory(Path.Combine(npdi.addon.defaultPath, npdi.info.addonName));
                }
                if (!File.Exists(Path.Combine(npdi.addon.defaultPath, npdi.info.addonName, "config.json")))
                {
                    File.WriteAllText(Path.Combine(npdi.addon.defaultPath, npdi.info.addonName, "config.json"), JsonConvert.SerializeObject(npdi.addon.Config, Formatting.Indented));
                }
                var cfg = (IConfig)JsonConvert.DeserializeObject(File.ReadAllText(Path.Combine(npdi.addon.defaultPath, npdi.info.addonName, "config.json")), npdi.addon.Config.GetType());
                File.WriteAllText(Path.Combine(npdi.addon.defaultPath, npdi.info.addonName, "config.json"), JsonConvert.SerializeObject(cfg, Formatting.Indented));
                npdi.addon.Config.CopyProperties(cfg);
            }
        }

        public List<CommandInfoPacket> GetCommands(string addonId)
        {
            List<CommandInfoPacket> cmds = new List<CommandInfoPacket>();
            if (commands.TryGetValue(addonId, out Dictionary<string, ICommand> outCmds))
            {
                foreach (var cmd in outCmds)
                {
                    cmds.Add(new CommandInfoPacket()
                    {
                        AddonID = addonId,
                        CommandName = cmd.Key,
                        Description = cmd.Value.Description,
                        Permission = cmd.Value.Permission,
                        isRaCommand = cmd.Value.IsRaCommand
                    });
                }
            }
            return cmds;
        }


        public static NPLogger Logger;
        public readonly NetPacketProcessor _netPacketProcessor = new NetPacketProcessor();
        public NetManager networkListener;
    }
}
