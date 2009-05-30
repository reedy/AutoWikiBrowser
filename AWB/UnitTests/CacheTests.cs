using System;
using System.Collections;
using NUnit.Framework;
using WikiFunctions;
using System.IO;
using System.Threading;

namespace UnitTests
{
    [TestFixture]
    public class CacheTests : RequiresInitialization
    {
        private ObjectCache Cache;

        [SetUp]
        public void SetUp()
        {
            Cache = new ObjectCache();
        }

        void Reload()
        {
            MemoryStream ms = new MemoryStream();

            Cache.Save(ms);
            ms.Position = 0;

            Cache = new ObjectCache();
            Cache.Load(ms);
        }

        [Test]
        public void GetAndSet()
        {
            Assert.IsNull(Cache.Get<int>("foo"));

            Cache.Set("foo", "bar");
            Assert.AreEqual("bar", Cache.Get<string>("foo"));
        }

        [Test]
        public void DontMixTypes()
        {
            Cache.Set("foo", "bar");
            Cache.AddType(typeof(int), new TimeSpan(5, 0, 0, 0));
            Cache.Set("foo", 42);

            Assert.AreEqual("bar", Cache.Get<string>("foo"));
            Assert.AreEqual(42, Cache.Get<int>("foo"));
        }

        [Test]
        public void Expiry()
        {
            var expiresSoon = new TimeSpan(0, 0, 0, 0, 20);
            Cache.AddType(typeof(int), expiresSoon);

            // using default expiry time
            Cache.Set("foo", 42);
            Assert.AreEqual(42, Cache.Get<int>("foo"));
            Thread.Sleep(60);
            Assert.IsNull(Cache.Get<int>("foo"));

            // using explicitly set time, absolute
            Cache.Set("foo", 42, DateTime.Now + expiresSoon);
            Assert.AreEqual(42, Cache.Get<int>("foo"));
            Thread.Sleep(60);
            Assert.IsNull(Cache.Get<int>("foo"));

            // ...and relative
            Cache.Set("foo", 42, expiresSoon);
            Assert.AreEqual(42, Cache.Get<int>("foo"));
            Thread.Sleep(60);
            Assert.IsNull(Cache.Get<int>("foo"));

            // also after save/load
            Cache.Set("foo", 42, expiresSoon);
            Assert.AreEqual(42, Cache.Get<int>("foo"));
            MemoryStream ms = new MemoryStream();
            Cache.Save(ms);
            ms.Position = 0;
            Thread.Sleep(60);
            Cache.Load(ms);
            Assert.IsNull(Cache.Get<int>("foo"));
        }

        [Test]
        public void Persistence()
        {
            Cache.Set("foo", "bar");
            Reload();
            Assert.AreEqual("bar", Cache.Get<string>("foo"));
        }

        [Test]
        public void LoadingErasesStorage()
        {
            Cache.Set("foo", "bar");

            MemoryStream ms = new MemoryStream();
            Cache.Save(ms);
            ms.Position = 0;

            Cache.Set("boz", "quux");

            Cache.Load(ms);
            Assert.IsNull(Cache.Get<string>("boz"));
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void DontStoreUnknownTypes()
        {
            Cache.Set("foo", 3);
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void DisallowNullKeys()
        {
            Cache.Set(null, "foo");
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void DisallowEmptyKeys()
        {
            Cache.Set("", "foo");
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void DontStoreNull()
        {
            Cache.Set("foo", null);
        }

        [Test]
        public void PresetCachesCreated()
        {
            Assert.IsNotNull(ObjectCache.Global);
        }
    }
}