using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Prosoft.Service
{
    #region IPagedList
    public interface IPagedList
    {
        int TotalRow { get; set; }
        int TotalPage { get; set; }
        int PageIndex { get; set; }
        int PageSize { get; set; }
        bool IsPreviousPage { get; }
        bool IsNextPage { get; }
    }
    #endregion

    #region PagedList
    public class PagedList<T> : List<T>, IPagedList
    {
        public PagedList(IQueryable<T> source, int index, int pageSize)
        {
            this.TotalRow = source.Count();
            this.PageSize = pageSize;
            this.PageIndex = index;
            this.AddRange(source.Skip((index - 1) * pageSize).Take(pageSize).ToList());
        }

        public PagedList(List<T> source, int index, int pageSize)
        {
            this.TotalRow = source.Count();
            this.TotalPage = CalculateTotalPage(TotalRow, pageSize);
            this.PageSize = pageSize;
            this.PageIndex = index;
            this.AddRange(source.Skip((index - 1) * pageSize).Take(pageSize).ToList());
        }

        public int TotalRow { get; set; }

        public int TotalPage { get; set; }

        public int PageIndex
        {
            get;
            set;
        }

        public int PageSize
        {
            get;
            set;
        }

        public bool IsPreviousPage
        {
            get
            {
                return (PageIndex > 0);
            }
        }

        public bool IsNextPage
        {
            get
            {
                return (PageIndex * PageSize) <= TotalRow;
            }
        }

        #region CalculateTotalPage
        protected int CalculateTotalPage(int TotalRow, int SizePage)
        {
            int TotalPage = 0;
            if (TotalRow > 0)
            {
                if (SizePage > 0)
                {
                    TotalPage = TotalRow / SizePage;
                    if (TotalRow % SizePage != 0) TotalPage++; //ถ้าหารไม่ลงตัวก็ให้เพิ่มมา 1 หน้า
                    TotalPage = (TotalPage == 0) ? 1 : TotalPage; //ถ้าหารลงตัวแล้วเป็นหน้าแรก
                }
                else
                    TotalPage = TotalRow;
            }
            return TotalPage;
        }
        #endregion
    }
    #endregion

    #region Pagination
    public static class Pagination
    {
        public static PagedList<T> ToPagedList<T>(this IQueryable<T> source, int PageIndex, int PageSize)
        {
            return new PagedList<T>(source, PageIndex, PageSize);
        }

        public static PagedList<T> ToPagedList<T>(this IQueryable<T> source, int PageIndex)
        {
            return new PagedList<T>(source, PageIndex, 10);
        }
    }
    #endregion
}