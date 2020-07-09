using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Mail;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;


namespace Prosoft.Service
{
    public class EmailManager
    {

        #region SendEmail

        public void SendEmail(string FromMail, string ToMail, string Subject, string Body)
        {
            var SmtpMail = new SmtpClient();

            MailMessage objEmail = new MailMessage(FromMail, ToMail, Subject, Body);
            objEmail.Priority = MailPriority.High;
            objEmail.IsBodyHtml = true;
            try
            {
                SmtpMail.Send(objEmail);
            }
            catch (Exception exc)
            {
                //dtoMember.Result.MessageError.Add(exc);
                //System.Message;
            }
            finally
            {
                objEmail = null;
            }

        }
        #endregion

        #region GenActivateCode
        public string GenActivateCode()
        {
            string ActivateCode = Guid.NewGuid().ToString();

            return ActivateCode;
        }

        #endregion

        #region Enumeration
        /// <summary>
        /// SMTP Reponse Enumeration
        /// </summary>
        private enum SMTPResponse : int
        {
            CONNECT_SUCCESS = 220,
            GENERIC_SUCCESS = 250,
            DATA_SUCCESS = 354,
            QUIT_SUCCESS = 221
        }
        #endregion

        #region Member
        //Mail Server Information
        private string mServer = string.Empty;
        private int mPort = 25;
        private int mTimeout = 0;
        private string mUserName = string.Empty;
        private string mPassword = string.Empty;
        private bool isAuthentication = true;
        //Message Information
        public string mFrom = string.Empty;
        public string mDisplayName = string.Empty;
        public string mAttachmentName = string.Empty;
        public string mAttachmentPath = string.Empty;
        public string mTo = string.Empty;
        private string mCC = string.Empty;
        private string mBcc = string.Empty;
        private string mSubject = string.Empty;
        private string mBody = string.Empty;
        private string mReply = string.Empty;
        //Message Option
        private bool isBodyHtml = true;
        private MailPriority mPriority = MailPriority.Normal;
        private Encoding mSubjectEncoding = Encoding.Default;
        private Encoding mBodyEncoding = Encoding.Default;
        private DeliveryNotificationOptions mDeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
        #endregion

        #region Constructer
        public EmailManager(string config_SMTPServer, string config_UserName, string config_Password, bool config_isAuthentication)
        {
            mServer = config_SMTPServer.Trim();
            mUserName = config_UserName.Trim();
            mPassword = config_Password.Trim();
            mFrom = config_UserName.Trim();
            mTo = string.Empty;
            mReply = string.Empty;
            isAuthentication = config_isAuthentication;
        }
        #endregion

        #region Properites
        #region Mail Server Information
        /// <summary>
        /// SMTP Server
        /// </summary>
        public string Server
        {
            get { return mServer; }
            set { mServer = value; }
        }
        /// <summary>
        /// SMPT Server Port
        /// </summary>
        //*********************8v
        public int Port
        {
            get { return mPort; }
            set { mPort = value; }
        }
        //*********************
        /// <summary>
        /// Sending Timeout
        /// </summary>
        public int Timeout
        {
            get { return mTimeout; }
            set { mTimeout = value; }
        }
        /// <summary>
        /// SMTP Username
        /// </summary>
        public string UserName
        {
            get { return mUserName; }
            set { mUserName = value; }
        }
        /// <summary>
        /// SMTP Password
        /// </summary>
        public string Password
        {
            get { return mPassword; }
            set { mPassword = value; }
        }
        /// <summary>
        /// Using SMTP Authentication
        /// </summary>
        public bool IsAuthentication
        {
            get { return isAuthentication; }
            set { isAuthentication = value; }
        }
        #endregion

