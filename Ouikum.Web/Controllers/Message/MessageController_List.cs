using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ouikum.Message;
using Ouikum;
using res = Prosoft.Resource.Web.Ouikum;

namespace Ouikum.Controllers
{
    public partial class MessageController : BaseController
    {
        //
        // GET: /MessageCenter/Message/List

        #region List

        #region Get: List
        [HttpGet]
        public ActionResult List(string MsgType = "Inbox")
        {
            if (RedirectToProduction())
                return Redirect(UrlProduction);

            RememberURL();
            if (!CheckIsLogin())
            {
                return Redirect(res.Pageviews.PvMemberSignUp);
            }
            else
            {
                GetStatusUser();
                CountMessage();
                CountQuotation();
                ViewBag.CateLevel1 = LogonCompLevel;
                ViewBag.MemberType = LogonMemberType;
                SetPager();
                ViewBag.MsgType = MsgType;
                ViewBag.PageType = "Message";
                ViewBag.MenuName = MsgType;
                return View();
            }
        }
        #endregion

        #region Post: List
        [HttpPost]
        public ActionResult List(FormCollection form)
        {
            MessageService svMessage = new MessageService();
            SelectList_PageSize();
            SetPager(form);
            CountMessage();
            List<view_Message> Messages;
            var SQLwhereSearch = "AND (Subject Like N'%" + form["SearchText"] + "%' OR FromCompName Like N'%" + form["SearchText"] + "%' OR FromName Like N'%"+form["SearchText"]+"%')";
            if (DataManager.ConvertToInteger(form["PIndex"]) == 1 )
            {
                ViewBag.PageIndex = DataManager.ConvertToInteger(form["PIndex"]);
            }
            if (form["TypeMessage"] == "Inbox")
            {
                var SQLWhere = svMessage.CreateWhereAction(MessageStatus.Inbox, LogonCompID) + SQLwhereSearch;
                Messages = svMessage.SelectData<view_Message>("*", SQLWhere, null, (int)ViewBag.PageIndex, (int)ViewBag.PageSize);
            }
            else if (form["TypeMessage"] == "Importance")
            {
                var SQLWhere = svMessage.CreateWhereAction(MessageStatus.Important, LogonCompID) + SQLwhereSearch;
                Messages = svMessage.SelectData<view_Message>("*", SQLWhere, null, (int)ViewBag.PageIndex, (int)ViewBag.PageSize);
            }
            else if (form["TypeMessage"] == "Draftbox")
            {
                var SQLWhere = svMessage.CreateWhereAction(MessageStatus.Draft, LogonCompID) + SQLwhereSearch;
                Messages = svMessage.SelectData<view_Message>("*", SQLWhere, null, (int)ViewBag.PageIndex, (int)ViewBag.PageSize);
            }
            else if (form["TypeMessage"] == "Sentbox")
            {
                var SQLWhere = svMessage.CreateWhereAction(MessageStatus.Sentbox, LogonCompID) + SQLwhereSearch;
                Messages = svMessage.SelectData<view_Message>("*", SQLWhere, null, (int)ViewBag.PageIndex, (int)ViewBag.PageSize);
            }
            else if (form["TypeMessage"] == "Trash")
            {
                var SQLWhere = svMessage.CreateWhereAction(MessageStatus.Trash, LogonCompID) + SQLwhereSearch;
                Messages = svMessage.SelectData<view_Message>("*", SQLWhere, null, (int)ViewBag.PageIndex, (int)ViewBag.PageSize);
            }
            else
            {
                var SQLWhere = svMessage.CreateWhereAction(MessageStatus.All, LogonCompID) + SQLwhereSearch;
                Messages = svMessage.SelectData<view_Message>("*", SQLWhere, null, (int)ViewBag.PageIndex, (int)ViewBag.PageSize);
            }
            ViewBag.Messages = Messages;
            ViewBag.MsgType = form["TypeMessage"];
            ViewBag.TotalPage = svMessage.TotalPage;
            ViewBag.TotalRow = svMessage.TotalRow;
            return PartialView("UC/MessageGrid");
        }
        #endregion

        #endregion

        #region Detail
        [HttpGet]
        public ActionResult Detail(int MessageID = 0, string MessageCode = "", string MsgType = "Inbox")
        {
            RememberURL();
            if (!CheckIsLogin())
            {
                return Redirect(res.Pageviews.PvMemberSignUp);
            }
            else
            {
                GetStatusUser();
                if (MessageID > 0)
                {
                    MessageService svMessage = new MessageService();
                    var Message = svMessage.SelectData<view_Message>("*", "IsDelete = 0 AND MessageID =" + MessageID).First();
                    ViewBag.Message = Message;
                    var MessageFile = svMessage.SelectData<emMessageAttach>("*", "IsDelete = 0 AND MessageID =" + MessageID);
                    ViewBag.MessageFile = MessageFile.Count() > 0 ? MessageFile.First() : null;
                    if (Message.FromCompID > 0)
                    {
                        var mem = svMessage.SelectData<view_CompMember>("*", "IsDelete = 0 AND CompID =" + Message.FromCompID);
                        ViewBag.Member = mem.Count() > 0 ? mem.First() : null;
                    }
                    #region Update IsRead
                    var Update_Message = svMessage.SelectData<emMessage>("*", "IsDelete = 0 AND MessageID =" + MessageID).First();
                    Update_Message.IsRead = true;
                    svMessage.SaveData<emMessage>(Update_Message, "MessageID");
                    #endregion
                    #region Set Contact Detail
                    if (MsgType == "Sentbox" || MsgType == "Draftbox")
                    {
                        #region Get ToCompID
                        var Message2 = svMessage.SelectData<view_Message>("MessageID, MessageCode, FromCompID, ToCompID, ToCompName", "IsDelete = 0 AND MessageCode ='" + Message.MessageCode + "'").First();
                        #endregion
                        if (Message2.ToCompID > 0)
                        {
                            GetCompany((int)Message2.ToCompID);
                            ViewBag.ToCompID = Message2.ToCompID;
                            ViewBag.ToCompName = Message2.ToCompName;
                        }
                        else
                        {
                            ViewBag.Company = null;
                            ViewBag.ToCompID = null;
                            ViewBag.ToCompName = null;
                        }
            
                    }
                    else
                    {
                        if (Message.FromCompID > 0)
                        {
                            GetCompany((int)Message.FromCompID);
                            ViewBag.ToCompID = null;
                            ViewBag.ToCompName = null;
                        }
                        else
                        {
                            ViewBag.Company = null;
                            ViewBag.ToCompID = null;
                            ViewBag.ToCompName = null;
                        }
                    }
                    #endregion

                    ViewBag.MsgType = MsgType;
                    ViewBag.msgtitle =  Message.Subject;
                    CountMessage();
                    ViewBag.PageType = "Message";
                    return View();

                }
                else
                {
                    return Redirect("~/Message/List?MsgType=Inbox");
                }
            }
        }
        #endregion
    }
}
