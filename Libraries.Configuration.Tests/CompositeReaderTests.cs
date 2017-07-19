using System;
using System.Linq;
using System.Collections.Generic;
using Libraries.Configuration.Providers;
using Libraries.Configuration.Readers;
using Libraries.Configuration.Converters;
using Libraries.Configuration.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;


namespace Libraries.Configuration.Tests
{
    [TestClass]
    public class CompositeReaderTests
    {
        private static ISettingsReader Reader
        {
            get; set;
        }

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            var dictionaryA = new Dictionary<string, string>
            {
                { "number", "5" },
                { "notinsecond", null }
            };
            var mockA = new Mock<ISettingsProvider>();
            mockA.Setup(x => x.Name).Returns("ProviderA");
            mockA.Setup(x => x.GetSettings()).Returns(dictionaryA);
            var readerA = new DiscreteSettingsReader(mockA.Object);

            var dictionaryB = new Dictionary<string, string>
            {
                { "number", "6" },
                { "notinfirst", null },
                { "boolArray", "true,false,TRUE,FALSE,T,F,t,f,1,0,yes,no,YES,NO,Y,N" }
            };
            var mockB = new Mock<ISettingsProvider>();
            mockB.Setup(x => x.Name).Returns("ProviderB");
            mockB.Setup(x => x.GetSettings()).Returns(dictionaryB);
            var readerB = new DiscreteSettingsReader(mockB.Object);

            Reader = CompositeSettingsReader.Create(readerA, readerB);
            ReaderConfiguration.RegisterTypeConverter(typeof(bool), new LogicalBooleanConverter());
        }

        [TestMethod]
        public void TestName()
        {
            Assert.IsTrue(Reader.ProviderName == "ProviderA,ProviderB");
        }

        [TestMethod]
        public void TestCount()
        {
            Assert.IsTrue(Reader.List.Count() == 5);
        }

        [TestMethod]
        public void TestPriority()
        {
            var value = Reader.Read<int>("number");
            Assert.AreEqual(value, 5);
        }

        [TestMethod]
        public void TestExistence()
        {
            Assert.IsTrue(Reader.Contains("notinfirst"));
            Assert.IsTrue(Reader.Contains("notinsecond"));
        }

        [TestMethod]
        public void TestConverter()
        {
            //Alternating true and false values
            var array = Reader.Read<IEnumerable<bool>>("boolArray");

            //Assert all even indexed elements are true
            var trueArray = array.Where((x, i) => i % 2 == 0);
            Assert.IsTrue(trueArray.Aggregate(true, (x, result) => x & result));

            //Assert all odd indexed elements are false
            var falseArray = array.Where((x, i) => i % 2 != 0);
            Assert.IsFalse(falseArray.Any(x => x == true));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ReadNullKey()
        {
            var value = Reader.Read<string>(null);
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void ReadMissingKey()
        {
            var value = Reader.Read<string>("missing");
        }

        [TestMethod]
        public void ReadMissingWithDefault()
        {
            var value = Reader.Read("missing", "default");
            Assert.AreEqual("default", value);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidCastException))]
        public void ReadIncompatibleCast()
        {
            var value = Reader.Read<int>("boolArray");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidCastException))]
        public void ReadUnsupportedType()
        {
            var converter = ReaderConfiguration.GetTypeConverter(typeof(object));
            if (converter != null)
                ReaderConfiguration.RegisterTypeConverter(typeof(object), null);

            try
            {
                var value = Reader.Read<object>("number");
            }
            finally
            {
                if (converter != null)
                    ReaderConfiguration.RegisterTypeConverter(typeof(object), converter);
            }
        }

        [TestMethod]
        public void ReadRegisteredConverter()
        {
            ReaderConfiguration.RegisterTypeConverter(typeof(object), new ObjectConverter());
            var value = Reader.Read<object>("number");
        }

        [TestMethod]
        public void EnsureNoCache()
        {
            Assert.IsFalse(Reader.Contains("absent"));
            var value = Reader.Read<int>("absent", 5);
            Assert.IsFalse(Reader.Contains("absent"));
        }

        [TestMethod]
        public void ReadCachedValue()
        {
            var reader = Reader.Combine(new CachedSettingsReader());
            Assert.IsFalse(reader.Contains("absent"));
            var value = reader.Read<int>("absent", 5);
            Assert.IsTrue(reader.Contains("absent"));
        }
    }
}
