using System.Web.Mvc;
using System.Web;

using System.Text;
using System;
using System.Configuration;
using Twitterizer;
using System.Collections.Generic;

namespace Prosoft.Service
{
    public class TwitterModel
    {
        public string Message { get; set; }
    }

    public class TwitterHelper
    {
        #region Member 
        private bool _isResult;
        private OAuthTokens _AccessToken { get; set; }
        #endregion

        #region Constructor
        public TwitterHelper()
        {
            _AccessToken = new OAuthTokens()
            {// ดึงมาจาก web.config appsetting key
                AccessToken = ConfigurationManager.AppSettings["accessToken"],
                AccessTokenSecret = ConfigurationManager.AppSettings["accessTokenSecret"],
                ConsumerKey = ConfigurationManager.AppSettings["consumerKey"],
                ConsumerSecret = ConfigurationManager.AppSettings["consumerSecret"]
            };

        }
        #endregion

        #region Method

        #region PostTweets
        #region PostTweets List  
        public bool PostTweets(List<TwitterModel> model)
        {
            _isResult = false;
            try
            {
                foreach (var item in model)
                {
                TwitterResponse<TwitterStatus> response = TwitterStatus.Update(
                    _AccessToken,
                   item.Message);
                }
                _isResult = true;
            }
            catch (Exception ex)
            {

                throw;
            }

            return _isResult;
        }
        #endregion

        #region PostTweets
        public bool PostTweets(TwitterModel model)
        {
            _isResult = false;
            try
            {
                TwitterResponse<TwitterStatus> response = TwitterStatus.Update(
                    _AccessToken,
                   model.Message);
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

        #endregion
    }
}