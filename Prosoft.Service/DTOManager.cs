using System;
using System.Collections;
using System.Xml.Serialization;
using System.IO;
using System.Collections.Generic;
using System.Data;

namespace Prosoft.Service
{
    #region interface
    public interface IDTO<DE>
    {
        DE DataEntity
        {
            get;
            set;
        }
    }
    #endregion

    #region DTO
    [Serializable()]
    internal sealed class DTO<DE> : IDTO<DE>
    {
        private DE dataEntity;
        public DE DataEntity
        {
            get { return dataEntity; }
            set { dataEntity = value; }
        }
    }
    #endregion

    #region DTO Container
    public sealed class DTOManager<DE>
    {
        #region internal class Nested
        internal class Nested
        {
            static internal readonly DTOManager<DE> dtoStore = new DTOManager<DE>();

            static Nested()
            {
                dtoCollection = Hashtable.Synchronized(new Hashtable());
            }
        }
        #endregion

        #region Member
        private static Hashtable dtoCollection = null;
        private static readonly object lockObj = new object();
        private static int key = -1;
        #endregion

        #region Property
        public static int Key
        {
            get
            {
                return key;
            }
        }

        public static DTOManager<DE> Instance
        {
            get
            {
                lock (lockObj)
                {
                    return Nested.dtoStore;
                }
            }
        }
        #endregion

        #region Constructor
        private DTOManager()
        {

        }
        #endregion

        #region CreateDTO
        public IDTO<DE> CreateDTO()
        {
            IDTO<DE> dto = new DTO<DE>();
            return dto;
        }
        #endregion

        #region GetData
        public IDTO<DE> GetData(int key)
        {
            lock (dtoCollection.SyncRoot)
            {
                if (dtoCollection.ContainsKey(key))
                    return (IDTO<DE>)dtoCollection[key];
            }
            return null;
        }
        #endregion

        #region GetDataType
        public Type GetDataType(int key)
        {
            Type dataType = null;
            if (dtoCollection.ContainsKey(key))
                dataType = dtoCollection[key].GetType();
            return dataType;
        }
        #endregion

        #region AddData
        public bool AddData(IDTO<DE> dtoObject)
        {
            if (dtoCollection.ContainsKey(dtoObject.GetHashCode()))
            {
                return false;
            }

            lock (dtoCollection.SyncRoot)
            {
                dtoCollection.Add(dtoObject.GetHashCode(), dtoObject);
                key = dtoObject.GetHashCode();
            }

            return true;
        }
        #endregion

        #region RemoveData
        public bool RemoveData(int key)
        {
            if (dtoCollection.ContainsKey(key))
            {
                lock (dtoCollection.SyncRoot)
                {
                    dtoCollection.Remove(key);
                    key = -1;
                    return true;
                }
            }
            return false;
        }

        public void RemoveAll()
        {
            lock (dtoCollection.SyncRoot)
            {
                dtoCollection.Clear();
            }
        }
        #endregion
    }
    #endregion
}
