using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Prosoft.Service;
//using Prosoft.Base;
using Ouikum.Web.Models;
using Ouikum.Common;
using Ouikum.Company;
using Ouikum.Message;
using res = Prosoft.Resource.Web.Ouikum;
using System.Collections;
using Ouikum.Product;

namespace Ouikum.Web.Controllers
{
    public partial class ApiController : BaseController
    {
        // 

        #region Get message Price Inbox
        public ActionResult inboxmessage(int compid)
        {
            var svMsg = new MessageService();
            var sqlwhere = svMsg.CreateWhereAction(MessageStatus.Inbox, compid);
            sqlwhere += " AND RootMessageID = 0";
            var message = svMsg.SelectData<view_Message>(" * ", sqlwhere);
            var msg = new List<MsgModel>();
            if (message != null && message.Count > 0)
            {
                var msgid = message.Select(m => m.MessageID).ToList();
                var wheresubmsg = CreateWhereIN(msgid, "RootMessageID");
                var submsg = svMsg.SelectData<view_Message>(" * ", "IsDelete = 0 AND " + wheresubmsg);

                #region Set Model Message
                foreach (var item in message)
                {
                    var m = new MsgModel();
                    m.messageid = item.MessageID;
                    m.messagecode = item.MessageCode;
                    m.rootmessageid = (int)item.RootMessageID;
                    m.fromcompid = (int)item.FromCompID;
                    m.fromname = item.FromCompName;
                    m.tocompid = (int)item.ToCompID;
                    m.toname = item.ToCompName;
                    m.subject = item.Subject;
                    m.msgdetail = item.MsgDetail;
                    m.isfavorite = (bool)item.IsFavorite;
                    m.isread = (bool)item.IsRead;
                    m.isreply = (bool)item.IsReply;
                    m.issend = (bool)item.IsSend;
                    m.fromcontactphone = item.FromContactPhone;
                    m.createdate = item.CreatedDate;
                    m.modifieddate = item.ModifiedDate;
                    m.submessage = new List<MsgModel>();
                    #region sub message
                    foreach (var sub in submsg.Where(it => it.RootMessageID == item.MessageID))
                    {
                        var s = new MsgModel();
                        s.messageid = sub.MessageID;
                        s.messagecode = sub.MessageCode;
                        s.rootmessageid = (int)sub.RootMessageID;
                        s.fromcompid = (int)sub.FromCompID;
                        s.fromname = sub.FromCompName;
                        s.tocompid = (int)sub.ToCompID;
                        s.toname = sub.ToCompName;
                        s.subject = sub.Subject;
                        s.msgdetail = sub.MsgDetail;
                        s.isfavorite = (bool)sub.IsFavorite;
                        s.isread = (bool)sub.IsRead;
                        s.isreply = (bool)sub.IsReply;
                        s.issend = (bool)sub.IsSend;
                        s.fromcontactphone = sub.FromContactPhone;
                        s.submessage = new List<MsgModel>();
                        s.createdate = sub.CreatedDate;
                        s.modifieddate = sub.ModifiedDate;
                        m.submessage.Add(s);
                    }
                    #endregion
                    msg.Add(m);
                }
                #endregion
            }
            return Json(new { message = msg }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Get message 
        public ActionResult sentboxmessage(int compid)
        {

            var svMsg = new MessageService();  

            var sqlwhere = svMsg.CreateWhereAction(MessageStatus.Sentbox, compid);
            sqlwhere += " AND RootMessageID = 0";
            var msg = new List<MsgModel>();
            var message = svMsg.SelectData<view_Message>(" * ", sqlwhere);

            if (message != null && message.Count > 0)
            {
                var msgid = message.Select(m => m.MessageID).ToList();
                var wheresubmsg = CreateWhereIN(msgid, "RootMessageID");
                var submsg = svMsg.SelectData<view_Message>(" * ", "IsDelete = 0 AND " + wheresubmsg);

                #region Set Model Message
                foreach (var item in message)
                {
                    var m = new MsgModel();
                    m.messageid = item.MessageID;
                    m.messagecode = item.MessageCode;
                    m.rootmessageid = (int)item.RootMessageID;
                    m.fromcompid = (int)item.FromCompID;
                    m.fromname = item.FromCompName;
                    m.tocompid = (int)item.ToCompID;
                    m.toname = item.ToCompName;
                    m.subject = item.Subject;
                    m.msgdetail = item.MsgDetail;
                    m.isfavorite = (bool)item.IsFavorite;
                    m.isread = (bool)item.IsRead;
                    m.isreply = (bool)item.IsReply;
                    m.issend = (bool)item.IsSend;
                    m.fromcontactphone = item.FromContactPhone;
                    m.createdate = item.CreatedDate;
                    m.modifieddate = item.ModifiedDate;
                    m.submessage = new List<MsgModel>();
                    #region sub message
                    foreach (var sub in submsg.Where(it => it.RootMessageID == item.MessageID))
                    {
                        var s = new MsgModel();
                        s.messageid = sub.MessageID;
                        s.messagecode = sub.MessageCode;
                        s.rootmessageid = (int)sub.RootMessageID;
                        s.fromcompid = (int)sub.FromCompID;
                        s.fromname = sub.FromCompName;
                        s.tocompid = (int)sub.ToCompID;
                        s.toname = sub.ToCompName;
                        s.subject = sub.Subject;
                        s.msgdetail = sub.MsgDetail;
                        s.isfavorite = (bool)sub.IsFavorite;
                        s.isread = (bool)sub.IsRead;
                        s.isreply = (bool)sub.IsReply;
                        s.issend = (bool)sub.IsSend;
                        s.fromcontactphone = sub.FromContactPhone;
                        s.submessage = new List<MsgModel>();
                        s.createdate = sub.CreatedDate;
                        s.modifieddate = sub.ModifiedDate;
                        m.submessage.Add(s);
                    }
                    #endregion
                    msg.Add(m);
                }
                #endregion
            }
            return Json(new { message = msg }, JsonRequestBehavior.AllowGet);
        }
        #endregion 
         
        #region Send message Price

        #region Post message  
        [HttpPost]
        public ActionResult sendmessage(
            string fromemail,
            string tel,
            string message,
            string subject,
            int tocompid,
            string msgstatus,           
            int fromcompid = 0,
            int rootmessageid = 0)
        {
                Ouikum.Message.MessageService svMessage = new Ouikum.Message.MessageService();
                var svCompany = new CompanyService(); 
                string ToCompID = tocompid.ToString(); 
                int rootMsgID =  rootmessageid ;
                string sqlWhere = "";
                string msgdetail = message; 
                var msgCtrl = new Ouikum.Controllers.MessageController();
             
                #region MyRegion
		  
                    var emMessageSender = new emMessage();
                    var emMessageReceiver = new emMessage();

                    #region Sender Message
                    emMessageSender.ToCompID = tocompid;
                    emMessageSender.FromCompID = fromcompid;
                    emMessageSender.IsFavorite = false;
                    emMessageSender.Subject = subject;
                    emMessageSender.MsgDetail = msgdetail;
                    emMessageSender.RootMessageID = (rootMsgID != 0) ? rootMsgID : 0;
                    emMessageSender.MessageCode = emMessageSender.ToCompID + "-" + GetTimeStamp() + "-" + svMessage.Generate_MessageCode();
                    emMessageSender.MsgStatus = "2";
                    emMessageSender.IsSend = true;
                    emMessageSender.FromName = fromemail;
                    emMessageSender.FromContactPhone = tel;

                    #region Save Sender Message
                    if (msgstatus == "Reply")
                    {
                        emMessageSender = svMessage.InsertMessageReply(emMessageSender, "Reply");
                    }
                    else
                    {
                        emMessageSender = svMessage.InsertMessage(emMessageSender);
                    }
                    #endregion
                    #endregion

                    #region Receiver Message
                    emMessageReceiver.ToCompID = tocompid;
                    emMessageReceiver.FromCompID = fromcompid;
                    emMessageReceiver.Subject = subject;
                    emMessageReceiver.MsgDetail = msgdetail;
                    emMessageReceiver.IsFavorite = false;
                    emMessageReceiver.RootMessageID = (rootMsgID != 0) ? rootMsgID : 0;
                    emMessageReceiver.MessageCode = emMessageSender.MessageCode;
                    emMessageReceiver.MsgStatus = "2";
                    emMessageReceiver.IsSend = false;
                    emMessageReceiver.FromName = fromemail;
                    emMessageReceiver.FromContactPhone = tel;

                    #region Save Receiver Message
                    if (msgstatus == "Reply")
                    {
                        emMessageReceiver = svMessage.InsertMessageReply(emMessageReceiver, "Reply");
                    } 
                    else
                    {
                        emMessageReceiver = svMessage.InsertMessage(emMessageReceiver);
                    }
                    #endregion

                    #endregion
                        
                    if (emMessageReceiver.ToCompID > 0)
                    {
                        #region GetToCompName
                        sqlWhere = "CompID = " + emMessageReceiver.ToCompID + " AND IsDelete = 0";
                        var Company = svCompany.SelectData<b2bCompany>("CompID,CompName,ContactEmail", sqlWhere).First();
                        var toCompName = Company.CompName;
                        var toCompEmail = Company.ContactEmail;
                        #endregion

                        #region Send Email
                        if (svMessage.IsResult)
                        {
                            msgCtrl.SendEmail(emMessageReceiver, toCompName, toCompEmail);
                        }
                        #endregion
                    }
                    else
                    {
                        if (svMessage.IsResult)
                        {
                            msgCtrl.SendEmailNoMember(emMessageReceiver, "", fromemail); 
                        }
                    } 
	            #endregion
             
                msgCtrl.CountMessage();

                return Json(new { status = svMessage.IsResult }, JsonRequestBehavior.AllowGet);
        }
        #endregion
          
        #endregion 

        #region Open  message Price
        public ActionResult openmessage(int messageid)
        {
            var svMsg = new MessageService();
            var msg = new List<MsgModel>();
            var message = svMsg.SelectData<view_Message>(" * ", " MessageID = " + messageid + "  "); 

            #region update is read
            if (svMsg.TotalRow > 0)
            { 
                svMsg.UpdateByCondition<emMessage>("IsRead = 1", " MessageID = " + messageid + "  ");
            }
            #endregion

            if (message != null && message.Count > 0)
            {
                var msgid = message.Select(m => m.MessageID).ToList();
                var wheresubmsg = CreateWhereIN(msgid, "RootMessageID");
                var submsg = svMsg.SelectData<view_Message>(" * ", "IsDelete = 0 AND " + wheresubmsg);

                #region Set Model Message
                foreach (var item in message)
                {
                    var m = new MsgModel();
                    m.messageid = item.MessageID;
                    m.messagecode = item.MessageCode;
                    m.rootmessageid = (int)item.RootMessageID;
                    m.fromcompid = (int)item.FromCompID;
                    m.fromname = item.FromCompName;
                    m.tocompid = (int)item.ToCompID;
                    m.toname = item.ToCompName;
                    m.subject = item.Subject;
                    m.msgdetail = item.MsgDetail;
                    m.isfavorite = (bool)item.IsFavorite;
                    m.isread = (bool)item.IsRead;
                    m.isreply = (bool)item.IsReply;
                    m.issend = (bool)item.IsSend;
                    m.submessage = new List<MsgModel>();

                    #region sub message
                    foreach (var sub in submsg.Where(it => it.RootMessageID == item.MessageID))
                    {
                        var s = new MsgModel();
                        s.messageid = sub.MessageID;
                        s.messagecode = sub.MessageCode;
                        s.rootmessageid = (int)sub.RootMessageID;
                        s.fromcompid = (int)sub.FromCompID;
                        s.fromname = sub.FromCompName;
                        s.tocompid = (int)sub.ToCompID;
                        s.toname = sub.ToCompName;
                        s.subject = sub.Subject;
                        s.msgdetail = sub.MsgDetail;
                        s.isfavorite = (bool)sub.IsFavorite;
                        s.isread = (bool)sub.IsRead;
                        s.isreply = (bool)sub.IsReply;
                        s.issend = (bool)sub.IsSend;
                        s.submessage = new List<MsgModel>();
                        m.submessage.Add(s);
                    }
                    #endregion
                    msg.Add(m);
                }
                #endregion

            }

            return Json(new { message = msg }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Delete message Price 
        public ActionResult deletemessage(int[] messageid)
        {
            var svMsg = new MessageService(); 
            var sqlwhere = CreateWhereIN(messageid, "messageid");
            var sql = " IsDelete =  1"; 
            svMsg.UpdateByCondition<emMessage>(sql, sqlwhere);
            return Json(new { status = svMsg.IsResult }, JsonRequestBehavior.AllowGet);
        } 
        #endregion


        #region Favorite message Price
        public ActionResult favmessage(int messageid, bool fav)
        {
            var svMsg = new MessageService();
            var sql = " IsFavorite =  0";
            if (fav)
                sql = " IsFavorite =  1";  //เปลี่ยนจาก IsImportance เป็น IsFavorite
            svMsg.UpdateByCondition<emMessage>(sql, "messageid = " + messageid);
            return Json(new { status = svMsg.IsResult }, JsonRequestBehavior.AllowGet);
        }
        #endregion

    }
}
