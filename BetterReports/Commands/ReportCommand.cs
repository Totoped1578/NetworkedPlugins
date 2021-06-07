using NetworkedPlugins.API.Attributes;
using NetworkedPlugins.API.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterReports.Commands
{
    [NPCommand]
    public class ReportCommand : ICommand
    {
        public string CommandName { get; } = "REPORT";

        public string Description { get; } = "Command for management of server reports.";

        public string Permission { get; } = "report";

        public bool IsRaCommand { get; } = true;

        public void Invoke(PlayerFuncs player, List<string> arguments)
        {
            if (arguments.Count == 0)
            {
                player.SendRAMessage(string.Concat(" Commands: ",
                    Environment.NewLine,
                    " - REPORT deny <id> <response> - Decline report.",
                    Environment.NewLine,
                    " - REPORT accept <id> <response> - Accept report.",
                    Environment.NewLine,
                    " - REPORT list - List of avaliable reports.",
                    Environment.NewLine,
                    " - REPORT goto <id> - Change current server to issuers server of report."));
            }
            else
            {
                switch (arguments[0].ToUpper())
                {
                    case "LIST":
                        var col = BetterReportsDedicated.db.GetCollection<ReportModel>("reports");
                        player.SendRAMessage("Avaliable reports: ");
                        foreach(var rep in col.FindAll())
                        {
                            if (rep.Status == (byte)0)
                                player.SendRAMessage($"\n   ID: <color=yellow>{rep.TicketID}</color>\n   Issuer: <color=green>{rep.IssuerNICK}</color>\n   Target: <color=green>{rep.TargetNICK}</color>\n   Reason: <color=green>{rep.Reason}</color>\n   Server: <color=green>{rep.ServerIP}:{rep.ServertPort}</color>");
                        }
                        break;
                    case "GOTO":
                        if (arguments.Count == 2)
                        {
                            if (int.TryParse(arguments[1], out int repID))
                            {
                                var col2 = BetterReportsDedicated.db.GetCollection<ReportModel>("reports");
                                var ticket = col2.FindById(repID);
                                if (ticket != null)
                                {
                                    foreach (var srv in BetterReportsDedicated.singelton.GetServers())
                                    {
                                        var srvPlr = srv.GetPlayer(ticket.TargetID);
                                        if (srvPlr != null)
                                        {
                                            string mem = BetterReportsDedicated.singelton.Config.ticket_admin_onway.Replace("%admin_name%", player.UserName);
                                            srvPlr.SendHint(mem, 5f);
                                            player.SendRAMessage($"Redirecting to server {srv.ServerPort}.");
                                            player.Redirect(srv.ServerPort);
                                        }
                                        break;
                                    }
                                    foreach (var srv in BetterReportsDedicated.singelton.GetServers())
                                    {
                                        var srvPlr = srv.GetPlayer(ticket.IssuerID);
                                        if (srvPlr != null)
                                        {
                                            string mem = BetterReportsDedicated.singelton.Config.ticket_admin_onway.Replace("%admin_name%", player.UserName);
                                            srvPlr.SendHint(mem, 5f);
                                            player.SendRAMessage($"Redirecting to server {srv.ServerPort}.");
                                            player.Redirect(srv.ServerPort);
                                        }
                                        break;
                                    }
                                    player.SendRAMessage($"Issuer or reported player is not playing on any server.");
                                }
                                else
                                {
                                    player.SendRAMessage($"Report not found with id <color=red>{repID}</color>.");
                                }
                            }
                        }
                        break;
                    case "DENY":
                        if (arguments.Count > 2)
                        {
                            if (int.TryParse(arguments[1], out int repID))
                            {
                                string message = string.Join(" ", arguments.Skip(2));
                                var col3 = BetterReportsDedicated.db.GetCollection<ReportModel>("reports");
                                var ticket = col3.FindById(repID);
                                if (ticket != null)
                                {
                                    if (ticket.Status == (byte)0)
                                    {
                                        ticket.Status = (byte)2;
                                        ticket.ClosedbyID = player.UserID;
                                        ticket.ClosedbyNICK = player.UserName;
                                        ticket.ClosedTime = DateTime.Now;
                                        ticket.Response = message;
                                        col3.Update(ticket.TicketID, ticket);
                                        string messageAdmin = BetterReportsDedicated.singelton.Config.ticket_declined.
                                            Replace("%response%", message).
                                            Replace("%id%", ticket.TicketID.ToString()).
                                            Replace("%issuer_id%", player.UserID).
                                            Replace("%issuer_nick%", player.UserName);
                                        string messageClient = BetterReportsDedicated.singelton.Config.ticket_declined_response.
                                            Replace("%response%", message).
                                            Replace("%id%", ticket.TicketID.ToString()).
                                            Replace("%issuer_id%", player.UserID).
                                            Replace("%issuer_nick%", player.UserName);

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
                                        player.SendRAMessage($"Report declined with response {message}.");
                                    }
                                    else
                                    {
                                        player.SendRAMessage($"Report is already {(ticket.Status == 1 ? "ACCEPTED" : "DECLINED")} by {ticket.ClosedbyNICK}.");
                                    }
                                }
                                else
                                {
                                    player.SendRAMessage($"Report not found with id <color=red>{repID}</color>.");
                                }
                            }
                        }
                        break;
                    case "ACCEPT":
                        if (arguments.Count > 2)
                        {
                            if (int.TryParse(arguments[1], out int repID))
                            {
                                string message = string.Join(" ", arguments.Skip(2));
                                var col4 = BetterReportsDedicated.db.GetCollection<ReportModel>("reports");
                                var ticket = col4.FindById(repID);
                                if (ticket != null)
                                {
                                    if (ticket.Status == (byte)0)
                                    {
                                        ticket.Status = (byte)1;
                                        ticket.ClosedbyID = player.UserID;
                                        ticket.ClosedbyNICK = player.UserName;
                                        ticket.ClosedTime = DateTime.Now;
                                        ticket.Response = message;
                                        col4.Update(ticket.TicketID, ticket);
                                        string messageAdmin = BetterReportsDedicated.singelton.Config.ticket_accepted_response.
                                            Replace("%response%", message).
                                            Replace("%id%", ticket.TicketID.ToString()).
                                            Replace("%issuer_id%", player.UserID).
                                            Replace("%issuer_nick%", player.UserName);
                                        string messageClient = BetterReportsDedicated.singelton.Config.ticket_accepted_response.
                                            Replace("%response%", message).
                                            Replace("%id%", ticket.TicketID.ToString()).
                                            Replace("%issuer_id%", player.UserID).
                                            Replace("%issuer_nick%", player.UserName);


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
                                        player.SendRAMessage($"Report accepted with response {message}.");
                                    }
                                    else
                                    {
                                        player.SendRAMessage($"Report is already {(ticket.Status == 1 ? "ACCEPTED" : "DECLINED")} by {ticket.ClosedbyNICK}.");
                                    }
                                }
                                else
                                {
                                    player.SendRAMessage($"Report not found with id <color=red>{repID}</color>.");
                                }
                            }
                        }
                        break;
                }
            }
        }
    }
}
