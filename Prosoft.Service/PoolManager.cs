using System;
using System.Web;
using System.Collections;
using System.Runtime.Caching;

namespace Prosoft.Service
{
    //public class PoolCollection
    //{
    //    #region Member
    //    public Hashtable htItemUse = new Hashtable();
    //    public Hashtable htObjectPool = new Hashtable();
    //    #endregion
    //}

    //public class PoolManager
    //{
    //    #region Member
    //    private PoolCollection poolCollection = new PoolCollection();
    //    private const string strCacheName = "CachePoolManager";
    //    #endregion

    //    #region ContainsKey
    //    public bool ContainsKey<T>()
    //    {
    //        Type typeObject = typeof(T);
    //        return poolCollection.htObjectPool.ContainsKey(typeObject.FullName);
    //    }
    //    #endregion

    //    #region GetObjectName
    //    public string GetObjectName(Type TypeObject)
    //    {
    //        string strName = string.Empty;
    //        strName = string.Concat(TypeObject.FullName, ".", TypeObject.Module.Name);
    //        return strName;
    //    }
    //    #endregion

    //    #region ObjectCache
    //    public void GetObjectCache()
    //    {
    //        ObjectCache cache = MemoryCache.Default;
    //        if (cache[strCacheName] != null)
    //        {
    //            poolCollection = (PoolCollection)cache[strCacheName];
    //        }
    //    }

    //    public void AddObjectCache()
    //    {
    //        ObjectCache cache = MemoryCache.Default;
    //        cache.Add(strCacheName, poolCollection, DateTime.Now.AddDays(1));
    //    }
    //    #endregion

    //    #region AddObject
    //    public void AddObject<T>(T value)
    //    {
    //        Type typeObject = typeof(T);
    //        string strName = GetObjectName(typeObject);

    //        if (poolCollection.htObjectPool.ContainsKey(strName))
    //        {
    //            poolCollection.htObjectPool[strName] = value;

    //        }
    //        else
    //        {
    //            poolCollection.htObjectPool.Add(strName, value);
    //        }
    //    }
    //    #endregion

    //    #region Allocate ObjectUsed
    //    private void AllocateObject(string ClassName)
    //    {
    //        int objectCount = 0;
    //        //ถ้ามีการนำไปใช้แล้วจะมี Count อยู่ใน HashTable
    //        if (poolCollection.htItemUse.ContainsKey(ClassName))
    //        {
    //            objectCount = (int)poolCollection.htItemUse[ClassName];
    //            //มีการนำ Instance ของ CLass นี้ไปใช้จะ + 1 เพิ่มเพื่อระบุว่า มีการนำไปใช้ห้ามลบ
    //            poolCollection.htItemUse[ClassName] = objectCount + 1;
    //        }
    //        else
    //            //มีการนำ Instance ของ CLass นี้ไปใช้จะ + 1 เพิ่มเพื่อระบุว่า มีการนำไปใช้ห้ามลบ
    //            poolCollection.htItemUse.Add(ClassName, objectCount + 1);
    //    }

    //    private void UnAllocateObject(string ClassName)
    //    {
    //        int objectCount = 0;
    //        //ถ้ามีการนำไปใช้แล้วจะมี Count อยู่ใน HashTable
    //        if (poolCollection.htItemUse.ContainsKey(ClassName))
    //        {
    //            //มีการคิน Resource ต้องทำการ - 1 เพื่อบอกว่านำมาคืนแล้ว
    //            objectCount = (int)poolCollection.htItemUse[ClassName];
    //            poolCollection.htItemUse[ClassName] = objectCount - 1;
    //            //ถ้าไม่มีการนำไปใช้เลย จะลบทิ้ง
    //            if (objectCount < 1)
    //                poolCollection.htItemUse.Remove(ClassName);
    //        }
    //    }

    //    public void UnAllocateObject<T>()
    //    {
    //        int objectCount = 0;
    //        Type typeObject = typeof(T);
    //        //ถ้ามีการนำไปใช้แล้วจะมี Count อยู่ใน HashTable
    //        if (poolCollection.htItemUse.ContainsKey(typeObject.Name))
    //        {
    //            //มีการคิน Resource ต้องทำการ - 1 เพื่อบอกว่านำมาคืนแล้ว
    //            objectCount = (int)poolCollection.htItemUse[typeObject.Name];
    //            poolCollection.htItemUse[typeObject.Name] = objectCount - 1;
    //            //ถ้าไม่มีการนำไปใช้เลย จะลบทิ้ง
    //            if (objectCount < 1)
    //                poolCollection.htItemUse.Remove(typeObject.Name);
    //        }
    //    }

    //    public void UnAllocateObject<T>(int InstanceCount)
    //    {
    //        int objectCount = 0;
    //        Type typeObject = typeof(T);
    //        //ถ้ามีการนำไปใช้แล้วจะมี Count อยู่ใน HashTable
    //        if (poolCollection.htItemUse.ContainsKey(typeObject.Name))
    //        {
    //            //มีการคิน Resource ต้องทำการ - 1 เพื่อบอกว่านำมาคืนแล้ว
    //            objectCount = (int)poolCollection.htItemUse[typeObject.Name];
    //            objectCount = objectCount - InstanceCount;
    //            poolCollection.htItemUse[typeObject.Name] = objectCount;
    //            //ถ้าไม่มีการนำไปใช้เลย จะลบทิ้ง
    //            if (objectCount < 1)
    //                poolCollection.htItemUse.Remove(typeObject.Name);
    //        }
    //    }
    //    #endregion

    //    #region GetObject
    //    /// <summary>
    //    /// Get Object In Collection
    //    /// </summary>
    //    /// <typeparam name="T"></typeparam>
    //    /// <returns></returns>
    //    public T GetObject<T>()
    //    {
    //        T objInstance;
    //        Type typeObject = typeof(T);
    //        string strName = GetObjectName(typeObject);
    //        //Get ข้อมูลมาจาก Cache
    //        GetObjectCache();

    //        if (poolCollection.htObjectPool.ContainsKey(strName))
    //        {
    //            objInstance = (T)poolCollection.htObjectPool[strName];
    //        }
    //        else
    //        {
    //            objInstance = (T)Activator.CreateInstance(typeObject);
    //            AddObject<T>(objInstance);
    //        }
    //        //มีการนำ Instance ของ CLass นี้ไปใช้จะ + 1 เพิ่มเพื่อระบุว่า มีการนำไปใช้ห้ามลบ
    //        AllocateObject(strName);
    //        //Add ข้อมูลล่าสุดเข้า Cache
    //        AddObjectCache();

    //        //ถ้าไม่มีการนำไปใช้เลย จะลบทิ้ง
    //        return objInstance;
    //    }
    //    #endregion

    //    #region RemoveObject
    //    public void RemoveObject<T>()
    //    {
    //        Type typeObject = typeof(T);
    //        string strName = GetObjectName(typeObject);

    //        poolCollection.htItemUse.Remove(strName);
    //        poolCollection.htObjectPool.Remove(strName);
    //    }
    //    #endregion
    //}
}
