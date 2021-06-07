using LiteNetLib.Utils;
using MySqlConnector;
using NetworkedPlugins.API;
using NetworkedPlugins.API.Attributes;
using NetworkedPlugins.API.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GlobalStats
{
    [NPAddonInfo(
      addonID = "Dp3pad3apWDwad",
      addonAuthor = "Killers0992",
      addonName = "GlobalStats",
      addonVersion = "1.0.0")]
    public class GlobalStatsDedicated : NPAddonDedicated<AddonConfig>
    {

        public string key = "";
        public override void OnEnable()
        {
            base.OnEnable();
            if (!File.Exists("connectionkey.json"))
                File.WriteAllText("connectionkey.json", "bad");
            key = File.ReadAllText("connectionkey.json");
            Logger.Info("Addon started.");
        }

        public override void OnConsoleCommand(string cmd, List<string> arguments)
        {
            switch (cmd.ToUpper())
            {
                case "STATUS":
                    string op = "";
                    foreach (var serv in GetServers())
                    {
                        op += Environment.NewLine + " Players: " + serv.Players.Count + "/" + serv.MaxPlayers + "";
                        foreach (var plr in serv.Players)
                        {
                            plr.Value.SendConsoleMessage("Status 0");
                            op += Environment.NewLine + " - " + plr.Value.UserName + ", Role: " + plr.Value.Role;
                        }
                    }
                    Logger.Info(op);
                    break;
            }

        }

        public override void OnMessageReceived(NPServer server, NetDataReader reader)
        {
            byte p = reader.GetByte();
            switch (p)
            {
                case 0:
                    string targetUserid = reader.GetString();
                    RoleTypeR targetRole = (RoleTypeR)reader.GetSByte();
                    bool targetDnt = reader.GetBool();
                    string killerUserid = reader.GetString();
                    RoleTypeR killerRole = (RoleTypeR)reader.GetSByte();
                    bool killerDnt = reader.GetBool();
                    if (!targetDnt)
                    {
                        Task.Factory.StartNew(async () =>
                        {
                            await Check(key, false, targetUserid);

                            await UpdateStats(key, targetUserid, targetRole, true);
                        });
                    }
                    if (!killerDnt)
                    {
                        if (targetUserid == killerUserid)
                            return;
                        Task.Factory.StartNew(async () =>
                        {
                            await Check(key, false, killerUserid);

                            await UpdateStats(key, killerUserid, killerRole, false);
                            await UpdateStats(key, killerUserid, targetRole, false, true);
                        });
                    }
                    break;
                case 1:
                    byte grenadeType = reader.GetByte();
                    string userid = reader.GetString();
                    Task.Factory.StartNew(async () =>
                    {
                        await Check(key, false, userid);

                        await UpdateStatsGrenade(key, userid, grenadeType == 0 ? "flash_thrown" : grenadeType == 1 ? "grenade_thrown" : "018_thrown");
                    });
                    break;
                case 2:
                    string userid2 = reader.GetString();
                    int dmgdeal = reader.GetInt();
                    int dmgreceived = reader.GetInt();
                    Task.Factory.StartNew(async () =>
                    {
                        await Check(key, false, userid2);
                        await UpdateDamage(key, userid2, dmgdeal, dmgreceived);
                    });
                    break;
                case 3:
                    string userid3 = reader.GetString();
                    RoleTypeR role = (RoleTypeR)reader.GetInt();
                    int time = reader.GetInt();
                    Task.Factory.StartNew(async () =>
                    {
                        await Check(key, false, userid3);
                        await UpdateTime(key, userid3, role, time);
                    });
                    break;
                case 4:
                    string userid4 = reader.GetString();
                    Task.Factory.StartNew(async () =>
                    {
                        await Check(key, false, userid4);
                        await UpdateWarhead(key, userid4, 1);
                    });
                    break;
                case 5:
                    string userid5 = reader.GetString();
                    Task.Factory.StartNew(async () =>
                    {
                        await Check(key, false, userid5);
                        await UpdateWarhead(key, userid5, 0);
                    });
                    break;
                case 6:
                    string userid6 = reader.GetString();
                    int type = reader.GetInt();
                    Task.Factory.StartNew(async () =>
                    {
                        await Check(key, false, userid6);
                        await UpdateDied(key, userid6, type);
                    });
                    break;
                case 7:
                    string userid7 = reader.GetString();
                    int type2 = reader.GetInt();
                    Task.Factory.StartNew(async () =>
                    {
                        await Check(key, false, userid7);
                        await UpdateMedical(key, userid7, type2);
                    });
                    break;
                case 8:
                    var useris = reader.GetStringArray();
                    foreach(var user in useris)
                    {
                        Task.Factory.StartNew(async () =>
                        {
                            await Check(key, false, user);
                            await UpdateRoundsPlayed(key, user);
                        });
                    }
                    break;
                case 9:
                    var useris2 = reader.GetStringArray();
                    foreach (var user in useris2)
                    {
                        Task.Factory.StartNew(async () =>
                        {
                            await Check(key, false, user);
                            await UpdateRoundsWon(key, user);
                        });
                    }
                    break;
                case 10:
                    var userid8 = reader.GetString();
                    int time2 = reader.GetInt();
                    Task.Factory.StartNew(async () =>
                    {
                        await Check(key, false, userid8);
                        await UpdateIntercomTime(key, userid8, time2);
                    });
                    break;
                case 11:
                    var userid9 = reader.GetString();
                    int shoots = reader.GetInt();
                    int shootshead = reader.GetInt();
                    Task.Factory.StartNew(async () =>
                    {
                        await Check(key, false, userid9);
                        await UpdateShoots(key, userid9, shoots, shootshead);
                    });
                    break;
                case 12:
                    var userid10 = reader.GetString();
                    int type22 = reader.GetInt();
                    int time3 = reader.GetInt();
                    Task.Factory.StartNew(async () =>
                    {
                        await Check(key, false, userid10);
                        await UpdateEscapeTime(key, userid10, type22, time3);
                    });
                    break;
                case 13:
                    var useris3 = reader.GetStringArray();
                    int type4 = reader.GetInt();
                    foreach (var user in useris3)
                    {
                        Task.Factory.StartNew(async () =>
                        {
                            await Check(key, false, user);
                            await UpdateRespawnTeam(key, user, type4);
                        });
                    }
                    break;
            }
        }

        public async Task Check(string con, bool isdnt, string UserID)
        {
            try
            {
                using (var dbcon = new MySqlConnection(con))
                {
                    await dbcon.OpenAsync();
                    int c = 0;
                    using (var cmd = new MySqlCommand("SELECT userid FROM `stats` WHERE userid = @a", dbcon))
                    {
                        cmd.Parameters.AddWithValue("@a", UserID);
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                c++;
                            }
                        }
                    }
                    if (c == 0)
                    {
                        if (!isdnt)
                        {
                            using (var cmd = new MySqlCommand("INSERT IGNORE INTO `stats` (userid) VALUES (@a)", dbcon))
                            {
                                cmd.Parameters.AddWithValue("@a", UserID);
                                await cmd.ExecuteNonQueryAsync();
                            }
                        }
                    }
                    else
                    {
                        if (isdnt)
                        {
                            using (var cmd = new MySqlCommand("DELETE FROM `stats` WHERE userid = @a", dbcon))
                            {
                                cmd.Parameters.AddWithValue("@a", UserID);
                                await cmd.ExecuteNonQueryAsync();
                            }
                        }
                    }
                    await dbcon.CloseAsync();
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
        }
        public async Task UpdateStatsGrenade(string con, string UserID, string fieldName)
        {

            if (string.IsNullOrEmpty(fieldName))
                return;
            using (var dbcon = new MySqlConnection(con))
            {
                await dbcon.OpenAsync();
                using (var cmd = new MySqlCommand("UPDATE `stats` SET " + fieldName + " = " + fieldName + " + 1 WHERE userid = @a", dbcon))
                {
                    cmd.Parameters.AddWithValue("@a", UserID);
                    await cmd.ExecuteNonQueryAsync();
                }
                await dbcon.CloseAsync();
            }

            Logger.Info("Register " + fieldName + " for " + UserID);
        }

        public async Task UpdateDied(string con, string UserID, int type)
        {
            string fieldName = "";
            switch (type)
            {
                case 0:
                    fieldName = "died_by_nuke";
                    break;
                case 1:
                    fieldName = "died_by_decon";
                    break;
                case 2:
                    fieldName = "died_by_gravity";
                    break;
                case 3:
                    fieldName = "died_by_pdim";
                    break;
                case 4:
                    fieldName = "scp939_bites";
                    break;
                case 5:
                    fieldName = "escapes_from_pocket";
                    break;
                case 6:
                    fieldName = "scp049_zombies";
                    break;
                case 7:
                    fieldName = "scp096_triggered";
                    break;
                case 8:
                    fieldName = "escaped_as_classd";
                    break;
                case 9:
                    fieldName = "escaped_as_scientist";
                    break;
            }

            if (string.IsNullOrEmpty(fieldName))
                return;
            using (var dbcon = new MySqlConnection(con))
            {
                await dbcon.OpenAsync();
                using (var cmd = new MySqlCommand("UPDATE `stats` SET " + fieldName + " = " + fieldName + " + 1 WHERE userid = @a", dbcon))
                {
                    cmd.Parameters.AddWithValue("@a", UserID);
                    await cmd.ExecuteNonQueryAsync();
                }
                await dbcon.CloseAsync();
            }

            Logger.Info("Register " + fieldName + " for " + UserID);
        }

        public async Task UpdateRespawnTeam(string con, string UserID, int type)
        {
            string fieldName = "";
            switch (type)
            {
                case 0:
                    fieldName = "respawned_as_chaos";
                    break;
                case 1:
                    fieldName = "respawned_as_ntf";
                    break;
            }

            if (string.IsNullOrEmpty(fieldName))
                return;
            using (var dbcon = new MySqlConnection(con))
            {
                await dbcon.OpenAsync();
                using (var cmd = new MySqlCommand("UPDATE `stats` SET " + fieldName + " = " + fieldName + " + 1 WHERE userid = @a", dbcon))
                {
                    cmd.Parameters.AddWithValue("@a", UserID);
                    await cmd.ExecuteNonQueryAsync();
                }
                await dbcon.CloseAsync();
            }

            Logger.Info("Register " + fieldName + " for " + UserID);
        }

        public async Task UpdateRoundsPlayed(string con, string UserID)
        {
            using (var dbcon = new MySqlConnection(con))
            {
                await dbcon.OpenAsync();
                using (var cmd = new MySqlCommand("UPDATE `stats` SET rounds_played = rounds_played + 1 WHERE userid = @a", dbcon))
                {
                    cmd.Parameters.AddWithValue("@a", UserID);
                    await cmd.ExecuteNonQueryAsync();
                }
                await dbcon.CloseAsync();
            }

            Logger.Info("Register rounds_played for " + UserID);
        }

        public async Task UpdateIntercomTime(string con, string UserID, int time)
        {
            using (var dbcon = new MySqlConnection(con))
            {
                await dbcon.OpenAsync();
                using (var cmd = new MySqlCommand("UPDATE `stats` SET intercom_time = intercom_time + @b WHERE userid = @a", dbcon))
                {
                    cmd.Parameters.AddWithValue("@a", UserID);
                    cmd.Parameters.AddWithValue("@b", time);
                    await cmd.ExecuteNonQueryAsync();
                }
                await dbcon.CloseAsync();
            }

            Logger.Info("Register intercom_time for " + UserID);
        }

        public async Task UpdateEscapeTime(string con, string UserID, int type, int time)
        {
            string fieldName = "";
            switch (type)
            {
                case 0:
                    fieldName = "fastest_escape_as_classd";
                    break;
                case 1:
                    fieldName = "fastest_escape_as_scientist";
                    break;
            }

            if (string.IsNullOrEmpty(fieldName))
                return;
            using (var dbcon = new MySqlConnection(con))
            {
                await dbcon.OpenAsync();
                using (var cmd = new MySqlCommand("UPDATE `stats` SET " + fieldName + " = " + fieldName + " + @b WHERE userid = @a AND " + fieldName + " < @b", dbcon))
                {
                    cmd.Parameters.AddWithValue("@a", UserID);
                    cmd.Parameters.AddWithValue("@b", time);
                    await cmd.ExecuteNonQueryAsync();
                }
                await dbcon.CloseAsync();
            }

            Logger.Info("Register " + fieldName + " for " + UserID);
        }

        public async Task UpdateShoots(string con, string UserID, int shoots, int shootshead)
        {
            using (var dbcon = new MySqlConnection(con))
            {
                await dbcon.OpenAsync();
                using (var cmd = new MySqlCommand("UPDATE `stats` SET shots_fired = shots_fired + @b WHERE userid = @a", dbcon))
                {
                    cmd.Parameters.AddWithValue("@a", UserID);
                    cmd.Parameters.AddWithValue("@b", shoots);
                    await cmd.ExecuteNonQueryAsync();
                }
                using (var cmd = new MySqlCommand("UPDATE `stats` SET head_shots = head_shots + @b WHERE userid = @a", dbcon))
                {
                    cmd.Parameters.AddWithValue("@a", UserID);
                    cmd.Parameters.AddWithValue("@b", shootshead);
                    await cmd.ExecuteNonQueryAsync();
                }
                await dbcon.CloseAsync();
            }

            Logger.Info("Register intercom_time for " + UserID);
        }

        public async Task UpdateRoundsWon(string con, string UserID)
        {
            using (var dbcon = new MySqlConnection(con))
            {
                await dbcon.OpenAsync();
                using (var cmd = new MySqlCommand("UPDATE `stats` SET rounds_won = rounds_won + 1 WHERE userid = @a", dbcon))
                {
                    cmd.Parameters.AddWithValue("@a", UserID);
                    await cmd.ExecuteNonQueryAsync();
                }
                await dbcon.CloseAsync();
            }

            Logger.Info("Register rounds_won for " + UserID);
        }

        public async Task UpdateMedical(string con, string UserID, int type)
        {
            string fieldName = "";
            switch (type)
            {
                case 0:
                    fieldName = "adrenaline_used";
                    break;
                case 1:
                    fieldName = "medkit_used";
                    break;
                case 2:
                    fieldName = "painkillers_used";
                    break;
                case 3:
                    fieldName = "scp500_used";
                    break;
            }

            if (string.IsNullOrEmpty(fieldName))
                return;
            using (var dbcon = new MySqlConnection(con))
            {
                await dbcon.OpenAsync();
                using (var cmd = new MySqlCommand("UPDATE `stats` SET " + fieldName + " = " + fieldName + " + 1 WHERE userid = @a", dbcon))
                {
                    cmd.Parameters.AddWithValue("@a", UserID);
                    await cmd.ExecuteNonQueryAsync();
                }
                await dbcon.CloseAsync();
            }

            Logger.Info("Register " + fieldName + " for " + UserID);
        }

        public async Task UpdateWarhead(string con, string UserID, int type)
        {
            string fieldName = "";
            switch (type)
            {
                case 0:
                    fieldName = "started_nuke";
                    break;
                case 1:
                    fieldName = "armed_nuke";
                    break;
            }
            if (string.IsNullOrEmpty(fieldName))
                return;
            using (var dbcon = new MySqlConnection(con))
            {
                await dbcon.OpenAsync();
                using (var cmd = new MySqlCommand("UPDATE `stats` SET " + fieldName + " = " + fieldName + " + 1 WHERE userid = @a", dbcon))
                {
                    cmd.Parameters.AddWithValue("@a", UserID);
                    await cmd.ExecuteNonQueryAsync();
                }
                await dbcon.CloseAsync();
            }

            Logger.Info("Register " + fieldName + " for " + UserID);
        }

        public async Task UpdateDamage(string con, string UserID, int deal, int received)
        {
            using (var dbcon = new MySqlConnection(con))
            {
                await dbcon.OpenAsync();
                using (var cmd = new MySqlCommand("UPDATE `stats` SET damage_received = damage_receivd + @b WHERE userid = @a", dbcon))
                {
                    cmd.Parameters.AddWithValue("@a", UserID);
                    cmd.Parameters.AddWithValue("@b", received);
                    await cmd.ExecuteNonQueryAsync();
                }
                using (var cmd = new MySqlCommand("UPDATE `stats` SET damage_done = damage_done + @b WHERE userid = @a", dbcon))
                {
                    cmd.Parameters.AddWithValue("@a", UserID);
                    cmd.Parameters.AddWithValue("@b", deal);
                    await cmd.ExecuteNonQueryAsync();
                }
                await dbcon.CloseAsync();
            }

            Logger.Info("Register damage for " + UserID);
        }

        public async Task UpdateStats(string con, string UserID, RoleTypeR role, bool isDeath = false, bool isKilled = false)
        {
            string fieldName = "";
            string isDeaths = isDeath ? "deaths_" : "kills_";
            isDeaths = isKilled ? "killed_" : isDeaths;
            switch (role)
            {
                case RoleTypeR.ChaosInsurgency:
                    fieldName = isDeaths + "chaos";
                    break;
                case RoleTypeR.ClassD:
                    fieldName = isDeaths + "classd";
                    break;
                case RoleTypeR.FacilityGuard:
                    fieldName = isDeaths + "guard";
                    break;
                case RoleTypeR.NtfCadet:
                    fieldName = isDeaths + "cadet";
                    break;
                case RoleTypeR.NtfCommander:
                    fieldName = isDeaths + "commander";
                    break;
                case RoleTypeR.NtfLieutenant:
                    fieldName = isDeaths + "lieutenant";
                    break;
                case RoleTypeR.NtfScientist:
                    fieldName = isDeaths + "ntfscientist";
                    break;
                case RoleTypeR.Scientist:
                    fieldName = isDeaths + "scientist";
                    break;
                case RoleTypeR.Scp049:
                    fieldName = isDeaths + "049";
                    break;
                case RoleTypeR.Scp0492:
                    fieldName = isDeaths + "0492";
                    break;
                case RoleTypeR.Scp079:
                    fieldName = isDeaths + "079";
                    break;
                case RoleTypeR.Scp096:
                    fieldName = isDeaths + "096";
                    break;
                case RoleTypeR.Scp106:
                    fieldName = isDeaths + "106";
                    break;
                case RoleTypeR.Scp173:
                    fieldName = isDeaths + "173";
                    break;
                case RoleTypeR.Scp93953:
                case RoleTypeR.Scp93989:
                    fieldName = isDeaths + "939";
                    break;
            }
            if (string.IsNullOrEmpty(fieldName))
                return;
            using (var dbcon = new MySqlConnection(con))
            {
                await dbcon.OpenAsync();
                using (var cmd = new MySqlCommand("UPDATE `stats` SET " + fieldName + " = " + fieldName + " + 1 WHERE userid = @a", dbcon))
                {
                    cmd.Parameters.AddWithValue("@a", UserID);
                    await cmd.ExecuteNonQueryAsync();
                }
                if (!isKilled)
                {
                    using (var cmd = new MySqlCommand("UPDATE `stats` SET " + (isDeath ? "deaths" : "kills") + " = " + (isDeath ? "deaths" : "kills") + " + 1 WHERE userid = @a", dbcon))
                    {
                        cmd.Parameters.AddWithValue("@a", UserID);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }

                await dbcon.CloseAsync();
            }
            Logger.Info("Register " + fieldName + " for " + UserID);

        }
        public async Task UpdateTime(string con, string UserID, RoleTypeR role, int time)
        {
            string fieldName = "";
            switch (role)
            {
                case RoleTypeR.ChaosInsurgency:
                    fieldName = "chaos";
                    break;
                case RoleTypeR.ClassD:
                    fieldName = "classd";
                    break;
                case RoleTypeR.FacilityGuard:
                    fieldName = "guard";
                    break;
                case RoleTypeR.NtfCadet:
                    fieldName = "cadet";
                    break;
                case RoleTypeR.NtfCommander:
                    fieldName = "commander";
                    break;
                case RoleTypeR.NtfLieutenant:
                    fieldName = "lieutenant";
                    break;
                case RoleTypeR.NtfScientist:
                    fieldName = "ntfscientist";
                    break;
                case RoleTypeR.Scientist:
                    fieldName = "scientist";
                    break;
                case RoleTypeR.Scp049:
                    fieldName = "049";
                    break;
                case RoleTypeR.Scp0492:
                    fieldName =  "0492";
                    break;
                case RoleTypeR.Scp079:
                    fieldName =  "079";
                    break;
                case RoleTypeR.Scp096:
                    fieldName = "096";
                    break;
                case RoleTypeR.Scp106:
                    fieldName =  "106";
                    break;
                case RoleTypeR.Scp173:
                    fieldName =  "173";
                    break;
                case RoleTypeR.Spectator:
                    fieldName = "spectator";
                    break;
                case RoleTypeR.Scp93953:
                case RoleTypeR.Scp93989:
                    fieldName = "939";
                    break;
                case RoleTypeR.Tutorial:
                    fieldName = "tutorial";
                    break;
            }
            if (string.IsNullOrEmpty(fieldName))
                return;
            using (var dbcon = new MySqlConnection(con))
            {
                await dbcon.OpenAsync();
                using (var cmd = new MySqlCommand($"UPDATE `stats` SET time_as_{fieldName} = time_as_{fieldName} + @b WHERE userid = @a", dbcon))
                {
                    cmd.Parameters.AddWithValue("@a", UserID);
                    cmd.Parameters.AddWithValue("@b", time);
                    await cmd.ExecuteNonQueryAsync();
                }

                await dbcon.CloseAsync();
            }
            Logger.Info($"Register time_as_{fieldName} for " + UserID);

        }

        public enum RoleTypeR : sbyte
        {
            None = -1,
            Scp173 = 0,
            ClassD = 1,
            Spectator = 2,
            Scp106 = 3,
            NtfScientist = 4,
            Scp049 = 5,
            Scientist = 6,
            Scp079 = 7,
            ChaosInsurgency = 8,
            Scp096 = 9,
            Scp0492 = 10,
            NtfLieutenant = 11,
            NtfCommander = 12,
            NtfCadet = 13,
            Tutorial = 14,
            FacilityGuard = 15,
            Scp93953 = 16,
            Scp93989 = 17
        }
    }
}
