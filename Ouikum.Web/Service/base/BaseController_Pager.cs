using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text.RegularExpressions;       // Regex
using res = Prosoft.Resource.Web.Ouikum;
using Prosoft.Base;
//using SoGoodWeb.Shop;

using Prosoft.Service;

namespace System.Web.Mvc
{
    public partial class BaseController : BaseClassController //สร้าง Base Controller ที่เป็นตัวกลางของการทำงาน
    {
        #region SetPager
        /// <summary>
        /// SetPager : โดยใช้ค่าเริ่มต้นของระบบ
        /// </summary>
        public void SetPager()
        {
            SetPager(0);
        }
        public void SetPager(int size,int TotalPage = 0,int TotalRow = 0)
        {
            ViewBag.PageIndex = 1;
            ViewBag.PageSize = size > 0 ? size : int.Parse(res.Config.DefaultPageSize);
            ViewBag.TotalPage = TotalPage;
            ViewBag.TotalRow = TotalRow;
            
        }

        /// <summary>
        /// SetPager : โดยกำหนดค่าที่ได้จาก FormCollection
        /// ยังไม่ได้ทำให้รองรับ Multi Grid
        /// </summary>
        /// <param name="form"></param>
        public void SetPager(FormCollection form, int TotalPage = 0, int TotalRow = 0)
        {
            int DPsize = DataManager.ConvertToInteger(res.Config.DefaultPageSize);
            int Psize = DataManager.ConvertToInteger(form["hidPageSize"], DPsize);
            int PIndex= DataManager.ConvertToInteger(form["hidPageIndex"], 1);
            ViewBag.PageIndex = PIndex;
            ViewBag.TextSearch = !string.IsNullOrEmpty(form["TextSearch"]) ? form["TextSearch"] : "";
            ViewBag.PageSize = (Psize > 0 && Psize < 1001) ? Psize : DPsize;

            ViewBag.KeySorter = form["hidKeySorter"];
            ViewBag.KeyOrder = form["hidKeyOrder"];
            ViewBag.TotalPage = TotalPage;
            ViewBag.TotalRow = TotalRow;
        }
        #endregion

        #region SQLOrderCause
        public string SQLOrderCause(bool IsHasListNo, bool IsHasLastModifiedDate)
        {
            string SQLOrder = string.Empty;
            string KeySorter = ViewBag.KeySorter;
            string KeyOrder = ViewBag.KeyOrder;

            if (!string.IsNullOrEmpty(KeySorter))
            {
                KeyOrder = (!string.IsNullOrEmpty(KeyOrder)) ? KeyOrder : " ASC ";
                SQLOrder += " " + KeySorter + " " + KeyOrder + " ";
            }
            else
            {
                SQLOrder += IsHasListNo ? " ListNo ASC ," : "";
                SQLOrder += IsHasLastModifiedDate ? " LastModifiedDate DESC " : " CreatedDate DESC ";
            }

            return SQLOrder;
        }
        #endregion
         
        #region enum MemberKey
        /// <summary>
        /// กำหนด Field Member ที่ต้องการ
        /// </summary>
        public enum MemberKey
        {
            CreatorID,
            ModifierID,
            MemberID
        }
        #endregion

        #region enum DateFieldKey
        public enum DateFieldKey
        {
            CreatedDate,
            ModifiedDate,
            LastModifiedDate
        }
        #endregion
        
        #region SQLWhereInitial
        /// <summary>
        /// SQLWhereInitial ใช้สำหรับสร้าง SQLWhere : (มี WebID) เพื่อเป็นค่าเริ่มต้นของระบบ (เนื่องจาก SQLWhere อื่นๆ จะมี AND นำหน้า)
        /// </summary>
        /// <param name="FlagAction">enum RowFlagAction</param>
        /// <returns>SQLWhere : WebID & RowFlag</returns>
        public string SQLWhereInitial(RowFlagAction FlagAction)
        {
            string SQLWhere = string.Empty;

            #region SQLWhere : RowFlag
            SQLWhere = string.Concat(SQLWhere, SQLWhereRowFlag(FlagAction));
            #endregion

            return SQLWhere;
        }
        #endregion

        #region SQLWhereRowFlag
        /// <summary>
        /// SQLWhereRowFlag ใช้สำหรับสร้าง SQLWhere : RowFlag
        /// </summary>
        /// <param name="Action">enum RowFlagAction</param>
        /// <returns>SQLWhere</returns>
        public string SQLWhereRowFlag(RowFlagAction Action)
        {
            string SQLWhere = string.Empty;
            if (Action == RowFlagAction.Trash)
                SQLWhere = " (IsJunk = 1 AND RowFlag > 0) ";
            else if (Action == RowFlagAction.BackEnd)
                SQLWhere = " (IsJunk = 0 AND RowFlag >= 0) ";
            else if (Action == RowFlagAction.FrontEnd)
                SQLWhere = " (RowFlag >= 4) ";
            return SQLWhere;
        }
        #endregion

