#region using System
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Xml.Linq;
using System.Web.UI;
using System.Drawing;                       // Drawing Image
using System.Text;                          // StringBuilder
using System.IO;                            // StringWriter
using System.Text.RegularExpressions;       // Regex
using System.Runtime.Caching;               // CacheFC
using System.Configuration;
using System.Collections;
using System.Security.Cryptography;
using System.Net.NetworkInformation;        // MacAddress
using System.Net;                           // GetIP4Address()
#endregion
using Prosoft.Base;
using Prosoft.Service;
using System.Threading;
using System.Globalization;
using Autofac;
using Ouikum;
using Ouikum.Common;
using Ouikum.Product;
using Ouikum.Buylead; 
using Ouikum.Company;
using Ouikum.Article;
using res = Prosoft.Resource.Web.Ouikum;
using Ouikum.BizType;
using Ouikum.Category;
using SendGridMail;
using System.Net.Mail;
using SendGridMail.Transport;

namespace System.Web.Mvc
{
    public partial class BaseController : BaseClassController
    {

        #region SendMail
        #region  OnSendErrorMail
        public bool OnSendErrorMail(Exception ex, string url = "", string logon = "")
        {

            List<string> lstMailTo = new List<string>();
            //lstMailTo.Add("narongrit@prosoft.co.th");
            //lstMailTo.Add("prasit@prosoft.co.th");
            //lstMailTo.Add("giettisak@prosoft.co.th");
            lstMailTo.Add("supawadee@prosoft.co.th");

            var Detail = "";
            Detail += "<table width='750' border='0' cellpadding='5' cellspacing='0'>";
            Detail += "<tr><th align='right' width='100' >Date :</th><td>" + DateTimeNow + "</td></tr>";
            Detail += "<tr><th align='right' width='100' >URL :</th><td>" + url + "</td></tr>";

            if (!string.IsNullOrEmpty(logon))
                Detail += "<tr><th align='right' width='100'>Longon :</th><td>" + logon + "</td></tr>";

            Detail += "<tr><th align='right' width='100'>Error :</th><td>" + ex.Message + "</td></tr>";
            Detail += "<tr><th align='right' width='100'>Stack Track :</th><td>" + ex.StackTrace + "</td></tr></table>";

            var sendSuccess = OnSendMail(
                "Error : " + res.Common.lblWebsite,
                res.Config.EmailSupport,
                lstMailTo, null, Detail);


            return sendSuccess;

        }
        #endregion
        #region OnSendMail
        public bool OnSendMail(string Subject, string MailFrom, List<string> MailTo, List<string> MailCC, string Detail)
        {
            //Set SMTP
            bool sendSuccess = false;

            var SMTP_Server = @res.Config.EmailServer;
            var SMTP_UserName = res.Config.EmailSupport;
            var SMTP_Password = "Support@6654";
            var SMTP_IsAuthentication = true;

            #region Mail To
            string strMailTo = string.Empty;
            foreach (string to in MailTo)
            {
                if (!string.IsNullOrEmpty(strMailTo))
                    strMailTo = string.Concat(strMailTo, ";");
                strMailTo = string.Concat(strMailTo, to.Replace(";", string.Empty));
            }
            #endregion

            #region Mail CC
            string strMailCC = string.Empty;
            if (MailCC != null && MailCC.Count > 0)
            {
                foreach (string cc in MailCC)
                {
                    if (!string.IsNullOrEmpty(strMailCC))
                        strMailCC = string.Concat(strMailCC, ";");
                    strMailCC = string.Concat(strMailCC, cc.Replace(";", string.Empty));
                }
            }
            #endregion

            #region Send Mail
            EmailManager emailManager = new EmailManager(SMTP_Server, SMTP_UserName, SMTP_Password, SMTP_IsAuthentication);
            emailManager.Port = 25;
            emailManager.IsBodyHtml = true;
            emailManager.Form = MailFrom;
            emailManager.mDisplayName = "Ouikum";
            emailManager.To = strMailTo;
            emailManager.CC = strMailCC;
            emailManager.Subject = Subject;
            emailManager.Body = Detail;
            emailManager.BodyEncoding = Encoding.UTF8;
            emailManager.SubjectEncoding = Encoding.UTF8;
            try
            {
                sendSuccess = emailManager.SendEmail();
            }
            catch { sendSuccess = false; }
            return sendSuccess;
            #endregion
        }
        #endregion

