using Libraries.Configuration.Readers;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Libraries.Configuration.Tests
{
    [TestClass]
    public class CachedReaderTests
    {
        private static ISettingsReader Reader
        {
            get; set;
        }

        [TestMethod]
        public void ReadCachedValue()
        {
            var reader = new CachedSettingsReader();
            Assert.IsFalse(reader.Contains("number"));
            var value = reader.Read<int>("number", 5);
            Assert.IsTrue(reader.Contains("number"));
        }
    }
}
