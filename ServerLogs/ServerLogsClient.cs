using Exiled.API.Extensions;
using Exiled.API.Features;
using LiteNetLib.Utils;
using NetworkedPlugins.API;
using NetworkedPlugins.API.Attributes;
using NetworkedPlugins.API.Models;
using Newtonsoft.Json;
using Scp914;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PlayerList;

namespace ServerLogs
{
    [NPAddonInfo(
        addonID = "ap3pAdp3wad",
        addonAuthor = "Killers0992",
        addonName = "ServerLogs",
        addonVersion = "1.0.0")]
    public class ServerLogsClient : NPAddonClient<ServerLogsConfig>
    {
        public override void OnEnable()
        {
            Exiled.Events.Handlers.Player.Verified += Player_Verified;
            Exiled.Events.Handlers.Player.Destroying += Player_Destroying;
            Exiled.Events.Handlers.Player.Died += Player_Died;
            Exiled.Events.Handlers.Player.TriggeringTesla += Player_TriggeringTesla;
            Exiled.Events.Handlers.Warhead.Detonated += Warhead_Detonated;
            Exiled.Events.Handlers.Map.GeneratorActivated += Map_GeneratorActivated;
            Exiled.Events.Handlers.Map.Decontaminating += Map_Decontaminating;
            Exiled.Events.Handlers.Warhead.Starting += Warhead_Starting;
            Exiled.Events.Handlers.Warhead.Stopping += Warhead_Stopping;
            Exiled.Events.Handlers.Scp914.UpgradingItems += Scp914_UpgradingItems;
            Exiled.Events.Handlers.Player.InsertingGeneratorTablet += Player_InsertingGeneratorTablet;
            Exiled.Events.Handlers.Player.OpeningGenerator += Player_OpeningGenerator;
            Exiled.Events.Handlers.Player.UnlockingGenerator += Player_UnlockingGenerator;
            Exiled.Events.Handlers.Scp106.Containing += Scp106_Containing;
            Exiled.Events.Handlers.Scp106.CreatingPortal += Scp106_CreatingPortal;
            Exiled.Events.Handlers.Player.ChangingItem += Player_ChangingItem;
            Exiled.Events.Handlers.Scp079.GainingExperience += Scp079_GainingExperience;
            Exiled.Events.Handlers.Scp079.GainingLevel += Scp079_GainingLevel;
            Exiled.Events.Handlers.Player.ReloadingWeapon += Player_ReloadingWeapon;
            Exiled.Events.Handlers.Player.ActivatingWarheadPanel += Player_ActivatingWarheadPanel;
            Exiled.Events.Handlers.Player.InteractingElevator += Player_InteractingElevator;
            Exiled.Events.Handlers.Player.InteractingLocker += Player_InteractingLocker;
            Exiled.Events.Handlers.Player.ClosingGenerator += Player_ClosingGenerator;
            Exiled.Events.Handlers.Player.EjectingGeneratorTablet += Player_EjectingGeneratorTablet;
            Exiled.Events.Handlers.Player.InteractingDoor += Player_InteractingDoor;
            Exiled.Events.Handlers.Scp914.Activating += Scp914_Activating;
            Exiled.Events.Handlers.Scp914.ChangingKnobSetting += Scp914_ChangingKnobSetting;
            Exiled.Events.Handlers.Player.EnteringPocketDimension += Player_EnteringPocketDimension;
            Exiled.Events.Handlers.Scp106.Teleporting += Scp106_Teleporting;
            Exiled.Events.Handlers.Scp079.InteractingTesla += Scp079_InteractingTesla;
            Exiled.Events.Handlers.Player.ThrowingGrenade += Player_ThrowingGrenade;
            Exiled.Events.Handlers.Player.MedicalItemUsed += Player_MedicalItemUsed;
            Exiled.Events.Handlers.Player.ChangingRole += Player_ChangingRole;
            Exiled.Events.Handlers.Player.RemovingHandcuffs += Player_RemovingHandcuffs;
            Exiled.Events.Handlers.Player.Handcuffing += Player_Handcuffing;
            Exiled.Events.Handlers.Player.Kicked += Player_Kicked;
            Exiled.Events.Handlers.Player.Banned += Player_Banned;
            Exiled.Events.Handlers.Player.IntercomSpeaking += Player_IntercomSpeaking;
            Exiled.Events.Handlers.Player.PickingUpItem += Player_PickingUpItem;
            Exiled.Events.Handlers.Player.ItemDropped += Player_ItemDropped;
            Exiled.Events.Handlers.Player.ChangingGroup += Player_ChangingGroup;
            Exiled.Events.Handlers.Server.SendingRemoteAdminCommand += Server_SendingRemoteAdminCommand;
            Exiled.Events.Handlers.Server.SendingConsoleCommand += Server_SendingConsoleCommand;
            Exiled.Events.Handlers.Server.ReportingCheater += Server_ReportingCheater;
            Exiled.Events.Handlers.Server.LocalReporting += Server_LocalReporting;
            Exiled.Events.Handlers.Server.WaitingForPlayers += Server_WaitingForPlayers;
            Exiled.Events.Handlers.Server.RoundStarted += Server_RoundStarted;
            Exiled.Events.Handlers.Server.RoundEnded += Server_RoundEnded;
            Exiled.Events.Handlers.Server.RespawningTeam += Server_RespawningTeam;
        }

