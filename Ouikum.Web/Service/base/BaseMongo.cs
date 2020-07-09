using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.GridFS;
using MongoDB.Driver.Linq;
using PonyTaleMarket.Product;
using Prosoft.Base;
using System.Configuration;

namespace PonyTaleMarket.Web
{

    public class BaseMongo
    {
        public DateTime DateTimeNow
        {
            get
            {
                DateTime MyTime = DateTime.Now;            //Waktu Indonesia Bagian Barat
                DateTime AsiaTimeZone = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(MyTime, "SE Asia Standard Time");
                return AsiaTimeZone;
            }
        }
        public long TotalRow { get; set; }
        public long TotalPage { get; set; }
        private int StartIndex { get; set; }
        public bool IsResult { get; set; }
        public List<ResultMsg> Result { get; set; }
        
        public string connectionString;
        public MongoClient client;
        public MongoServer server;
        public MongoDatabase db;

        public BaseMongo ()
	    {
            connectionString = ConfigurationManager.AppSettings["ConnectionMongo"];
            Result = new List<ResultMsg>();
            client = new MongoClient(connectionString);
            server = client.GetServer();
            db = server.GetDatabase(ConfigurationManager.AppSettings["MongoDatabase"]);

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



        #region CalculateTotalPage
        protected int CalculateStartPage(int PageIndex, int PageSize)
        {
            int Start = 0;
            Start = (PageIndex - 1) * PageSize;
            return Start;
        }
        #endregion


         

        public MongoCollection<T> SelectData<T>(string query)
        {

            Type t = typeof(T);
            var lstModel = new List<T>();

            var collection = db.GetCollection<T>(t.Name);

            //string printQueryname = "Query: " + query;
            if (!string.IsNullOrEmpty(query))
            {
                BsonDocument query1 = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<BsonDocument>(query);
                QueryDocument queryDoc1 = new QueryDocument(query1);

               collection.Find(queryDoc1);

                return collection;
            }
            else
            {
                return collection;
            }

        }

        public IQueryable<T> SelectData<T>(string query, int PageIndex, int PageSize = 10)
        {
            var rs = SelectData<T>(query);

            var q = rs.AsQueryable<T>();
            if (PageIndex > 0)
            {
                TotalRow = rs.Count();
                TotalPage = CalculateTotalPage((int)TotalRow, PageSize);
            }

            return PagingExtensions.Page<T>(q,PageIndex,PageSize);
        }

        public MongoCursor<T> SelectDataMongo<T>(string query, int PageIndex, int PageSize = 10)
        { 
            Type t = typeof(T);
            var lstModel = new List<T>();

            var collection = db.GetCollection<T>(t.Name);

            //string printQueryname = "Query: " + query;
            if (!string.IsNullOrEmpty(query))
            {
                BsonDocument query1 = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<BsonDocument>(query);
                QueryDocument queryDoc1 = new QueryDocument(query1);
                var queryResponse = collection.FindAs<BsonDocument>(queryDoc1);
               
               // return queryResponse;
                collection.Find(queryDoc1);
                if (PageIndex > 0)
                {
                    TotalRow = collection.Find(queryDoc1).Count();
                    TotalPage = CalculateTotalPage((int)TotalRow, (int)PageSize);

                    return collection.Find(queryDoc1)
                        .SetSortOrder(SortBy.Descending("Complevel").Descending("ModifiedDate"))
                        .SetSkip((PageIndex - 1) * PageSize).SetLimit(PageSize);
                }
                else
                {
                    return collection.Find(queryDoc1).SetSkip((PageIndex - 1) * PageSize).SetLimit(PageSize);
                }
            }
            else
            { 
                return collection.FindAll()
                    .SetSkip((PageIndex - 1) * PageSize).SetLimit(PageSize);; 
            } 

        }

    }

    public static class PagingExtensions
    {
        //used by LINQ to SQL
        public static IQueryable<TSource> Page<TSource>(this IQueryable<TSource> source, int page, int pageSize)
        {
            return source.Skip((page - 1) * pageSize).Take(pageSize);
        }

        //used by LINQ
        public static IEnumerable<TSource> Page<TSource>(this IEnumerable<TSource> source, int page, int pageSize)
        {
            return source.Skip((page - 1) * pageSize).Take(pageSize);
        }

    }
     
    public class ResultMsg
    {
        public bool IsResult { get; set; }
        public int Id { get; set; }
        public string Comment { get; set; }
    }
}