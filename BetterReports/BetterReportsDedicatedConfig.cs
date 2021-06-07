using NetworkedPlugins.API.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterReports
{
    public class BetterReportsDedicatedConfig : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public string BotToken { get; set; } = "";
        public string ticket_admin_onway { get; set; } = "[TicketSystem]\nAdministrator %admin_name% is connecting to server to investagiate.";
        public string tickets_message { get; set; } = "Tickets:";
        public string tickets_object { get; set; } = "ID: %id% | Time: %datetime% | Issuer: %issuer_nick% | Target: %target_nick% | Reason: %reason%";
        public string no_permission { get; set; } = "No permission.";
        public string new_ticket { get; set; } = "[TicketSystem]\nNew ticket %id%\nIssuer: %issuer_nick%\nTarget: %target_nick%\nReason: %reason%";
        public string ticket_accepted { get; set; } = "[TicketSystem]\nTicket %id% accepted by %issuer_nick%, Response:\n%response%";
        public string ticket_declined { get; set; } = "[TicketSystem]\nTicket %id% declined by %issuer_nick%, Response:\n%response%";
        public string ticket_accepted_response { get; set; } = "Ticket %id% accepted by %issuer_nick%, Response:\n%response%";
        public string ticket_declined_response { get; set; } = "Ticket %id% declined by %issuer_nick%, Response:\n%response%";
        public BotConfig bot { get; set; } = new BotConfig();
    }

    public class BotConfig
    {
        public ulong guild_id { get; set; } = 1;
        public ulong ticketslist_channel_id { get; set; } = 1;
        public ulong ticketslist_channel_message_id { get; set; } = 1;
        public ulong ticketsinfo_channel_id { get; set; } = 1;
        public Dictionary<string, JMessage> messages { get; set; } = new Dictionary<string, JMessage>()
        {
            { "newticket", new JMessage()
            {
                showtimestamp = true,
                content = "",
                color = "orange",
                title = "New ticket",
                field_title = "Ticket ID #%id%",
                fields = "Server: %server_name%\nIssuer: %issuer_nick% (%issuer_id%)\nTarget: %target_nick% (%target_id%)\nReason: %reason%"
            } },
            { "acceptticket", new JMessage()
            {
                showtimestamp = true,
                content = "",
                color = "green",
                title = "Ticket accepted",
                field_title = "Ticket ID #%id%",
                fields = "Server: %server_name%\nAccepted by: %issuer_nick% (%issuer_id%)\nResponse: %response%"
            } },
            { "denyticket", new JMessage()
            {
                showtimestamp = true,
                content = "",
                color = "red",
                title = "Ticket declined",
                field_title = "Ticket ID #%id%",
                fields = "Server: %server_name%\nDeclined by: %issuer_nick% (%issuer_id%)\nResponse: %response%"
            } }
        };
    }

    public class JMessage
    {
        public bool showtimestamp { get; set; } = true;
        public string content { get; set; } = "";
        public string color { get; set; } = "default";
        public string title { get; set; } = "DefaultTitle";
        public string field_title { get; set; } = "FieldTitle";
        public string fields { get; set; } = "Fields";
    }
}
