using NetworkedPlugins.API;
using NetworkedPlugins.API.Attributes;
using NetworkedPlugins.API.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalStats.Commands
{
    [NPCommand("test")]
    public class TestCmd : ICommand
    {
        public void Invoke(PlayerFuncs player, List<string> arguments)
        {
            NPManager.Logger.Info("Execute command TEST");
        }
    }
}
