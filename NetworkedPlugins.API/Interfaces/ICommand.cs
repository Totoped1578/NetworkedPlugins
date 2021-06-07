using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkedPlugins.API.Interfaces
{
    public interface ICommand
    {
        string CommandName { get; }
        string Description { get; }
        string Permission { get; }
        bool IsRaCommand { get; }
        void Invoke(PlayerFuncs player, List<string> arguments);
    }
}
