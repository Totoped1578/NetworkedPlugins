using NetworkedPlugins.API;
using NetworkedPlugins.API.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NPDedicatedApp
{
    public class Program
    {
        public static Host host;
        static void Main(string[] args)
        
        {
            if (!Directory.Exists("./dependencies"))
                Directory.CreateDirectory("./dependencies");
            string[] depsFIles = Directory.GetFiles("./dependencies", "*.dll");
            foreach (var deps in depsFIles)
            {
                Assembly a = Assembly.LoadFrom(deps);
            }
            host = new Host();
            while (true)
            {
                var line = Console.ReadLine();
                var proc = line.Split(' ');
                foreach(var h in host.addons)
                {
                    h.Value.addon.OnConsoleCommand(proc[0], proc.Skip(1).ToList());
                }
            }
        }
    }
}
