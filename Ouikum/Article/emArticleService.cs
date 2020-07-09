using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ouikum.Common
{
    public class emArticleService : BaseSC
    {
        public int? ViewCount { get; set; }

        #region UpdateArticleViewCount
        public bool UpdateArticleViewCount(int BlogID)
        {
            ViewCount = 0;
            var Article = SelectData<emArticle>("ArticleID,ViewCount", "ArticleID = " + BlogID, null, 1, 0, false);
            ViewCount = ((int)Article.First().ViewCount + 1);

            string sqlUpdate = " ViewCount = " + ViewCount;
            string sqlWhere = " ArticleID = " + BlogID;
            UpdateByCondition<emArticle>(sqlUpdate, sqlWhere);
            return IsResult;
        }
        #endregion
    }
}
