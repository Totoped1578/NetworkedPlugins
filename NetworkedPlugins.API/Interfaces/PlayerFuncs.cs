using NetworkedPlugins.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkedPlugins.API.Interfaces
{
    public abstract class PlayerFuncs
    {
        public virtual string UserName { get; set; }
        public virtual string UserID { get; set; }
        public virtual int Role { get; set; }
        public virtual bool DoNotTrack { get; set; }
        public virtual bool RemoteAdminAccess { get; set; }
        public virtual bool IsOverwatchEnabled { get; set; }
        public virtual string IPAddress { get; set; }
        public virtual bool IsMuted { get; set; }
        public virtual bool IsIntercomMuted { get; set; }
        public virtual bool IsGodModeEnabled { get; set; }
        public virtual float Health { get; set; }
        public virtual int MaxHealth { get; set; }
        public virtual string GroupName { get; set; }
        public virtual string RankColor { get; set; }
        public virtual string RankName { get; set; }
        public virtual int PlayerID { get; set; }
        public virtual Position Position { get; set; }
        public virtual Rotation Rotation { get; set; }
        public abstract void Kill();
        public abstract void SendReportMessage(string message);
        public abstract void SendRAMessage(string message);
        public abstract void SendConsoleMessage(string message, string color = "GREEN");
        public abstract void Redirect(ushort port);
        public abstract void Disconnect(string reason);
        public abstract void SendHint(string message, float duration);
        public abstract void SendPosition(bool state = false);
        public abstract void SendRotation(bool state = false);
        public abstract void Teleport(float x, float y, float z);
        public abstract void SetGodmode(bool state = false);
        public abstract void SetNoclip(bool state = false);
        public abstract void ClearInventory();
    }
}
