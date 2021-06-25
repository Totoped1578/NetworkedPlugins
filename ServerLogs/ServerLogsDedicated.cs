using LiteNetLib.Utils;
using MySqlConnector;
using NetworkedPlugins.API;
using NetworkedPlugins.API.Attributes;
using NetworkedPlugins.API.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerLogs
{
    [NPAddonInfo(
        addonID = "ap3pAdp3wad",
        addonAuthor = "Killers0992",
        addonName = "ServerLogs",
        addonVersion = "1.0.0")]
    public class ServerLogsDedicated : NPAddonDedicated<ServerLogsConfig>
    {
        public override void OnMessageReceived(NPServer server, NetDataReader reader)
        {
            byte b = reader.GetByte();
            DateTime time = new DateTime(reader.GetLong());
            switch (b)
            {
                //Leave event
                case 0:
                    Task.Factory.StartNew(async () =>
                    {
                        var connection = new MySqlConnection(Config.DBConnection);
                        await connection.OpenAsync();
                        using(var cmd = connection.CreateCommand())
                        {
                            cmd.CommandText = "INSERT INTO serverlogs (server_ip, server_port, round_time, time, type, userid, role, data, targets) VALUES (@server_ip, @server_port, @round_time, @time, @type, @userid, @role, @data, @targets)";
                            cmd.Parameters.AddWithValue("@server_ip", server.ServerAddress);
                            cmd.Parameters.AddWithValue("@server_port", (int)server.ServerPort);
                            cmd.Parameters.AddWithValue("@round_time", time);
                            cmd.Parameters.AddWithValue("@time", DateTime.Now);
                            cmd.Parameters.AddWithValue("@type", (int)b);
                            cmd.Parameters.AddWithValue("@userid", reader.GetString());
                            cmd.Parameters.AddWithValue("@role", -1);
                            cmd.Parameters.AddWithValue("@data", "{}");
                            cmd.Parameters.AddWithValue("@targets", "[]");
                            await cmd.ExecuteNonQueryAsync();
                        }
                        await connection.CloseAsync();
                    });
                    break;
                //Leave event
                case 1:
                    Task.Factory.StartNew(async () =>
                    {
                        var connection = new MySqlConnection(Config.DBConnection);
                        await connection.OpenAsync();
                        using (var cmd = connection.CreateCommand())
                        {
                            cmd.CommandText = "INSERT INTO serverlogs (server_ip, server_port, round_time, time, type, userid, role, data, targets) VALUES (@server_ip, @server_port, @round_time, @time, @type, @userid, @role, @data, @targets)";
                            cmd.Parameters.AddWithValue("@server_ip", server.ServerAddress);
                            cmd.Parameters.AddWithValue("@server_port", (int)server.ServerPort);
                            cmd.Parameters.AddWithValue("@round_time", time);
                            cmd.Parameters.AddWithValue("@time", DateTime.Now);
                            cmd.Parameters.AddWithValue("@type", (int)b);
                            cmd.Parameters.AddWithValue("@userid", reader.GetString());
                            cmd.Parameters.AddWithValue("@role", -1);
                            cmd.Parameters.AddWithValue("@data", "{}");
                            cmd.Parameters.AddWithValue("@targets", "[]");
                            await cmd.ExecuteNonQueryAsync();
                        }
                        await connection.CloseAsync();
                    });
                    break;
                //Died event
                case 2:
                    Task.Factory.StartNew(async () =>
                    {
                        var connection = new MySqlConnection(Config.DBConnection);
                        await connection.OpenAsync();
                        using (var cmd = connection.CreateCommand())
                        {
                            cmd.CommandText = "INSERT INTO serverlogs (server_ip, server_port, round_time, time, type, userid, role, data, targets) VALUES (@server_ip, @server_port, @round_time, @time, @type, @userid, @role, @data, @targets)";
                            cmd.Parameters.AddWithValue("@server_ip", server.ServerAddress);
                            cmd.Parameters.AddWithValue("@server_port", (int)server.ServerPort);
                            cmd.Parameters.AddWithValue("@round_time", time);
                            cmd.Parameters.AddWithValue("@time", DateTime.Now);
                            cmd.Parameters.AddWithValue("@type", (int)b);
                            cmd.Parameters.AddWithValue("@userid", reader.GetString());
                            cmd.Parameters.AddWithValue("@role", reader.GetInt());
                            cmd.Parameters.AddWithValue("@data", JsonConvert.SerializeObject(new Dictionary<string, object>()
                            {
                                { "TargetID", reader.GetString() },
                                { "TargetClass", reader.GetInt() },
                                { "DamageType", reader.GetString() }
                            }));
                            cmd.Parameters.AddWithValue("@targets", "[]");
                            await cmd.ExecuteNonQueryAsync();
                        }
                        await connection.CloseAsync();
                    });
                    break;
                //Trigger Tesla event
                case 3:
                    Task.Factory.StartNew(async () =>
                    {
                        var connection = new MySqlConnection(Config.DBConnection);
                        await connection.OpenAsync();
                        using (var cmd = connection.CreateCommand())
                        {
                            cmd.CommandText = "INSERT INTO serverlogs (server_ip, server_port, round_time, time, type, userid, role, data, targets) VALUES (@server_ip, @server_port, @round_time, @time, @type, @userid, @role, @data, @targets)";
                            cmd.Parameters.AddWithValue("@server_ip", server.ServerAddress);
                            cmd.Parameters.AddWithValue("@server_port", (int)server.ServerPort);
                            cmd.Parameters.AddWithValue("@round_time", time);
                            cmd.Parameters.AddWithValue("@time", DateTime.Now);
                            cmd.Parameters.AddWithValue("@type", (int)b);
                            cmd.Parameters.AddWithValue("@userid", reader.GetString());
                            cmd.Parameters.AddWithValue("@role", reader.GetInt());
                            cmd.Parameters.AddWithValue("@data", "{}");
                            cmd.Parameters.AddWithValue("@targets", "[]");
                            await cmd.ExecuteNonQueryAsync();
                        }
                        await connection.CloseAsync();
                    });
                    break;
                //Warhead detonate event
                case 4:
                    Task.Factory.StartNew(async () =>
                    {
                        var connection = new MySqlConnection(Config.DBConnection);
                        await connection.OpenAsync();
                        using (var cmd = connection.CreateCommand())
                        {
                            cmd.CommandText = "INSERT INTO serverlogs (server_ip, server_port, round_time, time, type, userid, role, data, targets) VALUES (@server_ip, @server_port, @round_time, @time, @type, @userid, @role, @data, @targets)";
                            cmd.Parameters.AddWithValue("@server_ip", server.ServerAddress);
                            cmd.Parameters.AddWithValue("@server_port", (int)server.ServerPort);
                            cmd.Parameters.AddWithValue("@round_time", time);
                            cmd.Parameters.AddWithValue("@time", DateTime.Now);
                            cmd.Parameters.AddWithValue("@type", (int)b);
                            cmd.Parameters.AddWithValue("@userid", "");
                            cmd.Parameters.AddWithValue("@role", -1);
                            cmd.Parameters.AddWithValue("@data", "{}");
                            cmd.Parameters.AddWithValue("@targets", "[]");
                            await cmd.ExecuteNonQueryAsync();
                        }
                        await connection.CloseAsync();
                    });
                    break;
                //Generator activated
                case 5:
                    Task.Factory.StartNew(async () =>
                    {
                        var connection = new MySqlConnection(Config.DBConnection);
                        await connection.OpenAsync();
                        using (var cmd = connection.CreateCommand())
                        {
                            cmd.CommandText = "INSERT INTO serverlogs (server_ip, server_port, round_time, time, type, userid, role, data, targets) VALUES (@server_ip, @server_port, @round_time, @time, @type, @userid, @role, @data, @targets)";
                            cmd.Parameters.AddWithValue("@server_ip", server.ServerAddress);
                            cmd.Parameters.AddWithValue("@server_port", (int)server.ServerPort);
                            cmd.Parameters.AddWithValue("@round_time", time);
                            cmd.Parameters.AddWithValue("@time", DateTime.Now);
                            cmd.Parameters.AddWithValue("@type", (int)b);
                            cmd.Parameters.AddWithValue("@userid", "");
                            cmd.Parameters.AddWithValue("@role", -1);
                            cmd.Parameters.AddWithValue("@data", JsonConvert.SerializeObject(new Dictionary<string, object>()
                            {
                                { "CurrentRoom", reader.GetString() },
                                { "Voltage", reader.GetInt() }
                            }));
                            cmd.Parameters.AddWithValue("@targets", "[]");
                            await cmd.ExecuteNonQueryAsync();
                        }
                        await connection.CloseAsync();
                    });
                    break;
                //Warhead start
                case 6:
                    Task.Factory.StartNew(async () =>
                    {
                        var connection = new MySqlConnection(Config.DBConnection);
                        await connection.OpenAsync();
                        using (var cmd = connection.CreateCommand())
                        {
                            cmd.CommandText = "INSERT INTO serverlogs (server_ip, server_port, round_time, time, type, userid, role, data, targets) VALUES (@server_ip, @server_port, @round_time, @time, @type, @userid, @role, @data, @targets)";
                            cmd.Parameters.AddWithValue("@server_ip", server.ServerAddress);
                            cmd.Parameters.AddWithValue("@server_port", (int)server.ServerPort);
                            cmd.Parameters.AddWithValue("@round_time", time);
                            cmd.Parameters.AddWithValue("@time", DateTime.Now);
                            cmd.Parameters.AddWithValue("@type", (int)b);
                            cmd.Parameters.AddWithValue("@userid", reader.GetString());
                            cmd.Parameters.AddWithValue("@role", reader.GetInt());
                            cmd.Parameters.AddWithValue("@data", JsonConvert.SerializeObject(new Dictionary<string, object>()
                            {
                                { "DetonationTime", reader.GetFloat() }
                            }));
                            cmd.Parameters.AddWithValue("@targets", "[]");
                            await cmd.ExecuteNonQueryAsync();
                        }
                        await connection.CloseAsync();
                    });
                    break;
                //Warhead stop
                case 7:
                    Task.Factory.StartNew(async () =>
                    {
                        var connection = new MySqlConnection(Config.DBConnection);
                        await connection.OpenAsync();
                        using (var cmd = connection.CreateCommand())
                        {
                            cmd.CommandText = "INSERT INTO serverlogs (server_ip, server_port, round_time, time, type, userid, role, data, targets) VALUES (@server_ip, @server_port, @round_time, @time, @type, @userid, @role, @data, @targets)";
                            cmd.Parameters.AddWithValue("@server_ip", server.ServerAddress);
                            cmd.Parameters.AddWithValue("@server_port", (int)server.ServerPort);
                            cmd.Parameters.AddWithValue("@round_time", time);
                            cmd.Parameters.AddWithValue("@time", DateTime.Now);
                            cmd.Parameters.AddWithValue("@type", (int)b);
                            cmd.Parameters.AddWithValue("@userid", reader.GetString());
                            cmd.Parameters.AddWithValue("@role", reader.GetInt());
                            cmd.Parameters.AddWithValue("@data", "{}");
                            cmd.Parameters.AddWithValue("@targets", "[]");
                            await cmd.ExecuteNonQueryAsync();
                        }
                        await connection.CloseAsync();
                    });
                    break;
                //SCP914 Upgrade Items
                case 8:
                    Task.Factory.StartNew(async () =>
                    {
                        var connection = new MySqlConnection(Config.DBConnection);
                        await connection.OpenAsync();
                        using (var cmd = connection.CreateCommand())
                        {
                            cmd.CommandText = "INSERT INTO serverlogs (server_ip, server_port, round_time, time, type, userid, role, data, targets) VALUES (@server_ip, @server_port, @round_time, @time, @type, @userid, @role, @data, @targets)";
                            cmd.Parameters.AddWithValue("@server_ip", server.ServerAddress);
                            cmd.Parameters.AddWithValue("@server_port", (int)server.ServerPort);
                            cmd.Parameters.AddWithValue("@round_time", time);
                            cmd.Parameters.AddWithValue("@time", DateTime.Now);
                            cmd.Parameters.AddWithValue("@type", (int)b);
                            cmd.Parameters.AddWithValue("@userid", "");
                            cmd.Parameters.AddWithValue("@role", -1);
                            cmd.Parameters.AddWithValue("@data", JsonConvert.SerializeObject(new Dictionary<string, object>()
                            {
                                { "Items", reader.GetIntArray() }
                            }));
                            cmd.Parameters.AddWithValue("@targets", JsonConvert.SerializeObject(reader.GetStringArray()));
                            await cmd.ExecuteNonQueryAsync();
                        }
                        await connection.CloseAsync();
                    });
                    break;
                //Gen tablet insert
                case 9:
                    Task.Factory.StartNew(async () =>
                    {
                        var connection = new MySqlConnection(Config.DBConnection);
                        await connection.OpenAsync();
                        using (var cmd = connection.CreateCommand())
                        {
                            cmd.CommandText = "INSERT INTO serverlogs (server_ip, server_port, round_time, time, type, userid, role, data, targets) VALUES (@server_ip, @server_port, @round_time, @time, @type, @userid, @role, @data, @targets)";
                            cmd.Parameters.AddWithValue("@server_ip", server.ServerAddress);
                            cmd.Parameters.AddWithValue("@server_port", (int)server.ServerPort);
                            cmd.Parameters.AddWithValue("@round_time", time);
                            cmd.Parameters.AddWithValue("@time", DateTime.Now);
                            cmd.Parameters.AddWithValue("@type", (int)b);
                            cmd.Parameters.AddWithValue("@userid", reader.GetString());
                            cmd.Parameters.AddWithValue("@role", reader.GetInt());
                            cmd.Parameters.AddWithValue("@data", "{}");
                            cmd.Parameters.AddWithValue("@targets", "[]");
                            await cmd.ExecuteNonQueryAsync();
                        }
                        await connection.CloseAsync();
                    });
                    break;
                //Open gen
                case 10:
                    Task.Factory.StartNew(async () =>
                    {
                        var connection = new MySqlConnection(Config.DBConnection);
                        await connection.OpenAsync();
                        using (var cmd = connection.CreateCommand())
                        {
                            cmd.CommandText = "INSERT INTO serverlogs (server_ip, server_port, round_time, time, type, userid, role, data, targets) VALUES (@server_ip, @server_port, @round_time, @time, @type, @userid, @role, @data, @targets)";
                            cmd.Parameters.AddWithValue("@server_ip", server.ServerAddress);
                            cmd.Parameters.AddWithValue("@server_port", (int)server.ServerPort);
                            cmd.Parameters.AddWithValue("@round_time", time);
                            cmd.Parameters.AddWithValue("@time", DateTime.Now);
                            cmd.Parameters.AddWithValue("@type", (int)b);
                            cmd.Parameters.AddWithValue("@userid", reader.GetString());
                            cmd.Parameters.AddWithValue("@role", reader.GetInt());
                            cmd.Parameters.AddWithValue("@data", "{}");
                            cmd.Parameters.AddWithValue("@targets", "[]");
                            await cmd.ExecuteNonQueryAsync();
                        }
                        await connection.CloseAsync();
                    });
                    break;
                //Unlock gen
                case 11:
                    Task.Factory.StartNew(async () =>
                    {
                        var connection = new MySqlConnection(Config.DBConnection);
                        await connection.OpenAsync();
                        using (var cmd = connection.CreateCommand())
                        {
                            cmd.CommandText = "INSERT INTO serverlogs (server_ip, server_port, round_time, time, type, userid, role, data, targets) VALUES (@server_ip, @server_port, @round_time, @time, @type, @userid, @role, @data, @targets)";
                            cmd.Parameters.AddWithValue("@server_ip", server.ServerAddress);
                            cmd.Parameters.AddWithValue("@server_port", (int)server.ServerPort);
                            cmd.Parameters.AddWithValue("@round_time", time);
                            cmd.Parameters.AddWithValue("@time", DateTime.Now);
                            cmd.Parameters.AddWithValue("@type", (int)b);
                            cmd.Parameters.AddWithValue("@userid", reader.GetString());
                            cmd.Parameters.AddWithValue("@role", reader.GetInt());
                            cmd.Parameters.AddWithValue("@data", "{}");
                            cmd.Parameters.AddWithValue("@targets", "[]");
                            await cmd.ExecuteNonQueryAsync();
                        }
                        await connection.CloseAsync();
                    });
                    break;
                //Contain 106
                case 12:
                    Task.Factory.StartNew(async () =>
                    {
                        var connection = new MySqlConnection(Config.DBConnection);
                        await connection.OpenAsync();
                        using (var cmd = connection.CreateCommand())
                        {
                            cmd.CommandText = "INSERT INTO serverlogs (server_ip, server_port, round_time, time, type, userid, role, data, targets) VALUES (@server_ip, @server_port, @round_time, @time, @type, @userid, @role, @data, @targets)";
                            cmd.Parameters.AddWithValue("@server_ip", server.ServerAddress);
                            cmd.Parameters.AddWithValue("@server_port", (int)server.ServerPort);
                            cmd.Parameters.AddWithValue("@round_time", time);
                            cmd.Parameters.AddWithValue("@time", DateTime.Now);
                            cmd.Parameters.AddWithValue("@type", (int)b);
                            cmd.Parameters.AddWithValue("@userid", reader.GetString());
                            cmd.Parameters.AddWithValue("@role", reader.GetInt());
                            cmd.Parameters.AddWithValue("@data", "{}");
                            cmd.Parameters.AddWithValue("@targets", "[]");
                            await cmd.ExecuteNonQueryAsync();
                        }
                        await connection.CloseAsync();
                    });
                    break;
                //create portal 106
                case 13:
                    Task.Factory.StartNew(async () =>
                    {
                        var connection = new MySqlConnection(Config.DBConnection);
                        await connection.OpenAsync();
                        using (var cmd = connection.CreateCommand())
                        {
                            cmd.CommandText = "INSERT INTO serverlogs (server_ip, server_port, round_time, time, type, userid, role, data, targets) VALUES (@server_ip, @server_port, @round_time, @time, @type, @userid, @role, @data, @targets)";
                            cmd.Parameters.AddWithValue("@server_ip", server.ServerAddress);
                            cmd.Parameters.AddWithValue("@server_port", (int)server.ServerPort);
                            cmd.Parameters.AddWithValue("@round_time", time);
                            cmd.Parameters.AddWithValue("@time", DateTime.Now);
                            cmd.Parameters.AddWithValue("@type", (int)b);
                            cmd.Parameters.AddWithValue("@userid", reader.GetString());
                            cmd.Parameters.AddWithValue("@role", reader.GetInt());
                            cmd.Parameters.AddWithValue("@data", "{}");
                            cmd.Parameters.AddWithValue("@targets", "[]");
                            await cmd.ExecuteNonQueryAsync();
                        }
                        await connection.CloseAsync();
                    });
                    break;
                //changing item
                case 14:
                    Task.Factory.StartNew(async () =>
                    {
                        var connection = new MySqlConnection(Config.DBConnection);
                        await connection.OpenAsync();
                        using (var cmd = connection.CreateCommand())
                        {
                            cmd.CommandText = "INSERT INTO serverlogs (server_ip, server_port, round_time, time, type, userid, role, data, targets) VALUES (@server_ip, @server_port, @round_time, @time, @type, @userid, @role, @data, @targets)";
                            cmd.Parameters.AddWithValue("@server_ip", server.ServerAddress);
                            cmd.Parameters.AddWithValue("@server_port", (int)server.ServerPort);
                            cmd.Parameters.AddWithValue("@round_time", time);
                            cmd.Parameters.AddWithValue("@time", DateTime.Now);
                            cmd.Parameters.AddWithValue("@type", (int)b);
                            cmd.Parameters.AddWithValue("@userid", reader.GetString());
                            cmd.Parameters.AddWithValue("@role", reader.GetInt());
                            cmd.Parameters.AddWithValue("@data", JsonConvert.SerializeObject(new Dictionary<string, object>()
                            {
                                { "OldItem", reader.GetInt() },
                                { "NewItem", reader.GetInt() }
                            }));
                            cmd.Parameters.AddWithValue("@targets", "[]");
                            await cmd.ExecuteNonQueryAsync();
                        }
                        await connection.CloseAsync();
                    });
                    break;
                //gain xp
                case 15:
                    Task.Factory.StartNew(async () =>
                    {
                        var connection = new MySqlConnection(Config.DBConnection);
                        await connection.OpenAsync();
                        using (var cmd = connection.CreateCommand())
                        {
                            cmd.CommandText = "INSERT INTO serverlogs (server_ip, server_port, round_time, time, type, userid, role, data, targets) VALUES (@server_ip, @server_port, @round_time, @time, @type, @userid, @role, @data, @targets)";
                            cmd.Parameters.AddWithValue("@server_ip", server.ServerAddress);
                            cmd.Parameters.AddWithValue("@server_port", (int)server.ServerPort);
                            cmd.Parameters.AddWithValue("@round_time", time);
                            cmd.Parameters.AddWithValue("@time", DateTime.Now);
                            cmd.Parameters.AddWithValue("@type", (int)b);
                            cmd.Parameters.AddWithValue("@userid", reader.GetString());
                            cmd.Parameters.AddWithValue("@role", reader.GetInt());
                            cmd.Parameters.AddWithValue("@data", JsonConvert.SerializeObject(new Dictionary<string, object>()
                            {
                                { "Amount", reader.GetFloat() },
                                { "GainType", reader.GetInt() }
                            }));
                            cmd.Parameters.AddWithValue("@targets", "[]");
                            await cmd.ExecuteNonQueryAsync();
                        }
                        await connection.CloseAsync();
                    });
                    break;
                //gain level
                case 16:
                    Task.Factory.StartNew(async () =>
                    {
                        var connection = new MySqlConnection(Config.DBConnection);
                        await connection.OpenAsync();
                        using (var cmd = connection.CreateCommand())
                        {
                            cmd.CommandText = "INSERT INTO serverlogs (server_ip, server_port, round_time, time, type, userid, role, data, targets) VALUES (@server_ip, @server_port, @round_time, @time, @type, @userid, @role, @data, @targets)";
                            cmd.Parameters.AddWithValue("@server_ip", server.ServerAddress);
                            cmd.Parameters.AddWithValue("@server_port", (int)server.ServerPort);
                            cmd.Parameters.AddWithValue("@round_time", time);
                            cmd.Parameters.AddWithValue("@time", DateTime.Now);
                            cmd.Parameters.AddWithValue("@type", (int)b);
                            cmd.Parameters.AddWithValue("@userid", reader.GetString());
                            cmd.Parameters.AddWithValue("@role", reader.GetInt());
                            cmd.Parameters.AddWithValue("@data", JsonConvert.SerializeObject(new Dictionary<string, object>()
                            {
                                { "OldLevel", reader.GetInt() },
                                { "NewLevel", reader.GetInt() }
                            }));
                            cmd.Parameters.AddWithValue("@targets", "[]");
                            await cmd.ExecuteNonQueryAsync();
                        }
                        await connection.CloseAsync();
                    });
                    break;
                //reload weapon
                case 17:
                    Task.Factory.StartNew(async () =>
                    {
                        var connection = new MySqlConnection(Config.DBConnection);
                        await connection.OpenAsync();
                        using (var cmd = connection.CreateCommand())
                        {
                            cmd.CommandText = "INSERT INTO serverlogs (server_ip, server_port, round_time, time, type, userid, role, data, targets) VALUES (@server_ip, @server_port, @round_time, @time, @type, @userid, @role, @data, @targets)";
                            cmd.Parameters.AddWithValue("@server_ip", server.ServerAddress);
                            cmd.Parameters.AddWithValue("@server_port", (int)server.ServerPort);
                            cmd.Parameters.AddWithValue("@round_time", time);
                            cmd.Parameters.AddWithValue("@time", DateTime.Now);
                            cmd.Parameters.AddWithValue("@type", (int)b);
                            cmd.Parameters.AddWithValue("@userid", reader.GetString());
                            cmd.Parameters.AddWithValue("@role", reader.GetInt());
                            cmd.Parameters.AddWithValue("@data", JsonConvert.SerializeObject(new Dictionary<string, object>()
                            {
                                { "CurrentItem", reader.GetInt() }
                            }));
                            cmd.Parameters.AddWithValue("@targets", "[]");
                            await cmd.ExecuteNonQueryAsync();
                        }
                        await connection.CloseAsync();
                    });
                    break;
                //Activate warhead panel
                case 18:
                    Task.Factory.StartNew(async () =>
                    {
                        var connection = new MySqlConnection(Config.DBConnection);
                        await connection.OpenAsync();
                        using (var cmd = connection.CreateCommand())
                        {
                            cmd.CommandText = "INSERT INTO serverlogs (server_ip, server_port, round_time, time, type, userid, role, data, targets) VALUES (@server_ip, @server_port, @round_time, @time, @type, @userid, @role, @data, @targets)";
                            cmd.Parameters.AddWithValue("@server_ip", server.ServerAddress);
                            cmd.Parameters.AddWithValue("@server_port", (int)server.ServerPort);
                            cmd.Parameters.AddWithValue("@round_time", time);
                            cmd.Parameters.AddWithValue("@time", DateTime.Now);
                            cmd.Parameters.AddWithValue("@type", (int)b);
                            cmd.Parameters.AddWithValue("@userid", reader.GetString());
                            cmd.Parameters.AddWithValue("@role", reader.GetInt());
                            cmd.Parameters.AddWithValue("@data", "{}");
                            cmd.Parameters.AddWithValue("@targets", "[]");
                            await cmd.ExecuteNonQueryAsync();
                        }
                        await connection.CloseAsync();
                    });
                    break;
                //Interact elevator
                case 19:
                    Task.Factory.StartNew(async () =>
                    {
                        var connection = new MySqlConnection(Config.DBConnection);
                        await connection.OpenAsync();
                        using (var cmd = connection.CreateCommand())
                        {
                            cmd.CommandText = "INSERT INTO serverlogs (server_ip, server_port, round_time, time, type, userid, role, data, targets) VALUES (@server_ip, @server_port, @round_time, @time, @type, @userid, @role, @data, @targets)";
                            cmd.Parameters.AddWithValue("@server_ip", server.ServerAddress);
                            cmd.Parameters.AddWithValue("@server_port", (int)server.ServerPort);
                            cmd.Parameters.AddWithValue("@round_time", time);
                            cmd.Parameters.AddWithValue("@time", DateTime.Now);
                            cmd.Parameters.AddWithValue("@type", (int)b);
                            cmd.Parameters.AddWithValue("@userid", reader.GetString());
                            cmd.Parameters.AddWithValue("@role", reader.GetInt());
                            cmd.Parameters.AddWithValue("@data", "{}");
                            cmd.Parameters.AddWithValue("@targets", "[]");
                            await cmd.ExecuteNonQueryAsync();
                        }
                        await connection.CloseAsync();
                    });
                    break;
                //Interact locker
                case 20:
                    Task.Factory.StartNew(async () =>
                    {
                        var connection = new MySqlConnection(Config.DBConnection);
                        await connection.OpenAsync();
                        using (var cmd = connection.CreateCommand())
                        {
                            cmd.CommandText = "INSERT INTO serverlogs (server_ip, server_port, round_time, time, type, userid, role, data, targets) VALUES (@server_ip, @server_port, @round_time, @time, @type, @userid, @role, @data, @targets)";
                            cmd.Parameters.AddWithValue("@server_ip", server.ServerAddress);
                            cmd.Parameters.AddWithValue("@server_port", (int)server.ServerPort);
                            cmd.Parameters.AddWithValue("@round_time", time);
                            cmd.Parameters.AddWithValue("@time", DateTime.Now);
                            cmd.Parameters.AddWithValue("@type", (int)b);
                            cmd.Parameters.AddWithValue("@userid", reader.GetString());
                            cmd.Parameters.AddWithValue("@role", reader.GetInt());
                            cmd.Parameters.AddWithValue("@data", "{}");
                            cmd.Parameters.AddWithValue("@targets", "[]");
                            await cmd.ExecuteNonQueryAsync();
                        }
                        await connection.CloseAsync();
                    });
                    break;
                //Decontamination
                case 21:
                    Task.Factory.StartNew(async () =>
                    {
                        var connection = new MySqlConnection(Config.DBConnection);
                        await connection.OpenAsync();
                        using (var cmd = connection.CreateCommand())
                        {
                            cmd.CommandText = "INSERT INTO serverlogs (server_ip, server_port, round_time, time, type, userid, role, data, targets) VALUES (@server_ip, @server_port, @round_time, @time, @type, @userid, @role, @data, @targets)";
                            cmd.Parameters.AddWithValue("@server_ip", server.ServerAddress);
                            cmd.Parameters.AddWithValue("@server_port", (int)server.ServerPort);
                            cmd.Parameters.AddWithValue("@round_time", time);
                            cmd.Parameters.AddWithValue("@time", DateTime.Now);
                            cmd.Parameters.AddWithValue("@type", (int)b);
                            cmd.Parameters.AddWithValue("@userid", "");
                            cmd.Parameters.AddWithValue("@role", -1);
                            cmd.Parameters.AddWithValue("@data", "{}");
                            cmd.Parameters.AddWithValue("@targets", "[]");
                            await cmd.ExecuteNonQueryAsync();
                        }
                        await connection.CloseAsync();
                    });
                    break;
                //Close gen
                case 22:
                    Task.Factory.StartNew(async () =>
                    {
                        var connection = new MySqlConnection(Config.DBConnection);
                        await connection.OpenAsync();
                        using (var cmd = connection.CreateCommand())
                        {
                            cmd.CommandText = "INSERT INTO serverlogs (server_ip, server_port, round_time, time, type, userid, role, data, targets) VALUES (@server_ip, @server_port, @round_time, @time, @type, @userid, @role, @data, @targets)";
                            cmd.Parameters.AddWithValue("@server_ip", server.ServerAddress);
                            cmd.Parameters.AddWithValue("@server_port", (int)server.ServerPort);
                            cmd.Parameters.AddWithValue("@round_time", time);
                            cmd.Parameters.AddWithValue("@time", DateTime.Now);
                            cmd.Parameters.AddWithValue("@type", (int)b);
                            cmd.Parameters.AddWithValue("@userid", reader.GetString());
                            cmd.Parameters.AddWithValue("@role", reader.GetInt());
                            cmd.Parameters.AddWithValue("@data", "{}");
                            cmd.Parameters.AddWithValue("@targets", "[]");
                            await cmd.ExecuteNonQueryAsync();
                        }
                        await connection.CloseAsync();
                    });
                    break;
                //Eject gen
                case 23:
                    Task.Factory.StartNew(async () =>
                    {
                        var connection = new MySqlConnection(Config.DBConnection);
                        await connection.OpenAsync();
                        using (var cmd = connection.CreateCommand())
                        {
                            cmd.CommandText = "INSERT INTO serverlogs (server_ip, server_port, round_time, time, type, userid, role, data, targets) VALUES (@server_ip, @server_port, @round_time, @time, @type, @userid, @role, @data, @targets)";
                            cmd.Parameters.AddWithValue("@server_ip", server.ServerAddress);
                            cmd.Parameters.AddWithValue("@server_port", (int)server.ServerPort);
                            cmd.Parameters.AddWithValue("@round_time", time);
                            cmd.Parameters.AddWithValue("@time", DateTime.Now);
                            cmd.Parameters.AddWithValue("@type", (int)b);
                            cmd.Parameters.AddWithValue("@userid", reader.GetString());
                            cmd.Parameters.AddWithValue("@role", reader.GetInt());
                            cmd.Parameters.AddWithValue("@data", "{}");
                            cmd.Parameters.AddWithValue("@targets", "[]");
                            await cmd.ExecuteNonQueryAsync();
                        }
                        await connection.CloseAsync();
                    });
                    break;
                //interact door
                case 24:
                    Task.Factory.StartNew(async () =>
                    {
                        var connection = new MySqlConnection(Config.DBConnection);
                        await connection.OpenAsync();
                        using (var cmd = connection.CreateCommand())
                        {
                            cmd.CommandText = "INSERT INTO serverlogs (server_ip, server_port, round_time, time, type, userid, role, data, targets) VALUES (@server_ip, @server_port, @round_time, @time, @type, @userid, @role, @data, @targets)";
                            cmd.Parameters.AddWithValue("@server_ip", server.ServerAddress);
                            cmd.Parameters.AddWithValue("@server_port", (int)server.ServerPort);
                            cmd.Parameters.AddWithValue("@round_time", time);
                            cmd.Parameters.AddWithValue("@time", DateTime.Now);
                            cmd.Parameters.AddWithValue("@type", (int)b);
                            cmd.Parameters.AddWithValue("@userid", reader.GetString());
                            cmd.Parameters.AddWithValue("@role", reader.GetInt());
                            cmd.Parameters.AddWithValue("@data", JsonConvert.SerializeObject(new Dictionary<string, object>()
                            {
                                { "IsOpen", reader.GetBool() },
                                { "Name", reader.GetString() }
                            }));
                            cmd.Parameters.AddWithValue("@targets", "[]");
                            await cmd.ExecuteNonQueryAsync();
                        }
                        await connection.CloseAsync();
                    });
                    break;
                //activate 914
                case 25:
                    Task.Factory.StartNew(async () =>
                    {
                        var connection = new MySqlConnection(Config.DBConnection);
                        await connection.OpenAsync();
                        using (var cmd = connection.CreateCommand())
                        {
                            cmd.CommandText = "INSERT INTO serverlogs (server_ip, server_port, round_time, time, type, userid, role, data, targets) VALUES (@server_ip, @server_port, @round_time, @time, @type, @userid, @role, @data, @targets)";
                            cmd.Parameters.AddWithValue("@server_ip", server.ServerAddress);
                            cmd.Parameters.AddWithValue("@server_port", (int)server.ServerPort);
                            cmd.Parameters.AddWithValue("@round_time", time);
                            cmd.Parameters.AddWithValue("@time", DateTime.Now);
                            cmd.Parameters.AddWithValue("@type", (int)b);
                            cmd.Parameters.AddWithValue("@userid", reader.GetString());
                            cmd.Parameters.AddWithValue("@role", reader.GetInt());
                            cmd.Parameters.AddWithValue("@data", JsonConvert.SerializeObject(new Dictionary<string, object>()
                            {
                                { "KnobState", reader.GetInt() }
                            }));
                            cmd.Parameters.AddWithValue("@targets", "[]");
                            await cmd.ExecuteNonQueryAsync();
                        }
                        await connection.CloseAsync();
                    });
                    break;
                //change knob state
                case 26:
                    Task.Factory.StartNew(async () =>
                    {
                        var connection = new MySqlConnection(Config.DBConnection);
                        await connection.OpenAsync();
                        using (var cmd = connection.CreateCommand())
                        {
                            cmd.CommandText = "INSERT INTO serverlogs (server_ip, server_port, round_time, time, type, userid, role, data, targets) VALUES (@server_ip, @server_port, @round_time, @time, @type, @userid, @role, @data, @targets)";
                            cmd.Parameters.AddWithValue("@server_ip", server.ServerAddress);
                            cmd.Parameters.AddWithValue("@server_port", (int)server.ServerPort);
                            cmd.Parameters.AddWithValue("@round_time", time);
                            cmd.Parameters.AddWithValue("@time", DateTime.Now);
                            cmd.Parameters.AddWithValue("@type", (int)b);
                            cmd.Parameters.AddWithValue("@userid", reader.GetString());
                            cmd.Parameters.AddWithValue("@role", reader.GetInt());
                            cmd.Parameters.AddWithValue("@data", JsonConvert.SerializeObject(new Dictionary<string, object>()
                            {
                                { "KnobState", reader.GetInt() }
                            }));
                            cmd.Parameters.AddWithValue("@targets", "[]");
                            await cmd.ExecuteNonQueryAsync();
                        }
                        await connection.CloseAsync();
                    });
                    break;
                //Enter PD
                case 27:
                    Task.Factory.StartNew(async () =>
                    {
                        var connection = new MySqlConnection(Config.DBConnection);
                        await connection.OpenAsync();
                        using (var cmd = connection.CreateCommand())
                        {
                            cmd.CommandText = "INSERT INTO serverlogs (server_ip, server_port, round_time, time, type, userid, role, data, targets) VALUES (@server_ip, @server_port, @round_time, @time, @type, @userid, @role, @data, @targets)";
                            cmd.Parameters.AddWithValue("@server_ip", server.ServerAddress);
                            cmd.Parameters.AddWithValue("@server_port", (int)server.ServerPort);
                            cmd.Parameters.AddWithValue("@round_time", time);
                            cmd.Parameters.AddWithValue("@time", DateTime.Now);
                            cmd.Parameters.AddWithValue("@type", (int)b);
                            cmd.Parameters.AddWithValue("@userid", reader.GetString());
                            cmd.Parameters.AddWithValue("@role", reader.GetInt());
                            cmd.Parameters.AddWithValue("@data", "{}");
                            cmd.Parameters.AddWithValue("@targets", "[]");
                            await cmd.ExecuteNonQueryAsync();
                        }
                        await connection.CloseAsync();
                    });
                    break;
                //106 TP
                case 28:
                    Task.Factory.StartNew(async () =>
                    {
                        var connection = new MySqlConnection(Config.DBConnection);
                        await connection.OpenAsync();
                        using (var cmd = connection.CreateCommand())
                        {
                            cmd.CommandText = "INSERT INTO serverlogs (server_ip, server_port, round_time, time, type, userid, role, data, targets) VALUES (@server_ip, @server_port, @round_time, @time, @type, @userid, @role, @data, @targets)";
                            cmd.Parameters.AddWithValue("@server_ip", server.ServerAddress);
                            cmd.Parameters.AddWithValue("@server_port", (int)server.ServerPort);
                            cmd.Parameters.AddWithValue("@round_time", time);
                            cmd.Parameters.AddWithValue("@time", DateTime.Now);
                            cmd.Parameters.AddWithValue("@type", (int)b);
                            cmd.Parameters.AddWithValue("@userid", reader.GetString());
                            cmd.Parameters.AddWithValue("@role", reader.GetInt());
                            cmd.Parameters.AddWithValue("@data", "{}");
                            cmd.Parameters.AddWithValue("@targets", "[]");
                            await cmd.ExecuteNonQueryAsync();
                        }
                        await connection.CloseAsync();
                    });
                    break;
                //079 interact tesla
                case 29:
                    Task.Factory.StartNew(async () =>
                    {
                        var connection = new MySqlConnection(Config.DBConnection);
                        await connection.OpenAsync();
                        using (var cmd = connection.CreateCommand())
                        {
                            cmd.CommandText = "INSERT INTO serverlogs (server_ip, server_port, round_time, time, type, userid, role, data, targets) VALUES (@server_ip, @server_port, @round_time, @time, @type, @userid, @role, @data, @targets)";
                            cmd.Parameters.AddWithValue("@server_ip", server.ServerAddress);
                            cmd.Parameters.AddWithValue("@server_port", (int)server.ServerPort);
                            cmd.Parameters.AddWithValue("@round_time", time);
                            cmd.Parameters.AddWithValue("@time", DateTime.Now);
                            cmd.Parameters.AddWithValue("@type", (int)b);
                            cmd.Parameters.AddWithValue("@userid", reader.GetString());
                            cmd.Parameters.AddWithValue("@role", reader.GetInt());
                            cmd.Parameters.AddWithValue("@data", "{}");
                            cmd.Parameters.AddWithValue("@targets", "[]");
                            await cmd.ExecuteNonQueryAsync();
                        }
                        await connection.CloseAsync();
                    });
                    break;
                //grenade throw
                case 30:
                    Task.Factory.StartNew(async () =>
                    {
                        var connection = new MySqlConnection(Config.DBConnection);
                        await connection.OpenAsync();
                        using (var cmd = connection.CreateCommand())
                        {
                            cmd.CommandText = "INSERT INTO serverlogs (server_ip, server_port, round_time, time, type, userid, role, data, targets) VALUES (@server_ip, @server_port, @round_time, @time, @type, @userid, @role, @data, @targets)";
                            cmd.Parameters.AddWithValue("@server_ip", server.ServerAddress);
                            cmd.Parameters.AddWithValue("@server_port", (int)server.ServerPort);
                            cmd.Parameters.AddWithValue("@round_time", time);
                            cmd.Parameters.AddWithValue("@time", DateTime.Now);
                            cmd.Parameters.AddWithValue("@type", (int)b);
                            cmd.Parameters.AddWithValue("@userid", reader.GetString());
                            cmd.Parameters.AddWithValue("@role", reader.GetInt());
                            cmd.Parameters.AddWithValue("@data", JsonConvert.SerializeObject(new Dictionary<string, object>()
                            {
                                { "Type", reader.GetInt() }
                            }));
                            cmd.Parameters.AddWithValue("@targets", "[]");
                            await cmd.ExecuteNonQueryAsync();
                        }
                        await connection.CloseAsync();
                    });
                    break;
                //medial item used
                case 31:
                    Task.Factory.StartNew(async () =>
                    {
                        var connection = new MySqlConnection(Config.DBConnection);
                        await connection.OpenAsync();
                        using (var cmd = connection.CreateCommand())
                        {
                            cmd.CommandText = "INSERT INTO serverlogs (server_ip, server_port, round_time, time, type, userid, role, data, targets) VALUES (@server_ip, @server_port, @round_time, @time, @type, @userid, @role, @data, @targets)";
                            cmd.Parameters.AddWithValue("@server_ip", server.ServerAddress);
                            cmd.Parameters.AddWithValue("@server_port", (int)server.ServerPort);
                            cmd.Parameters.AddWithValue("@round_time", time);
                            cmd.Parameters.AddWithValue("@time", DateTime.Now);
                            cmd.Parameters.AddWithValue("@type", (int)b);
                            cmd.Parameters.AddWithValue("@userid", reader.GetString());
                            cmd.Parameters.AddWithValue("@role", reader.GetInt());
                            cmd.Parameters.AddWithValue("@data", JsonConvert.SerializeObject(new Dictionary<string, object>()
                            {
                                { "Item", reader.GetInt() }
                            }));
                            cmd.Parameters.AddWithValue("@targets", "[]");
                            await cmd.ExecuteNonQueryAsync();
                        }
                        await connection.CloseAsync();
                    });
                    break;
                //changing role
                case 32:
                    Task.Factory.StartNew(async () =>
                    {
                        var connection = new MySqlConnection(Config.DBConnection);
                        await connection.OpenAsync();
                        using (var cmd = connection.CreateCommand())
                        {
                            cmd.CommandText = "INSERT INTO serverlogs (server_ip, server_port, round_time, time, type, userid, role, data, targets) VALUES (@server_ip, @server_port, @round_time, @time, @type, @userid, @role, @data, @targets)";
                            cmd.Parameters.AddWithValue("@server_ip", server.ServerAddress);
                            cmd.Parameters.AddWithValue("@server_port", (int)server.ServerPort);
                            cmd.Parameters.AddWithValue("@round_time", time);
                            cmd.Parameters.AddWithValue("@time", DateTime.Now);
                            cmd.Parameters.AddWithValue("@type", (int)b);
                            cmd.Parameters.AddWithValue("@userid", reader.GetString());
                            cmd.Parameters.AddWithValue("@role", reader.GetInt());
                            cmd.Parameters.AddWithValue("@data", JsonConvert.SerializeObject(new Dictionary<string, object>()
                            {
                                { "NewRole", reader.GetInt() }
                            }));
                            cmd.Parameters.AddWithValue("@targets", "[]");
                            await cmd.ExecuteNonQueryAsync();
                        }
                        await connection.CloseAsync();
                    });
                    break;
                //remove handcuff
                case 33:
                    Task.Factory.StartNew(async () =>
                    {
                        var connection = new MySqlConnection(Config.DBConnection);
                        await connection.OpenAsync();
                        using (var cmd = connection.CreateCommand())
                        {
                            cmd.CommandText = "INSERT INTO serverlogs (server_ip, server_port, round_time, time, type, userid, role, data, targets) VALUES (@server_ip, @server_port, @round_time, @time, @type, @userid, @role, @data, @targets)";
                            cmd.Parameters.AddWithValue("@server_ip", server.ServerAddress);
                            cmd.Parameters.AddWithValue("@server_port", (int)server.ServerPort);
                            cmd.Parameters.AddWithValue("@round_time", time);
                            cmd.Parameters.AddWithValue("@time", DateTime.Now);
                            cmd.Parameters.AddWithValue("@type", (int)b);
                            cmd.Parameters.AddWithValue("@userid", reader.GetString());
                            cmd.Parameters.AddWithValue("@role", reader.GetInt());
                            cmd.Parameters.AddWithValue("@data", JsonConvert.SerializeObject(new Dictionary<string, object>()
                            {
                                { "TargetID", reader.GetString() },
                                { "TargetClass", reader.GetInt() }
                            }));
                            cmd.Parameters.AddWithValue("@targets", "[]");
                            await cmd.ExecuteNonQueryAsync();
                        }
                        await connection.CloseAsync();
                    });
                    break;
                //handcuff
                case 34:
                    Task.Factory.StartNew(async () =>
                    {
                        var connection = new MySqlConnection(Config.DBConnection);
                        await connection.OpenAsync();
                        using (var cmd = connection.CreateCommand())
                        {
                            cmd.CommandText = "INSERT INTO serverlogs (server_ip, server_port, round_time, time, type, userid, role, data, targets) VALUES (@server_ip, @server_port, @round_time, @time, @type, @userid, @role, @data, @targets)";
                            cmd.Parameters.AddWithValue("@server_ip", server.ServerAddress);
                            cmd.Parameters.AddWithValue("@server_port", (int)server.ServerPort);
                            cmd.Parameters.AddWithValue("@round_time", time);
                            cmd.Parameters.AddWithValue("@time", DateTime.Now);
                            cmd.Parameters.AddWithValue("@type", (int)b);
                            cmd.Parameters.AddWithValue("@userid", reader.GetString());
                            cmd.Parameters.AddWithValue("@role", reader.GetInt());
                            cmd.Parameters.AddWithValue("@data", JsonConvert.SerializeObject(new Dictionary<string, object>()
                            {
                                { "TargetID", reader.GetString() },
                                { "TargetClass", reader.GetInt() }
                            }));
                            cmd.Parameters.AddWithValue("@targets", "[]");
                            await cmd.ExecuteNonQueryAsync();
                        }
                        await connection.CloseAsync();
                    });
                    break;
                //kicked
                case 35:
                    Task.Factory.StartNew(async () =>
                    {
                        var connection = new MySqlConnection(Config.DBConnection);
                        await connection.OpenAsync();
                        using (var cmd = connection.CreateCommand())
                        {
                            cmd.CommandText = "INSERT INTO serverlogs (server_ip, server_port, round_time, time, type, userid, role, data, targets) VALUES (@server_ip, @server_port, @round_time, @time, @type, @userid, @role, @data, @targets)";
                            cmd.Parameters.AddWithValue("@server_ip", server.ServerAddress);
                            cmd.Parameters.AddWithValue("@server_port", (int)server.ServerPort);
                            cmd.Parameters.AddWithValue("@round_time", time);
                            cmd.Parameters.AddWithValue("@time", DateTime.Now);
                            cmd.Parameters.AddWithValue("@type", (int)b);
                            cmd.Parameters.AddWithValue("@userid", reader.GetString());
                            cmd.Parameters.AddWithValue("@role", reader.GetInt());
                            cmd.Parameters.AddWithValue("@data", JsonConvert.SerializeObject(new Dictionary<string, object>()
                            {
                                { "Reason", reader.GetString() }
                            }));
                            cmd.Parameters.AddWithValue("@targets", "[]");
                            await cmd.ExecuteNonQueryAsync();
                        }
                        await connection.CloseAsync();
                    });
                    break;
                //banned
                case 36:
                    Task.Factory.StartNew(async () =>
                    {
                        var connection = new MySqlConnection(Config.DBConnection);
                        await connection.OpenAsync();
                        using (var cmd = connection.CreateCommand())
                        {
                            cmd.CommandText = "INSERT INTO serverlogs (server_ip, server_port, round_time, time, type, userid, role, data, targets) VALUES (@server_ip, @server_port, @round_time, @time, @type, @userid, @role, @data, @targets)";
                            cmd.Parameters.AddWithValue("@server_ip", server.ServerAddress);
                            cmd.Parameters.AddWithValue("@server_port", (int)server.ServerPort);
                            cmd.Parameters.AddWithValue("@round_time", time);
                            cmd.Parameters.AddWithValue("@time", DateTime.Now);
                            cmd.Parameters.AddWithValue("@type", (int)b);
                            cmd.Parameters.AddWithValue("@userid", reader.GetString());
                            cmd.Parameters.AddWithValue("@role", -1);
                            cmd.Parameters.AddWithValue("@data", JsonConvert.SerializeObject(new Dictionary<string, object>()
                            {
                                { "IssuerID", reader.GetString() },
                                { "Reason", reader.GetString() },
                                { "Expires", reader.GetLong() }
                            }));
                            cmd.Parameters.AddWithValue("@targets", "[]");
                            await cmd.ExecuteNonQueryAsync();
                        }
                        await connection.CloseAsync();
                    });
                    break;
                //intercom speak
                case 37:
                    Task.Factory.StartNew(async () =>
                    {
                        var connection = new MySqlConnection(Config.DBConnection);
                        await connection.OpenAsync();
                        using (var cmd = connection.CreateCommand())
                        {
                            cmd.CommandText = "INSERT INTO serverlogs (server_ip, server_port, round_time, time, type, userid, role, data, targets) VALUES (@server_ip, @server_port, @round_time, @time, @type, @userid, @role, @data, @targets)";
                            cmd.Parameters.AddWithValue("@server_ip", server.ServerAddress);
                            cmd.Parameters.AddWithValue("@server_port", (int)server.ServerPort);
                            cmd.Parameters.AddWithValue("@round_time", time);
                            cmd.Parameters.AddWithValue("@time", DateTime.Now);
                            cmd.Parameters.AddWithValue("@type", (int)b);
                            cmd.Parameters.AddWithValue("@userid", reader.GetString());
                            cmd.Parameters.AddWithValue("@role", reader.GetInt());
                            cmd.Parameters.AddWithValue("@data", "{}");
                            cmd.Parameters.AddWithValue("@targets", "[]");
                            await cmd.ExecuteNonQueryAsync();
                        }
                        await connection.CloseAsync();
                    });
                    break;
                //pickup item
                case 38:
                    Task.Factory.StartNew(async () =>
                    {
                        var connection = new MySqlConnection(Config.DBConnection);
                        await connection.OpenAsync();
                        using (var cmd = connection.CreateCommand())
                        {
                            cmd.CommandText = "INSERT INTO serverlogs (server_ip, server_port, round_time, time, type, userid, role, data, targets) VALUES (@server_ip, @server_port, @round_time, @time, @type, @userid, @role, @data, @targets)";
                            cmd.Parameters.AddWithValue("@server_ip", server.ServerAddress);
                            cmd.Parameters.AddWithValue("@server_port", (int)server.ServerPort);
                            cmd.Parameters.AddWithValue("@round_time", time);
                            cmd.Parameters.AddWithValue("@time", DateTime.Now);
                            cmd.Parameters.AddWithValue("@type", (int)b);
                            cmd.Parameters.AddWithValue("@userid", reader.GetString());
                            cmd.Parameters.AddWithValue("@role", reader.GetInt());
                            cmd.Parameters.AddWithValue("@data", JsonConvert.SerializeObject(new Dictionary<string, object>()
                            {
                                { "Item", reader.GetInt() }
                            }));
                            cmd.Parameters.AddWithValue("@targets", "[]");
                            await cmd.ExecuteNonQueryAsync();
                        }
                        await connection.CloseAsync();
                    });
                    break;
                //drop item
                case 39:
                    Task.Factory.StartNew(async () =>
                    {
                        var connection = new MySqlConnection(Config.DBConnection);
                        await connection.OpenAsync();
                        using (var cmd = connection.CreateCommand())
                        {
                            cmd.CommandText = "INSERT INTO serverlogs (server_ip, server_port, round_time, time, type, userid, role, data, targets) VALUES (@server_ip, @server_port, @round_time, @time, @type, @userid, @role, @data, @targets)";
                            cmd.Parameters.AddWithValue("@server_ip", server.ServerAddress);
                            cmd.Parameters.AddWithValue("@server_port", (int)server.ServerPort);
                            cmd.Parameters.AddWithValue("@round_time", time);
                            cmd.Parameters.AddWithValue("@time", DateTime.Now);
                            cmd.Parameters.AddWithValue("@type", (int)b);
                            cmd.Parameters.AddWithValue("@userid", reader.GetString());
                            cmd.Parameters.AddWithValue("@role", reader.GetInt());
                            cmd.Parameters.AddWithValue("@data", JsonConvert.SerializeObject(new Dictionary<string, object>()
                            {
                                { "Item", reader.GetInt() }
                            }));
                            cmd.Parameters.AddWithValue("@targets", "[]");
                            await cmd.ExecuteNonQueryAsync();
                        }
                        await connection.CloseAsync();
                    });
                    break;
                //change group
                case 40:
                    Task.Factory.StartNew(async () =>
                    {
                        var connection = new MySqlConnection(Config.DBConnection);
                        await connection.OpenAsync();
                        using (var cmd = connection.CreateCommand())
                        {
                            cmd.CommandText = "INSERT INTO serverlogs (server_ip, server_port, round_time, time, type, userid, role, data, targets) VALUES (@server_ip, @server_port, @round_time, @time, @type, @userid, @role, @data, @targets)";
                            cmd.Parameters.AddWithValue("@server_ip", server.ServerAddress);
                            cmd.Parameters.AddWithValue("@server_port", (int)server.ServerPort);
                            cmd.Parameters.AddWithValue("@round_time", time);
                            cmd.Parameters.AddWithValue("@time", DateTime.Now);
                            cmd.Parameters.AddWithValue("@type", (int)b);
                            cmd.Parameters.AddWithValue("@userid", reader.GetString());
                            cmd.Parameters.AddWithValue("@role", reader.GetInt());
                            cmd.Parameters.AddWithValue("@data", JsonConvert.SerializeObject(new Dictionary<string, object>()
                            {
                                { "GroupText", reader.GetString() },
                                { "GroupColor", reader.GetString() }
                            }));
                            cmd.Parameters.AddWithValue("@targets", "[]");
                            await cmd.ExecuteNonQueryAsync();
                        }
                        await connection.CloseAsync();
                    });
                    break;
                //RA CMD
                case 41:
                    Task.Factory.StartNew(async () =>
                    {
                        var connection = new MySqlConnection(Config.DBConnection);
                        await connection.OpenAsync();
                        using (var cmd = connection.CreateCommand())
                        {
                            cmd.CommandText = "INSERT INTO serverlogs (server_ip, server_port, round_time, time, type, userid, role, data, targets) VALUES (@server_ip, @server_port, @round_time, @time, @type, @userid, @role, @data, @targets)";
                            cmd.Parameters.AddWithValue("@server_ip", server.ServerAddress);
                            cmd.Parameters.AddWithValue("@server_port", (int)server.ServerPort);
                            cmd.Parameters.AddWithValue("@round_time", time);
                            cmd.Parameters.AddWithValue("@time", DateTime.Now);
                            cmd.Parameters.AddWithValue("@type", (int)b);
                            cmd.Parameters.AddWithValue("@userid", reader.GetString());
                            cmd.Parameters.AddWithValue("@role", reader.GetInt());
                            cmd.Parameters.AddWithValue("@data", JsonConvert.SerializeObject(new Dictionary<string, object>()
                            {
                                { "CommandName", reader.GetString() },
                                { "Args", reader.GetStringArray() }
                            }));
                            cmd.Parameters.AddWithValue("@targets", "[]");
                            await cmd.ExecuteNonQueryAsync();
                        }
                        await connection.CloseAsync();
                    });
                    break;
                //CONSOLE CMD
                case 42:
                    Task.Factory.StartNew(async () =>
                    {
                        var connection = new MySqlConnection(Config.DBConnection);
                        await connection.OpenAsync();
                        using (var cmd = connection.CreateCommand())
                        {
                            cmd.CommandText = "INSERT INTO serverlogs (server_ip, server_port, round_time, time, type, userid, role, data, targets) VALUES (@server_ip, @server_port, @round_time, @time, @type, @userid, @role, @data, @targets)";
                            cmd.Parameters.AddWithValue("@server_ip", server.ServerAddress);
                            cmd.Parameters.AddWithValue("@server_port", (int)server.ServerPort);
                            cmd.Parameters.AddWithValue("@round_time", time);
                            cmd.Parameters.AddWithValue("@time", DateTime.Now);
                            cmd.Parameters.AddWithValue("@type", (int)b);
                            cmd.Parameters.AddWithValue("@userid", reader.GetString());
                            cmd.Parameters.AddWithValue("@role", reader.GetInt());
                            cmd.Parameters.AddWithValue("@data", JsonConvert.SerializeObject(new Dictionary<string, object>()
                            {
                                { "CommandName", reader.GetString() },
                                { "Args", reader.GetStringArray() }
                            }));
                            cmd.Parameters.AddWithValue("@targets", "[]");
                            await cmd.ExecuteNonQueryAsync();
                        }
                        await connection.CloseAsync();
                    });
                    break;
                //Report cheater
                case 43:
                    Task.Factory.StartNew(async () =>
                    {
                        var connection = new MySqlConnection(Config.DBConnection);
                        await connection.OpenAsync();
                        using (var cmd = connection.CreateCommand())
                        {
                            cmd.CommandText = "INSERT INTO serverlogs (server_ip, server_port, round_time, time, type, userid, role, data, targets) VALUES (@server_ip, @server_port, @round_time, @time, @type, @userid, @role, @data, @targets)";
                            cmd.Parameters.AddWithValue("@server_ip", server.ServerAddress);
                            cmd.Parameters.AddWithValue("@server_port", (int)server.ServerPort);
                            cmd.Parameters.AddWithValue("@round_time", time);
                            cmd.Parameters.AddWithValue("@time", DateTime.Now);
                            cmd.Parameters.AddWithValue("@type", (int)b);
                            cmd.Parameters.AddWithValue("@userid", reader.GetString());
                            cmd.Parameters.AddWithValue("@role", reader.GetInt());
                            cmd.Parameters.AddWithValue("@data", JsonConvert.SerializeObject(new Dictionary<string, object>()
                            {
                                { "ReportedID", reader.GetString() },
                                { "ReportedClass", reader.GetInt() },
                                { "Reason", reader.GetString() }
                            }));
                            cmd.Parameters.AddWithValue("@targets", "[]");
                            await cmd.ExecuteNonQueryAsync();
                        }
                        await connection.CloseAsync();
                    });
                    break;
                //Report local
                case 44:
                    Task.Factory.StartNew(async () =>
                    {
                        var connection = new MySqlConnection(Config.DBConnection);
                        await connection.OpenAsync();
                        using (var cmd = connection.CreateCommand())
                        {
                            cmd.CommandText = "INSERT INTO serverlogs (server_ip, server_port, round_time, time, type, userid, role, data, targets) VALUES (@server_ip, @server_port, @round_time, @time, @type, @userid, @role, @data, @targets)";
                            cmd.Parameters.AddWithValue("@server_ip", server.ServerAddress);
                            cmd.Parameters.AddWithValue("@server_port", (int)server.ServerPort);
                            cmd.Parameters.AddWithValue("@round_time", time);
                            cmd.Parameters.AddWithValue("@time", DateTime.Now);
                            cmd.Parameters.AddWithValue("@type", (int)b);
                            cmd.Parameters.AddWithValue("@userid", reader.GetString());
                            cmd.Parameters.AddWithValue("@role", reader.GetInt());
                            cmd.Parameters.AddWithValue("@data", JsonConvert.SerializeObject(new Dictionary<string, object>()
                            {
                                { "ReportedID", reader.GetString() },
                                { "ReportedClass", reader.GetInt() },
                                { "Reason", reader.GetString() }
                            }));
                            cmd.Parameters.AddWithValue("@targets", "[]");
                            await cmd.ExecuteNonQueryAsync();
                        }
                        await connection.CloseAsync();
                    });
                    break;
                //Wait for players
                case 45:
                    Task.Factory.StartNew(async () =>
                    {
                        var connection = new MySqlConnection(Config.DBConnection);
                        await connection.OpenAsync();
                        using (var cmd = connection.CreateCommand())
                        {
                            cmd.CommandText = "INSERT INTO serverlogs (server_ip, server_port, round_time, time, type, userid, role, data, targets) VALUES (@server_ip, @server_port, @round_time, @time, @type, @userid, @role, @data, @targets)";
                            cmd.Parameters.AddWithValue("@server_ip", server.ServerAddress);
                            cmd.Parameters.AddWithValue("@server_port", (int)server.ServerPort);
                            cmd.Parameters.AddWithValue("@round_time", time);
                            cmd.Parameters.AddWithValue("@time", DateTime.Now);
                            cmd.Parameters.AddWithValue("@type", (int)b);
                            cmd.Parameters.AddWithValue("@userid", "");
                            cmd.Parameters.AddWithValue("@role", -1);
                            cmd.Parameters.AddWithValue("@data", "{}");
                            cmd.Parameters.AddWithValue("@targets", "[]");
                            await cmd.ExecuteNonQueryAsync();
                        }
                        await connection.CloseAsync();
                    });
                    break;
                //round start
                case 46:
                    Task.Factory.StartNew(async () =>
                    {
                        var connection = new MySqlConnection(Config.DBConnection);
                        await connection.OpenAsync();
                        using (var cmd = connection.CreateCommand())
                        {
                            cmd.CommandText = "INSERT INTO serverlogs (server_ip, server_port, round_time, time, type, userid, role, data, targets) VALUES (@server_ip, @server_port, @round_time, @time, @type, @userid, @role, @data, @targets)";
                            cmd.Parameters.AddWithValue("@server_ip", server.ServerAddress);
                            cmd.Parameters.AddWithValue("@server_port", (int)server.ServerPort);
                            cmd.Parameters.AddWithValue("@round_time", time);
                            cmd.Parameters.AddWithValue("@time", DateTime.Now);
                            cmd.Parameters.AddWithValue("@type", (int)b);
                            cmd.Parameters.AddWithValue("@userid", "");
                            cmd.Parameters.AddWithValue("@role", -1);
                            cmd.Parameters.AddWithValue("@data", "{}");
                            cmd.Parameters.AddWithValue("@targets", "[]");
                            await cmd.ExecuteNonQueryAsync();
                        }
                        await connection.CloseAsync();
                    });
                    break;
                //round end
                case 47:
                    Task.Factory.StartNew(async () =>
                    {
                        var connection = new MySqlConnection(Config.DBConnection);
                        await connection.OpenAsync();
                        using (var cmd = connection.CreateCommand())
                        {
                            cmd.CommandText = "INSERT INTO serverlogs (server_ip, server_port, round_time, time, type, userid, role, data, targets) VALUES (@server_ip, @server_port, @round_time, @time, @type, @userid, @role, @data, @targets)";
                            cmd.Parameters.AddWithValue("@server_ip", server.ServerAddress);
                            cmd.Parameters.AddWithValue("@server_port", (int)server.ServerPort);
                            cmd.Parameters.AddWithValue("@round_time", time);
                            cmd.Parameters.AddWithValue("@time", DateTime.Now);
                            cmd.Parameters.AddWithValue("@type", (int)b);
                            cmd.Parameters.AddWithValue("@userid", "");
                            cmd.Parameters.AddWithValue("@role", -1);
                            cmd.Parameters.AddWithValue("@data", JsonConvert.SerializeObject(new Dictionary<string, object>()
                            {
                                { "LeadingTeam", reader.GetInt() },
                                { "PlayersCount", reader.GetInt() }
                            }));
                            cmd.Parameters.AddWithValue("@targets", "[]");
                            await cmd.ExecuteNonQueryAsync();
                        }
                        await connection.CloseAsync();
                    });
                    break;
                //respawn team
                case 48:
                    Task.Factory.StartNew(async () =>
                    {
                        var connection = new MySqlConnection(Config.DBConnection);
                        await connection.OpenAsync();
                        using (var cmd = connection.CreateCommand())
                        {
                            cmd.CommandText = "INSERT INTO serverlogs (server_ip, server_port, round_time, time, type, userid, role, data, targets) VALUES (@server_ip, @server_port, @round_time, @time, @type, @userid, @role, @data, @targets)";
                            cmd.Parameters.AddWithValue("@server_ip", server.ServerAddress);
                            cmd.Parameters.AddWithValue("@server_port", (int)server.ServerPort);
                            cmd.Parameters.AddWithValue("@round_time", time);
                            cmd.Parameters.AddWithValue("@time", DateTime.Now);
                            cmd.Parameters.AddWithValue("@type", (int)b);
                            cmd.Parameters.AddWithValue("@userid", "");
                            cmd.Parameters.AddWithValue("@role", -1);
                            cmd.Parameters.AddWithValue("@data", JsonConvert.SerializeObject(new Dictionary<string, object>()
                            {
                                { "KnownTeam", reader.GetInt() },
                                { "PlayersCount", reader.GetInt() }
                            }));
                            cmd.Parameters.AddWithValue("@targets", "[]");
                            await cmd.ExecuteNonQueryAsync();
                        }
                        await connection.CloseAsync();
                    });
                    break;
            }
        }
    }
}