        #region SQLWhereMember
        /// <summary>
        /// SQLWhereMember ใช้สำหรับสร้าง SQLWhere : Member ของ ViewBag.CMSMemberID
        /// </summary>
        /// <param name="MemberKey">enum MemberKey</param>
        /// <returns>SQLWhere</returns>
        public string SQLWhereMember(MemberKey MemberKey)
        {
            string SQLWhere = string.Empty;
            SQLWhere = string.Concat(" AND ", MemberKey.ToString(), " = ", ViewBag.CMSMemberID);
            return SQLWhere;
        }
        #endregion

        #region SQLWhereTextFind
        /// <summary>
        /// SQLWhereTextFind ใช้สำหรับสร้าง SQLWhere : txtFind
        /// </summary>
        /// <param name="listTextField">ชุด FieldName ที่ต้องการสร้าง SQLWhere</param>
        /// <param name="txtFind">Keyword ที่ต้องการค้นหา</param>
        /// <returns>SQLWhere</returns>
        public string SQLWhereTextFind(List<string> listTextField, string txtFind)
        {
            string SQLWhere = string.Empty;

            #region SQLWhere : txtFind
            if (!string.IsNullOrEmpty(txtFind))
            {
                txtFind = txtFind.Replace("'", "''").Trim();
                foreach (string txtField in listTextField)
                {
                    SQLWhere = !string.IsNullOrEmpty(SQLWhere) ? SQLWhere + " OR " : SQLWhere;
                    SQLWhere = string.Concat(SQLWhere, " (", txtField, " COLLATE thai_bin LIKE '%", txtFind, "%' OR ", txtField, " LIKE '%", txtFind, "%') ");
                }
            }
            #endregion

            SQLWhere = !string.IsNullOrEmpty(SQLWhere) ? string.Concat(" AND (", SQLWhere, ") ") : SQLWhere;
            return SQLWhere;
        }
        #endregion

        #region SQLWhereFieldKey
        /// <summary>
        /// SQLWhereFieldKey ใช้สำหรับสร้าง SQLWhere : Status ใช้สำหรับทุกๆ กรณี
        /// </summary>
        /// <param name="FieldName">FieldName ที่ต้องการสร้าง SQLWhere</param>
        /// <param name="Value">Keyword ที่ต้องการค้นหา</param>
        /// <returns>SQLWhere</returns>
        public string SQLWhereFieldKey(string FieldName, string Value)
        {
            string SQLWhere = string.Empty;
            SQLWhere = string.Concat(SQLWhere, " AND (", FieldName, " =" + Value.Replace("'", "''").Trim() + ") ");
            return SQLWhere;
        }
        #endregion

        #region SQLWhereDropDown
        /// <summary>
        /// SQLWhereDropDown ใช้สำหรับสร้าง SQLWhere : FieldKey = txtFind
        /// </summary>
        /// <param name="FieldName">FieldName ที่ต้องการสร้าง SQLWhere</param>
        /// <param name="Value">ค่าจาก Select -> option value</param>
        /// <returns>SQLWhere</returns>
        public string SQLWhereDropDown(string FieldName, string Value)
        {
            string SQLWhere = string.Empty;

            #region SQLWhere : FieldKey = txtFind
            if (!string.IsNullOrEmpty(FieldName) && DataManager.ConvertToInteger(Value, -1) > -1)
                SQLWhere = string.Concat(SQLWhere, " AND (", FieldName, " =" + Value + ") ");
            #endregion

            return SQLWhere;
        }        
        #endregion

        #region SQLWhereDateTimeFromPeriod
        /// <summary>
        /// SQLWhereDateRange ใช้สำหรับสร้าง SQLWhere จากการส่ง ค่า Period 
        /// </summary>
        /// <param name="Period">รับวันที่เป็น string : ex. 2012/01/01 - 2012/01/02</param>
        /// <param name="KeyName">ชื่อฟิวส์ของเวลาที่ใช้เทียบ : ex. CreatedDate </param>
        /// <returns></returns>
        public string SQLWhereDateTimeFromPeriod(string Period, string KeyName)
        {
            var lstStr = Period.Split('-');
            if (lstStr.Count() > 1) 
                return SQLWhereDateRange(lstStr[0], lstStr[1], KeyName); 
            else 
                return SQLWhereDateRange(lstStr[0], lstStr[0], KeyName); 
        }
        #endregion

