using NetworkedPlugins.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NetworkedPlugins
{
    public class PluginLogger : NPLogger
    {
        public override void Debug(string message)
        {
            Exiled.API.Features.Log.Debug($"[{Assembly.GetCallingAssembly().GetName().Name}] " + message);
        }

        public override void Error(string message)
        {
            Exiled.API.Features.Log.Error($"[{Assembly.GetCallingAssembly().GetName().Name}] " + message);
        }

        public override void Info(string message)
        {
            Exiled.API.Features.Log.Info($"[{Assembly.GetCallingAssembly().GetName().Name}] " + message);
        }
    }
}
