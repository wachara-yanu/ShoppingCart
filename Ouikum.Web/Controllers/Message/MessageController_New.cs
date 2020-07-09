using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ouikum.Company;
using Ouikum.Product;
using Ouikum.Buylead;
using Ouikum;
using Ouikum.Message;
using Prosoft.Service;
using Ouikum.Common;
using res = Prosoft.Resource.Web.Ouikum;
using System.Runtime.Caching;
using System.Threading;

namespace Ouikum.Controllers
{
    public partial class MessageController : BaseController
    {
        CompanyService svCompany;
        ProductService svProduct;
        BuyleadService svBuylead;

        #region Get New
        public ActionResult New()
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
                CountMessage();
                CountQuotation();
                ViewBag.CateLevel1 = LogonCompLevel;
                ViewBag.MemberType = LogonMemberType;
                GetStatusUser();
                CountMessage();
                ViewBag.PageType = "Message";
                ViewBag.MenuName = "New";
                return View();
            }
        }
        #endregion

        #region New Message
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult New(FormCollection form)
        {

            if (LogonCompID > 0)
            {
                Ouikum.Message.MessageService svMessage = new Ouikum.Message.MessageService();
                svCompany = new CompanyService(); 
                string ToCompID = form["hidToCompID"];
                string[] sub_ToCompID = ToCompID.Split(',');
                int rootMsgID = DataManager.ConvertToInteger(form["hidMsgID"]);
                string sqlWhere = "";
                string msgdetail = form["MsgDetail"];
                string msgStatus = form["msgstatus"];
                var emMessageSender = new emMessage();
                var emMessageReceiver = new emMessage();
                var comp = svCompany.SelectData<b2bCompany>("CompID,CompPhone,CompName,ContactFirstName,ContactEmail", "CompID = " + LogonCompID).First();
                if (DataManager.ConvertToBool(form["IsImportance"]) == true)
                {
                    msgdetail += "<p><strong>" + res.Message_Center.lblContactImmediately + "</strong></p>";
                }
                // บันทึกในแบบร่าง
                if (msgStatus == "Draft")
                {
                    var emMessage = new emMessage();

                    var Messages = svMessage.SelectData<view_Message>("*", "MsgFolderID = 3 AND MessageID = " + DataManager.ConvertToInteger(form["hidMsgID"]));
                    #region Draft
                    if (Messages.Count > 0)
                    {
                        for (int i = 0; i < sub_ToCompID.Length; i++)
                        {
                            var sqlUpdate = "ToCompID = " + DataManager.ConvertToInteger(sub_ToCompID[i]) + " , Subject = N'" + form["txtSubject"] + "' , MsgDetail = N'" + msgdetail + "'";
                            if (!string.IsNullOrEmpty(form["hidImgFileName"]))
                            {
                                sqlUpdate += ", IsAttach = 1";
                            }
                            else
                            {
                                sqlUpdate += ", IsAttach = 0";
                            }
                            svMessage.UpdateByCondition<emMessage>(sqlUpdate, " MessageID = " + Messages[0].MessageID);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < sub_ToCompID.Length; i++)
                        {
                            emMessage.ToCompID = DataManager.ConvertToInteger(sub_ToCompID[i]);
                            emMessage.FromCompID = LogonCompID;
                            emMessage.IsFavorite = DataManager.ConvertToBool(form["IsImportance"]);
                            emMessage.Subject = form["txtSubject"];
                            emMessage.MsgDetail = msgdetail;
                            emMessage.MsgFolderID = 3;
                            emMessage.RootMessageID = 0;//rootMsgID
                            emMessage.MessageCode = emMessage.ToCompID + "-" + GetTimeStamp() + "-" + svMessage.Generate_MessageCode();
                            emMessage.MsgStatus = "3";
                            emMessage.IsSend = false;
                            emMessage.FromName = comp.ContactFirstName != null ? comp.ContactFirstName : comp.CompName;
                            emMessage.FromContactPhone = comp.CompPhone;
                            emMessage.FromEmail = comp.ContactEmail;
                            if (!string.IsNullOrEmpty(form["hidImgFileName"]))
                            {
                                emMessage.IsAttach = true;
                            }
                            else
                            {
                                emMessage.IsAttach = false;
                            }
                            #region Save Draft
                            emMessage = svMessage.InsertMessage(emMessage);
                            #endregion

                            // เช็คไฟล์แนบ
                            if (!string.IsNullOrEmpty(form["hidImgFileName"]))
                            {
                                #region Save Image Files
                                imgManager = new FileHelper();
                                imgManager.DirPath = "MessageFile/" + LogonCompID + "/" + emMessage.MessageID;
                                imgManager.DirTempPath = "Temp/MessageFile/" + LogonCompID;
                                SaveFileImage(
                                    imgManager.DirTempPath,
                                    imgManager.DirPath,
                                    form["hidImgFileName"]);
                                #endregion

                                string strFileName = form["hidImgFileName"];
                                string[] ArrayFileName = strFileName.Split('.');

                                var Attach = new emMessageAttach();
                                Attach.MessageID = emMessage.MessageID;
                                Attach.MessageCode = emMessage.MessageCode;
                                Attach.FileName = form["hidImgFileName"];
                                Attach.FileType = "." + ArrayFileName[1];
                                Attach.FilePath = "https://ouikumstorage.blob.core.windows.net/upload/MessageFile/" + LogonCompID + "/" + emMessage.MessageID + "/" + form["hidImgFileName"];
                                Attach.FileSize = 0;
                                Attach = svMessage.InsertMessageFile(Attach);
                            }
                        }
                    }
                    #endregion

                }
                else
                {
                    //ส่งข้อความ
                    for (int i = 0; i < sub_ToCompID.Length; i++)
                    {
                        // บันทึกข้อมูลลงกล่องขาออกของผู้ส่ง
                        #region Sender Message
                        emMessageSender.ToCompID = DataManager.ConvertToInteger(sub_ToCompID[i]);
                        emMessageSender.FromCompID = LogonCompID;
                        emMessageSender.IsFavorite = DataManager.ConvertToBool(form["IsImportance"]);
                        emMessageSender.Subject = form["txtSubject"];
                        emMessageSender.MsgDetail = msgdetail;
                        emMessageSender.MsgFolderID = 2;
                        emMessageSender.RootMessageID = (rootMsgID != 0) ? rootMsgID : 0;
                        emMessageSender.MessageCode = emMessageSender.ToCompID + "-" + GetTimeStamp() + "-" + svMessage.Generate_MessageCode();
                        emMessageSender.MsgStatus = "2";
                        emMessageSender.IsSend = true;
                        emMessageSender.FromName = comp.ContactFirstName != null ?comp.ContactFirstName : comp.CompName;
                        emMessageSender.FromContactPhone = comp.CompPhone;
                        emMessageSender.FromEmail = comp.ContactEmail;
                        if (!string.IsNullOrEmpty(form["hidImgFileName"]))
                        {
                            emMessageSender.IsAttach = true;
                        }
                        else
                        {
                            emMessageSender.IsAttach = false;
                        }

                        #region Save Sender Message
                        if (msgStatus == "Reply")
                        {
                            emMessageSender = svMessage.InsertMessageReply(emMessageSender, "Reply");
                        }
                        else if (msgStatus == "Forward")
                        {
                            emMessageSender = svMessage.InsertMessageReply(emMessageSender, "Forward");
                        }
                        else
                        {
                            emMessageSender = svMessage.InsertMessage(emMessageSender);
                        }
                        #endregion
                        #endregion
                        // บันทึกข้อมูลลงกล่องขาเข้าของผู้รับ
                        #region Receiver Message
                        emMessageReceiver.ToCompID = DataManager.ConvertToInteger(sub_ToCompID[i]);
                        emMessageReceiver.FromCompID = LogonCompID;
                        emMessageReceiver.Subject = form["txtSubject"];
                        emMessageReceiver.MsgDetail = form["MsgDetail"];
                        emMessageReceiver.MsgFolderID = 1;
                        emMessageReceiver.IsFavorite = DataManager.ConvertToBool(form["IsImportance"]);
                        emMessageReceiver.RootMessageID = (rootMsgID != 0) ? rootMsgID : 0;
                        emMessageReceiver.MessageCode = emMessageSender.MessageCode;
                        emMessageReceiver.MsgStatus = "1";
                        emMessageReceiver.IsSend = false;
                        emMessageReceiver.FromName = comp.ContactFirstName != null ?comp.ContactFirstName : comp.CompName;
                        emMessageReceiver.FromContactPhone = comp.CompPhone;
                        emMessageReceiver.FromEmail = comp.ContactEmail;
                        if (!string.IsNullOrEmpty(form["hidImgFileName"]))
                        {
                            emMessageReceiver.IsAttach = true;
                        }
                        else
                        {
                            emMessageReceiver.IsAttach = false;
                        }

                        #region Save Receiver Message

                        if (msgStatus == "Reply")
                        {
                            emMessageReceiver = svMessage.InsertMessageReply(emMessageReceiver, "Reply");
                        }
                        else if (msgStatus == "Forward")
                        {
                            emMessageReceiver = svMessage.InsertMessageReply(emMessageReceiver, "Forward");
                        }
                        else
                        {
                            emMessageReceiver = svMessage.InsertMessage(emMessageReceiver);
                        }
                        #endregion

                        #endregion
                        // เช็คไฟล์แนบ
                        if (!string.IsNullOrEmpty(form["hidImgFileName"]))
                        {
                            // บันทึกข้อมูลลงกล่องขาออกของผู้ส่ง
                            #region Save Image Files
                            imgManager = new FileHelper();
                            imgManager.DirPath = "MessageFile/" + LogonCompID + "/" + emMessageSender.MessageID;
                            imgManager.DirTempPath = "Temp/MessageFile/" + LogonCompID;
                            SaveFileImage(
                                imgManager.DirTempPath,
                                imgManager.DirPath,
                                form["hidImgFileName"]);
                            #endregion

                            string strFileName1 = form["hidImgFileName"];
                            string[] ArrayFileName1 = strFileName1.Split('.');

                            var Attach1 = new emMessageAttach();
                            Attach1.MessageID = emMessageSender.MessageID;
                            Attach1.MessageCode = emMessageSender.MessageCode;
                            Attach1.FileName = form["hidImgFileName"];
                            Attach1.FileType = "." + ArrayFileName1[1];
                            Attach1.FilePath = "https://ouikumstorage.blob.core.windows.net/upload/MessageFile/" + LogonCompID + "/" + emMessageSender.MessageID + "/" + form["hidImgFileName"];
                            Attach1.FileSize = 0;
                            Attach1 = svMessage.InsertMessageFile(Attach1);

                            // บันทึกข้อมูลลงกล่องขาเข้าของผู้รับ
                            #region Save Image Files
                            imgManager = new FileHelper();
                            imgManager.DirPath = "MessageFile/" + LogonCompID + "/" + emMessageReceiver.MessageID;
                            imgManager.DirTempPath = "Temp/MessageFile/" + LogonCompID;
                            SaveFileImage(
                                imgManager.DirTempPath,
                                imgManager.DirPath,
                                form["hidImgFileName"]);
                            #endregion

                            string strFileName2 = form["hidImgFileName"];
                            string[] ArrayFileName2 = strFileName2.Split('.');

                            var Attach2 = new emMessageAttach();
                            Attach2.MessageID = emMessageReceiver.MessageID;
                            Attach2.MessageCode = emMessageReceiver.MessageCode;
                            Attach2.FileName = form["hidImgFileName"];
                            Attach2.FileType = "." + ArrayFileName2[1];
                            Attach2.FilePath = "https://ouikumstorage.blob.core.windows.net/upload/MessageFile/" + LogonCompID + "/" + emMessageReceiver.MessageID + "/" + form["hidImgFileName"];
                            Attach2.FileSize = 0;
                            Attach2 = svMessage.InsertMessageFile(Attach2);
                        }

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
                                SendEmail(emMessageReceiver, toCompName, toCompEmail);
                            }
                            #endregion
                        }
                        else
                        {
                            if (svMessage.IsResult)
                            {
                                SendEmailNoMember(emMessageReceiver, form["hidNameNotMember"], form["hidEmailNotMember"]);
                            }
                        }
                    }
                }

                GetStatusUser();
                CountMessage();

                return View();
            }
            else
            {
                return Redirect(res.Pageviews.PvMemberSignUp);
            }

        }
        #endregion

        #region Get Contact

        [HttpGet]
        public ActionResult Contact(int? ToCompID, string ProductID, string type,string BuyleadID)
        {
            if (RedirectToProduction())
                return Redirect(UrlProduction);

            CommonService svCommon = new CommonService();
            #region Company Info
            if (ToCompID > 0)
            {

                svCompany = new CompanyService();
                string sqlSelect = "CompID,CompName,ContactEmail,CompPhone";
                string sqlWhere = svCompany.CreateWhereAction(CompStatus.Online, ToCompID);
                var countcompany = svCompany.CountData<view_Company>(sqlSelect, sqlWhere);
                if (countcompany > 0)
                {
                    var company = svCompany.SelectData<view_Company>(sqlSelect, sqlWhere).First();
                    ViewBag.Company = company;
                    ViewBag.CompName = company.CompName;
                }
                else
                {
                    return Redirect("~/Default/NotFound");
                }
            }
            #endregion

            #region Load Feature
            var feature = new List<view_HotFeaProduct>();
            if (MemoryCache.Default["LoadFeatureMessage"] != null)
            {
                feature = (List<view_HotFeaProduct>)MemoryCache.Default["LoadFeatureMessage"];
                feature = feature.OrderBy(x => Guid.NewGuid()).ToList();
                feature = feature.OrderByDescending(m => m.HotPrice).ToList();
            }
            else
            {
                var svHotFeat = new HotFeaProductService();
                var SQLSelect_Feat = "";

                SQLSelect_Feat = " ProductID,ProductName,CompID,ProductImgPath,ProRowFlag,CompRowFlag,ProvinceName,Price,Ispromotion,PromotionPrice,HotPrice";
                feature = svHotFeat.SelectHotProduct<view_HotFeaProduct>(SQLSelect_Feat, "Rowflag = 3 AND Status = 'P' AND ProductID > 0 AND ProRowFlag in(4) AND CompRowFlag in(2,4) AND ProductDelete = 0", "NEWID(),HotPrice DESC", 1, 3);//""
                if (svHotFeat.TotalRow > 0)
                {
                    MemoryCache.Default.Add("LoadFeatureMessage", feature, DateTime.Now.AddMinutes(10));
                }
            }
            ViewBag.FeatProducts = feature;
            #endregion

            #region Product
            if (!string.IsNullOrEmpty(ProductID))
            {
                svProduct = new ProductService();

                #region Set Cookie ProductID
                Response.Cookies["MsgContactProID"].Value = ProductID;
                Response.Cookies["MsgContactProID"].Expires = DateTime.Now.AddHours(1);
                #endregion

                string[] strProductID = ProductID.Split(',');
                string WhereIN = CreateWhereIN(strProductID, "ProductID");
                var product = svProduct.SelectData<b2bProduct>("ProductID,ProductName", WhereIN + " AND IsDelete = 0");
                ViewBag.ProductID = product;
                if (svProduct.TotalRow > 0) {
                    ViewBag.Title = "ติดต่อผู้ขายสินค้า " + product.First().ProductName + ", " + ViewBag.CompName + " – B2Bthai.com";
                    ViewBag.PronameUrl = @Url.ReplaceUrl(product.First().ProductName);
                }
                ViewBag.chkProductID = 1;
            }
            else
            {
                ViewBag.chkProductID = 0;
                ViewBag.Title = "ติดต่อบริษัท" + ViewBag.CompName + " – B2Bthai.com";
                Response.Cookies["MsgContactProID"].Value = null;
                Response.Cookies["MsgContactProID"].Expires = DateTime.Now.AddHours(1);
            }
            #endregion

            #region Buylead
            if (!string.IsNullOrEmpty(BuyleadID))
            {
                svBuylead = new BuyleadService();

                #region Set Cookie ProductID
                Response.Cookies["MsgContactBuyID"].Value = BuyleadID;
                Response.Cookies["MsgContactBuyID"].Expires = DateTime.Now.AddHours(1);
                #endregion

                string[] strBuyleadID = BuyleadID.Split(',');
                string WhereIN = CreateWhereIN(strBuyleadID, "BuyleadID");
                var buylead = svBuylead.SelectData<b2bBuylead>("BuyleadID,BuyleadName,BuyleadEmail,BuyleadCompanyName", WhereIN + " AND IsDelete = 0");
                ViewBag.BuyleadID = buylead;
                if (svBuylead.TotalRow > 0)
                {
                    ViewBag.Title = "ติดต่อผู้ประกาศซื้อสินค้า " + buylead.First().BuyleadName + ", " + buylead.First().BuyleadCompanyName + " – B2Bthai.com";
                    ViewBag.BuynameUrl = @Url.ReplaceUrl(buylead.First().BuyleadName);
                }
                ViewBag.chkBuyleadID = 1;
            }
            else
            {
                ViewBag.chkBuyleadID = 0;
                ViewBag.Title = "ติดต่อบริษัท" + ViewBag.CompName + " – B2Bthai.com";
                Response.Cookies["MsgContactBuyID"].Value = null;
                Response.Cookies["MsgContactBuyID"].Expires = DateTime.Now.AddHours(1);
            }
            #endregion

            #region check message type
            if (type == "SendtoFriend")
            {
                ViewBag.chkSendToFriend = 1;
                ViewBag.Title = "ส่งรายละเอียดให้เพื่อน " + ViewBag.CompName + " – B2Bthai.com";
            }
            else
            {
                ViewBag.chkSendToFriend = 0;
            }
            #endregion

            ViewBag.EnumServiceType = svCommon.SelectEnum(CommonService.EnumType.SearchByServiceType);
            GetStatusUser();
            ViewBag.MetaKeyword = ViewBag.Title;
            ViewBag.MetaDescription=ViewBag.Title;
            return View();


        }

        #endregion

        #region Post Contact
        [HttpPost, ValidateInput(false)]
        //[ValidateAntiForgeryToken]
        public ActionResult Contact(
            string hidToCompID,
            string hidToCompEmail,
            string MsgDetail,
            string IsImportance,
            string txtFromName,
            string txtFromEmail,
            string txtFromContactPhone,
            string txtSubject,
            string hidToCompName,
            string txtToEmail,
            string captcha,
            string captcha_id
        )
        {
            Ouikum.Message.MessageService svMessage = new Ouikum.Message.MessageService();
            var emMessageSender = new emMessage();
            var emMessageReceiver = new emMessage();
            int ToCompID = DataManager.ConvertToInteger(hidToCompID);
            string msgdetail = MsgDetail;
            var isResult = false;

            if (captcha == HttpContext.Session["captcha_" + captcha_id].ToString())
            {
                //if (DataManager.ConvertToBool(IsImportance) == true)
                //{
                //    msgdetail += "<p><strong>" + res.Message_Center.lblContactImmediately + "</strong></p>";
                //}

                #region Set Message

                if (LogonCompID > 0)
                {
                    #region Sender
                    emMessageSender.ToCompID = ToCompID;
                    emMessageSender.Subject = txtSubject;
                    emMessageSender.MsgDetail = msgdetail;
                    emMessageSender.RootMessageID = 0;
                    emMessageSender.MessageCode = emMessageSender.ToCompID + "-" + GetTimeStamp() + "-" + svMessage.Generate_MessageCode();
                    emMessageSender.MsgStatus = "2";
                    emMessageSender.IsSend = true;
                    emMessageSender.SendDate = DateTimeNow;
                    emMessageSender.IsFavorite = DataManager.ConvertToBool(IsImportance);
                    emMessageSender.FromCompID = LogonCompID;
                    emMessageSender.FromName = txtFromName;
                    emMessageSender.FromEmail = LogonEmail;
                    emMessageSender.MsgFolderID = 2;
                    emMessageSender.FromContactPhone = txtFromContactPhone;
                    emMessageSender.IsAttach = false;

                    emMessageSender = svMessage.InsertMessage(emMessageSender);
                    #endregion

                    #region Receiver
                    emMessageReceiver.ToCompID = ToCompID;
                    emMessageReceiver.Subject = txtSubject;
                    emMessageReceiver.MsgDetail = msgdetail;
                    emMessageReceiver.RootMessageID = 0;
                    emMessageReceiver.MessageCode = emMessageSender.MessageCode;
                    emMessageReceiver.MsgStatus = "1";
                    emMessageReceiver.IsSend = false;
                    emMessageReceiver.SendDate = DateTimeNow;
                    emMessageReceiver.IsFavorite = DataManager.ConvertToBool(IsImportance);
                    emMessageReceiver.FromCompID = LogonCompID;
                    emMessageReceiver.FromName = txtFromName;
                    emMessageReceiver.FromEmail = LogonEmail;
                    emMessageReceiver.MsgFolderID = 1;
                    emMessageReceiver.FromContactPhone = txtFromContactPhone;
                    emMessageReceiver.IsAttach = false;
                    #endregion

                    if (ToCompID > 0)
                    {
                        #region Save Message
                        emMessageReceiver = svMessage.InsertMessage(emMessageReceiver);
                        #endregion

                        #region Send Email
                        isResult = SendEmail(emMessageReceiver, hidToCompName, hidToCompEmail);
                        #endregion
                    }
                    else
                    {
                        isResult = SendEmailNoMember(emMessageReceiver, hidToCompName, txtToEmail);
                    }

                }
                else
                {
                    #region Receiver
                    emMessageReceiver.ToCompID = ToCompID;
                    emMessageReceiver.Subject = txtSubject;
                    emMessageReceiver.MsgDetail = msgdetail;
                    emMessageReceiver.RootMessageID = 0;
                    emMessageReceiver.MessageCode = emMessageReceiver.ToCompID + "-" + GetTimeStamp() + "-" + svMessage.Generate_MessageCode();
                    emMessageReceiver.MsgStatus = "1";
                    emMessageReceiver.MsgFolderID = 1;
                    emMessageReceiver.IsSend = false;
                    emMessageReceiver.SendDate = DateTimeNow;
                    emMessageReceiver.IsFavorite = DataManager.ConvertToBool(IsImportance);
                    emMessageReceiver.FromCompID = 0;
                    emMessageReceiver.FromName = txtFromName;
                    emMessageReceiver.FromEmail = txtFromEmail;
                    emMessageReceiver.FromContactPhone = txtFromContactPhone;
                    emMessageReceiver.IsAttach = false;
                    #endregion

                    if (ToCompID > 0)
                    {
                        #region Save Message
                        emMessageReceiver = svMessage.InsertMessage(emMessageReceiver);
                        #endregion

                        #region Send Email
                        isResult = SendEmail(emMessageReceiver, hidToCompName, hidToCompEmail);
                        #endregion
                    }
                    else
                    {
                        #region Sender
                        emMessageSender.ToCompID = ToCompID;
                        emMessageSender.Subject = txtSubject;
                        emMessageSender.MsgDetail = msgdetail;
                        emMessageSender.RootMessageID = 0;
                        emMessageSender.MessageCode = emMessageSender.ToCompID + "-" + GetTimeStamp() + "-" + svMessage.Generate_MessageCode();
                        emMessageSender.MsgStatus = "0";
                        emMessageSender.IsSend = false;
                        emMessageSender.SendDate = DateTimeNow;
                        emMessageSender.IsFavorite = DataManager.ConvertToBool(IsImportance);
                        emMessageSender.FromCompID = LogonCompID;
                        emMessageSender.FromName = txtFromName;
                        emMessageSender.FromEmail = LogonEmail;
                        emMessageSender.MsgFolderID = 0;
                        emMessageSender.FromContactPhone = txtFromContactPhone;
                        emMessageSender.IsAttach = false;

                        emMessageSender = svMessage.InsertMessage(emMessageSender);
                        #endregion

                        isResult = SendEmailNoMember(emMessageSender, hidToCompName, txtToEmail);
                    }


                }
            }
            //else
            //{
            //    return Json(new { IsResult = svCompany.IsResult, IsSendMail = isResult }, JsonRequestBehavior.AllowGet);
            //}
            #endregion

            GetStatusUser();

            #region Update ContactCount
            string MsgContactProID = Request.Cookies["MsgContactProID"].Value;
            if (!string.IsNullOrEmpty(MsgContactProID))
            {
                string[] strProductID = MsgContactProID.Split(',');
                for (int i = 0; i < strProductID.Length; i++)
                {
                    AddContactCount(DataManager.ConvertToInteger(strProductID[i]));
                }
            }

            #endregion
            return Json(new { IsResult = svMessage.IsResult, IsSendMail = isResult}, JsonRequestBehavior.AllowGet);
            //return Json(true);
        }

        #endregion

        #region Post ContactSupplier
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult ContactSupplier(string ToCompID, string FromCompID, string FromName, string FromEmail,
            string Subject, string MsgDetail, bool IsImportance, string ProductID, string FromContactPhone, string captcha, string captcha_id)
        {
            Ouikum.Message.MessageService svMessage = new Ouikum.Message.MessageService();
            var emMessageSender = new emMessage();
            var emMessageReceiver = new emMessage();
            svCompany = new CompanyService();
            var isResult = false;

            if (captcha == HttpContext.Session["captcha_" + captcha_id].ToString())
            {
                //if (IsImportance == true)
                //{
                //    MsgDetail += "<p><strong>" + res.Message_Center.lblContactImmediately + "</strong></p>";
                //}

                #region Set Message

                if (LogonCompID > 0)
                {
                    #region Sender
                    emMessageSender.ToCompID = DataManager.ConvertToInteger(ToCompID);
                    emMessageSender.Subject = Subject;
                    emMessageSender.MsgDetail = MsgDetail;
                    emMessageSender.RootMessageID = 0;
                    emMessageSender.MessageCode = emMessageSender.ToCompID + "-" + GetTimeStamp() + "-" + svMessage.Generate_MessageCode();
                    emMessageSender.MsgStatus = "2";
                    emMessageSender.IsSend = true;
                    emMessageSender.SendDate = DateTimeNow;
                    emMessageSender.IsFavorite = IsImportance;
                    emMessageSender.FromCompID = LogonCompID;
                    emMessageSender.FromName = FromName;
                    emMessageSender.FromEmail = LogonEmail;
                    emMessageSender.FromContactPhone = FromContactPhone;
                    emMessageSender.MsgFolderID = 2;
                    emMessageSender.IsAttach = false;

                    emMessageSender = svMessage.InsertMessage(emMessageSender);
                    #endregion

                    #region Receiver
                    emMessageReceiver.ToCompID = DataManager.ConvertToInteger(ToCompID);
                    emMessageReceiver.Subject = Subject;
                    emMessageReceiver.MsgDetail = MsgDetail;
                    emMessageReceiver.RootMessageID = 0;
                    emMessageReceiver.MessageCode = emMessageSender.MessageCode;
                    emMessageReceiver.MsgStatus = "1";
                    emMessageReceiver.IsSend = false;
                    emMessageReceiver.SendDate = DateTimeNow;
                    emMessageReceiver.IsFavorite = IsImportance;
                    emMessageReceiver.FromCompID = LogonCompID;
                    emMessageReceiver.FromName = FromName;
                    emMessageReceiver.FromEmail = LogonEmail;
                    emMessageReceiver.FromContactPhone = FromContactPhone;
                    emMessageReceiver.MsgFolderID = 1;
                    emMessageReceiver.IsAttach = false;

                    emMessageReceiver = svMessage.InsertMessage(emMessageReceiver);
                    #endregion

                    #region Send Email
                    if (ToCompID != "0")
                    {
                        var comp = svCompany.SelectData<b2bCompany>("CompID, CompName, ContactEmail", "CompID = " + emMessageReceiver.ToCompID).First();
                        isResult = SendEmail(emMessageReceiver, comp.CompName, comp.ContactEmail);
                    }
                    else
                    {
                        isResult = SendEmailNoMember(emMessageReceiver, FromName, FromEmail);
                    }
                    #endregion

                }
                else
                {
                    #region Receiver
                    emMessageReceiver.ToCompID = DataManager.ConvertToInteger(ToCompID);
                    emMessageReceiver.Subject = Subject;
                    emMessageReceiver.MsgDetail = MsgDetail;
                    emMessageReceiver.RootMessageID = 0;
                    emMessageReceiver.MessageCode = emMessageReceiver.ToCompID + "-" + GetTimeStamp() + "-" + svMessage.Generate_MessageCode();
                    emMessageReceiver.MsgStatus = "1";
                    emMessageReceiver.MsgFolderID = 1;
                    emMessageReceiver.IsSend = false;
                    emMessageReceiver.SendDate = DateTimeNow;
                    emMessageReceiver.IsFavorite = IsImportance;
                    emMessageReceiver.FromCompID = 0;
                    emMessageReceiver.FromName = FromName;
                    emMessageReceiver.FromEmail = FromEmail;
                    emMessageReceiver.FromContactPhone = FromContactPhone;
                    emMessageReceiver.IsAttach = false;

                    emMessageReceiver = svMessage.InsertMessage(emMessageReceiver);
                    #endregion

                    #region GetCompany
                    var comp = svCompany.SelectData<b2bCompany>("CompID, CompName, ContactEmail", "CompID = " + emMessageReceiver.ToCompID).First();
                    #endregion

                    #region Send Email

                    isResult = SendEmail(emMessageReceiver, comp.CompName, comp.ContactEmail);

                    #endregion


                }

                #endregion
            }
            GetStatusUser();

            #region Update ContactCount

            if (!string.IsNullOrEmpty(ProductID))
            {
                AddContactCount(DataManager.ConvertToInteger(ProductID));
            }

            #endregion
            return Json(new { IsResult = svMessage.IsResult, IsSendMail = isResult }, JsonRequestBehavior.AllowGet);
            //return Json(true);
        }

        #endregion

        #region Save File Create Message
        [HttpPost]
        public ActionResult SaveFileCreateMessage(HttpPostedFileBase FileCreateMessagePath)
        {
            imgManager = new FileHelper();

            if (LogonCompID > 0)
            {
                imgManager.UploadImage("Temp/MessageFile/" + LogonCompID, FileCreateMessagePath);
                Response.Cookies["CompID"].Value = Request.Cookies[res.Common.lblWebsite].Values["CompID"];
            }
            else
            {
                imgManager.UploadImage("Temp/MessageFile/0", FileCreateMessagePath);
                Response.Cookies["CompID"].Value = "0";
            }
            while (!imgManager.IsSuccess)
            {
                Thread.Sleep(100);
            }
            return Json(new { newimage = imgManager.ImageName }, "text/plain");
        }
        #endregion

        #region RemoveImage
        [HttpPost]
        public ActionResult RemoveFileCreateMessage()
        {
            imgManager = new FileHelper();
            imgManager.DeleteFilesInDir("Temp/MessageFile/" + LogonCompID);
            return Json(new { newimage = imgManager.ImageName });
        }
        #endregion

    }
}
