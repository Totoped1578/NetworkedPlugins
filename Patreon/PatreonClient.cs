using Exiled.API.Features;
using LiteNetLib.Utils;
using NetworkedPlugins.API;
using NetworkedPlugins.API.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Patreon
{
    [NPAddonInfo(
       addonID = "DWp3wqpdwaDOpwd",
       addonAuthor = "Killers0992",
       addonName = "Patreon",
       addonVersion = "1.0.0")]
    public class PatreonClient : NPAddonClient
    {
        public override void OnEnable()
        {
            base.OnEnable();
            Exiled.Events.Handlers.Player.Verified += Player_Verified;
        }

        private void Player_Verified(Exiled.Events.EventArgs.VerifiedEventArgs ev)
        {
            NetDataWriter wr = new NetDataWriter();
            wr.Put(ev.Player.UserId);
            SendData(wr);
        }

        public override void OnMessageReceived(NetDataReader reader)
        {
            string userid = reader.GetString();
            string role = reader.GetString();
            string roleColor = reader.GetString();
            var plr = Player.Get(userid);
            if (plr != null)
            {
                if (plr.Group == null)
                {

                        UserGroup r = new UserGroup();
                        r.BadgeColor = roleColor;
                        r.BadgeText = role;
                        r.Cover = false;
                        r.HiddenByDefault = false;
                        plr.ReferenceHub.serverRoles.SetGroup(r, false, true, true);
                }

            }
        }
    }
}
