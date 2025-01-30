﻿/*
AWB unit tests
Copyright (C) 2008 Max Semenik

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation; either version 2 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program; if not, write to the Free Software
Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA

Portions Copyright © 2002-2007 Charlie Poole or
Copyright © 2002-2004 James W. Newkirk, Michael C. Two, Alexei A. Vorontsov or
Copyright © 2000-2002 Philip A. Craig

 */

using System;
using System.IO;
using System.Threading;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using WikiFunctions;

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

        [TearDown]
        public void TearDown()
        {
            Cache.Dispose();
        }

        public void Reload()
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
            ClassicAssert.IsNull(Cache.Get<int>("foo"));

            Cache.Set("foo", "bar");
            Assert.That(Cache.Get<string>("foo"), Is.EqualTo("bar"));
        }

        [Test]
        public void DontMixTypes()
        {
            Cache.Set("foo", "bar");
            Cache.AddType(typeof(int), new TimeSpan(5, 0, 0, 0));
            Cache.Set("foo", 42);

            Assert.That(Cache.Get<string>("foo"), Is.EqualTo("bar"));
            Assert.That(Cache.Get<int>("foo"), Is.EqualTo(42));
        }

        [Test]
        public void Expiry()
        {
            var expiresSoon = new TimeSpan(0, 0, 0, 0, 20);
            Cache.AddType(typeof(int), expiresSoon);

            // using default expiry time
            Cache.Set("foo", 42);
            Assert.That(Cache.Get<int>("foo"), Is.EqualTo(42));
            Thread.Sleep(60);
            ClassicAssert.IsNull(Cache.Get<int>("foo"));

            // using explicitly set time, absolute
            Cache.Set("foo", 42, DateTime.Now + expiresSoon);
            Assert.That(Cache.Get<int>("foo"), Is.EqualTo(42));
            Thread.Sleep(60);
            ClassicAssert.IsNull(Cache.Get<int>("foo"));

            // ...and relative
            Cache.Set("foo", 42, expiresSoon);
            Assert.That(Cache.Get<int>("foo"), Is.EqualTo(42));
            Thread.Sleep(60);
            ClassicAssert.IsNull(Cache.Get<int>("foo"));

            // also after save/load
            Cache.Set("foo", 42, expiresSoon);
            Assert.That(Cache.Get<int>("foo"), Is.EqualTo(42));
            MemoryStream ms = new MemoryStream();
            Cache.Save(ms);
            ms.Position = 0;
            Thread.Sleep(60);
            Cache.Load(ms);
            ClassicAssert.IsNull(Cache.Get<int>("foo"));
        }

        [Test]
        public void Persistence()
        {
            Cache.Set("foo", "bar");
            Reload();
            Assert.That(Cache.Get<string>("foo"), Is.EqualTo("bar"));
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
            ClassicAssert.IsNull(Cache.Get<string>("boz"));
        }

        [Test]
        public void DontStoreUnknownTypes()
        {
            Assert.Throws<ArgumentException>(() => Cache.Set("foo", 3));
        }

        [Test]
        public void DisallowNullKeys()
        {
            Assert.Throws<ArgumentNullException>(() => Cache.Set(null, "foo"));
        }

        [Test]
        public void DisallowEmptyKeys()
        {
            Assert.Throws<ArgumentNullException>(() => Cache.Set("", "foo"));
        }

        [Test]
        public void DontStoreNull()
        {
            Assert.Throws<ArgumentNullException>(() => Cache.Set("foo", null));
        }

        [Test]
        public void PresetCachesCreated()
        {
            ClassicAssert.IsNotNull(ObjectCache.Global);
        }
    }
}