using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections;

using Ouikum.Message;
using Ouikum.Company;
using Prosoft.Service;
using Ouikum.Common;
using Ouikum;
using res = Prosoft.Resource.Web.Ouikum;
using Ouikum.Quotation;
using System.Web.Mail;

namespace Ouikum.Controllers
{
    public partial class MessageController : BaseController
    {
        //
        // GET: /Message/

        public ActionResult Index()
        {
            return View();
        }

        #region CountMessage
        public void CountMessage()
        {
            MessageService svMessage = new MessageService();
            ViewBag.CountInbox = svMessage.SelectData<emMessage>("*", svMessage.CreateWhereAction(MessageStatus.UnRead, LogonCompID), null, 0, 0).Count();
            ViewBag.CountImportance = svMessage.SelectData<emMessage>("*", svMessage.CreateWhereAction(MessageStatus.Important, LogonCompID), null, 0, 0).Count();
            ViewBag.CountDraftbox = svMessage.SelectData<emMessage>("*", svMessage.CreateWhereAction(MessageStatus.Draft, LogonCompID), null, 0, 0).Count();
            ViewBag.CountSentbox = svMessage.SelectData<emMessage>("*", svMessage.CreateWhereAction(MessageStatus.Sentbox, LogonCompID), null, 0, 0).Count();
        }
        #endregion

        #region CountQuotation
        public void CountQuotation()
        {
            var svQuotation = new QuotationService();
            ViewBag.Inbox = svQuotation.CountData<b2bQuotation>("*", "( IsDelete = 0 ) AND (IsRead = 'False') AND (IsOutBox = 'False')  AND (IsRead = 0) AND (ToCompID = " + LogonCompID + ")");
            ViewBag.Importance = svQuotation.CountData<b2bQuotation>("*", "( ToCompID = " + LogonCompID + " AND IsOutBox = 0 AND IsDelete = 0 AND  IsImportance = 1 ) OR (FromCompID = " + LogonCompID + " AND IsOutBox = 1 AND IsDelete = 0 AND  IsImportance = 1 )");
            ViewBag.Sentbox = svQuotation.CountData<b2bQuotation>("*", "( IsDelete = 0 AND  IsOutbox = 1 AND FromCompID = " + LogonCompID + " )");
        }
        #endregion

        #region ListContact
        [HttpPost]
        public ActionResult ListContact(FormCollection form)
        {
            SetPager(form);

            CompanyService svCompany = new CompanyService();
            string sqlSelect = "CompID,CompName,LogoImgPath,ContactEmail";
            string sqlWhere = svCompany.CreateWhereAction(CompStatus.Online,0);

            if (!string.IsNullOrEmpty(ViewBag.TextSearch))
            {
                sqlWhere += "AND CompName Like N'%" + ViewBag.TextSearch + "%'";
            }
            sqlWhere += "AND (CompID != " + LogonCompID + ")";
            var CompContact = svCompany.SelectData<b2bCompany>(sqlSelect, sqlWhere, null, ViewBag.PageIndex, ViewBag.PageSize);
            ViewBag.CompContact = CompContact;
            ViewBag.TotalPage = svCompany.TotalPage;
            ViewBag.TotalRow = svCompany.TotalRow;

            return PartialView("MessageCenter/ContactGrid");
        }
        #endregion
        
        #region MarkRead
        public bool MarkRead(List<bool> Check, List<int> ID, List<short> RowVersion, string PrimaryKeyName)
        {
            MessageService svMessage = new MessageService();
            svMessage.ChangeRead<emMessage>(Check, ID, RowVersion, PrimaryKeyName, "IsRead");
            if (svMessage.IsResult)
            {
                return svMessage.IsResult;
            }
            else
            {
                return svMessage.IsResult;
            }
        }
        #endregion

