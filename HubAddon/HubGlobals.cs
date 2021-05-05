using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using Mirror;
using NetworkedPlugins.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HubAddon
{
    public class HubGlobals
    {
        public static void InitLobby()
        {
            foreach (var pickup in UnityEngine.Object.FindObjectsOfType<Pickup>())
                NetworkServer.Destroy(pickup.gameObject);
            GameCore.RoundStart.LobbyLock = true;
            RoundSummary.RoundLock = true;
            CharacterClassManager.ForceRoundStart();
        }

        public static void SendClientToServer(Player hub, ushort port)
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

        private static int GetMethodHash(Type invokeClass, string methodName)
        {
            return invokeClass.FullName.GetStableHashCode() * 503 + methodName.GetStableHashCode();
        }
    }
}
