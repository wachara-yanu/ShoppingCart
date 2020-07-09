#region using System
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Web.UI;
using System.Drawing;                       // Drawing Image
using System.Text;                          // StringBuilder
using System.IO;                            // StringWriter
using System.Text.RegularExpressions;       // Regex
//using System.Runtime.Caching;               // CacheFC
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
using B2B;
using B2B.Common;
using res = Prosoft.Resource.Web.B2B;
 
//using IOS.Service; 


namespace System.Web.Mvc
{
    public class PushModel
    {
        public string message { get; set; }
        public string deviceid { get; set; }
        public int badge { get; set; }
        public bool IsResult { get; set; }

        private List<Exception> _MsgError;
         
        public List<Exception> MsgError
        {
            get
            {
                if (_MsgError == null)
                    _MsgError = new List<Exception>();
                return _MsgError;
            }
            set
            {
                if (value == null)
                    _MsgError = new List<Exception>();
                else
                    _MsgError = value;
            }
        }        


    }
    public partial class BaseController : BaseClassController
    {
        public PushModel PushNotification(PushModel model)
        { 
            try
            {
                Boolean bsandbox = true;

                string p12fileName = Server.MapPath("~/Upload/Prosoft/iOS/ck_dev.pem");
                string p12password = "pushchat";

                string deviceID1 = "ee98207ba890fe56bc6402991d03334f199688fbef943a9da7a33a0565c740d0"; // 

                string alert = model.message + " at " + DateTime.Now.ToLongTimeString();
                string soundstring = "default";
                var payload1 = new NotificationPayload(deviceID1, alert, model.badge , soundstring);


                var notificationList = new List<NotificationPayload> { payload1 };
                var push = new PushNotification(bsandbox, p12fileName, p12password);
                var rejected = push.SendToApple(notificationList);
                model.IsResult = true;
            }
            catch (Exception ex)
            {
                model.MsgError.Add(ex);
                model.IsResult = true;
            }
            return model;

        }
        public PushModel TestPushPro(PushModel model)
        { 
            try
            {
                Boolean bsandbox = false;
                string p12fileName = Server.MapPath("~/Upload/Prosoft/iOS/ck_pro.pem");

                string deviceID1 = "5c13e113f17582641b3eaca090c387a6ce11f86f5ead096283e1d5a9e91e8937"; //
                string p12password = "pushchat";
                 
                string alert = model.message + " at " + DateTime.Now.ToLongTimeString();
                string soundstring = "default";
                var payload1 = new NotificationPayload(deviceID1, alert, model.badge, soundstring);
                payload1.AddCustom("custom1", model.message);
                var notificationList = new List<NotificationPayload> { payload1 };
                var push = new PushNotification(bsandbox, p12fileName, p12password);
                var rejected = push.SendToApple(notificationList);
                model.IsResult = true; 
            }
            catch (Exception ex)
            {
                model.MsgError.Add(ex);
                model.IsResult = false;
            }
            return model;

        }

        public PushModel TestPushDev(PushModel model)
        {
            try
            {
                Boolean bsandbox = true;
                string p12fileName = Server.MapPath("~/Upload/Prosoft/iOS/ck_dev.pem");

                string deviceID1 = "ee98207ba890fe56bc6402991d03334f199688fbef943a9da7a33a0565c740d0"; //
                string p12password = "pushchat";

                string alert = model.message + " at " + DateTime.Now.ToLongTimeString();
                string soundstring = "default";
                var payload1 = new NotificationPayload(deviceID1, alert, model.badge, soundstring);
                payload1.AddCustom("custom1", model.message);

                var notificationList = new List<NotificationPayload> { payload1 };
                var push = new PushNotification(bsandbox, p12fileName, p12password);
                var rejected = push.SendToApple(notificationList);
                model.IsResult = true;


            }
            catch (Exception ex)
            {
                model.MsgError.Add(ex);
                model.IsResult = false;
            }
            return model;

        }

    }
     
}