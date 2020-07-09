using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using B2B.Common;
using Prosoft.Service;
using B2B;
using Prosoft.Base;
using res = Prosoft.Resource.Web.B2B;

namespace B2B.Web.MyChat
{
    public class MyChatController : BaseController
    {
        //
        // GET: /MyChat/ 
        //public ActionResult Index()
        //{
        //    ChatService svChat = new ChatService();
        //    var logOn = svChat.SelectData<view_logOn>("*", "CompID != "+LogonCompID+" AND IsOnline = 1","CompID ASC");
        //    ViewBag.LogOn = logOn;
        //    ViewBag.CompID = LogonCompID;
        //    GetStatusUser();
        //    return View();
        //}

        public ActionResult Chatter(int ToID = 0, string date = "topten")
        {
            if (RedirectToProduction())
                return Redirect(UrlProduction);

            if(!CheckIsLogin()){
                RememberURL();
                return Redirect(res.Pageviews.PvMemberSignIn);
            }else{
                GetStatusUser();
                if (ToID != 0 && ToID != LogonCompID)
                {
                    ChatService svChat = new ChatService();
                    ViewBag.fromCompID = LogonCompID;
                    ViewBag.toCompID = ToID;
                    ViewBag.CompName = LogonCompName;
                    ViewBag.CompImgLogo = LogonLogoImgPath;
                    var toComp = svChat.SelectData<view_CompMember>("CompID,CompName,Email,Phone", "IsDelete = 0 AND CompID = " + ToID);

                    if (svChat.TotalRow > 0)
                    {
                        #region Check User Room
                        ViewBag.toComp = toComp.First();
                        if (svChat.CheckExistUserInRoomAndCreateIfNot(LogonCompID, ToID))
                        {

                            if (date == "today")
                            {
                                var chatMsg = svChat.GetMessage(svChat._RoomCode, ChatService.Period.ToDay);
                                ViewBag.ChatMsg = chatMsg;
                            }
                            else if (date == "yesterday")
                            {
                                var chatMsg = svChat.GetMessage(svChat._RoomCode, ChatService.Period.Yesterday);
                                ViewBag.ChatMsg = chatMsg;
                            }
                            else if (date == "week")
                            {
                                var chatMsg = svChat.GetMessage(svChat._RoomCode, ChatService.Period.This_week);
                                ViewBag.ChatMsg = chatMsg;
                            }
                            else if (date == "month")
                            {
                                var chatMsg = svChat.GetMessage(svChat._RoomCode, ChatService.Period.This_month);
                                ViewBag.ChatMsg = chatMsg;
                            }
                            else if (date == "all")
                            {
                                var chatMsg = svChat.GetMessage(svChat._RoomCode, ChatService.Period.All);
                                ViewBag.ChatMsg = chatMsg;
                            }
                            else
                            {
                                var chatMsg = svChat.GetMessage(svChat._RoomCode, ChatService.Period.Topten);
                                ViewBag.ChatMsg = chatMsg;
                            }

                        }
                        #endregion

                        ViewBag.RoomCode = svChat._RoomCode;
                        ViewBag.RoomID = svChat._RoomID;
                    }
                    else
                    {
                         Redirect("~/MyChat/NotFoundUser");
                    }

                }
                else
                {
                    return Redirect("~/MyChat/NotFoundUser");
                }
            }
            return View();
        }


        public ActionResult NotFoundUser()
        {

            return View();
        }

        [HttpPost]
        public ActionResult HistoryChat(string Room,string date)
        {
            ChatService svChat = new ChatService();
            if (date == "today")
            {
                var chatMsg = svChat.GetMessage(Room, ChatService.Period.ToDay);
                if (chatMsg != null)
                {
                    ViewBag.ChatMsg = chatMsg.OrderBy(m => m.ChatID);
                }
                else
                {
                    ViewBag.ChatMsg = null;
                }
            }
            else if (date == "yesterday")
            {
                var chatMsg = svChat.GetMessage(Room, ChatService.Period.Yesterday);
                if (chatMsg != null)
                {
                    ViewBag.ChatMsg = chatMsg.OrderBy(m => m.ChatID);
                }
                else
                {
                    ViewBag.ChatMsg = null;
                }
            }
            else if (date == "week")
            {
                var chatMsg = svChat.GetMessage(Room, ChatService.Period.last_7_days);
                if (chatMsg != null)
                {
                    ViewBag.ChatMsg = chatMsg.OrderBy(m => m.ChatID);
                }
                else
                {
                    ViewBag.ChatMsg = null;
                }
            }
            else if (date == "month")
            {
                var chatMsg = svChat.GetMessage(Room, ChatService.Period.Last_30_Days);
                if (chatMsg != null)
                {
                    ViewBag.ChatMsg = chatMsg.OrderBy(m => m.ChatID);
                }
                else
                {
                    ViewBag.ChatMsg = null;
                }
            }
            else if (date == "all")
            {
                var chatMsg = svChat.GetMessage(Room, ChatService.Period.All);
                if (chatMsg != null)
                {
                    ViewBag.ChatMsg = chatMsg.OrderBy(m => m.ChatID);
                }
                else
                {
                    ViewBag.ChatMsg = null;
                }
            }
            else
            {
                var chatMsg = svChat.GetMessage(svChat._RoomCode, ChatService.Period.Topten);
                if (chatMsg != null)
                {
                    ViewBag.ChatMsg = chatMsg.OrderBy(m => m.ChatID);
                }
                else
                {
                    ViewBag.ChatMsg = null;
                }
            }
            return Json(new { ChatMsg = ViewBag.ChatMsg });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetFriendStatus()
        {
            var _svChat = new ChatService();
            int ID = LogonCompID;
            IOrderedEnumerable<view_ChatFriend> Friends = null;
            try{
                Friends = _svChat.GetFriend(ID).OrderByDescending(m => m.CreatedDate);
            }
            catch(Exception ex)
            {
               Friends = null;
            }
            var UnReads = _svChat.GetMessageNotRead(ID);
            return Json(new {ID = ID, Friends = Friends, UnReads = UnReads });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="FriendID"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetStatusFriendByID(int FriendID)
        {
            var _svChat = new ChatService();
            var unreads = _svChat.SelectData<view_ChatMessageReadCount>(" * ", " FromID = " + FriendID + " AND ToID = " + LogonCompID + " AND IsRead = 0 ", " ReadCount DESC ");
            return Json(unreads);
        }
        
        [HttpPost]
        public void SaveMessage(int roomid, int fromid, int toid, string message)
        {
            ChatService svChat = new ChatService();
            svChat.SaveMessage(roomid, fromid, toid, message);
        }

        [HttpPost]
        public void ReadingMessage(int toid,int roomid)
        {
            ChatService svChat = new ChatService();
            svChat.ReadingMessage(toid, roomid);
        }
    }
}