using Exiled.API.Enums;
using LiteNetLib.Utils;
using NetworkedPlugins.API;
using NetworkedPlugins.API.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalStats
{
    [NPAddonInfo(
       addonID = "Dp3pad3apWDwad",
       addonAuthor = "Killers0992",
       addonName = "GlobalStats",
       addonVersion = "1.0.0")]
    public class GlobalStatsClient : NPAddonClient
    {
        public override void OnMessageReceived(global::LiteNetLib.Utils.NetDataReader reader)
        {
        }

        public override void OnEnable()
        {
            Exiled.Events.Handlers.Player.Died += Player_Died;
            Exiled.Events.Handlers.Player.ThrowingGrenade += Player_ThrowingGrenade;
            Logger.Info("Addon started, register eventhandlers.");
        }

        private void Player_ThrowingGrenade(Exiled.Events.EventArgs.ThrowingGrenadeEventArgs ev)
        {
            NetDataWriter wr = new NetDataWriter();
            switch (ev.Type)
            {
                case GrenadeType.Flashbang:
                    wr.Put((byte)1);
                    wr.Put((byte)0);
                    wr.Put(ev.Player.UserId);
                    break;
                case GrenadeType.FragGrenade:
                    wr.Put((byte)1);
                    wr.Put((byte)1);
                    wr.Put(ev.Player.UserId);
                    break;
                case GrenadeType.Scp018:
                    wr.Put((byte)1);
                    wr.Put((byte)2);
                    wr.Put(ev.Player.UserId);
                    break;
            }
            SendData(wr);
        }

        private void Player_Died(Exiled.Events.EventArgs.DiedEventArgs ev)
        {
            NetDataWriter wr = new NetDataWriter();
            wr.Put((byte)0);
            wr.Put(ev.Target.UserId);
            wr.Put((sbyte)ev.Target.Role);
            wr.Put(ev.Target.DoNotTrack);
            wr.Put(ev.Killer.UserId);
            wr.Put((sbyte)ev.Killer.Role);
            wr.Put(ev.Killer.DoNotTrack);
            SendData(wr);
        }
    }
}