        #region SQLWhereDateRange
        /// <summary>
        /// SQLWhereDateRange ใช้สำหรับสร้าง SQLWhere : Date Range
        /// </summary>
        /// <param name="txtStartDate"></param>
        /// <param name="txtEndDate"></param>
        /// <param name="FieldKey"></param>
        /// <returns></returns>
        public string SQLWhereDateRange(string txtStartDate, string txtEndDate, string FieldKey)
        {
            string SQLWhere = string.Empty;
            FieldKey = !string.IsNullOrEmpty(FieldKey) ? FieldKey : "ModifiedDate";

            #region Set Value
            if (!string.IsNullOrEmpty(txtStartDate) || !string.IsNullOrEmpty(txtEndDate))
            {
                if (!string.IsNullOrEmpty(txtStartDate) && !string.IsNullOrEmpty(txtEndDate))
                {
                    var start = ConvertStringToDateTime(txtStartDate, true);
                    var end = ConvertStringToDateTime(txtEndDate, true);
                    if (start > end)
                    {
                        string Temp = txtStartDate;
                        txtStartDate = txtEndDate;
                        txtEndDate = Temp;
                    }
                    var txtStart = ConvertStringToDateTime(txtStartDate, false);
                    var txtEnd = ConvertStringToDateTime(txtEndDate, true);

                    SQLWhere = string.Concat(SQLWhere, " AND (", FieldKey, " BETWEEN CONVERT(datetime, '", ConvertDateTimeToStringFormat(txtStart, false), "' , 103) ");
                    SQLWhere = string.Concat(SQLWhere, " AND CONVERT(datetime, '", ConvertDateTimeToStringFormat(txtEnd, true), "', 103)) ");
                }
                else if (!string.IsNullOrEmpty(txtStartDate))
                {
                    var txtStart = ConvertStringToDateTime(txtStartDate, false);
                    SQLWhere = string.Concat(SQLWhere, " AND (", FieldKey, " >= CONVERT(datetime, '", ConvertDateTimeToStringFormat(txtStart, false), "', 103)) ");
                }
                else if (!string.IsNullOrEmpty(txtEndDate))
                {
                    var txtEnd = ConvertStringToDateTime(txtEndDate, true);
                    SQLWhere = string.Concat(SQLWhere, " AND (", FieldKey, " <= CONVERT(datetime, '", ConvertDateTimeToStringFormat(txtEnd, true), "', 103)) ");
                }
            }
            #endregion

            return SQLWhere;
        }
        #endregion

