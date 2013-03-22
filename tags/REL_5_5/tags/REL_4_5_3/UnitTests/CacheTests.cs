using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using WikiFunctions;
using System.IO;
using System.Threading;

namespace UnitTests
{
    
    [TestFixture]
    public class CacheTests : RequiresInitialization
    {
        private ObjectCache cache;

        [SetUp]
        public void SetUp()
        {
            cache = new ObjectCache();
        }

        void Reload()
        {
            MemoryStream ms = new MemoryStream();

            cache.Save(ms);
            ms.Position = 0;

            cache = new ObjectCache();
            cache.Load(ms);
        }

        //string SaveToString()
        //{
        //    StringStream 
        //}

        [Test]
        public void GetAndSet()
        {
            Assert.IsNull(cache.Get<int>("foo"));

            cache.Set("foo", "bar");
            Assert.AreEqual("bar", cache.Get<string>("foo"));
        }

        [Test]
        public void DontMixTypes()
        {
            cache.Set("foo", "bar");
            cache.AddType(typeof(int), new TimeSpan(5, 0, 0, 0));
            cache.Set("foo", 42);

            Assert.AreEqual("bar", cache.Get<string>("foo"));
            Assert.AreEqual(42, cache.Get<int>("foo"));
        }

        [Test]
        public void Expiry()
        {
            var expiresSoon = new TimeSpan(0, 0, 0, 0, 50);
            cache.AddType(typeof(int), expiresSoon);

            // using default expiry time
            cache.Set("foo", 42);
            Assert.AreEqual(42, cache.Get<int>("foo"));
            Thread.Sleep(60);
            Assert.IsNull(cache.Get<int>("foo"));

            // using explicitly set time, absolute
            cache.Set("foo", 42, DateTime.Now + expiresSoon);
            Assert.AreEqual(42, cache.Get<int>("foo"));
            Thread.Sleep(60);
            Assert.IsNull(cache.Get<int>("foo"));

            // ...and relative
            cache.Set("foo", 42, expiresSoon);
            Assert.AreEqual(42, cache.Get<int>("foo"));
            Thread.Sleep(60);
            Assert.IsNull(cache.Get<int>("foo"));

            // also after save/load
            cache.Set("foo", 42, expiresSoon);
            Assert.AreEqual(42, cache.Get<int>("foo"));
            MemoryStream ms = new MemoryStream();
            cache.Save(ms);
            ms.Position = 0;
            Thread.Sleep(60);
            cache.Load(ms);
            Assert.IsNull(cache.Get<int>("foo"));
        }

        [Test]
        public void Persistence()
        {
            cache.Set("foo", "bar");
            Reload();
            Assert.AreEqual("bar", cache.Get<string>("foo"));
        }

        [Test]
        public void LoadingErasesStorage()
        {
            cache.Set("foo", "bar");

            MemoryStream ms = new MemoryStream();
            cache.Save(ms);
            ms.Position = 0;

            cache.Set("boz", "quux");

            cache.Load(ms);
            Assert.IsNull(cache.Get<string>("boz"));
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void DontStoreUnknownTypes()
        {
            cache.Set("foo", 3);
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void DisallowNullKeys()
        {
            cache.Set(null, "foo");
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void DisallowEmptyKeys()
        {
            cache.Set("", "foo");
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void DontStoreNull()
        {
            cache.Set("foo", null);
        }

        [Test]
        public void PresetCachesCreated()
        {
            Assert.IsNotNull(ObjectCache.Global);
            //Assert.IsNotNull(ObjectCache.UserCache);
        }
    }
}