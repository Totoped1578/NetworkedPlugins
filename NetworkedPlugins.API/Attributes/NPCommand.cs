using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkedPlugins.API.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class NPCommand : Attribute
    {
        public NPCommand(string commandName, string permission = "", bool isRaCommand = false)
        {
            this.CommandName = commandName;
            this.Permission = permission;
            this.IsRaCommand = isRaCommand;
        }

        public string CommandName { get; }
        public string Permission { get; }
        public bool IsRaCommand { get; }
    }
}
