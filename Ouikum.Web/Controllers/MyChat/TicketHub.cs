using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using B2B.Web;
using B2B.Common;
using Prosoft.Service;
using B2B;
using Prosoft.Base;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
namespace B2B.Web
{   
    public class TicketHub : Hub
    {
        static int TotalTickets = 10;

        public void GetTicketCount()
        {
            Clients.All.updateTicketCount(TotalTickets);
        }

        public void BuyTicket()
        {
            if (TotalTickets > 0)
                TotalTickets -= 1;
            Clients.All.updateTicketCount(TotalTickets);
        }

        public void AddTicket()
        {
            if (TotalTickets > 0)
                TotalTickets++;
            Clients.All.updateTicketCount(TotalTickets);
        }
    }
}