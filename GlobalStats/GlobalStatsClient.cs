using Exiled.API.Enums;
using Exiled.API.Features;
using LiteNetLib.Utils;
using MEC;
using NetworkedPlugins.API;
using NetworkedPlugins.API.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GlobalStats
{
    [NPAddonInfo(
       addonID = "Dp3pad3apWDwad",
       addonAuthor = "Killers0992",
       addonName = "GlobalStats",
       addonVersion = "1.0.0")]
    public class GlobalStatsClient : NPAddonClient<AddonConfig>
    {
        public Dictionary<string, int> damageReceived { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> damageDeal { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> shootsFired { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> shootsFiredhead { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, Dictionary<RoleType, int>> timePlayed { get; set; } = new Dictionary<string, Dictionary<RoleType, int>>();

        public override void OnEnable()
        {
            Exiled.Events.Handlers.Player.Died += Player_Died;
            Exiled.Events.Handlers.Player.ThrowingGrenade += Player_ThrowingGrenade;
            Exiled.Events.Handlers.Server.RestartingRound += Server_RestartingRound;
            Exiled.Events.Handlers.Player.Hurting += Player_Hurting;
            Exiled.Events.Handlers.Player.Destroying += Player_Destroying;
            Exiled.Events.Handlers.Player.ActivatingWarheadPanel += Player_ActivatingWarheadPanel;
            Exiled.Events.Handlers.Warhead.Starting += Warhead_Starting;
            Exiled.Events.Handlers.Player.MedicalItemUsed += Player_MedicalItemUsed;
            Exiled.Events.Handlers.Server.RoundEnded += Server_RoundEnded;
            Exiled.Events.Handlers.Player.Shot += Player_Shot;
            Exiled.Events.Handlers.Player.EscapingPocketDimension += Player_EscapingPocketDimension;
            Exiled.Events.Handlers.Scp049.FinishingRecall += Scp049_FinishingRecall;
            Exiled.Events.Handlers.Scp096.Enraging += Scp096_Enraging;
            Exiled.Events.Handlers.Player.Escaping += Player_Escaping;
            Exiled.Events.Handlers.Server.RespawningTeam += Server_RespawningTeam;
            Logger.Info("Addon started, register eventhandlers.");
            Timing.RunCoroutine(TimeCollector());
        }

        private void Server_RespawningTeam(Exiled.Events.EventArgs.RespawningTeamEventArgs ev)
        {
            if (ev.NextKnownTeam == Respawning.SpawnableTeamType.ChaosInsurgency)
            {
                List<string> players = new List<string>();
                foreach (var plr in ev.Players)
                {
                    players.Add(plr.UserId);
                }
                var wr = new NetDataWriter();
                wr.Put((byte)13);
                wr.PutArray(players.ToArray());
                wr.Put(0);
                SendData(wr);

            }
            else if (ev.NextKnownTeam == Respawning.SpawnableTeamType.NineTailedFox)
            {
                List<string> players = new List<string>();
                foreach (var plr in ev.Players)
                {
                    players.Add(plr.UserId);
                }
                var wr = new NetDataWriter();
                wr.Put((byte)13);
                wr.PutArray(players.ToArray());
                wr.Put(1);
                SendData(wr);
            }
        }

        private void Player_Escaping(Exiled.Events.EventArgs.EscapingEventArgs ev)
        {
            if (ev.IsAllowed)
            {
                switch (ev.NewRole)
                {
                    case RoleType.ChaosInsurgency:
                        var wr = new NetDataWriter();
                        wr.Put((byte)6);
                        wr.Put(ev.Player.UserId);
                        wr.Put(8);
                        SendData(wr);
                        wr = new NetDataWriter();
                        wr.Put((byte)12);
                        wr.Put(ev.Player.UserId);
                        wr.Put(0);
                        wr.Put(Round.ElapsedTime.TotalSeconds);
                        SendData(wr);
                        break;
                    case RoleType.NtfScientist:
                        var wr2 = new NetDataWriter();
                        wr2.Put((byte)6);
                        wr2.Put(ev.Player.UserId);
                        wr2.Put(9);
                        SendData(wr2);
                        wr2 = new NetDataWriter();
                        wr2.Put((byte)12);
                        wr2.Put(ev.Player.UserId);
                        wr2.Put(1);
                        wr2.Put(Round.ElapsedTime.TotalSeconds);
                        SendData(wr2);
                        break;
                }
            }
        }

        private void Scp096_Enraging(Exiled.Events.EventArgs.EnragingEventArgs ev)
        {
            var wr = new NetDataWriter();
            wr.Put((byte)6);
            wr.Put(ev.Player.UserId);
            wr.Put(7);
            SendData(wr);
        }

        private void Scp049_FinishingRecall(Exiled.Events.EventArgs.FinishingRecallEventArgs ev)
        {
            var wr = new NetDataWriter();
            wr.Put((byte)6);
            wr.Put(ev.Scp049.UserId);
            wr.Put(6);
            SendData(wr);
        }

        private void Player_EscapingPocketDimension(Exiled.Events.EventArgs.EscapingPocketDimensionEventArgs ev)
        {
            if (ev.IsAllowed)
            {
                var wr = new NetDataWriter();
                wr.Put((byte)6);
                wr.Put(ev.Player.UserId);
                wr.Put(5);
                SendData(wr);
            }
        }

        private void Player_Shot(Exiled.Events.EventArgs.ShotEventArgs ev)
        {
            if (!shootsFired.ContainsKey(ev.Shooter.UserId))
                shootsFired.Add(ev.Shooter.UserId, 0);
            shootsFired[ev.Shooter.UserId]++;

            if (ev.HitboxTypeEnum == HitBoxType.HEAD)
            {
                if (!shootsFiredhead.ContainsKey(ev.Shooter.UserId))
                    shootsFiredhead.Add(ev.Shooter.UserId, 0);
                shootsFiredhead[ev.Shooter.UserId]++;
            }
        }

        private void Server_RoundEnded(Exiled.Events.EventArgs.RoundEndedEventArgs ev)
        {
            NetDataWriter wr2 = new NetDataWriter();
            wr2.Put((byte)8);
            List<string> players = new List<string>();
            foreach (var plr in Player.List) 
            {
                players.Add(plr.UserId);
            }
            wr2.PutArray(players.ToArray());
            SendData(wr2);
            wr2 = new NetDataWriter();
            wr2.Put((byte)9);
            players = new List<string>();
            foreach (var plr in Player.List)
            {
                switch (ev.LeadingTeam)
                {
                    case LeadingTeam.Anomalies:
                        if (plr.Team == Team.SCP)
                            players.Add(plr.UserId);
                        break;
                    case LeadingTeam.ChaosInsurgency:
                        if (plr.Team == Team.CHI || plr.Team == Team.CDP)
                            players.Add(plr.UserId);
                        break;
                    case LeadingTeam.Draw:
                    case LeadingTeam.FacilityForces:
                        if (plr.Team == Team.MTF || plr.Team == Team.RSC)
                            players.Add(plr.UserId);
                        break;
                }
            }
            wr2.PutArray(players.ToArray());
            SendData(wr2);
        }

        private void Player_MedicalItemUsed(Exiled.Events.EventArgs.UsedMedicalItemEventArgs ev)
        {
            NetDataWriter wr2 = new NetDataWriter();

            switch (ev.Item)
            {
                case ItemType.Adrenaline:
                    wr2.Put((byte)7);
                    wr2.Put(ev.Player.UserId);
                    wr2.Put(0);
                    SendData(wr2);
                    break;
                case ItemType.Medkit:
                    wr2.Put((byte)7);
                    wr2.Put(ev.Player.UserId);
                    wr2.Put(1);
                    SendData(wr2);
                    break;
                case ItemType.Painkillers:
                    wr2.Put((byte)7);
                    wr2.Put(ev.Player.UserId);
                    wr2.Put(2);
                    SendData(wr2);
                    break;
                case ItemType.SCP500:
                    wr2.Put((byte)7);
                    wr2.Put(ev.Player.UserId);
                    wr2.Put(3);
                    SendData(wr2);
                    break;
            }
        }

        private void Warhead_Starting(Exiled.Events.EventArgs.StartingEventArgs ev)
        {
            if (ev.Player != null)
            {
                NetDataWriter wr2 = new NetDataWriter();
                wr2.Put((byte)5);
                wr2.Put(ev.Player.UserId);
                SendData(wr2);
            }
        }

        private void Player_ActivatingWarheadPanel(Exiled.Events.EventArgs.ActivatingWarheadPanelEventArgs ev)
        {
            if (!ev.IsAllowed) return;
            NetDataWriter wr2 = new NetDataWriter();
            wr2.Put((byte)4);
            wr2.Put(ev.Player.UserId);
            SendData(wr2);
        }

        public GameObject lastSpeaker = null;
        public int speakingTime = 0;

        public IEnumerator<float> TimeCollector()
        {
            while (true)
            {
                yield return Timing.WaitForSeconds(1f);
                try
                {
                    foreach (var plr in Player.List)
                    {
                        if (!timePlayed.ContainsKey(plr.UserId))
                            timePlayed.Add(plr.UserId, new Dictionary<RoleType, int>());
                        if (!timePlayed[plr.UserId].ContainsKey(plr.Role))
                            timePlayed[plr.UserId].Add(plr.Role, 0);
                        timePlayed[plr.UserId][plr.Role]++;
                    }
                    if (Intercom.host.Networkspeaker != null)
                    {
                        lastSpeaker = Intercom.host.Networkspeaker;
                        speakingTime++;
                    }
                    else
                    {
                        if (lastSpeaker != null)
                        {
                            var hub = ReferenceHub.GetHub(lastSpeaker);
                            if (hub != null)
                            {
                                NetDataWriter wr2 = new NetDataWriter();
                                wr2.Put((byte)10);
                                wr2.Put(hub.characterClassManager.UserId);
                                wr2.Put(speakingTime);
                                SendData(wr2);
                            }
                            speakingTime = 0;
                            lastSpeaker = null;
                        }
                    }
                }
                catch (Exception)
                {

                }

            }
        }

        private void Player_Destroying(Exiled.Events.EventArgs.DestroyingEventArgs ev)
        {
            if (!damageReceived.ContainsKey(ev.Player.UserId))
                damageReceived.Add(ev.Player.UserId, 0);
            if (!damageDeal.ContainsKey(ev.Player.UserId))
                damageDeal.Add(ev.Player.UserId, 0);
            if (!shootsFired.ContainsKey(ev.Player.UserId))
                shootsFired.Add(ev.Player.UserId, 0);
            if (!shootsFiredhead.ContainsKey(ev.Player.UserId))
                shootsFiredhead.Add(ev.Player.UserId, 0);
            if (timePlayed.ContainsKey(ev.Player.UserId))
            {
                foreach(var role in timePlayed[ev.Player.UserId])
                {
                    NetDataWriter wr2 = new NetDataWriter();
                    wr2.Put((byte)3);
                    wr2.Put(ev.Player.UserId);
                    wr2.Put((int)role.Key);
                    wr2.Put(role.Value);
                    SendData(wr2);
                }
                timePlayed.Remove(ev.Player.UserId);

            }
            NetDataWriter wr = new NetDataWriter();
            wr.Put((byte)2);
            wr.Put(ev.Player.UserId);
            wr.Put(damageDeal[ev.Player.UserId]);
            wr.Put(damageReceived[ev.Player.UserId]);
            SendData(wr);
            wr = new NetDataWriter();
            wr.Put((byte)11);
            wr.Put(ev.Player.UserId);
            wr.Put(shootsFired[ev.Player.UserId]);
            wr.Put(shootsFiredhead[ev.Player.UserId]);
            SendData(wr);
        }


        private void Player_Hurting(Exiled.Events.EventArgs.HurtingEventArgs ev)
        {
            if (!damageReceived.ContainsKey(ev.Target.UserId))
                damageReceived.Add(ev.Target.UserId, 0);
            damageReceived[ev.Target.UserId] = damageReceived[ev.Target.UserId] + (int)ev.Amount;
            if (ev.Attacker == ev.Target)
                return;
            if (!damageDeal.ContainsKey(ev.Attacker.UserId))
                damageDeal.Add(ev.Attacker.UserId, 0);
            damageDeal[ev.Attacker.UserId] = damageDeal[ev.Attacker.UserId] + (int)ev.Amount;
        }

        private void Server_RestartingRound()
        {
            damageReceived = new Dictionary<string, int>();
            damageDeal = new Dictionary<string, int>();
            shootsFired = new Dictionary<string, int>();
            shootsFiredhead = new Dictionary<string, int>();
            timePlayed.Clear();
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
            if (ev.HitInformations.GetDamageType() == DamageTypes.Nuke)
            {
                wr = new NetDataWriter();
                wr.Put((byte)6);
                wr.Put(ev.Target.UserId);
                wr.Put(0);
                SendData(wr);
            }else if (ev.HitInformations.GetDamageType() == DamageTypes.Decont)
            {
                wr = new NetDataWriter();
                wr.Put((byte)6);
                wr.Put(ev.Target.UserId);
                wr.Put(1);
                SendData(wr);
            } else if (ev.HitInformations.GetDamageType() == DamageTypes.Falldown)
            {
                wr = new NetDataWriter();
                wr.Put((byte)6);
                wr.Put(ev.Target.UserId);
                wr.Put(2);
                SendData(wr);
            } else if (ev.HitInformations.GetDamageType() == DamageTypes.Pocket)
            {
                wr = new NetDataWriter();
                wr.Put((byte)6);
                wr.Put(ev.Target.UserId);
                wr.Put(3);
                SendData(wr);
            } else if (ev.HitInformations.GetDamageType() == DamageTypes.Scp939)
            {
                wr = new NetDataWriter();
                wr.Put((byte)6);
                wr.Put(ev.Target.UserId);
                wr.Put(4);
                SendData(wr);
            }
           
        }
    }
}