        #region SQLWhereCause Old เตรียมลบ
        /// <summary>
        /// 
        /// </summary>
        /// <param name="PAction">Index | Trash | Create | Edit | Detail | etc.</param>
        /// <param name="Section">FrontEnd | BackEnd | Admin</param>
        /// <returns></returns>
        public string SQLWhereCauses(string Section, string PAction)
        {
            return SQLWhereCauses(Section, PAction, "");
        }
        public string SQLWhereCauses(string Section, string PAction, string MemberKey)
        {
            string SQLWhere = string.Empty;
            string OriginalWhere = string.Empty;

            #region SQLWhere : ViewBag.DisplayStatus
            if (!string.IsNullOrEmpty(DataManager.ConvertToString(ViewBag.DisplayStatus)) && DataManager.ConvertToInteger(ViewBag.DisplayStatus) >= 0)
           
                SQLWhere = string.Concat(SQLWhere, " AND RowFlag = ", ViewBag.DisplayStatus);
            #endregion

            #region SQLWhere : ViewBag.WorkStatus
            if (!string.IsNullOrEmpty(DataManager.ConvertToString(ViewBag.WorkStatus)) && DataManager.ConvertToInteger(ViewBag.WorkStatus) >= 0)

                SQLWhere = string.Concat(SQLWhere, " AND RowFlag = ", ViewBag.WorkStatus);
            #endregion
           
            //#region SQLWhere : DurationDate
            //if ((!string.IsNullOrEmpty(ViewBag.txtStartDate)) && (!string.IsNullOrEmpty(ViewBag.txtEndDate)))
            //    SQLWhere = string.Concat(SQLWhere, " AND ", SQLWhereDuration(ViewBag.txtStartDate, ViewBag.txtEndDate, ViewBag.DateField));
            //#endregion

            //Domain : Shop
            #region SQLWhere : ViewBag.Category
            if (!string.IsNullOrEmpty(DataManager.ConvertToString(ViewBag.CategoryID)) && DataManager.ConvertToInteger(ViewBag.CategoryID) > 0)
                SQLWhere = string.Concat(SQLWhere, " AND ", SQLWhereCategoryNode(ViewBag.CategoryID));
            #endregion

            #region SQLWhere : ViewBag.ProductStatus
            if (!string.IsNullOrEmpty(DataManager.ConvertToString(ViewBag.ProductStatus)) || !string.IsNullOrEmpty(ViewBag.ProductFilter))
            {
                string Field = !string.IsNullOrEmpty(ViewBag.ProductStatus) ? ViewBag.ProductStatus : ViewBag.ProductFilter;
                switch (Field)
                {
                    case "HotProduct":
                    case "RecomProduct":
                    case "NewProduct":
                        SQLWhere = string.Concat(SQLWhere, " AND (Is", Field, " = 1) ");
                        break;
                }
            }
            #endregion

            #region SQLWhere : ViewBag.Orderstatus
            if (!string.IsNullOrEmpty(DataManager.ConvertToString(ViewBag.OrderStatus)) && DataManager.ConvertToInteger(ViewBag.OrderStatus) > 0)
            {
                SQLWhere = string.Concat(SQLWhere, " AND (OrderStatus =" + ViewBag.OrderStatus + ")");
            }
            #endregion

            #region SQLWhere : ViewBag.DeliveryStatus
            if (!string.IsNullOrEmpty(DataManager.ConvertToString(ViewBag.DeliveryStatus)) && DataManager.ConvertToInteger(ViewBag.DeliveryStatus) > 0)
            {
                SQLWhere = string.Concat(SQLWhere, " AND (DeliverySelect =", ViewBag.DeliveryStatus, ")");
            }
            #endregion

            //Domain :Website > Menu
            #region SQLWhere : ViewBag.MenuGroupID
            if (!string.IsNullOrEmpty(DataManager.ConvertToString(ViewBag.MenuGroupID)) && DataManager.ConvertToInteger(ViewBag.MenuGroupID) > 0)
            {
                SQLWhere = string.Concat(SQLWhere, " AND MenuGroupID = ", ViewBag.MenuGroupID);
            }
            #endregion 

            #region SQLWhere : ViewBag.MenuPosition
            if (!string.IsNullOrEmpty(DataManager.ConvertToString(ViewBag.MenuPosition)) && DataManager.ConvertToInteger(ViewBag.MenuPosition) > 0)
            {
                SQLWhere = string.Concat(SQLWhere, " AND Position = ", ViewBag.MenuPosition);
            }
            #endregion 

            //Domain : Member > CMSMember
            #region SQLWhere : ViewBag.MemberType
            if (!string.IsNullOrEmpty(DataManager.ConvertToString(ViewBag.MemberType)) && DataManager.ConvertToInteger(ViewBag.MemberType) >= 0)
            {
                SQLWhere = string.Concat(SQLWhere, " AND (MemberTypeID = ", ViewBag.MemberType, ") ");
            }
            //else  
            //{
            //    SQLWhere = string.Concat(SQLWhere, " AND (MemberTypeID > 0) "); 
            //}
            #endregion

            #region SQLWhere : ViewBag.MemberStatus
            if (!string.IsNullOrEmpty(DataManager.ConvertToString(ViewBag.MemberStatus)) && DataManager.ConvertToInteger(ViewBag.MemberStatus) >= 0)
            {
                SQLWhere = string.Concat(SQLWhere, " AND (RowFlag  = ", ViewBag.MemberStatus, ") ");
            }
            #endregion

            //#region SQLWhere : ViewBag.WebCategoryID
            //if (!string.IsNullOrEmpty(DataManager.ConvertToString(ViewBag.WebCategoryID)) && DataManager.ConvertToInteger(ViewBag.WebCategoryID) > 0)
            //    SQLWhere = string.Concat(SQLWhere, " AND ", SQLWhereCategoryNode(ViewBag.WebCategoryID, "WebCategoryID", "ParentCategory"));
            //#endregion

            //Domain : ContactUs
            #region SQLWhere : ViewBag.CaptionID
            if (!string.IsNullOrEmpty(DataManager.ConvertToString(ViewBag.CaptionID)) && DataManager.ConvertToInteger(ViewBag.CaptionID) > 0)
            {
                SQLWhere = string.Concat(SQLWhere, " AND CaptionID = ", ViewBag.CaptionID);
            }
            #endregion 
            
            //Domain : WebboardTheme
            #region SQLWhere :  ViewBag.ThemeCategory
            if (!string.IsNullOrEmpty(ViewBag.ThemeCategory) && DataManager.ConvertToInteger(ViewBag.ThemeCategory) > 0)
            {
                SQLWhere = string.Concat(SQLWhere, " AND (ThemeCateID = ", ViewBag.ThemeCategory, ") ");
            }
            #endregion

            //Backend / ManageWebboard > Topic
            //#region SQLWhere : ViewBag.FindText
            //if (!string.IsNullOrEmpty(ViewBag.FindText))
            //{
            //    string FindText = (ViewBag.FindText).Replace("'", "''");
            //    string SearchWith = (ViewBag.SearchWith).Replace("'", "''");
            //    SQLWhere = string.Concat(SQLWhere, " AND ((" + SearchWith + " like '%", FindText, "%'))");// OR (TopicCode like '%", FindText, "%'))");
            //}
            //#endregion

            #region SQLWhere : ViewBag.ForumID
            if ((!string.IsNullOrEmpty(ViewBag.ForumID)) && (DataManager.ConvertToInteger(ViewBag.ForumID) > 0))
            {
                //if (DataManager.ConvertToInteger(ViewBag.ForumID) > 3)
                //    SQLWhere = string.Concat(SQLWhere, " AND (ForumID IN (4,5)) ");
                //else
                    SQLWhere = string.Concat(SQLWhere, " AND (ForumID =", ViewBag.ForumID, ") ");
            }
            #endregion

            #region SQLWhere : ViewBag.TopicCateID
            if (!string.IsNullOrEmpty(ViewBag.TopicCateID) && DataManager.ConvertToInteger(ViewBag.TopicCateID) > 0)
            {
                //if (DataManager.ConvertToInteger(ViewBag.TopicCateID) > 3)
                //    SQLWhere = string.Concat(SQLWhere, " AND (CategoryID IN (4,5)) ");
                //else
                    SQLWhere = string.Concat(SQLWhere, " AND (CategoryID =", ViewBag.TopicCateID, ") ");
            }
            #endregion

            //Domain : ManageWebboard > Icon
            #region SQLWhere : ViewBag.TypeIcon
            if (!string.IsNullOrEmpty(ViewBag.TypeIcon))
            {
                if (ViewBag.txtFind != null)
                {
                    string FindText = (ViewBag.txtFind).Replace("'", "''");

                    if (ViewBag.TypeIcon == "1")
                    {
                        SQLWhere = string.Concat(SQLWhere, " AND IconType ='1' ");
                    }
                    else
                    {
                        SQLWhere = string.Concat(SQLWhere, " AND  IconType ='2' ");
                    }
                }
                else
                {
                    if (ViewBag.TypeIcon == "1")
                    {
                        SQLWhere = string.Concat(SQLWhere, " AND IconType ='1' ");
                    }
                    else
                    {
                        SQLWhere = string.Concat(SQLWhere, " AND  IconType ='2' ");
                    }

                }
            }
            #endregion

            //Domain : ManageWebboard > Emoticon
            #region SQLWhere :  ViewBag.EmoticonSearch
            if (!string.IsNullOrEmpty(ViewBag.EmoticonSearch))
            {
                string FindText = (ViewBag.txtFind).Replace("'", "''");

                if (ViewBag.EmoticonSearch == "1")
                {
                    SQLWhere = string.Concat(SQLWhere, " AND (EmoticonName like '%", FindText, "%') ");
                }
                else if (ViewBag.EmoticonSearch == "2")
                {
                    SQLWhere = string.Concat(SQLWhere, " AND (EmoticonCode like '%", FindText, "%') ");
                }
                else
                {
                    SQLWhere = string.Concat(SQLWhere, " AND (EmoticonName like '%", FindText, "%' OR EmoticonCode like '%", FindText, "%') ");
                }
            }
            #endregion

            //Domain : ManageWebboard > TopicComment
            #region SQLWhere : ViewBag.TopicID
            if (!string.IsNullOrEmpty(ViewBag.TopicID) && DataManager.ConvertToInteger(ViewBag.TopicID) > 0)
            {
                //if (DataManager.ConvertToInteger(ViewBag.TopicID) > 3)
                //    SQLWhere = string.Concat(SQLWhere, " AND (TopicID IN (4,5)) ");
                //else
                    SQLWhere = string.Concat(SQLWhere, " AND (TopicID =", ViewBag.TopicID, ") ");
            }
            #endregion

            //Domain : Banner
            #region SQLWhere : ViewBag.BannerGroup
            if (!string.IsNullOrEmpty(ViewBag.BannerGroup) && DataManager.ConvertToInteger(ViewBag.BannerGroup) > 0)
            {
                SQLWhere = string.Concat(SQLWhere, " AND (BannerGroupID = ", ViewBag.BannerGroup, ") ");
            }
            #endregion                        

            //Backend / ManageWebboard > WebboardPoll
            #region SQLWhere :  ViewBag.WebboardPoll
            if (!string.IsNullOrEmpty(ViewBag.WebboardPoll) && DataManager.ConvertToInteger(ViewBag.WebboardPoll) > 0)
            {
                SQLWhere = string.Concat(SQLWhere, " AND (PollID = ", ViewBag.WebboardPoll, ") ");
            }
            #endregion
            
            #region SQLWhere : ViewBag.DisplayStatus
            if (!string.IsNullOrEmpty(ViewBag.DisplayStatus) && DataManager.ConvertToInteger(ViewBag.DisplayStatus) > -1)
            {
                SQLWhere = string.Concat(SQLWhere, " AND (RowFlag  = ", ViewBag.DisplayStatus, ") ");
            }
            #endregion

            //Domain : MamageWebboard > WebboardMember

            //Backend / ManageWebboard > DeleteRequest
            #region SQLWhere :  ViewBag.RequestDelete

            #region SQLWhere :  ViewBag.DisplaySearch
            if (!string.IsNullOrEmpty(ViewBag.DisplaySearch))
            {
                string FindText = (ViewBag.txtFind).Replace("'", "''");

               if(ViewBag.DisplaySearch == "1")
                {
                    SQLWhere = string.Concat(SQLWhere, " AND (DisplayName like '%", FindText, "%') ");
                }
                
               
            }

            #endregion

            #region SQLWhere : ViewBag.DisplayStatusDeleteRequest
            if (!string.IsNullOrEmpty(ViewBag.DisplayStatusDeleteRequest) && DataManager.ConvertToInteger(ViewBag.DisplayStatusDeleteRequest) > -1)
            {
                SQLWhere = string.Concat(SQLWhere, " AND (Status  = ", ViewBag.DisplayStatusDeleteRequest, ") ");
            }
            #endregion

            #region SQLWhere : ViewBag.Searchtype
            if (!string.IsNullOrEmpty(ViewBag.Searchtype) && DataManager.ConvertToInteger(ViewBag.Searchtype) > -1)
            {
                SQLWhere = string.Concat(SQLWhere, " AND (DeleteRequestType  = ", ViewBag.Searchtype, ") ");
            }
            #endregion

            #region SQLWhere :  ViewBag.DeleteRequest
            if (!string.IsNullOrEmpty(ViewBag.DeleteRequest) && DataManager.ConvertToInteger(ViewBag.DeleteRequest) > 0)
            {
                SQLWhere = string.Concat(SQLWhere, " AND (CauseID = ", ViewBag.DeleteRequest, ") ");
            }
            #endregion

            #endregion

            #region SQLWhere :  ViewBag.RankName
            if (!string.IsNullOrEmpty(ViewBag.RankName))
            {
                if (ViewBag.RankName != "0")
                {
                    string FindText = (ViewBag.txtFind).Replace("'", "''");

                    SQLWhere = string.Concat(SQLWhere, " AND (RankID = ", ViewBag.RankName, ") ");
                }
            }
            #endregion

            //Backend / ManageContent > Content
            #region SQLWhere : ViewBag.ContentName & ViewBag.ContentCateID
            if (!string.IsNullOrEmpty(ViewBag.ContentName))
                SQLWhere = string.Concat(SQLWhere, " AND ContentName LIKE '%", ViewBag.ContentName, "%' ");
            if (!string.IsNullOrEmpty(ViewBag.ContentCateID))
                SQLWhere = string.Concat(SQLWhere, " AND (ContentCategoryID = " + ViewBag.ContentCateID + " OR ParentCategory LIKE '%,"+ ViewBag.ContentCateID +",%' OR ParentCategory LIKE '"+ ViewBag.ContentCateID +",%' OR ParentCategory LIKE '"+ ViewBag.ContentCateID +"') ");
            #endregion

            //Backend / ManageContent > Category
            #region SQLWhere : ViewBag.ContentCateName & ViewBag.ContentCategoryID
            if (!string.IsNullOrEmpty(ViewBag.ContentCateName))
                SQLWhere = string.Concat(SQLWhere," AND ContentCategoryName LIKE '%", ViewBag.ContentCateName, "%'");
            if (ViewBag.ContentCategoryID != null && (int)ViewBag.ContentCategoryID > 0)
                SQLWhere = string.Concat(SQLWhere," AND (ContentCategoryID =" + ViewBag.ContentCategoryID + " OR ParentCategory IN ( '"+ViewBag.ContentCategoryID+"' , '" + ViewBag.ContentCategoryID + ",%' , '%," + ViewBag.ContentCategoryID + ",%' , '%," + ViewBag.ContentCategoryID + "'))");
            #endregion
                    
            return OriginalWhere + SQLWhere;
        }
        #endregion

