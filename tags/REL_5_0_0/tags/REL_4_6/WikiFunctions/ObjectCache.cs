/* 
 * This file contains classes to be used for persisting different kinds of crap we currently load from network.
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml.Serialization;
using System.IO;

namespace WikiFunctions
{
    //[PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    public class ObjectCache : IDisposable
    {
        public ObjectCache()
        {
            AddType(typeof(string), DefaultLifespan);
            AddType(typeof(List<string>), DefaultLifespan);
            //AddType(typeof(string[]), DefaultLifespan);
            AddType(typeof(SiteInfo), DefaultLifespan);
        }

        public ObjectCache(string fileName)
            : this()
        {
            Load(fileName);
        }

        static ObjectCache()
        {
            Global = new ObjectCache(Path.Combine(AwbDirs.UserData, "ObjectCache.xml"));//AppData);
            Global.AddType(typeof(SiteInfo), DefaultLifespan);
            //UserCache = new ObjectCache(AwbDirs.UserData);
        }

        ~ObjectCache()
        {
            Dispose();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            if (FileName == null) return;

            try
            {
                Save();
            }
            catch (Exception ex)
            {
                ReportException(ex);
            }
            FileName = null;
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 
        /// </summary>
        public static ObjectCache Global
        { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public string FileName
        { get; private set; }

        private class StoredData
        {
            public readonly object Data;
            public readonly DateTime Expires;

            public StoredData(object data, DateTime expires)
            {
                Data = data;
                Expires = expires;
            }
        }

        private static readonly TimeSpan DefaultLifespan = new TimeSpan(5, 0, 0, 0);
        private readonly Dictionary<Type, TimeSpan> SupportedTypes = new Dictionary<Type, TimeSpan>();

        private readonly Dictionary<Type, Dictionary<string, StoredData>> Storage
            = new Dictionary<Type, Dictionary<string, StoredData>>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="what"></param>
        /// <param name="lifeSpan"></param>
        public void AddType(Type what, TimeSpan lifeSpan)
        {
            if (what == null) throw new ArgumentNullException("what");

            SupportedTypes[what] = lifeSpan;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object this[string key]
        {
            set
            {
                Set(key, value);
            }
            get
            {
                return Get<object>(key);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public object Get<T>(string key)
        {
            lock (Storage)
            {
                Type type = typeof(T);
                if (Storage.ContainsKey(type) && Storage[type].ContainsKey(key))
                {
                    StoredData found = Storage[type][key];
                    if (found.Expires < DateTime.Now)
                    {
                        Storage[type].Remove(key);
                        return null;
                    }

                    return (T)found.Data; // extra typecast to detect type mismatch earlier
                }

                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Set(string key, object value)
        {
            Set(key, value, DateTime.MinValue);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="duration"></param>
        public void Set(string key, object value, TimeSpan duration)
        {
            Set(key, value, DateTime.Now + duration);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiry"></param>
        public void Set(string key, object value, DateTime expiry)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");
            if (value == null) throw new ArgumentNullException("value");

            Type type = value.GetType();
            if (!SupportedTypes.ContainsKey(type))
                throw new ArgumentException("Caching of type " + value.GetType().Name + " is not supported",
                                            "value");

            if (expiry == DateTime.MinValue) expiry = DateTime.Now + SupportedTypes[type];

            lock (Storage)
            {
                if (!Storage.ContainsKey(type)) Storage[type] = new Dictionary<string, StoredData>();
                Storage[type][key] = new StoredData(value, expiry);
            }
        }

        private XmlSerializer serializer;
        private XmlSerializer Serializer
        {
            get
            {
                if (serializer != null) return serializer;

                var usedTypes = new List<Type>();
                foreach (var type in SupportedTypes)
                {
                    usedTypes.Add(type.Key);
                }
                serializer = new XmlSerializer(typeof(Internal.CacheRoot), usedTypes.ToArray());
                return serializer;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Save()
        {
            Save(FileName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        public void Save(string fileName)
        {
            FileName = fileName;

            using (FileStream fs = File.OpenWrite(fileName))
            {
                Save(fs);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        public void Save(Stream str)
        {
            var root = new Internal.CacheRoot();

            DateTime now = DateTime.Now;

            lock (Storage)
            {
                foreach (var type in Storage)
                {
                    var typeRoot = new Internal.Type { Name = type.Key.ToString() };
                    foreach (var value in type.Value)
                    {
                        if (value.Value.Expires < now) continue;
                        typeRoot.Items.Add(new Internal.Item { Value = value.Value.Data, Expires = value.Value.Expires, Key = value.Key });
                    }
                    if (typeRoot.Items.Count > 0) root.Types.Add(typeRoot);
                }
            }

            Serializer.Serialize(str, root);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ex"></param>
        private void ReportException(Exception ex)
        {
            Trace.WriteLine("Exception caught in ObjectCache: " + ex.Message);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        public void Load(string fileName)
        {
            if (fileName == null) throw new ArgumentNullException("fileName");

            FileName = fileName;
            try
            {
                using (FileStream fs = File.Open(fileName, FileMode.Open))
                {
                    if (!Load(fs) && File.Exists(fileName)) File.Delete(fileName);
                }
            }
            catch
            { } // give up silently
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public bool Load(Stream str)
        {
            if (str == null) throw new ArgumentNullException("str");

            try
            {
                var loaded = (Internal.CacheRoot)Serializer.Deserialize(str);
                if (loaded.Version != Variables.WikiFunctionsVersion.ToString()) return false;

                lock (Storage)
                {
                    Storage.Clear();

                    foreach (var entry in loaded.Types)
                    {
                        try
                        {
                            Type type = Type.GetType(entry.Name);
                            if (type == null || !SupportedTypes.ContainsKey(type)) continue;

                            Storage[type] = new Dictionary<string, StoredData>();
                            foreach (var data in entry.Items)
                                Storage[type][data.Key] = new StoredData(data.Value, data.Expires);
                        }
                        catch (Exception ex)
                        {
                            // Ignore possible exceptions, attempting
                            ReportException(ex);
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                ReportException(ex);
                return false;
            }
        }
    }

    namespace Internal
    {
        [Serializable]
        public class Item
        {
            [XmlAttribute("key")]
            public string Key;

            [XmlAttribute("expires")]
            public DateTime Expires;

            //[XmlText]
            public object Value;
        }

        [Serializable/*, XmlElement("Type")*/]
        public class Type
        {
            [XmlAttribute("name")]
            public string Name;

            public readonly List<Item> Items = new List<Item>();
        }

        [Serializable, XmlRoot("Cache")]
        public class CacheRoot
        {
            public CacheRoot()
            { }

            public CacheRoot(string version)
            {
                Version = version;
            }

            [XmlAttribute("version")]
            public readonly string Version = Variables.WikiFunctionsVersion.ToString();

            //[XmlText]
            [XmlArray("Types")]
            public readonly List<Type> Types = new List<Type>();
        }
    }
}
