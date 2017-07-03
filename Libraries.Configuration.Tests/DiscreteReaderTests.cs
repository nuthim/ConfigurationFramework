using System;
using System.Linq;
using Libraries.Configuration.Readers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Libraries.Configuration.Converters;
using Libraries.Configuration.Providers;
using System.Collections.Generic;
using Moq;

namespace Libraries.Configuration.Tests
{
    [TestClass]
    public class DiscreteReaderTests
    {
        private static ISettingsReader Reader
        {
            get; set;
        }

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            var dictionary = new Dictionary<string, string>();
            dictionary.Add("number", "5");
            dictionary.Add("array", "1, 2, 3, 4, 5");
            dictionary.Add("true", "true");
            dictionary.Add("false", "false");
            dictionary.Add("boolArray", "true,false,TRUE,FALSE,T,F,t,f,1,0,yes,no,YES,NO,Y,N");

            var mock = new Mock<ISettingsProvider>();
            mock.Setup(x => x.Name).Returns("ProviderA");
            mock.Setup(x => x.GetSettings()).Returns(dictionary);

            Reader = new DiscreteSettingsReader(mock.Object);
            ReaderConfiguration.RegisterTypeConverter(typeof(bool), new LogicalBooleanConverter());
        }

        [TestMethod]
        public void TestName()
        {
            Assert.IsTrue(Reader.ProviderName == "ProviderA");
        }

        [TestMethod]
        public void ReadPrimitive()
        {
            Assert.IsTrue(Reader.Read<int>("number") == 5);
            Assert.IsTrue(Reader.Read<string>("number") == "5");
            Assert.IsTrue(Reader.Read<float>("number") == 5.0d);
            Assert.IsTrue(Reader.Read<decimal>("number") == 5.0m);
            Assert.IsTrue(Reader.Read<uint>("number") == 5);
            Assert.IsTrue(Reader.Read<short>("number") == 5);
        }

        [TestMethod]
        public void ReadBoolean()
        {
            Assert.IsTrue(Reader.Read<bool>("true") == true);
            Assert.IsTrue(Reader.Read<bool>("false") == false);
        }

        [TestMethod]
        public void BooleanConverterTest()
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
        public void ReadAsArray()
        {
            Assert.IsTrue(Reader.Read<string[]>("array").Count() == 5);
            Assert.IsTrue(Reader.Read<int[]>("array").Count() == 5);
            Assert.IsTrue(Reader.Read<float[]>("array").Count() == 5);
            Assert.IsTrue(Reader.Read<decimal[]>("array").Count() == 5);
            Assert.IsTrue(Reader.Read<uint[]>("array").Count() == 5);
            Assert.IsTrue(Reader.Read<short[]>("array").Count() == 5);
        }

        [TestMethod]
        public void ReadAsEnumerable()
        {
            Assert.IsTrue(Reader.Read<IEnumerable<string>>("array").Count() == 5);
            Assert.IsTrue(Reader.Read<IEnumerable<int>>("array").Count() == 5);
            Assert.IsTrue(Reader.Read<IEnumerable<float>>("array").Count() == 5);
            Assert.IsTrue(Reader.Read<IEnumerable<decimal>>("array").Count() == 5);
            Assert.IsTrue(Reader.Read<IEnumerable<uint>>("array").Count() == 5);
            Assert.IsTrue(Reader.Read<IEnumerable<short>>("array").Count() == 5);
        }

        [TestMethod]
        public void ReadAsList()
        {
            Assert.IsTrue(Reader.Read<IList<string>>("array").Count() == 5);
            Assert.IsTrue(Reader.Read<IList<int>>("array").Count() == 5);
            Assert.IsTrue(Reader.Read<IList<float>>("array").Count() == 5);
            Assert.IsTrue(Reader.Read<IList<decimal>>("array").Count() == 5);
            Assert.IsTrue(Reader.Read<IList<uint>>("array").Count() == 5);
            Assert.IsTrue(Reader.Read<IList<short>>("array").Count() == 5);
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
            var value = Reader.Read<int>("array");
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
                var value = Reader.Read<object>("array");
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
            var value = Reader.Read<object>("array");
        }

        [TestMethod]
        public void EnsureNoCache()
        {
            Assert.IsFalse(Reader.Contains("absent"));
            var value = Reader.Read<int>("absent", 5);
            Assert.IsFalse(Reader.Contains("absent"));
        }
    }
}