        #region SQLWhereCategoryNode
        public string SQLWhereCategoryNode(string CategoryID)
        {
            return SQLWhereCategoryNode(DataManager.ConvertToInteger(CategoryID));
        }
        public string SQLWhereCategoryNode(int CategoryID)
        {
            return SQLWhereCategoryNode(CategoryID, "CategoryID","ParentCategory");
        }
        public string SQLWhereCategoryNode(int CategoryID,string IDField ,string ParentField)
        {
            string SQLWhere = string.Empty;

            if (CategoryID > 0)
            {
                SQLWhere = string.Concat(SQLWhere, " ( (", IDField, " =" + CategoryID + ") ");
                SQLWhere = string.Concat(SQLWhere, " OR (", ParentField, " LIKE '" + CategoryID + ",%') OR (", ParentField, " ='" + CategoryID + "') ");
                SQLWhere = string.Concat(SQLWhere, " OR (", ParentField, " LIKE '%," + CategoryID + ",%') OR (", ParentField, " LIKE '%," + CategoryID + "') ) ");
            }

            return SQLWhere;
        }
        #endregion
        
        #region CreateWhereIN
        /// <summary>
        /// คำอธิบาย : ใช้สร้าง เงื่อนไขใน SQL โดยมีเงื่อนไขเป็น IN 
        /// จะได้ตัวอย่างดังนี้ CategoryID IN (1,2,3,4)
        /// </summary>
        /// <param name="models">ระบุชื่อ ตาราง</param>
        /// <param name="KeyName">ระบุชื่อคีย์ ที่ต้องการSelect</param>
        /// <returns>IF SQLWhere NOT NULL 'KeyName IN (ID,ID,ID)' ELSE ''</returns>
        public string CreateWhereIN(IEnumerable<object> models, string KeyName)
        {
            string SQLWhere = "";
            foreach (object obj in models)
            {
                if (!string.IsNullOrEmpty(SQLWhere))
                    SQLWhere += ",";
                SQLWhere += obj.GetType().GetProperty(KeyName).GetValue(obj, null);
            }

            SQLWhere = !string.IsNullOrEmpty(SQLWhere) ? " AND ( " + KeyName + " IN (" + SQLWhere + ")) " : "";
            return SQLWhere;
        }
        #endregion

