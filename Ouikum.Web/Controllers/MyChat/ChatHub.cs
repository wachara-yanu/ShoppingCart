using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using B2B.Web;
using B2B.Common;
using Prosoft.Service; 
using Prosoft.Base;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hub;
using res = Prosoft.Resource.Web.B2B;

namespace B2B.Web
{

    public class ChatHub : Hub
    {
        ChatService _svChat = new ChatService();

        public void Send(int roomid, int toid, string CompImgLogo, string name, string message, string roomcode)
        {
            if (LogonCompID > 0)
            {
                // Call the broadcastMessage method to update clients.
                Clients.Group(roomcode).Message(CompImgLogo, name, message, roomcode);
                UpdateStatus(roomid, LogonCompID, toid);
                AlertMessage(name, roomcode);
            }
            else
            {
                Clients.Group(roomcode).LogoutMessage(name, roomcode);
            }
        }
        public void CreateGroup(string roomcode)
        {
            Groups.Add(Context.ConnectionId, roomcode);
        }
        public void Open(int toid, string name, string roomcode)
        {
            Clients.Group(roomcode).OpenBrowser(toid, name, roomcode);
        }

        public void Close(int toid, string name, string roomcode)
        {
            Clients.Group(roomcode).CloseBrowser(toid, name, roomcode);
        }
         
        public void GetFriendStatus(int ID)
        {
            Clients.All.getFriendStatus(ID);
        }

        public void UpdateStatus(int RoomID, int FromID, int ToID)
        {
            //ปรับปรุง
            Clients.All.getStatusFriendByID(FromID, ToID);
            Clients.All.getStatusUnReadFriendByID(ToID, FromID);
        }
        public void AlertMessage(string compName, string roomcode)
        {
            Clients.Group(roomcode).AlertMessage(LogonCompID, compName);
        }

        #region LogonCompID
        protected int LogonCompID
        {
            get
            {
                if (System.Web.HttpContext.Current.Request.Cookies.AllKeys.Contains(res.Common.lblWebsite))
                {
                    return DataManager.ConvertToInteger(System.Web.HttpContext.Current.Request.Cookies[res.Common.lblWebsite].Values["CompID"]);
                }
                else
                {
                    return 0;
                }
            }
        }
        #endregion
    }
}