        #region OnSendByAlertEmailNew (แบบใหม่)
        protected internal bool OnSendByAlertEmailNew(string Subject, string MailFrom, List<string> MailTo, List<string> MailCC, string Detail, string MailFromDisplay = "B2BThai", string ReplyTo = "", string ReplyDisplay = "B2BThai")
        {
            try
            {
                var myMessage = SendGrid.GetInstance();

                #region Mail To
                bool IsProsoft = false;
                bool IsSend = false;

                if (ViewBag.IsLocalHost != null)
                {
                    //MailTo = new List<string>() { "supawadee@prosoft.co.th" };
                    MailTo = new List<string>();

                    MailCC = new List<string>(1);
                }

                foreach (string to in MailTo)
                {
                    myMessage.AddTo(to);
                    if (to.Contains("prosoft.co.th"))
                        IsProsoft = true;
                }

                #endregion

                #region Mail CC
                string strMailCC = string.Empty;
                foreach (string cc in MailCC)
                {
                    myMessage.AddCc(cc);
                }
                #endregion

                #region Send Grid Mail
                //if (!IsProsoft && ViewBag.IsLocalHost == null)
                //{

                //    if (!string.IsNullOrEmpty(ReplyTo))
                //    {
                //        var addReplyTo = new MailAddress[1];
                //        addReplyTo[0] = new MailAddress(ReplyTo, ReplyDisplay);
                //        myMessage.ReplyTo = addReplyTo;
                //    }

                //    //For Check Email
                //    //myMessage.AddBcc("chatdanai@prosoft.co.th");
                //    //myMessage.AddBcc("satita@prosoft.co.th");
                //    //myMessage.AddBcc("teerapong@prosoft.co.th");
                //    //myMessage.AddBcc("worawit@prosoft.co.th");
                //    //myMessage.AddBcc("nutthapong@prosoft.co.th");
                //    //myMessage.AddBcc("wasinee@prosoft.co.th");

                //    myMessage.From = new MailAddress(MailFrom, MailFromDisplay);
                //    Console.WriteLine("1. Add MailFrom Success...");
                //    myMessage.Subject = Subject;
                //    myMessage.Html = Detail;

                //    #region SMTP
                //    string[] SMTP_Username = new string[] { 
                //        "azure_c1c6237e12cc262397865f738deeeaba@azure.com" ,
                //        "azure_0d500a3077296c97deb0a861a28c5a97@azure.com"
                //    };

                //    string[] SMTP_Password = new string[] { 

                //        "8yldvfxt" ,
                //        "qjlpwiei"
                //    };

                //    var rng = new Random();
                //    int ran_no = rng.Next(SMTP_Username.Count());
                //    // Create credentials, specifying your user name and password.
                //    var credentials = new NetworkCredential(SMTP_Username[ran_no], SMTP_Password[ran_no]);

                //    // Create an SMTP transport for sending email.
                //    var transportSMTP = SMTP.GetInstance(credentials);

                   
                //    #endregion

                //    try
                //    {
                //        transportSMTP.Deliver(myMessage);
                //        IsSend = true;                        
                //    }
                //    catch { 
                //        IsSend = false; 
                //    }
                //    return IsSend;
                //}
                //else
                //{
                #endregion

                #region Old Send Email For Test Local
                #region Set SMTP
                bool sendSuccess = false;
                EmailManager emailManager = new EmailManager("mail.ouikum.com" , "support@ouikum.com", "Support@ouikum.com", true);
                    //EmailManager emailManager = new EmailManager(GetAppSetting("SMTP_Server"), GetAppSetting("SMTP_UserName"), GetAppSetting("SMTP_Password"), Convert.ToBoolean(GetAppSetting("SMTP_IsAuthentication")));
                    #endregion

                    #region Mail To
                    string strMailTo = string.Empty;
                    foreach (string to in MailTo)
                    {
                        if (!string.IsNullOrEmpty(strMailTo))
                            strMailTo = string.Concat(strMailTo, ";");
                        strMailTo = string.Concat(strMailTo, to.Replace(";", string.Empty));
                    }
                    #endregion

                    #region Mail CC
                    //string strMailCC = string.Empty;
                    foreach (string cc in MailCC)
                    {
                        if (!string.IsNullOrEmpty(strMailCC))
                            strMailCC = string.Concat(strMailCC, ";");
                        strMailCC = string.Concat(strMailCC, cc.Replace(";", string.Empty));
                    }
                    #endregion

                    #region Send Mail
                    //emailManager.Bcc = "chatdanai@prosoft.co.th;satita@prosoft.co.th;";
                    emailManager.Bcc = "info@ouikum.com;";
                    emailManager.Port = 25;
                    emailManager.BodyEncoding = Encoding.UTF8;
                    emailManager.SubjectEncoding = Encoding.UTF8;
                    emailManager.IsBodyHtml = true;
                    emailManager.Form = MailFrom;
                    emailManager.To = strMailTo;
                    emailManager.CC = strMailCC;
                    emailManager.Subject = Subject;
                    emailManager.Body = Detail;
                    //emailManager.Reply = "chatdanai@prosoft.co.th";
                    try
                    {
                        sendSuccess = emailManager.SendEmail();
                    }
                    catch { sendSuccess = false; }
                    return sendSuccess;
                    #endregion
                    #endregion

                //}

            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion

        #region OnSendByAlertEmailOld2
        // send to Custormer Email From MsgCenter
        public bool OnSendByAlertEmail2(string Subject, string MailFrom, string MailTo, List<string> MailCC, string Detail, string AttachmentName = "", string AttachmentPath = "")
        {
            //Set SMTP
            bool sendSuccess = false;
            //var SMTP_Server = res.Config.EmailServer;
            //var SMTP_UserName = res.Config.EmailAlert;
            //var SMTP_Password = "alert@12345";
            //var SMTP_Server = "mail.prosoft.co.th";
            //var SMTP_UserName = "ppnet@prosoft.co.th";
            //var SMTP_Password = "ppnet@12345";

            var SMTP_Server = "mail.ouikum.com";
            var SMTP_UserName = "support@ouikum.com";
            var SMTP_Password = "Support@6655";
            var SMTP_IsAuthentication = true;

            #region Mail To
            string strMailTo = string.Empty;
            strMailTo = MailTo;
            //foreach (string to in MailTo)
            //{
                //if (!string.IsNullOrEmpty(strMailTo))
                //    strMailTo = string.Concat(strMailTo, ";");
                //strMailTo = string.Concat(strMailTo, to.Replace(";", string.Empty));
            //}
            #endregion

            #region Mail CC
            string strMailCC = string.Empty;
            foreach (string cc in MailCC)
            {
                if (!string.IsNullOrEmpty(strMailCC))
                    strMailCC = string.Concat(strMailCC, ";");
                strMailCC = string.Concat(strMailCC, cc.Replace(";", string.Empty));
            }
            #endregion

            #region Send Mail
            EmailManager emailManager = new EmailManager(SMTP_Server, SMTP_UserName, SMTP_Password, SMTP_IsAuthentication);
            emailManager.Port = 25;
            emailManager.IsBodyHtml = true;
            emailManager.Form = MailFrom;
            //emailManager.mFrom = MailFrom;
            emailManager.mDisplayName = "Ouikum.com";
            emailManager.To = strMailTo;
            emailManager.CC = strMailCC;
            emailManager.Subject = Subject;
            emailManager.Body = Detail;
            emailManager.BodyEncoding = Encoding.UTF8;
            emailManager.SubjectEncoding = Encoding.UTF8;

            if (AttachmentName != null && AttachmentName != "")
            {
                if (AttachmentPath != null && AttachmentPath != "")
                {
                    emailManager.mAttachmentName = AttachmentName;
                    emailManager.mAttachmentPath = AttachmentPath;
                }
            }

            try
            {
                sendSuccess = emailManager.SendEmail();
            }
            catch { sendSuccess = false; }
            return sendSuccess;
            #endregion
        }

        #endregion


        #region OnSendByAlertEmailOld
        // send to Custormer Email From MsgCenter
        public bool OnSendByAlertEmail(string Subject, string MailFrom, List<string> MailTo, List<string> MailCC, string Detail, string AttachmentName = "", string AttachmentPath = "")
        {
            //Set SMTP
            bool sendSuccess = false;
            //var SMTP_Server = res.Config.EmailServer;
            //var SMTP_UserName = res.Config.EmailAlert;
            //var SMTP_Password = "alert@12345";
            //var SMTP_Server = "mail.prosoft.co.th";
            //var SMTP_UserName = "ppnet@prosoft.co.th";
            //var SMTP_Password = "ppnet@12345";

            var SMTP_Server = "mail.ouikum.com";
            var SMTP_UserName = "support@ouikum.com";
            var SMTP_Password = "Support@6655";
            var SMTP_IsAuthentication = true;

            #region Mail To
            string strMailTo = string.Empty;
            foreach (string to in MailTo)
            {
                if (!string.IsNullOrEmpty(strMailTo))
                    strMailTo = string.Concat(strMailTo, ";");
                strMailTo = string.Concat(strMailTo, to.Replace(";", string.Empty));
            }
            #endregion

            #region Mail CC
            string strMailCC = string.Empty;
            foreach (string cc in MailCC)
            {
                if (!string.IsNullOrEmpty(strMailCC))
                    strMailCC = string.Concat(strMailCC, ";");
                strMailCC = string.Concat(strMailCC, cc.Replace(";", string.Empty));
            }
            #endregion

            #region Send Mail
            EmailManager emailManager = new EmailManager(SMTP_Server, SMTP_UserName, SMTP_Password, SMTP_IsAuthentication);
            emailManager.Port = 25;
            emailManager.IsBodyHtml = true;
            emailManager.Form = MailFrom;
            //emailManager.mFrom = MailFrom;
            emailManager.mDisplayName = "Ouikum.com";
            emailManager.To = strMailTo;
            emailManager.CC = strMailCC;
            emailManager.Subject = Subject;
            emailManager.Body = Detail;
            emailManager.BodyEncoding = Encoding.UTF8;
            emailManager.SubjectEncoding = Encoding.UTF8;

            if (AttachmentName != null && AttachmentName != "")
            {
                if (AttachmentPath != null && AttachmentPath != "")
                {
                    emailManager.mAttachmentName = AttachmentName;
                    emailManager.mAttachmentPath = AttachmentPath;
                }
            }

            try
            {
                sendSuccess = emailManager.SendEmail();
            }
            catch { sendSuccess = false; }
            return sendSuccess;
            #endregion
        }

        #endregion

        #region GetMailListB2BAdmin
        public List<string> GetMailListB2BAdmin()
        {
            var listMail = new List<string>();

            #region Add listMail

            listMail.Add("info@ouikum.com");
            listMail.Add("admin@ouikum.com");
            //listMail.Add("specialist@B2BThai.com");
            //listMail.Add("warakorn@prosoft.co.th");
            //listMail.Add("supawadee@prosoft.co.th");

            #endregion

            return listMail;
        }
        #endregion

        #region GetMailListB2BAdminSubport
        public List<string> GetMailListB2BAdminSubport()
        {
            var listMail = new List<string>();
            #region Add listMail
            listMail.Add("info@ouikum.com");
            listMail.Add("admin@ouikum.com");
            //listMail.Add("specialist@B2BThai.com");
            //listMail.Add("warakorn@prosoft.co.th");
            //listMail.Add("supawadee@prosoft.co.th");
            //listMail.Add("titirut@prosoft.co.th");
            #endregion
            return listMail;
        }
        #endregion

        #region GetUserForSendMail
        public void GetUserForSendMail()
        {
            if (CheckIsLogin())
            {
                var svCompany = new CompanyService();
                ViewBag.MailCompany = svCompany.SelectData<b2bCompany>(" * ", " CompID = " + LogonCompID).First();
            }
        }
        #endregion

        #region OnSendMailContactUs
        public bool OnSendMailContactUs(FormCollection form)
        {
            #region variable
            var Detail = "";
            var mailTo = new List<string>();
            var mailCC = new List<string>();
            Hashtable EmailDetail = new Hashtable();
            #endregion

            #region Set Content & Value For Send Email

            string Subject = "ติดต่อ-สอบถาม เรื่อง : " + form["Subject"];
            string b2bthai_url = res.Pageviews.UrlWeb;
            string pathlogo = b2bthai_url + "/Content/Default/logo/Ouikum/img_Logo120x74.png";



            EmailDetail["b2bthaiUrl"] = b2bthai_url;
            EmailDetail["pathLogo"] = pathlogo;


            EmailDetail["Name"] = form["Name"];
            EmailDetail["CompName"] = form["CompName"];
            EmailDetail["Email"] = form["Email"];
            EmailDetail["Phone"] = form["Phone"];
            EmailDetail["Subject"] = Subject;
            EmailDetail["Detail"] = form["Detail"];

            // data for set msg detail
            ViewBag.Data = EmailDetail;
            Detail = PartialViewToString("UC/Email/ContactUs");

            var mailFrom = res.Config.EmailNoReply;
            #endregion

            return OnSendMail(Subject, mailFrom, GetMailListB2BAdminSubport(), mailCC, Detail);
        }
        #endregion

        #region OnSendMailInformUserName
        //public bool OnSendMailInformUserName(int CompID)
        //{
        //    var svComp= new CompanyService();
        //    var company = svComp.GetCompMemberByCompID(CompID);
        //    #region variable
        //    var Detail = "";
        //    var mailTo = new List<string>();
        //    var mailCC = new List<string>();
        //    Hashtable EmailDetail = new Hashtable();
        //    #endregion

        //    #region Set Content & Value For Send Email
             
        //    string b2bthai_url = res.Pageviews.UrlWeb;
        //    string pathlogo = b2bthai_url + "/Content/Default/Images/bg_Logo.jpg";
        //    var subject = "แจ้งข้อมูลการสมัคร b2bthai.com";


        //    EmailDetail["b2bthaiUrl"] = b2bthai_url;
        //    EmailDetail["pathLogo"] = pathlogo;


        //    EmailDetail["CompName"] = company.CompName;
        //    EmailDetail["UserName"] = company.UserName;
        //    EmailDetail["Password"] = company.Password;

        //    mailTo.Add(company.ContactEmail);
        //    // data for set msg detail
        //    ViewBag.Data = EmailDetail;
        //    Detail = PartialViewToString("UC/Email/InformUserName");

        //    var mailFrom = res.Config.EmailNoReply;
        //    #endregion

        //    return OnSendByAlertEmail(subject, mailFrom, mailTo, mailCC, Detail);
        //}
        #endregion

        #region OnSendMailInformUserName
        public bool OnSendMailInformUserName(string UserName, string Password, string FirstName, string Email, int CompLevel)
        {
            var svComp = new CompanyService();

            #region variable
            var Detail = "";
            var mailTo = new List<string>();
            var mailCC = new List<string>();
            Hashtable EmailDetail = new Hashtable();
            #endregion

            #region Set Content & Value For Send Email

            string b2bthai_url = res.Pageviews.UrlWeb;
            string pathlogo = b2bthai_url + "/Content/Default/logo/Ouikum/img_Logo120x74.png";
            string pathGold = "https://ouikumstorage.blob.core.windows.net/upload/Content/Email/images/icon_Gold130.png";
            string pathF = b2bthai_url + "/Content/Default/images/icon_freesmall.png";
            string pathG = b2bthai_url + "/Content/Default/images/icon_goldsmall.png";
            var subject = res.Email.lblSubject;

            EmailDetail["b2bthaiUrl"] = b2bthai_url;
            EmailDetail["pathLogo"] = pathlogo;
            EmailDetail["pathGold"] = pathGold;

            if (CompLevel == 1)
            {
                EmailDetail["pathPackage"] = pathF;
                EmailDetail["pathGF"] = " Free Member";
                EmailDetail["color"] = "#0099CC";
            }
            else if (CompLevel == 3)
            {
                EmailDetail["pathPackage"] = pathG;
                EmailDetail["pathGF"] = " Gold Member";
                EmailDetail["color"] = "#FF9933";
            }
            else
            {
                EmailDetail["pathPackage"] = "";
                EmailDetail["pathGF"] = "";
                EmailDetail["color"] = "";
            }

            EmailDetail["FirstName"] = FirstName;
            EmailDetail["UserName"] = UserName;
            EmailDetail["Password"] = Password;

            mailTo.Add(Email);
            ViewBag.Data = EmailDetail;
            Detail = PartialViewToString("UC/Email/SendMailInformUserName");

            var mailFrom = res.Config.EmailNoReply;
            #endregion

            return OnSendByAlertEmail(subject, mailFrom, mailTo, mailCC, Detail);
        }
        #endregion


        #region OnSendMailActivate
        //public bool OnSendMailActivate(string FirstName, string Email, string CompName)
        //{
        //    #region variable
        //    var Detail = "";
        //    var mailTo = new List<string>();
        //    var mailCC = new List<string>();
        //    Hashtable EmailDetail = new Hashtable();
        //    #endregion

        //    #region Set Content & Value For Send Email

        //    string b2bthai_url = res.Pageviews.UrlWeb;
        //    string pathlogo = b2bthai_url + "/Content/Default/logo/b2bthai/img_Logo120x74.png";

        //    var subject = res.Email.lblActivateSubject;

        //    EmailDetail["b2bthaiUrl"] = b2bthai_url;
        //    EmailDetail["pathLogo"] = pathlogo;

        //    EmailDetail["FirstName"] = FirstName;
        //    EmailDetail["CompName"] = CompName;

        //    mailTo.Add(Email);
        //    //mailTo.Add("supawadee@prosoft.co.th");
        //    ViewBag.Data = EmailDetail;
        //    Detail = PartialViewToString("UC/Email/SendMailActivate");

        //    var mailFrom = res.Config.EmailNoReply;
        //    #endregion

        //    return OnSendByAlertEmail(subject, mailFrom, mailTo, mailCC, Detail);
        //}
        #endregion

        #endregion

    }
}