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

        public override bool DoNotTrack => _player.DoNotTrack;

        public override string GroupName => _player.GroupName;

        public override string RankColor => _player.RankColor;

        public override string RankName => _player.RankName;

        public override string IPAddress => _player.IPAddress;

        public override bool IsGodModeEnabled => _player.IsGodModeEnabled;

        public override float Health => _player.Health;

        public override int MaxHealth => _player.MaxHealth;

        public override bool IsIntercomMuted => _player.IsIntercomMuted;

        public override bool IsMuted => _player.IsMuted;

        public override bool IsOverwatchEnabled => _player.IsOverwatchEnabled;

        public override bool RemoteAdminAccess => _player.RemoteAdminAccess;

        public override int PlayerID => _player.Id;

        public override Position Position
        {
            get
            {
                return new Position() { x = _player.Position.x, y = _player.Position.y, z = _player.Position.z };
            }
            set
            {
                Position = value;
            }
        }

        public override Rotation Rotation
        {
            get
            {
                return new Rotation() { x = _player.Rotation.x, y = _player.Rotation.y, z = _player.Rotation.z };
            }
            set
            {
                Rotation = value;
            }
        }

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

        public override void SendHint(string message, float duration)
        {
            _player.ShowHint(message, duration);
        }

        public override void SendPosition(bool state = false)
        {
            if (!_player.SessionVariables.ContainsKey("SP"))
                _player.SessionVariables.Add("SP", state);
            _player.SessionVariables["SP"] = state;
        }

        public override void SendRotation(bool state = false)
        {
            if (!_player.SessionVariables.ContainsKey("SR"))
                _player.SessionVariables.Add("SR", state);
            _player.SessionVariables["SR"] = state;
        }

        public override void Teleport(float x, float y, float z)
        {
            _player.Position = new UnityEngine.Vector3(x, y, z);
        }

        public override void SetGodmode(bool state = false)
        {
            _player.IsGodModeEnabled = state;
        }

        public override void SetNoclip(bool state = false)
        {
            _player.NoClipEnabled = state;
        }

        public override void ClearInventory()
        {
            _player.ClearInventory();
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