        #region Message Information
        /// <summary>
        /// Send Message Form
        /// </summary>
        public string Form
        {
            get { return mFrom; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    if (CheckEmailFormat(value))
                        mFrom = value;
                }
            }
        }
        /// <summary>
        /// Send Message Reply
        /// </summary>
        public string Reply
        {
            get { return mReply; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    if (CheckEmailFormat(value))
                        mReply = value;
                }
            }
        }
        /// <summary>
        /// Send Message To
        /// </summary>
        public string To
        {
            get { return mTo; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    if (CheckEmailFormat(value))
                        mTo = value;
                }
            }
        }
        /// <summary>
        /// CC Message to
        /// </summary>
        public string CC
        {
            get { return mCC; }
            set { mCC = value; }
        }
        /// <summary>
        /// Bcc Message to
        /// </summary>
        public string Bcc
        {
            get { return mBcc; }
            set { mBcc = value; }
        }
        /// <summary>
        /// Subject's Message
        /// </summary>
        public string Subject
        {
            get { return mSubject; }
            set { mSubject = value; }
        }
        /// <summary>
        /// Body's Message
        /// </summary>
        public string Body
        {
            get { return mBody; }
            set { mBody = value; }
        }
        #endregion

        #region Message Option
        /// <summary>
        /// Use HTML Body or User Paint Text Style
        /// </summary>
        public bool IsBodyHtml
        {
            get { return isBodyHtml; }
            set { isBodyHtml = value; }
        }
        /// <summary>
        /// Mail Priority
        /// </summary>
        public MailPriority Priority
        {
            get { return mPriority; }
            set { mPriority = value; }
        }
        /// <summary>
        /// Subject Encoding Setting
        /// </summary>
        public Encoding SubjectEncoding
        {
            get { return mSubjectEncoding; }
            set { mSubjectEncoding = value; }
        }
        /// <summary>
        /// Subject Encoding Setting
        /// </summary>
        public Encoding BodyEncoding
        {
            get { return mBodyEncoding; }
            set { mBodyEncoding = value; }
        }
        /// <summary>
        /// Delivery Notification Options
        /// </summary>
        public DeliveryNotificationOptions MDeliveryNotificationOptions
        {
            get { return mDeliveryNotificationOptions; }
            set { mDeliveryNotificationOptions = value; }
        }
        #endregion
        #endregion

        #region Public Method

        #region Send Email
        /// <summary>
        /// Send Email Method
        /// </summary>
        public bool SendEmail()
        {
            bool sendSuccess = false;
            try
            {
                //Confuguratation SMTP Client
                SmtpClient smtpClient = new SmtpClient();
                smtpClient.Host = mServer;
                smtpClient.Port = mPort;

                //Input new time out
                if (mTimeout > 0)
                {
                    smtpClient.Timeout = mTimeout;
                }
                //Check Authentication Mode
                if (isAuthentication)
                {
                    //Create Network Credentail if SMTP Server Turn On Authentication Mode
                    NetworkCredential credentials = new NetworkCredential();
                    credentials.UserName = mUserName;
                    credentials.Password = mPassword;
                    smtpClient.Credentials = credentials;
                    smtpClient.EnableSsl = false;
                    smtpClient.UseDefaultCredentials = false;
              
                }
                else
                {
                    smtpClient.UseDefaultCredentials = true;
                }
                
                //Configuration Mail Information
                //Create Mail Message
                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress(mFrom, mDisplayName);
                mailMessage.Sender = new MailAddress(mFrom);

                if (mAttachmentName != null && mAttachmentName != "")
                {
                    if (mAttachmentPath != null && mAttachmentPath != "")
                    {
                        var stream = new System.Net.WebClient().OpenRead(mAttachmentPath);
                        Attachment attachement = new Attachment(stream, System.IO.Path.GetFileName(mAttachmentPath));
                        mailMessage.Attachments.Add(attachement);
                    }
                }

                mTo = mTo.Replace(',', ';');
                string[] mTos = mTo.Split(';');
                for (int i = 0; i < mTos.Length; i++)
                {
                    if (!string.IsNullOrEmpty(mTos[i]))
                        mailMessage.To.Add(mTos[i]);
                }

                mReply = mReply.Replace(',', ';');
                string[] mReplys = mReply.Split(';');
                for (int i = 0; i < mReplys.Length; i++)
                {
                    if (!string.IsNullOrEmpty(mReplys[i]))
                        mailMessage.ReplyToList.Add(mReplys[i]);
                }

                mCC = mCC.Replace(',', ';');
                string[] mCCs = mCC.Split(';');
                for (int i = 0; i < mCCs.Length; i++)
                {
                    if (!string.IsNullOrEmpty(mCCs[i]))
                        mailMessage.CC.Add(mCCs[i]);
                }

                mBcc = mBcc.Replace(',', ';');
                string[] mBccs = mBcc.Split(';');
                for (int i = 0; i < mBccs.Length; i++)
                {
                    if (!string.IsNullOrEmpty(mBccs[i]))
                        mailMessage.Bcc.Add(mBccs[i]);
                }

                mailMessage.Subject = mSubject;
                mailMessage.Body = mBody;

                //Configuration Mail Option
                mailMessage.IsBodyHtml = isBodyHtml;
                mailMessage.SubjectEncoding = mSubjectEncoding;
                mailMessage.BodyEncoding = mBodyEncoding;
                mailMessage.Priority = mPriority;
                mailMessage.DeliveryNotificationOptions = mDeliveryNotificationOptions;

                //Link Event Handler
                smtpClient.SendCompleted += new SendCompletedEventHandler(smtpClient_SendCompleted);

                //Send Email
                smtpClient.Send(mailMessage);
                sendSuccess = true;
            }
            catch (FormatException fe)
            {
                throw new FormatException("Format Exception: " + fe.Message);
            }
            catch (SmtpException se)
            {
                throw new SmtpException("SMTP Exception: " + se.Message);
            }
            catch (Exception ex)
            {
                throw new Exception("SMTP Exception: " + ex.Message);
            }
            return sendSuccess;
        }
        #endregion

        #region Check Email Format
        /// <summary>
        /// Check Email Format
        /// True : Input Email Format is Correct
        /// False : Input Email Format is incorrect
        /// </summary>
        /// <param name="inputEmailAddress">Input Email Format</param>
        /// <returns>Checking Result</returns>
        public bool CheckEmailFormat(string inputEmailAddress)
        {
            inputEmailAddress = (string.IsNullOrEmpty(inputEmailAddress)) ? "" : inputEmailAddress;
            string strRegex = @"[a-zA-Z0-9]+(.[a-zA-Z0-9_]+)*@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9_\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)";
            Regex re = new Regex(strRegex);
            if (re.IsMatch(inputEmailAddress)) return true;
            else return false;
        }
        #endregion

        #region Verify SMTP Server
        /// <summary>
        /// Verify SMTP Server Status
        /// </summary>
        /// <param name="smtpServerName">SMTP Server Name</param>
        /// <param name="smtpPort">SMTP Port</param>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <returns></returns>
        public bool VerifySMTPServer(string smtpServerName, int smtpPort, string username, string password)
        {
            IPHostEntry IPhst = Dns.GetHostEntry(smtpServerName);
            IPEndPoint endPt = new IPEndPoint(IPhst.AddressList[0], smtpPort);

            Socket s = new Socket(endPt.AddressFamily,
                         SocketType.Stream, ProtocolType.Tcp);
            try
            {
                s.Connect(endPt);
            }
            catch
            {
                //System.Diagnostics.Debug.Print(er.Message);
                //Helper.LogFile.WriteLogMessage(er.Message);
                return false;
            }

            //Attempting to connect
            if (!Check_Response(s, SMTPResponse.CONNECT_SUCCESS))
            {
                s.Close();
                return false;
            }

            //HELO server
            Senddata(s, string.Format("HELO {0}\r\n", Dns.GetHostName()));
            if (!Check_Response(s, SMTPResponse.GENERIC_SUCCESS))
            {
                s.Close();
                return false;
            }
            return true;
        }

        /// <summary>
        /// Send Data
        /// </summary>
        /// <param name="socket">Socket</param>
        /// <param name="message">Message</param>
        private void Senddata(Socket socket, string message)
        {
            byte[] _msg = Encoding.ASCII.GetBytes(message);
            socket.Send(_msg, 0, _msg.Length, SocketFlags.None);
        }

        /// <summary>
        /// Check SMTP Server Response
        /// </summary>
        /// <param name="socket">Socket</param>
        /// <param name="response_expected">SMTP Response Status</param>
        /// <returns></returns>
        private bool Check_Response(Socket socket, SMTPResponse response_expected)
        {
            string sResponse;
            int response;
            byte[] bytes = new byte[1024];
            while (socket.Available == 0)
            {
                System.Threading.Thread.Sleep(100);
            }

            socket.Receive(bytes, 0, socket.Available, SocketFlags.None);
            sResponse = Encoding.ASCII.GetString(bytes);
            response = Convert.ToInt32(sResponse.Substring(0, 3));
            if (response != (int)response_expected)
                return false;
            return true;
        }
        #endregion

        #region Get Html Template Content
        /// <summary>
        /// Get Html Template Content
        /// </summary>
        /// <param name="FilePath"></param>
        /// <returns>htmlContent</returns>
        public static string GetHtmlTemplateContent(string FilePath)
        {
            string htmlContent = string.Empty;
            if (File.Exists(FilePath))
            {
                StreamReader rd = new StreamReader(FilePath);
                htmlContent = rd.ReadToEnd();
                rd.Close();
            }
            return htmlContent;
        }
        #endregion

        #region Replace  New Value From Key in Html Template Conent
        /// <summary>
        /// GenerateHtmlTemplateContent
        /// </summary>
        /// <param name="htmlContent"></param>
        /// <param name="hastKeyValue"></param>
        /// <returns>Generated Html Content</returns>
        public static string GenerateHtmlTemplateContent(string htmlContent, Hashtable hastKeyValue)
        {
            foreach (DictionaryEntry hast in hastKeyValue)
            {
                if (hast.Value == null)
                {
                    htmlContent = htmlContent.Replace(hast.Key.ToString(), string.Empty);
                }
                else
                {
                    htmlContent = htmlContent.Replace(hast.Key.ToString(), hast.Value.ToString());
                }
            }
            return htmlContent;
        }
        #endregion

        #endregion

        #region Private Event
        /// <summary>
        /// Event : This event raise on message sending succesful.
        /// Driven :
        /// </summary>
        private void smtpClient_SendCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        #endregion

    }
}