        #region DelData
        public ActionResult DelData(List<bool> Check, List<int> ID, List<short> RowVersion, string PrimaryKeyName)
        {
            MessageService svMessage = new MessageService();
            for (int i = 0; i < ID.Count(); i++)
            {
                if (Check[i] == true)
                {
                    svMessage.UpdateByCondition<emMessage>("MsgFolderID = 4", " MessageID = " + ID[i]);
                }
            }
            //svMessage.DelData<emMessage>(Check, ID, RowVersion, PrimaryKeyName);
            
            if (svMessage.IsResult)
            {
                var CountImportance = svMessage.SelectData<emMessage>("*", svMessage.CreateWhereAction(MessageStatus.Important, LogonCompID), null, 0, 0).Count();
                var CountInbox = svMessage.SelectData<emMessage>("*", svMessage.CreateWhereAction(MessageStatus.UnRead, LogonCompID), null, 0, 0).Count();
                var CountDraftbox = svMessage.SelectData<emMessage>("*", svMessage.CreateWhereAction(MessageStatus.Draft, LogonCompID), null, 0, 0).Count();
                var CountSentbox = svMessage.SelectData<emMessage>("*", svMessage.CreateWhereAction(MessageStatus.Sentbox, LogonCompID), null, 0, 0).Count();
                return Json(new
                {
                    Result = true,
                    CountImportance = CountImportance,
                    CountInbox = CountInbox,
                    CountDraftbox = CountDraftbox,
                    CountSentbox = CountSentbox
                });
            }
            else
            {
                return Json(new { Result = false });
            }
        }
        #endregion

        #region DelMessData
        public ActionResult DelMessData(List<bool> Check, List<int> ID, List<short> RowVersion, string PrimaryKeyName)
        {
            MessageService svMessage = new MessageService();
            for (int i = 0; i < ID.Count(); i++)
            {
                if (Check[i] == true)
                {
                    svMessage.UpdateByCondition<emMessage>("IsDelete = 1", " MessageID = " + ID[i]);
                }
            }
            
            if (svMessage.IsResult)
            {
                var CountImportance = svMessage.SelectData<emMessage>("*", svMessage.CreateWhereAction(MessageStatus.Important, LogonCompID), null, 0, 0).Count();
                var CountInbox = svMessage.SelectData<emMessage>("*", svMessage.CreateWhereAction(MessageStatus.UnRead, LogonCompID), null, 0, 0).Count();
                var CountDraftbox = svMessage.SelectData<emMessage>("*", svMessage.CreateWhereAction(MessageStatus.Draft, LogonCompID), null, 0, 0).Count();
                var CountSentbox = svMessage.SelectData<emMessage>("*", svMessage.CreateWhereAction(MessageStatus.Sentbox, LogonCompID), null, 0, 0).Count();
                return Json(new
                {
                    Result = true,
                    CountImportance = CountImportance,
                    CountInbox = CountInbox,
                    CountDraftbox = CountDraftbox,
                    CountSentbox = CountSentbox
                });
            }
            else
            {
                return Json(new { Result = false });
            }
        }
        #endregion

        #region DelAllData
        public ActionResult DelAllData()
        {
            MessageService svMessage = new MessageService();
            var ID = svMessage.SelectData<emMessage>("*", svMessage.CreateWhereAction(MessageStatus.Trash, LogonCompID), null, 0, 0);
            if (ID.Count() > 0)
            {
                for (int i = 0; i < ID.Count(); i++)
                {
                    svMessage.UpdateByCondition<emMessage>("IsDelete = 1", " MessageID = " + ID[i].MessageID);
                }
            }

            if (svMessage.IsResult)
            {
                var CountImportance = svMessage.SelectData<emMessage>("*", svMessage.CreateWhereAction(MessageStatus.Important, LogonCompID), null, 0, 0).Count();
                var CountInbox = svMessage.SelectData<emMessage>("*", svMessage.CreateWhereAction(MessageStatus.UnRead, LogonCompID), null, 0, 0).Count();
                var CountDraftbox = svMessage.SelectData<emMessage>("*", svMessage.CreateWhereAction(MessageStatus.Draft, LogonCompID), null, 0, 0).Count();
                var CountSentbox = svMessage.SelectData<emMessage>("*", svMessage.CreateWhereAction(MessageStatus.Sentbox, LogonCompID), null, 0, 0).Count();
                return Json(new
                {
                    Result = true,
                    CountImportance = CountImportance,
                    CountInbox = CountInbox,
                    CountDraftbox = CountDraftbox,
                    CountSentbox = CountSentbox
                });
            }
            else
            {
                return Json(new { Result = false });
            }
        }
        #endregion

