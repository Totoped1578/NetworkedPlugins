using Discord.WebSocket;
using LiteNetLib.Utils;
using MySqlConnector;
using NetworkedPlugins.API;
using NetworkedPlugins.API.Attributes;
using NetworkedPlugins.API.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Patreon
{
    [NPAddonInfo(
      addonID = "DWp3wqpdwaDOpwd",
      addonAuthor = "Killers0992",
      addonName = "Patreon",
      addonVersion = "1.0.0")]
    public class PatreonDedicated : NPAddonDedicated
    {
        public string key = "";
        public string bottoken = "";

        public DiscordSocketClient _client;
        public override void OnEnable()
        {
            base.OnEnable();
            Task.Factory.StartNew(async () =>
            {
                try
                {
                    await StartDiscordBot();

                }
                catch (Exception ex)
                {
                    Logger.Error(ex.ToString());
                }
            });
            if (!File.Exists("connectionkey_main.json"))
                File.WriteAllText("connectionkey_main.json", "bad");
            key = File.ReadAllText("connectionkey_main.json");
            if (!File.Exists("bot_token.json"))
                File.WriteAllText("bot_token.json", "bad");
            bottoken = File.ReadAllText("bot_token.json");
            Logger.Info("Addon started.");

        }

        public async Task StartDiscordBot()
        {
            var cfg = new DiscordSocketConfig();
            cfg.AlwaysDownloadUsers = true;
            cfg.GatewayIntents = Discord.GatewayIntents.GuildMembers | Discord.GatewayIntents.Guilds | Discord.GatewayIntents.GuildPresences;

            _client = new DiscordSocketClient(cfg);
            _client.Ready += ReadyAsync;
            await _client.LoginAsync(Discord.TokenType.Bot, bottoken);
            await _client.StartAsync();
            await Task.Delay(Timeout.Infinite);
        }

        public override void OnMessageReceived(NPServer server, global::LiteNetLib.Utils.NetDataReader reader)
        {
            string userid = reader.GetString();
            Task.Factory.StartNew(async () =>
            {
                await GetUserDiscordID(userid, server.FullAddress);
            });
        }

        public async Task GetUserDiscordID(string userid, string serverID)
        {
            try
            {
                using (var dbcon = new MySqlConnection(key))
                {
                    await dbcon.OpenAsync();
                    int x = 0;
                    using (var cmd = new MySqlCommand("SELECT discord_id,patreon_role_name,patreon_role_color FROM `playerdata` WHERE steamid = @a", dbcon))
                    {
                        cmd.Parameters.AddWithValue("@a", userid);
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                x++;
                                var did = (ulong)((System.Int64)reader[0]);
                                var usr = _client.GetGuild(493060787540197393).GetUser(did);
                                if (usr != null)
                                {
                                    Dictionary<ulong, int> patreonRoles = new Dictionary<ulong, int>()
                                {
                                    { 618189549264306186,1 },
                                    { 618189547754487895,2 },
                                    { 618189545585770535,3 },
                                    { 618189541865554133,4 },
                                    { 618221260115148800,5 }
                                };
                                    int bestTier = 0;
                                    foreach (var role in usr.Roles)
                                    {
                                        if (patreonRoles.ContainsKey(role.Id))
                                        {
                                            if (bestTier < patreonRoles[role.Id])
                                                bestTier = patreonRoles[role.Id];
                                        }
                                    }
                                    if (bestTier != 0)
                                    {
                                        if (bestTier >= 3)
                                        {
                                            NetDataWriter wr = new NetDataWriter();
                                            wr.Put(userid);
                                            wr.Put(reader[1].ToString());
                                            wr.Put(reader[2].ToString());
                                            string[] d = serverID.Split(':');
                                            Logger.Info("User " + userid + " gets patreon role tier custom " + bestTier + ", server: " + serverID);
                                            SendData(d[0], ushort.Parse(d[1]), wr);
                                        }
                                        else
                                        {
                                            NetDataWriter wr = new NetDataWriter();
                                            wr.Put(userid);
                                            wr.Put("Patreon");
                                            wr.Put("yellow");
                                            string[] d = serverID.Split(':');
                                            Logger.Info("User " + userid + " gets patreon role tier " + bestTier + ", server: " + serverID);
                                            SendData(d[0], ushort.Parse(d[1]), wr);
                                        }

                                    }
                                    else
                                    {
                                        Logger.Info("User " + userid + " without patreon beneficts, server: " + serverID);
                                    }
                                }
                                else
                                {
                                    if (did == 0)
                                    {

                                    }
                                    else
                                    {
                                        Logger.Info("User " + userid + " not found on Kingsplayground discord, DiscordID: " + did.ToString() + ", server: " + serverID);
                                    }
                                }

                            }
                        }
                    }
                    if (x == 0)
                    {
                        Logger.Info("User " + userid + " dont have discord connected with account, server: " + serverID);
                    }
                    await dbcon.CloseAsync();
                }
            }
            catch(Exception ex)
            {
                Logger.Error(ex.ToString());
            }
          
        }

        private Task ReadyAsync()
        {
            Logger.Info($"{_client.CurrentUser} is connected!");

            return Task.CompletedTask;
        }
    }
}