        public DateTime timeRound = DateTime.Now;

        private void Player_Verified(Exiled.Events.EventArgs.VerifiedEventArgs ev)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put((byte)0);
            writer.Put(timeRound.Ticks);
            writer.Put(ev.Player.UserId);
            SendData(writer);
        }


        private void Player_Destroying(Exiled.Events.EventArgs.DestroyingEventArgs ev)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put((byte)1);
            writer.Put(timeRound.Ticks);
            writer.Put(ev.Player.UserId);
            SendData(writer);
        }


        private void Player_Died(Exiled.Events.EventArgs.DiedEventArgs ev)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put((byte)2);
            writer.Put(timeRound.Ticks);
            writer.Put(ev.Killer.UserId);
            writer.Put((int)ev.Killer.Role);
            writer.Put(ev.Target.UserId);
            writer.Put((int)ev.Target.Role); 
            writer.Put(DamageTypes.FromIndex(ev.HitInformations.Tool).name);
            SendData(writer);
        }

        private void Player_TriggeringTesla(Exiled.Events.EventArgs.TriggeringTeslaEventArgs ev)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put((byte)3);
            writer.Put(timeRound.Ticks);
            writer.Put(ev.Player.UserId);
            writer.Put((int)ev.Player.Role);
            SendData(writer);
        }

        private void Warhead_Detonated()
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put((byte)4);
            writer.Put(timeRound.Ticks);
            SendData(writer);
        }

        private void Map_GeneratorActivated(Exiled.Events.EventArgs.GeneratorActivatedEventArgs ev)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put((byte)5);
            writer.Put(timeRound.Ticks);
            writer.Put(ev.Generator.CurRoom);
            writer.Put(ev.Generator.totalVoltage + 1);
            SendData(writer);
        }



        private void Warhead_Starting(Exiled.Events.EventArgs.StartingEventArgs ev)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put((byte)6);
            writer.Put(timeRound.Ticks);
            writer.Put(ev.Player == null ? "" : ev.Player.UserId);
            writer.Put(ev.Player == null ? -1 : (int)ev.Player.Role);
            writer.Put(Warhead.DetonationTimer);
            SendData(writer);
        }

        private void Warhead_Stopping(Exiled.Events.EventArgs.StoppingEventArgs ev)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put((byte)7);
            writer.Put(timeRound.Ticks);
            writer.Put(ev.Player == null ? "" : ev.Player.UserId);
            writer.Put(ev.Player == null ? -1 : (int)ev.Player.Role);
            SendData(writer);
        }

        private void Scp914_UpgradingItems(Exiled.Events.EventArgs.UpgradingItemsEventArgs ev)
        {
            List<int> itemIds = new List<int>();
            List<string> playersIds = new List<string>();
            foreach (var item in ev.Items)
                itemIds.Add((int)item.itemId);
            foreach (var player in ev.Players)
                playersIds.Add(player.UserId);
            NetDataWriter writer = new NetDataWriter();
            writer.Put((byte)8);
            writer.Put(timeRound.Ticks);
            writer.PutArray(itemIds.ToArray());
            writer.PutArray(playersIds.ToArray());
            SendData(writer);
        }

        private void Player_InsertingGeneratorTablet(Exiled.Events.EventArgs.InsertingGeneratorTabletEventArgs ev)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put((byte)9);
            writer.Put(timeRound.Ticks);
            writer.Put(ev.Player == null ? "" : ev.Player.UserId);
            writer.Put(ev.Player == null ? -1 : (int)ev.Player.Role);
            SendData(writer);
        }


        private void Player_OpeningGenerator(Exiled.Events.EventArgs.OpeningGeneratorEventArgs ev)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put((byte)10);
            writer.Put(timeRound.Ticks);
            writer.Put(ev.Player == null ? "" : ev.Player.UserId);
            writer.Put(ev.Player == null ? -1 : (int)ev.Player.Role);
            SendData(writer);
        }


        private void Player_UnlockingGenerator(Exiled.Events.EventArgs.UnlockingGeneratorEventArgs ev)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put((byte)11);
            writer.Put(timeRound.Ticks);
            writer.Put(ev.Player == null ? "" : ev.Player.UserId);
            writer.Put(ev.Player == null ? -1 : (int)ev.Player.Role);
            SendData(writer);
        }

        private void Scp106_Containing(Exiled.Events.EventArgs.ContainingEventArgs ev)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put((byte)12);
            writer.Put(timeRound.Ticks);
            writer.Put(ev.Player == null ? "" : ev.Player.UserId);
            writer.Put(ev.Player == null ? -1 : (int)ev.Player.Role);
            SendData(writer);
        }


        private void Scp106_CreatingPortal(Exiled.Events.EventArgs.CreatingPortalEventArgs ev)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put((byte)13);
            writer.Put(timeRound.Ticks);
            writer.Put(ev.Player == null ? "" : ev.Player.UserId);
            writer.Put(ev.Player == null ? -1 : (int)ev.Player.Role);
            SendData(writer);
        }


        private void Player_ChangingItem(Exiled.Events.EventArgs.ChangingItemEventArgs ev)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put((byte)14);
            writer.Put(timeRound.Ticks);
            writer.Put(ev.Player == null ? "" : ev.Player.UserId);
            writer.Put(ev.Player == null ? -1 : (int)ev.Player.Role);
            writer.Put((int)ev.OldItem.id);
            writer.Put((int)ev.NewItem.id);
            SendData(writer);
        }


        private void Scp079_GainingExperience(Exiled.Events.EventArgs.GainingExperienceEventArgs ev)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put((byte)15);
            writer.Put(timeRound.Ticks);
            writer.Put(ev.Player == null ? "" : ev.Player.UserId);
            writer.Put(ev.Player == null ? -1 : (int)ev.Player.Role);
            writer.Put(ev.Amount);
            writer.Put((int)ev.GainType);
            SendData(writer);
        }

        private void Scp079_GainingLevel(Exiled.Events.EventArgs.GainingLevelEventArgs ev)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put((byte)16);
            writer.Put(timeRound.Ticks);
            writer.Put(ev.Player == null ? "" : ev.Player.UserId);
            writer.Put(ev.Player == null ? -1 : (int)ev.Player.Role);
            writer.Put(ev.OldLevel);
            writer.Put(ev.NewLevel);
            SendData(writer);
        }

        private void Player_ReloadingWeapon(Exiled.Events.EventArgs.ReloadingWeaponEventArgs ev)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put((byte)17);
            writer.Put(timeRound.Ticks);
            writer.Put(ev.Player == null ? "" : ev.Player.UserId);
            writer.Put(ev.Player == null ? -1 : (int)ev.Player.Role);
            writer.Put((int)ev.Player.CurrentItem.id);
            SendData(writer);
        }

        private void Player_ActivatingWarheadPanel(Exiled.Events.EventArgs.ActivatingWarheadPanelEventArgs ev)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put((byte)18);
            writer.Put(timeRound.Ticks);
            writer.Put(ev.Player == null ? "" : ev.Player.UserId);
            writer.Put(ev.Player == null ? -1 : (int)ev.Player.Role);
            SendData(writer);
        }

        private void Player_InteractingElevator(Exiled.Events.EventArgs.InteractingElevatorEventArgs ev)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put((byte)19);
            writer.Put(timeRound.Ticks);
            writer.Put(ev.Player == null ? "" : ev.Player.UserId);
            writer.Put(ev.Player == null ? -1 : (int)ev.Player.Role);
            SendData(writer);
        }

        private void Player_InteractingLocker(Exiled.Events.EventArgs.InteractingLockerEventArgs ev)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put((byte)20);
            writer.Put(timeRound.Ticks);
            writer.Put(ev.Player == null ? "" : ev.Player.UserId);
            writer.Put(ev.Player == null ? -1 : (int)ev.Player.Role);
            SendData(writer);
        }

        private void Player_ClosingGenerator(Exiled.Events.EventArgs.ClosingGeneratorEventArgs ev)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put((byte)22);
            writer.Put(timeRound.Ticks);
            writer.Put(ev.Player == null ? "" : ev.Player.UserId);
            writer.Put(ev.Player == null ? -1 : (int)ev.Player.Role);
            SendData(writer);
        }

        private void Player_EjectingGeneratorTablet(Exiled.Events.EventArgs.EjectingGeneratorTabletEventArgs ev)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put((byte)23);
            writer.Put(timeRound.Ticks);
            writer.Put(ev.Player == null ? "" : ev.Player.UserId);
            writer.Put(ev.Player == null ? -1 : (int)ev.Player.Role);
            SendData(writer);
        }

        private void Player_InteractingDoor(Exiled.Events.EventArgs.InteractingDoorEventArgs ev)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put((byte)24);
            writer.Put(timeRound.Ticks);
            writer.Put(ev.Player == null ? "" : ev.Player.UserId);
            writer.Put(ev.Player == null ? -1 : (int)ev.Player.Role);
            writer.Put(ev.Door.NetworkTargetState);
            writer.Put(ev.Door.GetNametag());
            SendData(writer);
        }

        private void Scp914_Activating(Exiled.Events.EventArgs.ActivatingEventArgs ev)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put((byte)25);
            writer.Put(timeRound.Ticks);
            writer.Put(ev.Player == null ? "" : ev.Player.UserId);
            writer.Put(ev.Player == null ? -1 : (int)ev.Player.Role);
            writer.Put((int)Scp914Machine.singleton.knobState);
            SendData(writer);
        }

        private void Scp914_ChangingKnobSetting(Exiled.Events.EventArgs.ChangingKnobSettingEventArgs ev)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put((byte)26);
            writer.Put(timeRound.Ticks);
            writer.Put(ev.Player == null ? "" : ev.Player.UserId);
            writer.Put(ev.Player == null ? -1 : (int)ev.Player.Role);
            writer.Put((int)ev.KnobSetting);
            SendData(writer);
        }

        private void Player_EnteringPocketDimension(Exiled.Events.EventArgs.EnteringPocketDimensionEventArgs ev)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put((byte)27);
            writer.Put(timeRound.Ticks);
            writer.Put(ev.Player == null ? "" : ev.Player.UserId);
            writer.Put(ev.Player == null ? -1 : (int)ev.Player.Role);
            SendData(writer);
        }

        private void Scp106_Teleporting(Exiled.Events.EventArgs.TeleportingEventArgs ev)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put((byte)28);
            writer.Put(timeRound.Ticks);
            writer.Put(ev.Player == null ? "" : ev.Player.UserId);
            writer.Put(ev.Player == null ? -1 : (int)ev.Player.Role);
            SendData(writer);
        }

        private void Scp079_InteractingTesla(Exiled.Events.EventArgs.InteractingTeslaEventArgs ev)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put((byte)29);
            writer.Put(timeRound.Ticks);
            writer.Put(ev.Player == null ? "" : ev.Player.UserId);
            writer.Put(ev.Player == null ? -1 : (int)ev.Player.Role);
            SendData(writer);
        }

        private void Player_ThrowingGrenade(Exiled.Events.EventArgs.ThrowingGrenadeEventArgs ev)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put((byte)30);
            writer.Put(timeRound.Ticks);
            writer.Put(ev.Player == null ? "" : ev.Player.UserId);
            writer.Put(ev.Player == null ? -1 : (int)ev.Player.Role);
            writer.Put((int)ev.Type);
            SendData(writer);
        }

        private void Player_MedicalItemUsed(Exiled.Events.EventArgs.UsedMedicalItemEventArgs ev)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put((byte)31);
            writer.Put(timeRound.Ticks);
            writer.Put(ev.Player == null ? "" : ev.Player.UserId);
            writer.Put(ev.Player == null ? -1 : (int)ev.Player.Role);
            writer.Put((int)ev.Item);
            SendData(writer);
        }

        private void Player_ChangingRole(Exiled.Events.EventArgs.ChangingRoleEventArgs ev)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put((byte)32);
            writer.Put(timeRound.Ticks);
            writer.Put(ev.Player == null ? "" : ev.Player.UserId);
            writer.Put(ev.Player == null ? -1 : (int)ev.Player.Role);
            writer.Put((int)ev.NewRole);
            SendData(writer);
        }

        private void Player_RemovingHandcuffs(Exiled.Events.EventArgs.RemovingHandcuffsEventArgs ev)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put((byte)33);
            writer.Put(timeRound.Ticks);
            writer.Put(ev.Cuffer == null ? "" : ev.Cuffer.UserId);
            writer.Put(ev.Cuffer == null ? -1 : (int)ev.Cuffer.Role);
            writer.Put(ev.Target == null ? "" : ev.Target.UserId);
            writer.Put(ev.Target == null ? -1 : (int)ev.Target.Role);
            SendData(writer);
        }

        private void Player_Handcuffing(Exiled.Events.EventArgs.HandcuffingEventArgs ev)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put((byte)34);
            writer.Put(timeRound.Ticks);
            writer.Put(ev.Cuffer == null ? "" : ev.Cuffer.UserId);
            writer.Put(ev.Cuffer == null ? -1 : (int)ev.Cuffer.Role);
            writer.Put(ev.Target == null ? "" : ev.Target.UserId);
            writer.Put(ev.Target == null ? -1 : (int)ev.Target.Role);
            SendData(writer);
        }

        private void Player_Kicked(Exiled.Events.EventArgs.KickedEventArgs ev)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put((byte)35);
            writer.Put(timeRound.Ticks);
            writer.Put(ev.Target == null ? "" : ev.Target.UserId);
            writer.Put(ev.Target == null ? -1 : (int)ev.Target.Role);
            writer.Put(ev.Reason);
            SendData(writer);
        }

        private void Player_Banned(Exiled.Events.EventArgs.BannedEventArgs ev)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put((byte)36);
            writer.Put(timeRound.Ticks);
            writer.Put(ev.Details.Id);
            writer.Put(ev.Details.Issuer);
            writer.Put(ev.Details.Reason);
            writer.Put(ev.Details.Expires);
            SendData(writer);
        }

        private void Player_IntercomSpeaking(Exiled.Events.EventArgs.IntercomSpeakingEventArgs ev)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put((byte)37);
            writer.Put(timeRound.Ticks);
            writer.Put(ev.Player == null ? "" : ev.Player.UserId);
            writer.Put(ev.Player == null ? -1 : (int)ev.Player.Role);
            SendData(writer);
        }

        private void Player_PickingUpItem(Exiled.Events.EventArgs.PickingUpItemEventArgs ev)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put((byte)38);
            writer.Put(timeRound.Ticks);
            writer.Put(ev.Player == null ? "" : ev.Player.UserId);
            writer.Put(ev.Player == null ? -1 : (int)ev.Player.Role);
            writer.Put((int)ev.Pickup.ItemId);
            SendData(writer);
        }

        private void Player_ItemDropped(Exiled.Events.EventArgs.ItemDroppedEventArgs ev)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put((byte)39);
            writer.Put(timeRound.Ticks);
            writer.Put(ev.Player == null ? "" : ev.Player.UserId);
            writer.Put(ev.Player == null ? -1 : (int)ev.Player.Role);
            writer.Put((int)ev.Pickup.ItemId);
            SendData(writer);
        }

        private void Player_ChangingGroup(Exiled.Events.EventArgs.ChangingGroupEventArgs ev)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put((byte)40);
            writer.Put(timeRound.Ticks);
            writer.Put(ev.Player == null ? "" : ev.Player.UserId);
            writer.Put(ev.Player == null ? -1 : (int)ev.Player.Role);
            writer.Put(ev.NewGroup.BadgeText);
            writer.Put(ev.NewGroup.BadgeColor);
            SendData(writer);
        }

        private void Server_SendingRemoteAdminCommand(Exiled.Events.EventArgs.SendingRemoteAdminCommandEventArgs ev)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put((byte)41);
            writer.Put(timeRound.Ticks);
            writer.Put(ev.CommandSender.SenderId ?? "");
            writer.Put((int)ev.Sender.Role);
            writer.Put(ev.Name);
            writer.PutArray(ev.Arguments.ToArray());
            SendData(writer);
        }

        private void Server_SendingConsoleCommand(Exiled.Events.EventArgs.SendingConsoleCommandEventArgs ev)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put((byte)42);
            writer.Put(timeRound.Ticks);
            writer.Put(ev.Player.UserId ?? "");
            writer.Put((int)ev.Player.Role);
            writer.Put(ev.Name);
            writer.PutArray(ev.Arguments.ToArray());
            SendData(writer);
        }

        private void Server_ReportingCheater(Exiled.Events.EventArgs.ReportingCheaterEventArgs ev)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put((byte)43);
            writer.Put(timeRound.Ticks);
            writer.Put(ev.Reporter.UserId);
            writer.Put((int)ev.Reporter.Role);
            writer.Put(ev.Reported.UserId);
            writer.Put((int)ev.Reported.Role);
            writer.Put(ev.Reason);
            SendData(writer);
        }

        private void Server_LocalReporting(Exiled.Events.EventArgs.LocalReportingEventArgs ev)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put((byte)44);
            writer.Put(timeRound.Ticks);
            writer.Put(ev.Issuer.UserId);
            writer.Put((int)ev.Issuer.Role);
            writer.Put(ev.Target.UserId);
            writer.Put((int)ev.Target.Role);
            writer.Put(ev.Reason);
            SendData(writer);
        }

        private void Server_WaitingForPlayers()
        {
            timeRound = DateTime.Now;
            NetDataWriter writer = new NetDataWriter();
            writer.Put((byte)45);
            writer.Put(timeRound.Ticks);
            SendData(writer);
        }

        private void Server_RoundStarted()
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put((byte)46);
            writer.Put(timeRound.Ticks);
            SendData(writer);
        }

        private void Server_RoundEnded(Exiled.Events.EventArgs.RoundEndedEventArgs ev)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put((byte)47);
            writer.Put(timeRound.Ticks);
            writer.Put((int)ev.LeadingTeam);
            writer.Put(Player.Dictionary.Count);
            SendData(writer);
        }

        private void Server_RespawningTeam(Exiled.Events.EventArgs.RespawningTeamEventArgs ev)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put((byte)48);
            writer.Put(timeRound.Ticks);
            writer.Put((int)ev.NextKnownTeam);
            writer.Put(ev.Players.Count);
            SendData(writer);
            
        }

        private void Map_Decontaminating(Exiled.Events.EventArgs.DecontaminatingEventArgs ev)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put((byte)21);
            writer.Put(timeRound.Ticks);
            SendData(writer);
        }

    }
}
