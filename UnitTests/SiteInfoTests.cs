using NUnit.Framework;

namespace UnitTests
{
    [TestFixture]
    internal class SiteInfoTests
    {

        [Test]
        public void VersionCompare()
        {
            Assert.Greater("1.25", "1.24");
            Assert.Greater("1.25", "1.24.1");
            Assert.Greater("1.25.0", "1.24.1");
            Assert.Greater("1.24wmf1", "1.24");

            Assert.Less("1.23", "1.24");
            Assert.Less("1.23wmf12", "1.24");
            Assert.Less("1.23wmf12", "1.24wmf1");
        }
    }
}