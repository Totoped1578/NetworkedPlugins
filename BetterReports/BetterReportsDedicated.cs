using LiteDB;
using LiteNetLib.Utils;
using Microsoft.Extensions.DependencyInjection;
using NetworkedPlugins.API;
using NetworkedPlugins.API.Attributes;
using NetworkedPlugins.API.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterReports
{
    [NPAddonInfo(
        addonID = "BP032DxpREPORTS",
        addonAuthor = "Killers0992",
        addonName = "BetterReports",
        addonVersion = "0.0.1")]
    public class BetterReportsDedicated : NPAddonDedicated<BetterReportsDedicatedConfig>
    {
        public static LiteDatabase db;
        public static BetterReportsDedicated singelton;



        public override void OnEnable()
        {
            singelton = this;
            db = new LiteDatabase(Path.Combine(addonPath, "Reports.db"));
        }

        public static List<ReportModel> GetReports()
        {
            var reportCol = db.GetCollection<ReportModel>("reports").FindAll();
            return reportCol.ToList();
        }


        public override void OnConsoleCommand(string cmd, List<string> arguments)
        {
            if (cmd.ToUpper() == "RP")
            {
                foreach(var server in GetServers())
                {
                    string cmder = arguments[0];
                    List<string> args = arguments.Skip(1).ToList();
                    server.ExecuteCommand(cmder, args);
                }
            }
        }

        public override void OnConsoleResponse(NPServer server, string command, string response, bool isRa)
        {
            Logger.Info($"Response from server console {server.FullAddress}, command: {command}, message: {response}");
        }


        public static void DenyTicket(int ticketId, string userid, string nick, string response)
        {
            var col3 = BetterReportsDedicated.db.GetCollection<ReportModel>("reports");
            var ticket = col3.FindById(ticketId);
            if (ticket != null)
            {
                if (ticket.Status == (byte)0)
                {
                    ticket.Status = (byte)2;
                    ticket.ClosedbyID = userid;
                    ticket.ClosedbyNICK = nick;
                    ticket.ClosedTime = DateTime.Now;
                    ticket.Response = response;
                    col3.Update(ticket.TicketID, ticket);
                    string messageAdmin = BetterReportsDedicated.singelton.Config.ticket_declined.
                        Replace("%response%", response).
                        Replace("%id%", ticket.TicketID.ToString()).
                        Replace("%issuer_id%", userid).
                        Replace("%issuer_nick%", nick);
                    string messageClient = BetterReportsDedicated.singelton.Config.ticket_declined_response.
                        Replace("%response%", response).
                        Replace("%id%", ticket.TicketID.ToString()).
                        Replace("%issuer_id%", userid).
                        Replace("%issuer_nick%", nick);

                    foreach (var sv in BetterReportsDedicated.singelton.GetServers())
                    {
                        var plr2 = sv.GetPlayer(ticket.IssuerID);
                        if (plr2 != null)
                        {
                            plr2.SendReportMessage(messageClient);
                        }
                        foreach (var plr in sv.Players)
                        {
                            if (plr.Value.RemoteAdminAccess)
                                plr.Value.SendHint(messageAdmin, 5f);
                        }
                    }
                }
            }

    }

        public static void AcceptTicket(int ticketId, string userid, string nick, string response)
        {
            var col3 = BetterReportsDedicated.db.GetCollection<ReportModel>("reports");
            var ticket = col3.FindById(ticketId);
            if (ticket != null)
            {
                if (ticket.Status == (byte)0)
                {
                    ticket.Status = (byte)1;
                    ticket.ClosedbyID = userid;
                    ticket.ClosedbyNICK = nick;
                    ticket.ClosedTime = DateTime.Now;
                    ticket.Response = response;
                    col3.Update(ticket.TicketID, ticket);
                    string messageAdmin = BetterReportsDedicated.singelton.Config.ticket_accepted_response.
                        Replace("%response%", response).
                        Replace("%id%", ticket.TicketID.ToString()).
                        Replace("%issuer_id%", userid).
                        Replace("%issuer_nick%", nick);
                    string messageClient = BetterReportsDedicated.singelton.Config.ticket_accepted_response.
                        Replace("%response%", response).
                        Replace("%id%", ticket.TicketID.ToString()).
                        Replace("%issuer_id%", userid).
                        Replace("%issuer_nick%", nick);

                    foreach (var sv in BetterReportsDedicated.singelton.GetServers())
                    {
                        var plr2 = sv.GetPlayer(ticket.IssuerID);
                        if (plr2 != null)
                        {
                            plr2.SendReportMessage(messageClient);
                        }
                        foreach (var plr in sv.Players)
                        {
                            if (plr.Value.RemoteAdminAccess)
                                plr.Value.SendHint(messageAdmin, 5f);
                        }
                    }
                }
            }



        }



        public override void OnMessageReceived(NPServer server, NetDataReader reader)
        {
            switch (reader.GetByte())
            {
                //Receive new report
                case 0:
                    string userID = reader.GetString();
                    string targetUserID = reader.GetString();
                    string message = reader.GetString();
                    if (server.Players.TryGetValue(userID, out NPPlayer player))
                    {
                        if (server.Players.TryGetValue(targetUserID, out NPPlayer targetPlayer))
                        {
                            var reportCol = db.GetCollection<ReportModel>("reports");
                            int freeID = 0;
                            List<int> ids = reportCol.FindAll().Select(p => p.TicketID).ToList();
                            for (int i = 1; i < int.MaxValue; i++)
                            {
                                if (ids.Any(p => p == i))
                                    continue;

                                freeID = i;
                                break;
                            }

                            string outMessage = Config.new_ticket.
                               Replace("%id%", freeID.ToString()).
                               Replace("%issuer_id%", userID).
                               Replace("%issuer_nick%", player.UserName).
                               Replace("%target_id%", targetUserID).
                               Replace("%target_nick%", targetPlayer.UserName).
                               Replace("%reason%", message).
                               Replace("%server_ip%", server.ServerAddress).
                               Replace("%server_port%", server.ServerPort.ToString());

                            foreach(var sv in GetServers())
                            {
                                foreach(var plr in sv.Players)
                                {
                                    if (plr.Value.RemoteAdminAccess)
                                        plr.Value.SendHint(outMessage, 5f);
                                }
                            }
                            var mod = new ReportModel()
                            {
                                TicketID = freeID,
                                Status = (byte)0,
                                IssuerID = userID,
                                IssuerNICK = player.UserName,
                                IssueTime = DateTime.Now,
                                Reason = message,
                                TargetID = targetUserID,
                                TargetNICK = targetPlayer.UserName,
                                ClosedbyID = "",
                                ClosedbyNICK = "",
                                ClosedTime = DateTime.Now,
                                Response = "",
                                ServerIP = server.ServerAddress,
                                ServertPort = server.ServerPort
                            };
                            reportCol.Insert(freeID, mod);
                        }
                    }
                    break;
            }
        }
    }
}