        #region CreateWhereIN array string
        public string CreateWhereIN(string[] strIDs, string FieldKey)
        {
            string Seperated = string.Join(",", strIDs.ToArray());
            return " (" + FieldKey + " IN ( " + (!string.IsNullOrEmpty(Seperated) ? Seperated : "0") + ")) ";
        }
        public string CreateWhereINString(string[] strIDs, string FieldKey)
        {
            var sqlwhere = "";
            if (strIDs != null & strID.Count() > 0)
            {
                foreach (var item in strIDs)
                {
                    sqlwhere += "'" + strID + "' ,";
                }
                sqlwhere = sqlwhere.Substring(0, sqlwhere.Length - 1);
                sqlwhere = " (" + FieldKey + " IN ( " + sqlwhere + ")) ";
            }

            return sqlwhere;
        }
        public string CreateWhereINString(List<string> strIDs, string FieldKey)
        {
            var sqlwhere = "";
            if (strIDs != null & strIDs.Count() > 0)
            {
                foreach (var item in strIDs)
                {
                    sqlwhere += "'" + item + "' ,";
                }
                sqlwhere = sqlwhere.Substring(0, sqlwhere.Length - 1);
                sqlwhere = " (" + FieldKey + " IN ( " + sqlwhere + ")) ";
            }

            return sqlwhere;
        }

