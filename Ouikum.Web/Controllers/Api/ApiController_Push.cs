using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Prosoft.Service;
using Prosoft.Base;
using B2B.Web.Models;
using B2B.Common;
using B2B.Company;
using res = Prosoft.Resource.Web.B2B;
using B2B.Message;
using B2B.Quotation;
using B2B.Product;
using JdSoft.Apple.Apns.Notifications;
namespace B2B.Web.Controllers
{
    public partial class ApiController : BaseController
    { 
        
        public string Message;
        public ApiController()
        {
            Message = "";
        }

        #region PushPro
        public ActionResult PushPro()
        {
            var model = new PushModel();
            model.badge = 14;
            model.message = "antCart App test ";
            TestPushPro(model);
            return Json(new
            {
                status = model.IsResult,
                msgerror = GenerateMsgError(model.MsgError)
            }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region PushDev
        public ActionResult PushDev()
        {
            var model = new PushModel();
            model.badge = 14;
            model.message = "antCart App test ";
            TestPushDev(model);
            return Json(new
            {
                status = model.IsResult,
                msgerror = GenerateMsgError(model.MsgError)
            }, JsonRequestBehavior.AllowGet);
        }
        #endregion



        public ActionResult PushDevice()
        {
            //Variables you may need to edit:
            //---------------------------------

            //True if you are using sandbox certificate, or false if using production
            bool sandbox = false;

            //Put your device token in here
            string testDeviceToken = "5c13e113f17582641b3eaca090c387a6ce11f86f5ead096283e1d5a9e91e8937";

            //Put your PKCS12 .p12 or .pfx filename here.
            // Assumes it is in the same directory as your app


            //This is the password that you protected your p12File 
            //  If you did not use a password, set it as null or an empty string
            string p12FilePassword = "pushchat";

            //Number of notifications to send
            int count = 3;

            //Number of milliseconds to wait in between sending notifications in the loop
            // This is just to demonstrate that the APNS connection stays alive between messages
            int sleepBetweenNotifications = 15000;

            string path = Server.MapPath("~/Upload/Prosoft/iOS/ck_pro.pem");
            //Actual Code starts below:
            //--------------------------------

            string p12Filename = path; //System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, p12File);

            NotificationService service = new NotificationService(sandbox, p12Filename, p12FilePassword, 1);

            service.SendRetries = 5; //5 retries before generating notificationfailed event
            service.ReconnectDelay = 5000; //5 seconds

            service.Error += new NotificationService.OnError(service_Error);
            service.NotificationTooLong += new NotificationService.OnNotificationTooLong(service_NotificationTooLong);

            service.BadDeviceToken += new NotificationService.OnBadDeviceToken(service_BadDeviceToken);
            service.NotificationFailed += new NotificationService.OnNotificationFailed(service_NotificationFailed);
            service.NotificationSuccess += new NotificationService.OnNotificationSuccess(service_NotificationSuccess);
            service.Connecting += new NotificationService.OnConnecting(service_Connecting);
            service.Connected += new NotificationService.OnConnected(service_Connected);
            service.Disconnected += new NotificationService.OnDisconnected(service_Disconnected);

            //The notifications will be sent like this:
            //		Testing: 1...
            //		Testing: 2...
            //		Testing: 3...
            // etc...
            for (int i = 1; i <= count; i++)
            {
                //Create a new notification to send
                Notification alertNotification = new Notification(testDeviceToken);

                alertNotification.Payload.Alert.Body = string.Format("Testing {0}...", i);
                alertNotification.Payload.Sound = "default";
                alertNotification.Payload.Badge = i;

                //Queue the notification to be sent
                if (service.QueueNotification(alertNotification))
                    Message += " - Notification Queued!";
                else
                    Message += " - Notification Failed to be Queued!";

                //Sleep in between each message
                if (i < count)
                {
                    Message += " - Sleeping " + sleepBetweenNotifications + " milliseconds before next Notification...";
                    System.Threading.Thread.Sleep(sleepBetweenNotifications);
                }
            }

            Message += " - Cleaning Up...";

            //First, close the service.  
            //This ensures any queued notifications get sent befor the connections are closed
            service.Close();

            //Clean up
            service.Dispose();

            Message += " - Done!";
            Message += " - Press enter to exit...";
            //   Console.ReadLine();
             
            return Json(new
            { 
                Message = Message
            }, JsonRequestBehavior.AllowGet);
        }



        public void service_BadDeviceToken(object sender, BadDeviceTokenException ex)
        {
            Message += " - Bad Device Token: " + ex.Message;
        }

        public void service_Disconnected(object sender)
        {
            Message += " - Disconnected...";
        }

        public void service_Connected(object sender)
        {
            Message += " - Connected...";
        }

        public void service_Connecting(object sender)
        {
            Message += " - Connecting...";
        }

        public void service_NotificationTooLong(object sender, NotificationLengthException ex)
        {
            Message += string.Format("Notification Too Long: {0}", ex.Notification.ToString());
        }

        public void service_NotificationSuccess(object sender, Notification notification)
        {
            Message += string.Format("Notification Success:  " + notification.ToString());
        }

        public void service_NotificationFailed(object sender, Notification notification)
        {
            Message += string.Format("Notification Failed: {0}" + notification.ToString());
        }

        public void service_Error(object sender, Exception ex)
        {
            Message += string.Format("Error: {0}", ex.Message);
        }
    }
}