        #region MoveData
        public ActionResult MoveData(List<bool> Check, List<int> ID, List<short> RowVersion, string PrimaryKeyName)
        {
            MessageService svMessage = new MessageService();
            for (int i = 0; i < ID.Count(); i++)
            {
                if (Check[i] == true)
                {
                    var isSend = svMessage.SelectData<emMessage>("*", "MessageID = " + ID[i], null, 0, 0);
                    svMessage.UpdateByCondition<emMessage>("MsgFolderID = " + isSend[0].MsgStatus , " MessageID = " + ID[i]);
                }
            }

            if (svMessage.IsResult)
            {
                var CountImportance = svMessage.SelectData<emMessage>("*", svMessage.CreateWhereAction(MessageStatus.Important, LogonCompID), null, 0, 0).Count();
                var CountInbox = svMessage.SelectData<emMessage>("*", svMessage.CreateWhereAction(MessageStatus.UnRead, LogonCompID), null, 0, 0).Count();
                var CountDraftbox = svMessage.SelectData<emMessage>("*", svMessage.CreateWhereAction(MessageStatus.Draft, LogonCompID), null, 0, 0).Count();
                var CountSentbox = svMessage.SelectData<emMessage>("*", svMessage.CreateWhereAction(MessageStatus.Sentbox, LogonCompID), null, 0, 0).Count();
                return Json(new
                {
                    Result = true,
                    CountImportance = CountImportance,
                    CountInbox = CountInbox,
                    CountDraftbox = CountDraftbox,
                    CountSentbox = CountSentbox
                });
            }
            else
            {
                return Json(new { Result = false });
            }
        }

        public ActionResult MoveDataDetail(int ID, short RowVersion, string PrimaryKeyName)
        {
            MessageService svMessage = new MessageService();
            
            var isSend = svMessage.SelectData<emMessage>("*", "MessageID = " + ID, null, 0, 0);
            svMessage.UpdateByCondition<emMessage>("MsgFolderID = " + isSend[0].MsgStatus, " MessageID = " + ID);
                

            if (svMessage.IsResult)
            {
                var CountImportance = svMessage.SelectData<emMessage>("*", svMessage.CreateWhereAction(MessageStatus.Important, LogonCompID), null, 0, 0).Count();
                var CountInbox = svMessage.SelectData<emMessage>("*", svMessage.CreateWhereAction(MessageStatus.UnRead, LogonCompID), null, 0, 0).Count();
                var CountDraftbox = svMessage.SelectData<emMessage>("*", svMessage.CreateWhereAction(MessageStatus.Draft, LogonCompID), null, 0, 0).Count();
                var CountSentbox = svMessage.SelectData<emMessage>("*", svMessage.CreateWhereAction(MessageStatus.Sentbox, LogonCompID), null, 0, 0).Count();
                return Json(new
                {
                    Result = true,
                    CountImportance = CountImportance,
                    CountInbox = CountInbox,
                    CountDraftbox = CountDraftbox,
                    CountSentbox = CountSentbox
                });
            }
            else
            {
                return Json(new { Result = false });
            }
        }
        #endregion

        #region ChangeTag
        public ActionResult ChangeTag(FormCollection form)
        {
            MessageService svMessage = new MessageService();
            var Messages = svMessage.SelectData<emMessage>("*", "IsDelete = 0 AND MessageID = " + form["id"] + " AND (ToCompID = " + LogonCompID+" OR FromCompID = " + LogonCompID+")", null, 0, 0);

            #region set ค่า  & Update emMessage
            Messages[0].IsFavorite = true;//DataManager.ConvertToBool(form["IsFavorite"])
            Messages = svMessage.SaveData<emMessage>(Messages, "MessageID");
            #endregion
            if (svMessage.IsResult)
            {
                var CountImportance = svMessage.SelectData<emMessage>("*", svMessage.CreateWhereAction(MessageStatus.Important, LogonCompID), null, 0, 0).Count();
                // var CountInbox = svMessage.SelectData<emMessage>("*", svMessage.CreateWhereAction(MessageStatus.All, LogonCompID), null, 0, 0).Count();
                return Json(new
                {
                    Result = true,
                    CountImportance = CountImportance//, 
                    //CountInbox = CountInbox 
                });
            }
            else
            {
                return Json(new { Result = false });
            }
        }

