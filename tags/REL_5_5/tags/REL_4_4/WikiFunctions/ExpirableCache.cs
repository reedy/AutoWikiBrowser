/* 
 * This file contains classes to be used for persisting different kinds of crap we currently load from network.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace WikiFunctions
{
    /// <summary>
    /// This is base interface for classes that store
    /// </summary>
    public interface IExpirableCache : IEnumerable
    {
        [XmlAttribute("type")]
        Type StoredType
        { get; }

        [XmlAttribute]
        TimeSpan LifeTime
        { get; set; }

        [XmlIgnore]
        int Count
        { get; }

        void Add(object data);
        bool Remove(object data);
        bool RemoveByKey(string key);
        void RemoveOld();
        void Clear();
    }


    /*
    public class ExpirableCache<TType> : IExpirableCache
    {
        Dictionary<string, TType> Data = new Dictionary<string,TType>();

        #region IEnumerable Members

        public IEnumerator GetEnumerator()
        {
            RemoveOld();
            return Data.Values.GetEnumerator();
        }

        #endregion

        #region IExpirableCache Members

        public Type StoredType
        {
            get { return typeof(TType); }
        }

        TimeSpan m_LifeTime = new TimeSpan(24, 0, 0);

        public TimeSpan LifeTime
        {
            get
            {
                return m_LifeTime;
            }
            set
            {
                m_LifeTime = value;
            }
        }

        public int Count
        {
            get { return Data.Count; }
        }

        public object this[string key]
        {
            get
            {
                return
            }
            set
            {
                Data[key] = (TType)value;
            }
        }

        public void Add(string key, object data)
        {
            throw new NotImplementedException();
        }

        public bool Remove(object data)
        {
            throw new NotImplementedException();
        }

        public bool RemoveByKey(string key)
        {
            throw new NotImplementedException();
        }

        public void RemoveOld()
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        #endregion
    }//*/
}
