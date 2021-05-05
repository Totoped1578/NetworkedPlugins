using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkedPlugins.API
{
    public abstract class NPLogger
    {
        public abstract void Info(string message);
        public abstract void Error(string message);
        public abstract void Debug(string message);
    }
}
