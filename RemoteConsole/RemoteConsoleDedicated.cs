using Discord;
using Discord.WebSocket;
using NetworkedPlugins.API;
using NetworkedPlugins.API.Attributes;
using NetworkedPlugins.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RemoteConsole
{
    [NPAddonInfo(
      addonID = "0xpA3dDeCoa3xa",
      addonAuthor = "Killers0992",
      addonName = "RemoteConsole",
      addonVersion = "1.0.0")]
    public class RemoteConsoleDedicated : NPAddonDedicated<RemoteConsoleConfig>
    {
        private DiscordSocketClient _client;

        public override void OnEnable()
        {
            _client = new DiscordSocketClient(new DiscordSocketConfig()
            {
                GatewayIntents = GatewayIntents.GuildMembers
                    | GatewayIntents.GuildMessages
                    | GatewayIntents.GuildMessageTyping
                    | GatewayIntents.GuildIntegrations
                    | GatewayIntents.Guilds
                    | GatewayIntents.DirectMessages,
                AlwaysDownloadUsers = true
            });

            _client.Ready += ReadyAsync;
            _client.MessageReceived += MessageReceivedAsync;
            Task.Factory.StartNew(StartDiscordBot);
        }

        public string lastExecuted = "";

        public override void OnConsoleCommand(string cmd, List<string> arguments)
        {
            switch (cmd.ToUpper())
            {
                case "STA":
                    string cmdArg = arguments[0];
                    lastExecuted = cmdArg;
                    List<string> cmdArgs = arguments.Count == 1 ? new List<string>() : arguments.Skip(1).ToList();
                    foreach(var server in GetServers())
                    {
                        server.ExecuteCommand(cmdArg, cmdArgs);
                    }
                    break;
            }
        }

        private async Task Updater()
        {
            while (true)
            {
                await Task.Delay(1000);

                foreach (var ra in Config.RemoteConsoles)
                {
                    try
                    {

                        NPServer server = null;

                        foreach (var srv in GetServers())
                        {
                            if (srv.ServerPort == ra.Value.ServerPort)
                            {
                                server = srv;
                            }
                        }

                        if (server == null)
                        {
                            Logger.Info("Server is null.  " + ra.Value.ServerPort);
                            continue;
                        }

                        var textChannel = _client.GetGuild(ra.Value.GuildID).GetTextChannel(ra.Value.ChannelID);
                        var pi = await textChannel.GetMessageAsync(ra.Value.PlayerInfoMessageID);

                        if (pi == null)
                        {
                            await textChannel.SendMessageAsync("", false, GeneratePlayerInfoEmbed(server).Build());
                        }
                        else
                        {
                            if (pi is IUserMessage msg)
                            {
                                await msg.ModifyAsync(x =>
                                {
                                    x.Content = "";
                                    x.Embed = GeneratePlayerInfoEmbed(server).Build();
                                });
                            }
                        }

                        var main = await textChannel.GetMessageAsync(ra.Value.MainMessageID);

                        if (main == null)
                        {
                            await textChannel.SendMessageAsync("", false, GenerateRAEmbed(server).Build());
                        }
                        else
                        {
                            if (main is IUserMessage msg)
                            {
                                await msg.ModifyAsync(x =>
                                {
                                    x.Content = "";
                                    x.Embed = GenerateRAEmbed(server).Build();
                                });
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex.ToString());
                    }
                }

            }
        }

        public Dictionary<string, List<string>> logs = new Dictionary<string, List<string>>();

        public override void OnConsoleResponse(NPServer server, string command, string response, bool isRa)
        {
            var sp = command.Split(' ')[0];
            if (lastExecuted == sp)
            {
                Logger.Info($"Command {sp} executed on server {server.FullAddress}, response {response}");
                return;
            }
            if (logs.ContainsKey(server.FullAddress))
            {
                logs[server.FullAddress].RemoveAt(0);
                logs[server.FullAddress].RemoveAt(0);
                logs[server.FullAddress].Add(command);
                logs[server.FullAddress].Add(response);
            }
        }

        public EmbedBuilder GenerateRAEmbed(NPServer server)
        {
            EmbedBuilder builder = new EmbedBuilder();
            builder.WithTitle($"Remote Console");
            if (server != null)
            {
                string plrs = "";
                if (!logs.ContainsKey(server.FullAddress))
                    logs.Add(server.FullAddress, new List<string>() { "-", "-", "-", "-", "-", "-", "-" });
                foreach(var lg in logs[server.FullAddress])
                {
                    plrs += $"\n {lg}";
                }
                builder.AddField("Logs", plrs);
            }
            else
            {
                builder.AddField("Logs", "Server is offline.");
            }
            return builder;
        }

        public EmbedBuilder GeneratePlayerInfoEmbed(NPServer server)
        {
            EmbedBuilder builder = new EmbedBuilder();
            builder.WithTitle($"Player Info");
            if (server != null)
            {
                if (server.Players.Count == 0)
                {
                    builder.AddField("Players", "Server is empty");
                }
                else
                {
                    string plrs = "";
                    string usr = "";
                    string roles = "";
                    int staff = 0;
                    int play = 0;
                    foreach(var plr in server.Players)
                    {
                        if (plr.Value.RemoteAdminAccess)
                            staff++;
                        else
                            play++;
                        plrs += $"\n``[{plr.Value.PlayerID}]`` {(string.IsNullOrEmpty(plr.Value.UserName) ? "``Unknown Name``" : $"``{plr.Value.UserName}``")}";

                        //plrs += $"\n{(!string.IsNullOrEmpty(plr.Value.RankName) ? $"``[{(plr.Value.RankName)}]`` " : "")}{(plr.Value.RemoteAdminAccess ? " **[RA]**" :"")} {(string.IsNullOrEmpty(plr.Value.UserName) ? "``Unknown Name``" : $"``{plr.Value.UserName}``")}";
                        usr += $"\n``{(string.IsNullOrEmpty(plr.Key) ? "0@steam" : plr.Key)}``";
                        roles += $"\n``{((RoleTypeR)plr.Value.Role)}`` **{(int)plr.Value.Health}**/**{plr.Value.MaxHealth}**";
                    }
                    builder.AddField("Players online", "Players ``" + play + "``, staff ``" + staff + "``");
                    builder.AddField("Player Name", plrs, true);
                    builder.AddField("User ID", usr, true);
                    builder.AddField("Role / HP", roles, true);
                }
            }
            else
            {
                builder.AddField("Players", "Server is offline.");
            }
            return builder;
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


        private async Task MessageReceivedAsync(SocketMessage message)
        {
            if (message.Author.Id == _client.CurrentUser.Id)
                return;

            foreach(var cfg in Config.RemoteConsoles)
            {
                if (cfg.Value.ChannelID == message.Channel.Id)
                {
                    foreach(var server in GetServers())
                    {
                        if (server.ServerPort == cfg.Value.ServerPort)
                        {
                            string[] con = message.Content.Split(' ');
                            string cmd = con[0];
                            server.ExecuteCommand(cmd, con.Length != 1 ? con.Skip(1).ToList() : new List<string>());
                        }
                    }
                    await message.DeleteAsync();
                }
            }
        }

        private Task ReadyAsync()
        {
            Logger.Info($"RemoteConsole bot is ready! ({_client.CurrentUser})");
            return Task.CompletedTask;
        }

        public async Task StartDiscordBot()
        {
            await _client.LoginAsync(TokenType.Bot, Config.BotToken);
            await _client.StartAsync();

            await Updater();
        }
    }
}
