using Exiled.API.Features;
using Mirror;
using NetworkedPlugins.API.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkedPlugins.API.Models
{
    public class NormalPlayer : PlayerFuncs
    {
        public NormalPlayer(Player p)
        {
            this._player = p;
        }
        private Player _player;

        public override string UserName => _player.Nickname;

        public override string UserID => _player.UserId;

        public override int Role => (int)_player.Role;

        public override void Disconnect(string reason)
        {
            ServerConsole.Disconnect(_player.GameObject, reason);
        }

        public override void Kill()
        {
            _player.Kill();
        }

        public override void Redirect(ushort port)
        {
            SendClientToServer(_player, port);
        }

        public override void SendConsoleMessage(string message, string color = "GREEN")
        {
            _player.SendConsoleMessage(message, color);
        }

        public override void SendRAMessage(string message)
        {
            _player.RemoteAdminMessage(message, true, "NP");
        }

        public override void SendReportMessage(string message)
        {
            _player.SendConsoleMessage("[REPORTING] " + message, "GREEN");
        }

        private void SendClientToServer(Player hub, ushort port)
        {
            var serverPS = hub.ReferenceHub.playerStats;
            NetworkWriter writer = NetworkWriterPool.GetWriter();
            writer.WriteSingle(1f);
            writer.WriteUInt16(port);
            RpcMessage msg = new RpcMessage
            {
                netId = serverPS.netId,
                componentIndex = serverPS.ComponentIndex,
                functionHash = GetMethodHash(typeof(PlayerStats), "RpcRoundrestartRedirect"),
                payload = writer.ToArraySegment()
            };
            hub.Connection.Send<RpcMessage>(msg, 0);
            NetworkWriterPool.Recycle(writer);
        }

        private int GetMethodHash(Type invokeClass, string methodName)
        {
            return invokeClass.FullName.GetStableHashCode() * 503 + methodName.GetStableHashCode();
        }
    }
}
