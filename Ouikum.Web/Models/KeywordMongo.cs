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
using AutoMapper;
using PonyTaleMarket.Common;

namespace PonyTaleMarket.Web.Models
{

    public class KeywordMongo : BaseMongo
    { 

        public tbProduct GetProductMongoById(int ProductID)
        {
            var collection = db.GetCollection<tbProduct>("tbProduct");
            var data = new tbProduct();
            var query = Query<tbProduct>.EQ(e => e.ProductID, (int)ProductID);
            data = collection.FindOne(query);

            return data;

        }

        public bool UpdateProductViewCount(int ProductID)
        { 
            try
            {
                // ทดสอบการปิด ViewCount
                return true;

                var collection = db.GetCollection<tbProduct>("tbProduct");
                var data = new tbProduct();
                var query = Query<tbProduct>.EQ(e => e.ProductID, (int)ProductID);
                data = collection.FindOne(query);
                if (data != null)
                {
                    if (data.ViewCount == 0)
                    {
                        data.ViewCount = 1;
                    }
                    else
                    {
                        short a = 1;
                        data.ViewCount = (short)(data.ViewCount + a);
                    }
                    data.ModifiedDate = DateTimeNow;
                    collection.Save(data);
                }
                IsResult = true;
            }
            catch (Exception ex)
            {
                IsResult = false;
            }
            return IsResult;
        }
        public bool UpdateProductContactCount(int ProductID)
        {

            try
            {
                var collection = db.GetCollection<tbProduct>("tbProduct");
                var data = new tbProduct();
                var query = Query<tbProduct>.EQ(e => e.ProductID, (int)ProductID);
                data = collection.FindOne(query);
                if (data != null)
                {
                    if (data.ContactCount == 0)
                    {
                        data.ContactCount = 1;
                    }
                    else
                    {
                        short a = 1;
                        data.ContactCount = (short)(data.ContactCount + a);
                    }
                    data.ModifiedDate = DateTimeNow;
                    collection.Save(data);
                }
                IsResult = true;
            }
            catch (Exception ex)
            {
                IsResult = false;
                throw;
            }
            return IsResult;
        }

        #region UpdateMongoProduct

        public bool UpdateMongoProduct(int ProductID)
        {
            IsResult = false;
            var sv = new ProductService();
            var sql = @"select  it.* FROM view_ProductMobileApp AS it 
     WHERE  ProductID = " + ProductID; //322138
            //      AND RowFlag IN (4,5,6) AND CompRowFlag IN (2,4)
            //AND CompIsDelete = 0) AND ( IsShow = 1 AND IsJunk = 0
            var products = sv.qDB.ExecuteQuery<view_ProductMobileApp>(sql).ToList();
            if (products != null && products.Count() > 0)
            {
                var svAddress = new AddressService();
                var provinces = svAddress.GetProvinceAll();

                foreach (var item in products)
                {
                    var res = new ResultMsg();
                    if (item.IsDelete == false &&
                       item.CompIsDelete == false &&
                        item.IsShow == true &&
                        item.IsJunk == false &&
                        (item.RowFlag == 4 || item.RowFlag == 5 || item.RowFlag == 6) &&
                        (item.CompRowFlag == 2 || item.CompRowFlag == 4))
                    {

                        var ProvinceName = provinces.Where(m => m.ProvinceID == item.CompProvinceID).FirstOrDefault().ProvinceName;
                        res.IsResult = SaveKeywordMongo(item, ProvinceName);
                        res.Id = item.ProductID;
                        res.Comment = "save";
                    }
                    else
                    {
                        res.IsResult = RemoveProductKeywordMongo(item.ProductID);
                        res.Id = item.ProductID;
                        res.Comment = "remove";
                    }
                    Result.Add(res);
                }

            }
            return IsResult;
        }

