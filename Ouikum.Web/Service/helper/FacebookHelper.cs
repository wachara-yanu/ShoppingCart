using System.Web.Mvc;
using System.Web;
using Newtonsoft.Json.Linq;
using System.Text;
using System;
using System.Configuration;
using System.Collections.Generic;
using Facebook;
namespace Prosoft.Service
{
    public class FacebookModel
    {
        public string ID { get; set; }
        public string Message { get; set; }
        public string Picture { get; set; }
        public string Link { get; set; }
    }

    public class FacebookHelper
    {
        #region Member
        private string _appId;
        private string _appSecret;
        private string _accessToken;
        private bool _isResult;
        #endregion

        #region Properties
        public string AccessToken
        {
            get
            {
                if (_accessToken == null)
                    GetAccessToken();

                return _accessToken;
            }
            set { _accessToken = value; }
        }

        #endregion

        #region Constructor    
        public FacebookHelper()
        {
            this._appId = System.Configuration.ConfigurationManager.AppSettings["Facebook_API_Key"];
            this._appSecret = System.Configuration.ConfigurationManager.AppSettings["Facebook_API_Secret"];
        }
        #endregion

        #region Method

        #region Get Me
        public void GetAccessToken()
        {  
            try 
	        { 
                _accessToken = System.Configuration.ConfigurationManager.AppSettings["FBPageAccessToken"];
	        }
	        catch (Exception ex)
	        {

	        }
        }
        #endregion

        #region Get Me
        public void GetMe()
        {
            GetAccessToken();
            var client = new FacebookClient();
               dynamic user = client.Get("/me",
                  new {
                    fields = "first_name,last_name,email",
                      access_token = _accessToken
                  });

               var userfb = user;
        }
        #endregion

        #region GetFacebookAccessToken
        public static string GetFacebookAccessToken(string code, string returnUrl, string fbRedirectUri)
        {
            var f = new FacebookClient();
            dynamic result = f.Get("oauth/access_token", new
            {
                client_id = System.Configuration.ConfigurationManager.AppSettings["Facebook_API_Key"],
                client_secret = System.Configuration.ConfigurationManager.AppSettings["Facebook_API_Secret"],
                redirect_uri = fbRedirectUri,
                code = code,
                state = returnUrl
            });
            return result.access_token as string;
        }
        #endregion

        #region GetFacebookResponse
        public static dynamic GetFacebookResponse(string actionUrl, string accessToken)
        {
            FacebookClient FbApp;
            if (string.IsNullOrEmpty(accessToken))
            {
                FbApp = new FacebookClient();
            }
            else
            {
                FbApp = new FacebookClient(accessToken);
            }
            return FbApp.Get(actionUrl) as JsonObject;
        }
        #endregion

        #region PostFeed
        public bool PostFeed(List<FacebookModel> model)
        {
            _isResult = false;
            try
            {
                GetAccessToken();
                var fb = new FacebookClient(_accessToken);
                foreach (var item in model)
                {
                    dynamic id = fb.Post("me/feed", new
                    {
                        message = item.Message,
                        picture = item.Picture,
                        link = item.Link
                    });
                } 
                _isResult = true;
            }
            catch (Exception ex)
            {

                throw;
            }

            return _isResult;
        }

        public bool PostFeed(FacebookModel model)
        {
            _isResult = false;
            try
            {
                GetAccessToken();
                var fb = new FacebookClient(_accessToken);
                
                dynamic id = fb.Post("me/feed", new
                {
                    message = model.Message,
                    picture = model.Picture,
                    link = model.Link
                });
                _isResult = true;
            }
            catch (Exception ex)
            {
                throw;
            }

            return _isResult;
        }
        #endregion

        #region Post Group Feed

        public object FeedB2BThaiGroup()
        {
            dynamic data;
            try
            {
                GetAccessToken();
                var fb = new FacebookClient(_accessToken); 
                data = fb.Get("462945393740021/feed");   
            }
            catch (Exception ex)
            { 
                throw;
            }

            return data;
        }

        public bool PostB2BThaiGroup(FacebookModel model)
        {
            _isResult = false;
            try
            {
                GetAccessToken();
                var fb = new FacebookClient(_accessToken);

                dynamic id = fb.Post("462945393740021/feed", new
                {
                    message = model.Message,
                    picture = model.Picture,
                    link = model.Link
                });
                _isResult = true;
            }
            catch (Exception ex)
            {
                throw;
            }

            return _isResult;
        }
        #endregion

        #endregion

    }

}