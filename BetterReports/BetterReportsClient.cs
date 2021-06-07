using Exiled.Events.EventArgs;
using LiteNetLib.Utils;
using NetworkedPlugins.API;
using NetworkedPlugins.API.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterReports
{
    [NPAddonInfo(
        addonID = "BP032DxpREPORTS",
        addonAuthor = "Killers0992",
        addonName = "BetterReports",
        addonVersion = "0.0.1")]
    public class BetterReportsClient : NPAddonClient<BetterReportsClientConfig>
    {
        public override void OnEnable()
        {
            Exiled.Events.Handlers.Server.LocalReporting += OnLocalReport;
        }

        private void OnLocalReport(LocalReportingEventArgs ev)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put((byte)0);
            writer.Put(ev.Issuer.UserId);
            writer.Put(ev.Target.UserId);
            writer.Put(ev.Reason);
            SendData(writer);
        }
    }
}