        public bool UpdateMongoProduct(int StartId, int EndId)
        {
            IsResult = false;
            var sv = new ProductService();

            var sql = @"select it.ProductiD,it.ProductName,it.ProductKeyword,it.ShortDescription  ,it.ProductGroupID
     ,it.CateLV1,it.CateLV2,it.CateLV3,it.Price,it.Qty,it.QtyUnit
     ,it.ProductImgPath,it.ContactMobile,it.ServiceType,it.CompID,it.CompName,it.Complevel
     ,it.CompProvinceID, it.CompDistrictID,it.BizTypeID,it.BizTypeOther,it.CreatedDate ,it.ListNo
     ,it.RowFlag,it.ModifiedDate,it.ViewCount,it.ContactCount  ,it.IsDelete , it.CompIsDelete , it.IsShow , it.IsJunk , it.CompRowflag
     FROM view_ProductMobileApp AS it WHERE ( it.ProductID >= " + StartId + " AND it.ProductID <= " + EndId + " ) "; //322138

            var products = sv.qDB.ExecuteQuery<view_ProductMobileApp>(sql).ToList();
            if (products != null && products.Count() > 0)
            {
                var svAddress = new AddressService();
                var provinces = svAddress.GetProvinceAll();
                foreach (var item in products)
                {
                    var res = new ResultMsg();
                    if (item.IsDelete == false &&
                       item.CompIsDelete == false &&
                        item.IsShow == true &&
                        item.IsJunk == false &&
                        (item.RowFlag == 4 || item.RowFlag == 5 || item.RowFlag == 6) &&
                        (item.CompRowFlag == 2 || item.CompRowFlag == 4))
                    {

                        var ProvinceName = provinces.Where(m => m.ProvinceID == item.CompProvinceID).FirstOrDefault().ProvinceName;
                        IsResult = SaveKeywordMongo(item, ProvinceName);
                        res.Id = item.ProductID;
                        res.Comment = "save";
                    }
                    else
                    {
                        IsResult = RemoveProductKeywordMongo(item.ProductID);
                        res.Id = item.ProductID;
                        res.Comment = "remove";
                    }

                    //  Result.Add(res);
                }

            }
            return IsResult;
        }
         
        #endregion

        public bool RemoveProductKeywordMongo(int ProductID)
        {
            try
            {
                var database = server.GetDatabase("dbb2bthai");
                var collection = database.GetCollection<tbProduct>("tbProduct");
                collection.Remove(Query.EQ("ProductID", ProductID));
                IsResult = true;
            }
            catch (Exception ex)
            {
                IsResult = false;
                throw;
            }
            return IsResult;
        }