        public string CreateWhereIN(int[] strIDs, string FieldKey)
        {
            string Seperated = string.Join(",", strIDs.ToArray());
            return " (" + FieldKey + " IN ( " + (!string.IsNullOrEmpty(Seperated) ? Seperated : "0") + ")) ";
        }

        public string CreateWhereIN(List<int> id, string FieldKey)
        {
            string Seperated = string.Join(",", id.ToArray());
            return " (" + FieldKey + " IN ( " + (!string.IsNullOrEmpty(Seperated) ? Seperated : "0") + ")) ";
        }
        #endregion

        #region SQLWhereList
        public string SQLWhereListInt(List<int> IDs, string FieldKey)
        {
            List<string> strIDs = new List<string>();
            foreach (int id in IDs)
                strIDs.Add(id.ToString());            
            return SQLWhereListString(strIDs, FieldKey);
        }
        public string SQLWhereListString(List<string> strIDs, string FieldKey)
        {
            string Seperated = string.Join(",", strIDs.ToArray());
            return " (" + FieldKey + " IN ( " + (!string.IsNullOrEmpty(Seperated) ? Seperated : "0") + ")) ";
        }
        #endregion       
        
        #region SQLWhereMinMax
        public string SQLWhereMinMax(int num1, int num2)
        {
            return SQLWhereMinMax(DataManager.ConvertToDecimal(num1), DataManager.ConvertToDecimal(num2));
        }
        public string SQLWhereMinMax(decimal num1, decimal num2)
        {
            if (num1 > num2)
            {
                decimal Temp = num1;
                num1 = num2;
                num2 = Temp;
            }

            string SQLWhere = string.Empty;
            SQLWhere += " (";
            SQLWhere += "(MinBuy <= " + num1 + " AND MaxBuy >= " + num1 + ") OR ";
            SQLWhere += "(MinBuy <= " + num1 + " AND MaxBuy >= " + num2 + ") OR ";
            SQLWhere += "(MinBuy <= " + num2 + " AND MaxBuy >= " + num2 + ") ";
            SQLWhere += ") ";
            return SQLWhere;
        }
        #endregion

