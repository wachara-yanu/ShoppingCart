using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ouikum.Company;
using Ouikum.Article;
using Ouikum.Common;
using System.Collections;

//using Prosoft.Base;
using Prosoft.Service;
using res = Prosoft.Resource.Web.Ouikum;

namespace Ouikum.Controllers
{
    public partial class CompWebsiteController : BaseController
    {
        #region Member
        Ouikum.Common.JobService svJob;
        #endregion

        #region Get Job
        public ActionResult Jobs(int id)
        {
            if (RedirectToProduction())
                return Redirect(UrlProduction);

            string page = "Jobs";
            int countcompany = DefaultWebsite(id, page);

            if (countcompany > 0)
            {
                if (ViewBag.CountJob > 0)
                {
                    svJob = new JobService();

                    SelectCompanyContactInfo(id, null);

                    string sqlSelect = "JobID, JobName, JobDescription,CompID, CompName, CompWebsiteUrl, ProvinceName, ModifiedDate, IsDelete";
                    string sqlWhere = "CompID =" + ViewBag.WebEmCompID + " AND IsDelete = 0";
                    var emJobs = svJob.SelectData<view_emJob>(sqlSelect, sqlWhere);
                    ViewBag.emJobs = emJobs;

                    GetStatusUser();
                    return View();
                }
                else
                {
                    GetStatusUser();
                    LinkPathCompanyWebsite((string)ViewBag.WebCompName, id);
                    return Redirect(PathWebsiteHome);
                }

            }
            else
            {
                return Redirect(res.Pageviews.PvNotFound);
            }

        }
        #endregion

        #region Job Detail
        [HttpGet]
        public ActionResult JobDetail(int id, int JobID)
        {
            svJob = new JobService();
            svCompany = new CompanyService();
            svArticle = new ArticleService();

            string page = "Jobs";
            DefaultWebsite(id, page);

            string sqlWhere = "JobID = " + JobID + " AND IsDelete = 0";
            var emJobs = svJob.SelectData<view_emJob>("*", sqlWhere).First();
            ViewBag.emJobs = emJobs;

            string sqlSelect = "ArticleID, ArticleName ,CompID";
            sqlWhere = "CompID =" + id + " AND IsDelete = 0";
            var emArticles = svArticle.SelectData<b2bArticle>(sqlSelect, sqlWhere);
            ViewBag.emArticles = emArticles;

            GetStatusUser();
            return View();
        }
        #endregion

        #region Send job to friend
        public ActionResult SendJobToFriend(FormCollection form)
        {

            #region variable
            bool IsSend = true;
            var Detail = "";
            var mailTo = new List<string>();
            var mailCC = new List<string>();
            Hashtable EmailDetail = new Hashtable();
            #endregion

            #region Set Content & Value For Send Email

            string Subject = "คุณ " + form["txtName"] + " ได้แนะนำตำแหน่งงาน " + form["hidJobName"] + " ผ่านเว็บไชต์ B2Bthai.com";
            string b2bthai_url = res.Pageviews.UrlWeb;
            string pathlogo = b2bthai_url + "/Content/Default/logo/b2bthai/img_Logo120x74.png";

            string job_url = form["hidJobUrl"];

            EmailDetail["b2bthaiUrl"] = b2bthai_url;
            EmailDetail["pathLogo"] = pathlogo;
            EmailDetail["jobUrl"] = job_url;
            EmailDetail["fromName"] = form["txtName"];
            EmailDetail["friendName"] = form["txtFriendName"];
            EmailDetail["jobTitle"] = form["hidJobName"];
            EmailDetail["comment"] = form["txtComment"];
            //EmailDetail["date"] = DateTime.Now.ToShortDateString();
            // data for set msg detail
            ViewBag.Data = EmailDetail;
            Detail = PartialViewToString("UC/Email/WebsiteJobToFriend");


            var mailFrom = res.Config.EmailAlert;
            mailTo.Add(form["txtFriendEmail"]);
            #endregion

            IsSend = OnSendByAlertEmail(Subject, mailFrom, mailTo, mailCC, Detail);


            return Json(IsSend);
        }
        #endregion

    }
}