        public bool SaveKeywordMongo(view_ProductMobileApp model, string ProvinceName = "")
        {
            var products = new List<tbProduct>();
            var isResult = false;
            var database = server.GetDatabase("dbb2bthai");
            var collection = database.GetCollection<tbProduct>("tbProduct");
            try
            {
                if (model != null && model.ProductID > 0)
                {
                    #region Set Keyword

                    var product = new tbProduct();
                    product = GetProductMongoById((int)model.ProductID);
                    if (product != null && product.ProductID > 0)
                    {
                        var query = Query<tbProduct>.EQ(e => e.ProductID, model.ProductID);

                        #region Edit
                        product.ProductName = model.ProductName;
                        product.ProductID = model.ProductID;
                        product.ProductGroupID = model.ProductGroupID;
                        product.ShortDescription = model.ShortDescription;
                        product.CateLV1 = (int)model.CateLV1;
                        product.CateLV2 = (int)model.CateLV2;
                        product.CateLV3 = (int)model.CateLV3;
                        product.ListNo = (int)model.ListNo;
                        product.Price = (decimal)model.Price;
                        product.Qty = (decimal)model.Qty;
                        product.QtyUnit = model.QtyUnit;
                        product.ProductImgPath = model.ProductImgPath;
                        product.ContactMobile = model.ContactMobile;
                        product.ServiceType = (int)model.ServiceType;
                        product.CompID = (int)model.CompID;
                        product.CompName = model.CompName;
                        product.Complevel = (int)model.Complevel;
                        product.CompProvinceID = (int)model.CompProvinceID;
                        product.ProvinceName = ProvinceName;
                        product.CompDistrictID = (int)model.CompDistrictID;
                        product.BizTypeID = (int)model.BizTypeID;
                        product.BizTypeOther = model.BizTypeOther;
                        product.CreatedDate = model.CreatedDate;
                        product.RowFlag = model.RowFlag;
                        product.ModifiedDate = model.ModifiedDate;
                        product.ViewCount = (short)model.ViewCount;
                        product.ContactCount = (short)model.ContactCount;
                        product.IsSME = false;
                        product.IsDelete = false; 

                        product.Keyword = new List<string>();
                        SetProductKeyword(product.Keyword, model.ProductName);
                        SetProductKeyword(product.Keyword, model.ProductKeyword);
                        #endregion

                        collection.Save(product);
                        isResult = true;
                    }
                    else
                    {
                        #region Insert
                        product = new tbProduct();
                        product.Keyword = new List<string>();
                        product.ProductName = model.ProductName;
                        product.ProductID = model.ProductID;
                        product.ProductGroupID = model.ProductGroupID;
                        product.ProductName = model.ProductName;
                        product.ProductKeyword = model.ProductKeyword;
                        product.ShortDescription = model.ShortDescription;
                        product.CateLV1 = (int)model.CateLV1;
                        product.CateLV2 = (int)model.CateLV2;
                        product.CateLV3 = (int)model.CateLV3;
                        product.Price = (decimal)model.Price;
                        product.Qty = (decimal)model.Qty;
                        product.QtyUnit = model.QtyUnit;
                        product.ProductImgPath = model.ProductImgPath;
                        product.ContactMobile = model.ContactMobile;
                        product.ServiceType = (int)model.ServiceType;
                        product.CompID = (int)model.CompID;
                        product.CompName = model.CompName;
                        product.Complevel = (int)model.Complevel;
                        product.CompProvinceID = (int)model.CompProvinceID;
                        product.ProvinceName = ProvinceName;
                        product.CompDistrictID = (int)model.CompDistrictID;
                        product.BizTypeID = (int)model.BizTypeID;
                        product.BizTypeOther = model.BizTypeOther;
                        product.CreatedDate = model.CreatedDate;
                        product.RowFlag = model.RowFlag;
                        product.ModifiedDate = model.ModifiedDate;
                        product.ViewCount = (short)model.ViewCount;
                        product.ContactCount = (short)model.ContactCount;
                        product.IsSME = false;
                        product.IsDelete = false; 

                        SetProductKeyword(product.Keyword, model.ProductName);
                        SetProductKeyword(product.Keyword, model.ProductKeyword);
                        #endregion

                        collection.Insert(product);
                        isResult = true;
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                isResult = false;
                throw;
            }

            return isResult;
        }

        public bool SaveKeywordMongo(List<view_ProductMobileApp> model)
        {
            var products = new List<tbProduct>();
            var isResult = false;
            try
            {

                if (model != null && model.Count > 0)
                {
                    var svAddress = new AddressService();
                    var provinces = svAddress.GetProvinceAll();
                    #region Set Keyword
                    foreach (var item in model)
                    {
                        var ProvinceName = provinces.Where(m => m.ProvinceID == item.CompProvinceID).FirstOrDefault().ProvinceName;
                        SaveKeywordMongo(item, ProvinceName);
                    }
                    #endregion
                    IsResult = true;
                }

            }
            catch (Exception ex)
            {
                isResult = false;
                throw;
            }

            return isResult;
        }

        public void SetProductKeyword(List<string> model, string Keyword)
        {


            #region ProductKeyword
            //เช็ค product keyword ว่ามีค่า?
            if (!string.IsNullOrEmpty(Keyword))
            {
                //ทำการ splite คำลงตัวแปร array
                var arrKW = Keyword.Split('~').ToArray();
                // ถ้า array มีค่าทำต่อ
                #region check value
                if (arrKW != null && arrKW.Length > 0)
                {

                    //วน loop ตามจำนวน array
                    foreach (var a in arrKW)
                    {
                        //เช็คว่า มีค่า?
                        if (!string.IsNullOrEmpty(a))
                        {
                            //ถ้ามี ให้ insert ลง ตารางคีย์ word
                            if (!model.Any(m => m == a))
                                model.Add(a);
                        }

                        // ทำ การ split คำที่เป็น ช่องว่าง เพื่อ insert ลง

                        var subArrKW = a.Split(' ').ToList();
                        var subArrKW2 = a.Split(',').ToList();
                        if (subArrKW2 != null && subArrKW2.Count() > 1)
                        {
                            for (int i = 0; i < subArrKW2.Count - 1; i++)
                            {
                                if (!string.IsNullOrEmpty(subArrKW2[i]))
                                {
                                    if (!model.Any(m => m == subArrKW2[i]))
                                        model.Add(subArrKW2[i]);
                                }
                            }
                        }
                        // ถ้า กรณีที่มี ช่องว่าง
                        if (subArrKW != null && subArrKW.Count() > 1)
                        {

                            // ให้ทำการวนลูป insert ค่า
                            for (int i = 0; i < subArrKW.Count(); i++)
                            {
                                subArrKW[i] = subArrKW[i].Replace(",", "").Replace(".", "").Replace(" ", "").Replace("\n", "");

                                if (!string.IsNullOrEmpty(subArrKW[i]) && subArrKW[i].Length > 1)
                                {
                                    // กรณี ที่คำมีตัวอักษรน้อยเกินไปมันจะดูไม่ความหมายเรา จะรวม array
                                    if (subArrKW[i].Length <= 2)
                                    {
                                        //ถ้า รอบ มันน้อย กว่า รอบสุดท้าย รวมได้ ไม่ใช่ index มันจะเกินส่งผลให้เกิด error
                                        if (i < subArrKW.Count())
                                        {
                                            var last = (i + 1);
                                            if (last < subArrKW.Count())
                                            {
                                                var str = subArrKW[i] + subArrKW[i + 1];
                                                if (!model.Any(m => m == str))
                                                    model.Add(str);
                                                i++;
                                            }
                                        }
                                        else
                                        {
                                            var str = subArrKW[i];
                                            if (!model.Any(m => m == str))
                                                model.Add(str);
                                        }
                                    }
                                    else
                                    {
                                        var str = subArrKW[i];
                                        if (!model.Any(m => m == str))
                                            model.Add(str);
                                    }

                                }
                            }
                        }
                    }

                }
                #endregion

            }

            #endregion

        }

        public List<view_ProductMobileApp> SearchViewProductMongo(
            int? Sort = 1,
            int PIndex = 1, int PSize = 20,
            string searchKey = "", int? BizTypeID = 0,
            int CateLevel = 0, int CategoryID = 0,
            int? Complevel = 0, int ProvinceID = 0)
        {
            var keywords = SearchProductMongo(Sort, PIndex, PSize, searchKey, BizTypeID, CateLevel, CategoryID, Complevel, ProvinceID);
            var products = new List<view_ProductMobileApp>();
            foreach (var item in keywords)
            {
                Mapper.CreateMap<Models.tbProduct, view_ProductMobileApp>();
                view_ProductMobileApp model = Mapper.Map<Models.tbProduct, view_ProductMobileApp>(item);
                products.Add(model);
            }
            return products;
        }
        public string ReplaceSearchKey(string searchKey)
        {
            if(!string.IsNullOrEmpty(searchKey)){
                string[] names = new string[] { "(", "/", "^", "\\" };
                //var a = names.Contains(searchKey);
                for (int i = 0; i < names.Length; i++)
                {
                    var len = searchKey.IndexOf(names[i]);
                    if (len > 0)
                    {
                        searchKey = searchKey.Substring(0, len);
                        i = names.Length;
                    } 
                }
            }
            return searchKey;
        }
        public List<tbProduct> SearchProductMongo(
            int? Sort = 1,
            int PIndex = 1, int PSize = 20,
            string searchKey = "", int? BizTypeID = 0,
            int CateLevel = 0, int CategoryID = 0,
            int? Complevel = 0, int ProvinceID = 0

            )
        {
            var products = new List<tbProduct>();
            string querytext = "";
            string where = "";
            var x = 0; 
            if (!string.IsNullOrEmpty(searchKey))
            {

                //$regex : "C13S050650 (S050652)*"
              //  where += "{ Keyword: { $elemMatch: {  $in: [ /^" + searchKey + "/] } }},";
                where += "{ Keyword: /^" + ReplaceSearchKey(searchKey) + "/},";
                x++;
            }

            #region Search Category Level & CategoryID
            if (CateLevel == 1)
            {
                where += " { CateLV1 : " + CategoryID + "},";
                x++;
            }
            else if (CateLevel == 2)
            {
                where += " { CateLV2 : " + CategoryID + "},";
                x++;
            }
            else if (CateLevel == 3)
            {
                where += " { CateLV3 : " + CategoryID + "},";
                x++;
            }
            #endregion

            #region Search Company Level
            if (Complevel > 0)
            {
                where += " { Complevel : " + Complevel + "},";
                x++;
            }
            #endregion

            #region Search BiztypeID
            if (BizTypeID > 0)
            {
                where += " { BizTypeID : " + BizTypeID + "},";
                x++;
            }
            #endregion


            #region Search ProvinceID
            if (ProvinceID > 0)
            {
                where += " { CompProvinceID : " + ProvinceID + "},";
                x++;
            }
            #endregion

            #region Where
            if (!string.IsNullOrEmpty(where))
            {
                where = where.Substring(0, where.Length - 1);
                if (x > 1)
                {
                    querytext += "{ $and : [" + where + "] }";
                }
                else
                {
                    querytext = where;
                }
            }
            #endregion
             
            var collection = db.GetCollection<tbProduct>("tbProduct");

            if (!string.IsNullOrEmpty(querytext))
            {
                BsonDocument query1 = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<BsonDocument>(querytext);
                QueryDocument queryDoc1 = new QueryDocument(query1);

                #region Find Sort
                switch (Sort)
                {
                    case 1:
                        products = collection.Find(queryDoc1)
                            //  .SetSortOrder(SortBy.Descending("Complevel").Descending("ModifiedDate"))
                            .SetSortOrder(SortBy.Descending("ModifiedDate"))
                            .SetSkip((PIndex - 1) * PSize).SetLimit(PSize).ToList();

                        break;
                    case 2:
                        //collection.Find(queryDoc1)
                        //    .SetSortOrder(SortBy.Descending("CreatedDate"))
                        //    .SetSkip((PIndex - 1) * PSize).SetLimit(PSize);

                        products = collection.Find(queryDoc1)
                            .SetSortOrder(SortBy.Descending("CreatedDate"))
                            .SetSkip((PIndex - 1) * PSize).SetLimit(PSize).ToList();
                        break;
                    case 3:
                        products = collection.Find(queryDoc1)
                            .SetSortOrder(SortBy.Descending("ViewCount"))
                            .SetSkip((PIndex - 1) * PSize).SetLimit(PSize).ToList();
                        break;
                    case 4:
                        products = collection.Find(queryDoc1)
                            .SetSortOrder(SortBy.Descending("ContactCount"))
                            .SetSkip((PIndex - 1) * PSize).SetLimit(PSize).ToList();
                        break;
                }
                #endregion
                 
                TotalRow = collection.Find(queryDoc1).Count();
                TotalPage = CalculateTotalPage((int)TotalRow, PSize);
            }
            else
            {
                querytext = " { ProductID: { $gte: 0 } } ,";
                BsonDocument query1 = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<BsonDocument>(querytext);
                QueryDocument queryDoc1 = new QueryDocument(query1);

                #region FindAll Sort
                switch (Sort)
                {

                    case 1:

                        products = collection.Find(queryDoc1)
                           // .SetSortOrder(SortBy.Descending("Complevel").Descending("ModifiedDate"))
                           .SetSortOrder(SortBy.Descending("ModifiedDate"))
                            .SetSkip((PIndex - 1) * PSize).SetLimit(PSize).ToList();
                        break;
                    case 2:

                        products = collection.Find(queryDoc1)
                            .SetSortOrder(SortBy.Descending("CreatedDate"))
                            .SetSkip((PIndex - 1) * PSize).SetLimit(PSize).ToList();
                        break;
                    case 3:
                        products = collection.Find(queryDoc1)
                            .SetSortOrder(SortBy.Descending("ViewCount") )
                            .SetSkip((PIndex - 1) * PSize).SetLimit(PSize).ToList();
                        break;
                    case 4:
                        products = collection.Find(queryDoc1)
                            .SetSortOrder(SortBy.Descending("ContactCount") )
                            .SetSkip((PIndex - 1) * PSize).SetLimit(PSize).ToList();
                        break;
                }
                #endregion

                TotalRow = collection.FindAll().Count();
                TotalPage = CalculateTotalPage((int)TotalRow, PSize);

            }

            return products;
        }

        public bool UpdateCompNameByCompID(int CompID, string CompName, bool? IsSME = false, int? Complevel = 0)
        {
            try
            {
                // var Id = (ObjectId)id;
                var collection = db.GetCollection<tbProduct>("tbProduct");
                var query = Query<tbProduct>.EQ(e => e.CompID, CompID);
                var update = Update<tbProduct>.Set(e => e.CompName, CompName).Set(e => e.IsSME, IsSME).Set(e => e.Complevel, Complevel); // update modifiers
                collection.Update(query, update, new MongoUpdateOptions
                {
                    Flags = UpdateFlags.Multi
                });
                IsResult = true;
            }
            catch (Exception ex)
            {
                IsResult = false;

            }
            return IsResult;

        }
    }


    //public class tbProduct
    //{
    //    public ObjectId Id { get; set; }
    //    public int? ProductID { get; set; }
    //    public string ProductKeyword { get; set; }
    //    public int? ProductGroupID { get; set; }
    //    public string ProductName { get; set; }
    //    public string ShortDescription { get; set; }
    //    public decimal Price { get; set; }
    //    public decimal Qty { get; set; }
    //    public string QtyUnit { get; set; }
    //    public string ProductImgPath { get; set; }
    //    public short ViewCount { get; set; }
    //    public int ListNo { get; set; }
    //    public int CateLV1 { get; set; }
    //    public int CateLV2 { get; set; }
    //    public int CateLV3 { get; set; }
    //    public DateTime CreatedDate { get; set; }
    //    public DateTime ModifiedDate { get; set; }
    //    public int RowFlag { get; set; }
    //    public string ContactMobile { get; set; }
    //    public int CompID { get; set; }
    //    public string CompName { get; set; }
    //    public int Complevel { get; set; }
    //    public int ServiceType { get; set; }
    //    public int CompProvinceID { get; set; }
    //    public string ProvinceName { get; set; }
    //    public int CompDistrictID { get; set; }
    //    public int BizTypeID { get; set; }
    //    public string BizTypeName { get; set; }
    //    public string BizTypeOther { get; set; }
    //    public int ContactCount { get; set; }
    //    public bool? IsSME { get; set; }
    //    public bool? IsDelete { get; set; }
    //    public List<string> Keyword { get; set; }

    //}
}