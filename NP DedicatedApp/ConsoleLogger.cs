using NetworkedPlugins.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NPDedicatedApp
{
    public class ConsoleLogger : NPLogger
    {
        public override void Debug(string message)
        {
            Console.WriteLine($" [{DateTime.Now.ToString("T")}] [DEBUG] [{Assembly.GetCallingAssembly().GetName().Name}] " + message);
        }

        public override void Error(string message)
        {
            Console.WriteLine($" [{DateTime.Now.ToString("T")}] [ERROR] [{Assembly.GetCallingAssembly().GetName().Name}] " + message);
        }

        public override void Info(string message)
        {
            Console.WriteLine($" [{DateTime.Now.ToString("T")}] [INFO] [{Assembly.GetCallingAssembly().GetName().Name}] " + message);
        }
    }
}
