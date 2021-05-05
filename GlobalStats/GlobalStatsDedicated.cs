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
    public class GlobalStatsDedicated : NPAddonDedicated
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
                        foreach(var plr in serv.Players)
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
