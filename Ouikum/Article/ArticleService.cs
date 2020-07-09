using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using Prosoft.Base;

namespace Ouikum.Article
{
    public class ArticleService : BaseSC
    {
        public int? ViewCount { get; set; }
        #region Method Article

        #region UpdateArticleViewCount
        public bool UpdateArticleViewCount(int BlogID)
        {
            var Article = SelectData<b2bArticle>("ArticleID,ViewCount", "ArticleID = " + BlogID, null, 1, 0, false).First();
            if (Article.ViewCount == 0)
            {
                ViewCount = 1;
            }
            else
            {
                ViewCount = ((int)Article.ViewCount + 1);
            }

            string sqlUpdate = " ViewCount = " + ViewCount;
            string sqlWhere = " ArticleID = " + BlogID;
            UpdateByCondition<b2bArticle>(sqlUpdate, sqlWhere);
            return IsResult;
        }
        #endregion

       
        #endregion
    }
}