        public ActionResult ChangeTagList(List<bool> Check, List<int> ID, List<short> RowVersion, string PrimaryKeyName)
        {
            MessageService svMessage = new MessageService();

            for (int i = 0; i < ID.Count(); i++)
            {
                if (Check[i] == true)
                {
                    var Messages = svMessage.SelectData<emMessage>("*", "IsDelete = 0 AND MessageID = " + ID[i] + " AND (ToCompID = " + LogonCompID + " OR FromCompID = " + LogonCompID + ")", null, 0, 0);

                    #region set ค่า  & Update emMessage
                    Messages[0].IsFavorite = DataManager.ConvertToBool(0);
                    Messages = svMessage.SaveData<emMessage>(Messages, "MessageID");
                    #endregion
                }
            }

            if (svMessage.IsResult)
            {
                var CountImportance = svMessage.SelectData<emMessage>("*", svMessage.CreateWhereAction(MessageStatus.Important, LogonCompID), null, 0, 0).Count();
                // var CountInbox = svMessage.SelectData<emMessage>("*", svMessage.CreateWhereAction(MessageStatus.All, LogonCompID), null, 0, 0).Count();
                return Json(new
                {
                    Result = true,
                    CountImportance = CountImportance//, 
                    //CountInbox = CountInbox 
                });
            }
            else
            {
                return Json(new { Result = false });
            }
        }
        #endregion

        #region GetMessageDetail for reply&forward
        public ActionResult GetMessageDetail(int msgid, string type)
        {
            Hashtable data = new Hashtable();
            MessageService svMessage = new MessageService();
            var messageFile = new emMessageAttach();
            string SqlWhere = "MessageID = "+ msgid + " AND IsDelete = 0";
            var Message = svMessage.SelectData<view_Message>("*",SqlWhere).First();
            if (Message.IsAttach == true)
            {
                messageFile = svMessage.SelectData<emMessageAttach>("*", "MessageID = " + Message.MessageID).First();
            }
            #region Prefix for Reply & Forward
            string prefixType = "";
            //check have prefix
            var subSubject = Message.Subject;
            subSubject = subSubject.Substring(0, 4);

            if (type == "Reply" && subSubject != "RE: ")
            {
                prefixType = "RE: ";//4 character
                data.Add("msgSubject", prefixType + Message.Subject);
            }
            else if (type == "Forward" && subSubject != "FW: ")
            {
                prefixType = "FW: ";//4 character
                data.Add("msgSubject", prefixType + Message.Subject);
            }
            else
            {
                data.Add("msgSubject", Message.Subject);
            }
            #endregion

            #region set history message
            var SendDate = "";
            var SendTime = "";
            if (Message.SendDate != null)
            {

                DateTime convertedDate = DateTime.Parse(Message.SendDate.ToString());
                DateTime localDate = convertedDate.ToLocalTime();

                SendDate = (DateTime.Parse(convertedDate.ToString()).ToString("dd/MM/yyyy")).ToString();
                SendTime = (DateTime.Parse(convertedDate.ToString()).ToString("HH:mm")).ToString();
            }
            if (type == "Reply")
            {    
                string toEmail = "";
                string toName = "";
                if (Message.FromCompID == 0)
                {
                    toName = Message.FromName;
                    toEmail = Message.FromEmail;
                    string[] strName = toName.Split('(');
                    toName = strName[0].Substring(0, strName[0].Length - 1);
                    //toEmail = strEmail[1].Substring(0, strEmail[1].Length - 1);
                }
                string historydetail = "<br><br>จาก : " + Message.FromName + " (" + Message.FromEmail + ")";
                historydetail += "<div style=\"border-left:1px #ccc solid;padding-left:2ex;margin-left: 10px;\">ถึง : " + Message.ToCompName;
                historydetail += "<br>ส่งเมื่อวันที่ : " + SendDate + " || เวลา" + SendTime + " น.";
                historydetail += "<br>หัวข้อ : " + Message.Subject;
                historydetail += "<br><br>" + Message.MsgDetail + "<br></div>";

                data.Add("msgDetail", historydetail);
                data.Add("msgToCompID", Message.ToCompID);
                data.Add("msgToCompName", Message.ToCompName);
                data.Add("msgFromCompID", Message.FromCompID);
                data.Add("msgFileName", messageFile.FileName);
                data.Add("msgFromCompName", Message.FromCompName);
                data.Add("emailNotMember", toEmail);
                data.Add("nameNotMember", toName);
            }
            else
            {
                string historydetail = "<br><br>จาก : " + Message.FromName + " (" + Message.FromEmail + ")";
                historydetail += "<div style=\"border-left:1px #ccc solid;padding-left:2ex;margin-left: 10px;\">ถึง : " + Message.ToCompName;
                historydetail += "<br>ส่งเมื่อวันที่ : " + SendDate + " || เวลา" + SendTime + " น.";
                historydetail += "<br>หัวข้อ : " + Message.Subject;
                historydetail += "<br><br>" + Message.MsgDetail + "<br></div>";

                data.Add("msgDetail", historydetail);
                data.Add("msgFileName", messageFile.FileName);
            }

            #endregion

            return Json(data);
        }
        #endregion

