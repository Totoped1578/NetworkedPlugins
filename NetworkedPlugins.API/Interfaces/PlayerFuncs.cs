using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkedPlugins.API.Interfaces
{
    public abstract class PlayerFuncs
    {
        public abstract string UserName { get; }
        public abstract string UserID { get; }
        public abstract int Role { get; }
        public abstract void Kill();
        public abstract void SendReportMessage(string message);
        public abstract void SendRAMessage(string message);
        public abstract void SendConsoleMessage(string message, string color = "GREEN");
        public abstract void Redirect(ushort port);
        public abstract void Disconnect(string reason);
    }
}