        #region ConvertStringToDateTime
        /// <summary>
        /// ใช้ในการ convert stringของวันที่ ให้เป็น DateTime และ มี Formate ที่ถูกต้อง 
        /// </summary>
        /// <param name="date">string date</param>
        /// <param name="IsFinishDate">วันสิ้นสุด หรือ วันเริ่มต้น</param>
        /// <returns></returns>
       /* public string ConvertStringToDateTime(string date, bool IsFinishDate)
        {
            string DateFormat = "yyyy-MM-dd";
            DateTime dateTime;
            IFormatProvider theCultureInfo = new System.Globalization.CultureInfo(ApplicationManager.CurrentLanguage, true);
            dateTime = DateTime.Parse(date, theCultureInfo);
            //var dt = dateTime.ToString(DateFormat).Split('/');
            //if (dt.Length > 0)
            //{
            //    var i = int.Parse( dt[dt.Length -1]);
            //    if (i > 2300) 
            //        date = dt[0] + "/" + dt[1] + "/" + dt[2];

            //    return dateTime.ToString(DateFormat) + (IsFinishDate ? " 23:59:59" : " 00:00:00");
            //}
            return dateTime.ToString(DateFormat) + (IsFinishDate ? " 23:59:59" : " 00:00:00");
        }*/

        public DateTime ConvertStringToDateTime(string date, bool IsFinishDate)
        {
            string DateFormat = "yyyy-MM-dd";
            DateTime dateTime; 
            //IFormatProvider theCultureInfo = new System.Globalization.CultureInfo("en-US", true);
           // System.IFormatProvider format = new System.Globalization.CultureInfo.CurrentCulture;
        //    dateTime = DateTime.Parse(date, System.Globalization.CultureInfo.CurrentCulture);
            dateTime = Convert.ToDateTime(date, System.Globalization.CultureInfo.CurrentCulture);
            //var dt = dateTime.ToString(DateFormat).Split('/');
            //if (dt.Length > 0)
            //{
            //    var i = int.Parse( dt[dt.Length -1]);
            //    if (i > 2300) 
            //        date = dt[0] + "/" + dt[1] + "/" + dt[2];

            //    return dateTime.ToString(DateFormat) + (IsFinishDate ? " 23:59:59" : " 00:00:00");
            //}
            return DataManager.ConvertToDateTime( dateTime.ToString(DateFormat) + (IsFinishDate ? " 23:59:59" : " 00:00:00"));
        }
         

        /// <summary>
        /// ใช้ในการ convert stringของวันที่ ให้เป็น DateTime และ มี Formate ที่ถูกต้อง 
        /// </summary>
        /// <param name="date">string date</param>
        /// <param name="IsFinishDate">วันสิ้นสุด หรือ วันเริ่มต้น</param>
        /// <returns></returns>
        public DateTime ConvertStringToDateTime(string date)
        {
            DateTime dateTime;
            dateTime = Convert.ToDateTime(date, System.Globalization.CultureInfo.CurrentCulture);
            return dateTime;
        }
        #endregion

        public string ConvertDateTimeToStringFormat(DateTime date, bool IsFinishDate, string Format = "dd/MM/yyyy")
        {
            return date.ToString(Format) + (IsFinishDate ? " 23:59:59" : " 00:00:00");
        }
    }
}
