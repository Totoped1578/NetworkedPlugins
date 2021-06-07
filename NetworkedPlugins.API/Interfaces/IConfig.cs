using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkedPlugins.API.Interfaces
{
    public interface IConfig
    {
        bool IsEnabled { get; set; }
    }
}
