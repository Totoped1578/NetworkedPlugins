using NetworkedPlugins.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NPDedicatedApp
{
    public class Program
    {
        public static Host host;
        static void Main(string[] args)
        {
            host = new Host();
            while (true)
            {
                var line = Console.ReadLine();
                var proc = line.Split(' ');
                foreach(var h in host.addons)
                {
                    (h.Value.addon as NPAddonDedicated).OnConsoleCommand(proc[0], proc.Skip(1).ToList());
                }
            }
        }
    }
}
