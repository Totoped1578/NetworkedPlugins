using LiteNetLib;
using NetworkedPlugins.API.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace NetworkedPlugins
{
    public class CustomConsoleExecutor : CommandSender
    {
		private NPClient client;
		private string Command;

		public CustomConsoleExecutor(NPClient client, string cmd)
        {
			this.client = client;
			this.Command = cmd;
        }

		public override string SenderId
		{
			get
			{
				return "GAME CONSOLE";
			}
		}

		public override string Nickname
		{
			get
			{
				return "GAME CONSOLE";
			}
		}

		public override ulong Permissions
		{
			get
			{
				return ServerStatic.GetPermissionsHandler().FullPerm;
			}
		}

		public override byte KickPower
		{
			get
			{
				return byte.MaxValue;
			}
		}

		public override bool FullPermissions
		{
			get
			{
				return true;
			}
		}

		public override void RaReply(string text, bool success, bool logToConsole, string overrideDisplay)
		{
			GameCore.Console.AddLog("[RA Reply] " + text, success ? Color.green : Color.red, false);
			this.client._netPacketProcessor.Send<ConsoleResponsePacket>(this.client.networkListener, new ConsoleResponsePacket()
			{
				Command = Command,
				isRemoteAdmin = true,
				Response = text
			}, DeliveryMethod.ReliableOrdered);
		}

		public override void Print(string text)
		{
			GameCore.Console.AddLog(text, Color.green, false);
			this.client._netPacketProcessor.Send<ConsoleResponsePacket>(this.client.networkListener, new ConsoleResponsePacket()
			{
				Command = Command,
				isRemoteAdmin = false,
				Response = text
			}, DeliveryMethod.ReliableOrdered);
		}
	}
}