        #region GetDraftDetail
        public ActionResult GetDraftDetail(int msgid)
        {
            Hashtable data = new Hashtable();
            MessageService svMessage = new MessageService();
            string SqlWhere = "MessageID = " + msgid + " AND IsDelete = 0";
            var Message = svMessage.SelectData<view_Message>("*", SqlWhere).First();

            data.Add("msgDetail", Message.MsgDetail);
            data.Add("msgToCompID", Message.ToCompID);
            data.Add("msgToCompName", Message.ToCompName);
            data.Add("msgFromCompID", Message.FromCompID);
            data.Add("msgFromCompName", Message.FromCompName);
            data.Add("msgSubject", Message.Subject);

            return Json(data);
        }
        #endregion

        #region SendEmail
        public bool SendEmail(emMessage model, string toCompName, string toCompEmail)
        {
            #region variable
            MessageService svMessage = new MessageService();
            bool IsSend = true;
            var Detail = "";
            var url = "";
            var mailTo = new List<string>();
            List<string> mailToAdmin = GetMailListB2BAdmin();
            var mailCC = new List<string>();
            var Sender = "";
            var Receiver = "";
            svCompany = new CompanyService();
            string fromName = "";
            string fromPhone = "";
            string fromEmail = "";
            #endregion
            #region set from info
            if (model.FromCompID > 0)
            {
                string sqlselect = "CompID,CompName,ContactFirstName,ContactLastName,ContactEmail,ContactPhone,CompPhone";
                string sqlwhere = "CompID = " + model.FromCompID + " AND IsDelete = 0";
                var fromcomp = svCompany.SelectData<b2bCompany>(sqlselect, sqlwhere).Count() > 0 ? svCompany.SelectData<b2bCompany>(sqlselect, sqlwhere).First() : new b2bCompany();
                if (!string.IsNullOrEmpty(model.FromContactPhone))
                {
                    fromPhone = model.FromContactPhone;
                }
                else if (!string.IsNullOrEmpty(fromcomp.ContactPhone))
                {
                    fromPhone = fromcomp.ContactPhone;
                }
                else
                {
                    fromPhone = fromcomp.CompPhone;
                }

            }
            else
            {
                fromPhone = model.FromContactPhone;
            }
            Sender = model.FromName;
            string[] subName = model.FromName.Split(new char[] { '(', ')', ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (subName.Length > 1)
            {
                fromName = subName[0].Substring(0, subName[0].Length);
                fromEmail = subName[1].Substring(0, subName[1].Length );
            }
            else
            {
                fromName = subName[0].Substring(0, subName[0].Length);
                fromEmail = model.FromEmail;  
            }
            var MessageFile = new List<emMessageAttach>();
            if (model.IsAttach == true)
            {
                MessageFile = svMessage.SelectData<emMessageAttach>("AttachFilesID,FilePath,FileName", "IsDelete = 0 AND MessageID =" + model.MessageID);
            }
            #endregion

            Receiver = toCompName;

            #region Set Content & Value For Send Email
            string urlb2bthai = res.Pageviews.UrlWeb;
            url = urlb2bthai + "/Message/Detail?MessageID=" + model.MessageID + "&MessageCode=" + model.MessageCode + "&MsgType=Inbox";
            
            //test path logo
            string b2bthai_url = res.Pageviews.UrlWeb;
            string pathlogo = b2bthai_url + "/Content/Default/logo/Ouikum/img_Logo120x74.png";

            Hashtable EmailDetail = new Hashtable();
            EmailDetail["ToFirstName"] = toCompName;
            EmailDetail["FromFirstName"] = fromName;
            EmailDetail["FromPhone"] = fromPhone;
            EmailDetail["FromEmail"] = fromEmail;
            EmailDetail["MsgDetail"] = model.MsgDetail;
            EmailDetail["pathLogo"] = pathlogo;
            EmailDetail["FilePath"] = MessageFile.Count > 0 ? MessageFile[0].FilePath : null;
            EmailDetail["FileName"] = MessageFile.Count > 0 ? MessageFile[0].FileName : null;
            EmailDetail["IsAttach"] = model.IsAttach;
            EmailDetail["IsForward"] = model.IsForward;
            EmailDetail["url"] = url;
            if (model.IsFavorite == true)
            {
                EmailDetail["IsFavorite"] = "กรุณาติดต่อกลับด่วน";
            }
            ViewBag.Data = EmailDetail;

            string Subject = "คุณ "+fromName+", ได้ติดต่อคุณ ผ่านทาง Ouikum.com";
            Detail = PartialViewToString("UC/Email/SendMessage");

            var mailFrom = res.Config.EmailNoReply;
            var AttachmentName = MessageFile.Count > 0 ? MessageFile[0].FileName : null;
            var AttachmentPath = MessageFile.Count > 0 ? MessageFile[0].FilePath : null;
            mailTo.Add(toCompEmail);
            //mailTo.Add("katak.supavadee.pengmol@gmail.com");
            //mailTo.Add("supavadee_katak@hotmail.com");
            #endregion
            IsSend = OnSendByAlertEmail(Subject, mailFrom, mailTo, mailCC, Detail, AttachmentName, AttachmentPath);

            //url = urlb2bthai + "/Admin/Stat/MessageDetail?MessageID=" + model.MessageID + "&MessageCode=" + model.MessageCode;
            //Subject = res.Message_Center.lblKhun + " " + Sender + " " + res.Message_Center.lblContacted + " " + Receiver + " " + res.Message_Center.lblVia;
            //Detail = "<table ><tr><td>" + res.Message_Center.lblKhun + " " + Sender + " " + res.Message_Center.lblContacted + " " + Receiver + " "+ res.Message_Center.lblVia+" </td></tr>";
            //Detail += "<tr><td><br>" + model.MsgDetail + "</td></tr>";
            //Detail += "<tr><td><br><br>" + res.Message_Center.lblWantToViewMsg + " <a href=\"" + url + "\" >" + url + "</a><br></td></tr>";
            //Detail += "</table>";
            //IsSend = OnSendByAlertEmail(Subject, mailFrom, mailToAdmin, mailCC, Detail);

            return IsSend;
        }

        #endregion

        #region SendEmail No Member
        public bool SendEmailNoMember(emMessage model, string name, string email)
        {
            #region variable
            bool IsSend = true;
            var Detail = "";
            var url = "";
            var mailTo = new List<string>();
            List<string> mailToAdmin = GetMailListB2BAdmin();
            var mailCC = new List<string>();
            var Sender = "";
            var Receiver = "";
            svCompany = new CompanyService();
            string fromName = "";
            string fromPhone = "";
            string fromEmail = "";
            #endregion
            #region set from info
            if (model.FromCompID > 0)
            {
                string sqlselect = "CompID,CompName,ContactFirstName,ContactLastName,ContactEmail,ContactPhone,CompPhone";
                string sqlwhere = "CompID = " + model.FromCompID + " AND IsDelete = 0";
                var fromcomp = svCompany.SelectData<b2bCompany>(sqlselect, sqlwhere).Count() > 0 ? svCompany.SelectData<b2bCompany>(sqlselect, sqlwhere).First() : new b2bCompany();
                if (!string.IsNullOrEmpty(model.FromContactPhone))
                {
                    fromPhone = model.FromContactPhone;
                }
                else if (!string.IsNullOrEmpty(fromcomp.ContactPhone))
                {
                    fromPhone = fromcomp.ContactPhone;
                }
                else
                {
                    fromPhone = fromcomp.CompPhone;
                }

            }
            else
            {
                fromPhone = model.FromContactPhone;
            }
            Sender = model.FromName;
            string[] subName = model.FromName.Split(new char[] { '(', ')', ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (subName.Length > 1)
            {
                fromName = subName[0].Substring(0, subName[0].Length);
                fromEmail = subName[1].Substring(0, subName[1].Length);
            }
            else
            {
                fromName = subName[0].Substring(0, subName[0].Length);
                fromEmail = model.FromEmail;
            }

            #endregion

            Receiver = name;

            #region Set Content & Value For Send Email
            string urlb2bthai = res.Pageviews.UrlWeb;
            url = urlb2bthai + "/Message/Detail?MessageID=" + model.MessageID + "&MessageCode=" + model.MessageCode + "&MsgType=Inbox";

            //test path logo
            string b2bthai_url = res.Pageviews.UrlWeb;
            string pathlogo = b2bthai_url + "/Content/Default/logo/Ouikum/img_Logo120x74.png";

            Hashtable EmailDetail = new Hashtable();
            EmailDetail["ToFirstName"] = email;
            EmailDetail["FromFirstName"] = fromName;
            EmailDetail["FromPhone"] = fromPhone;
            EmailDetail["FromEmail"] = fromEmail;
            EmailDetail["MsgDetail"] = model.MsgDetail;
            EmailDetail["pathLogo"] = pathlogo;
            EmailDetail["url"] = url;
            if (model.IsFavorite == true)
            {
                EmailDetail["IsFavorite"] = "กรุณาติดต่อกลับด่วน";
            }
            ViewBag.Data = EmailDetail;

            string Subject = "คุณ " + fromName + ", ได้ติดต่อคุณ ผ่านทาง Ouikum.com";
            
            Detail = PartialViewToString("UC/Email/SendMessage");

            var mailFrom = res.Config.EmailNoReply;
            mailTo.Add(email);
            //mailTo.Add("katak.supavadee.pengmol@gmail.com");
            //mailTo.Add("supavadee_katak@hotmail.com");
            #endregion

            IsSend = OnSendByAlertEmail(Subject, mailFrom, mailTo, mailCC, Detail);

            //url = urlb2bthai + "/Admin/Stat/MessageDetail?MessageID=" + model.MessageID + "&MessageCode=" + model.MessageCode;
            //Subject = res.Message_Center.lblKhun + " " + Sender + " " + res.Message_Center.lblContacted + " " + Receiver + " " + res.Message_Center.lblVia;
            //Detail = "<table ><tr><td>" + res.Message_Center.lblKhun + " " + Sender + " " + res.Message_Center.lblContacted + " " + Receiver + " " + res.Message_Center.lblVia + " </td></tr>";
            //Detail += "<tr><td><br>" + model.MsgDetail + "</td></tr>";
            //Detail += "<tr><td><br><br>" + res.Message_Center.lblWantToViewMsg + " <a href=\"" + url + "\" >" + url + "</a><br></td></tr>";
            //Detail += "</table>";
            //IsSend = OnSendByAlertEmail(Subject, mailFrom, mailToAdmin, mailCC, Detail);

            return IsSend;
        }

        #endregion

    }
}
