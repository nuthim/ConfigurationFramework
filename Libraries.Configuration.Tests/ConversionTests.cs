using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Libraries.Configuration.Converters;
using System.Collections.Generic;

namespace Libraries.Configuration.Tests
{
    [TestClass]
    public class ConversionTests
    {
        private static StringConverterImpl Converter
        {
            get; set;
        }

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            Converter = new StringConverterImpl();
            ReaderConfiguration.RegisterTypeConverter(typeof(bool), new LogicalBooleanConverter());
            ReaderConfiguration.RegisterTypeConverter(typeof(object), new ObjectConverter());
        }

        [TestMethod]
        public void ConvertToPrimitive()
        {
            Assert.IsTrue(Converter.ConvertTo<int>("5") == 5);
            Assert.IsTrue(Converter.ConvertTo<uint>("5") == 5u);
            Assert.IsTrue(Converter.ConvertTo<long>("5") == 5);
            Assert.IsTrue(Converter.ConvertTo<ulong>("5") == 5);
            Assert.IsTrue(Converter.ConvertTo<string>("5") == "5");
            Assert.IsTrue(Converter.ConvertTo<string>(null) == null);
            Assert.IsTrue(Converter.ConvertTo<string>(string.Empty) == string.Empty);
            Assert.IsTrue(Converter.ConvertTo<float>("5") == 5.0f);
            Assert.IsTrue(Converter.ConvertTo<double>("5") == 5.0d);
            Assert.IsTrue(Converter.ConvertTo<decimal>("5") == 5.0m);
            Assert.IsTrue(Converter.ConvertTo<short>("5") == 5);
            Assert.IsTrue(Converter.ConvertTo<ushort>("5") == 5);
            Assert.IsTrue(Converter.ConvertTo<byte>("5") == 5);
            Assert.IsTrue(Converter.ConvertTo<sbyte>("5") == 5);
        }

        [TestMethod]
        public void ConvertFromPrimitive()
        {
            Assert.IsTrue(Converter.ConvertFrom<int>(5) == "5");
            Assert.IsTrue(Converter.ConvertFrom<uint>(5u) == "5");
            Assert.IsTrue(Converter.ConvertFrom<long>(5) == "5");
            Assert.IsTrue(Converter.ConvertFrom<ulong>(5) == "5");
            Assert.IsTrue(Converter.ConvertFrom<string>("5") == "5");
            Assert.IsTrue(Converter.ConvertFrom<string>(null) == null);
            Assert.IsTrue(Converter.ConvertFrom<string>(string.Empty) == string.Empty);
            Assert.IsTrue(Converter.ConvertFrom<float>(5f) == "5");
            Assert.IsTrue(Converter.ConvertFrom<double>(5.0d) == "5");
            Assert.IsTrue(Converter.ConvertFrom<decimal>(0.55m) == "0.55");
            Assert.IsTrue(Converter.ConvertFrom<short>(5) == "5");
            Assert.IsTrue(Converter.ConvertFrom<ushort>(5) == "5");
            Assert.IsTrue(Converter.ConvertFrom<byte>(5) == "5");
            Assert.IsTrue(Converter.ConvertFrom<sbyte>(5) == "5");
        }

        [TestMethod]
        public void ConvertToBoolean()
        {
            Assert.IsTrue(Converter.ConvertTo<bool>("true") == true);
            Assert.IsTrue(Converter.ConvertTo<bool>("FALSE") == false);
            Assert.IsTrue(Converter.ConvertTo<bool>("1") == true);
            Assert.IsTrue(Converter.ConvertTo<bool>("0") == false);
            Assert.IsTrue(Converter.ConvertTo<bool>("T") == true);
            Assert.IsTrue(Converter.ConvertTo<bool>("f") == false);
            Assert.IsTrue(Converter.ConvertTo<bool>("Y") == true);
            Assert.IsTrue(Converter.ConvertTo<bool>("n") == false);
            Assert.IsTrue(Converter.ConvertTo<bool>("Yes") == true);
            Assert.IsTrue(Converter.ConvertTo<bool>("NO") == false);
        }

        [TestMethod]
        public void ConvertFromBoolean()
        {
            Assert.IsTrue(Converter.ConvertFrom<bool>(true) == bool.TrueString);
            Assert.IsTrue(Converter.ConvertFrom<bool>(false) == bool.FalseString);
        }

        [TestMethod]
        public void ConvertToIEnumerableBoolean()
        {
            //Alternating true and false values
            var array = Converter.ConvertTo<IEnumerable<bool>>("true,false,TRUE,FALSE,T,F,t,f,1,0,yes,no,YES,NO,Y,N");

            //Assert all even indexed elements are true
            var trueArray = array.Where((x, i) => i % 2 == 0);
            Assert.IsTrue(trueArray.Aggregate(true, (x, result) => x & result));

            //Assert all odd indexed elements are false
            var falseArray = array.Where((x, i) => i % 2 != 0);
            Assert.IsFalse(falseArray.Any(x => x == true));
        }

        [TestMethod]
        public void ConvertToArray()
        {
            string array = "1, 2, 3, 4, 5";
            Assert.IsTrue(Converter.ConvertTo<string[]>(array).Count() == 5);
            Assert.IsTrue(Converter.ConvertTo<int[]>(array).Count() == 5);
            Assert.IsTrue(Converter.ConvertTo<float[]>(array).Count() == 5);
            Assert.IsTrue(Converter.ConvertTo<decimal[]>(array).Count() == 5);
            Assert.IsTrue(Converter.ConvertTo<uint[]>(array).Count() == 5);
            Assert.IsTrue(Converter.ConvertTo<short[]>(array).Count() == 5);
        }

        [TestMethod]
        public void ConvertFromArray()
        {
            Assert.IsTrue(Converter.ConvertFrom<string[]>(new[] {"1", null, string.Empty, "!@#$%^&*()", "happy mother's day!" }) == "1,,,!@#$%^&*(),happy mother's day!");
            Assert.IsTrue(Converter.ConvertFrom<int[]>(new[] { 1, 2, 3, 4, 5 }) == "1,2,3,4,5");
            Assert.IsTrue(Converter.ConvertFrom<decimal[]>(new decimal[] { 0, 2.5m, 3, 445.45m, 5 }) == "0,2.5,3,445.45,5");
            Assert.IsTrue(Converter.ConvertFrom<ushort[]>(new ushort[] { 1, 2, 3, 4, 5 }) == "1,2,3,4,5");
        }

        [TestMethod]
        public void ConvertToEnumerable()
        {
            string array = "1, 2, 3, 4, 5";
            Assert.IsTrue(Converter.ConvertTo<IEnumerable<string>>(array).Count() == 5);
            Assert.IsTrue(Converter.ConvertTo<IEnumerable<int>>(array).Count() == 5);
            Assert.IsTrue(Converter.ConvertTo<IEnumerable<float>>(array).Count() == 5);
            Assert.IsTrue(Converter.ConvertTo<IEnumerable<decimal>>(array).Count() == 5);
            Assert.IsTrue(Converter.ConvertTo<IEnumerable<uint>>(array).Count() == 5);
            Assert.IsTrue(Converter.ConvertTo<IEnumerable<short>>(array).Count() == 5);
        }

        [TestMethod]
        public void ConvertFromEnumerable()
        {
            Assert.IsTrue(Converter.ConvertFrom<IEnumerable<string>>(new[] { "1", "2", "3", "4", "5" }) == "1,2,3,4,5");
            Assert.IsTrue(Converter.ConvertFrom<IEnumerable<int>>(new[] { 1, 2, 3, 4, 5 }) == "1,2,3,4,5");
            Assert.IsTrue(Converter.ConvertFrom<IEnumerable<decimal>>(new decimal[] { 1, 2, 3, 4, 5 }) == "1,2,3,4,5");
            Assert.IsTrue(Converter.ConvertFrom<IEnumerable<ushort>>(new ushort[] { 1, 2, 3, 4, 5 }) == "1,2,3,4,5");
        }

        [TestMethod]
        public void ConvertToIList()
        {
            string array = "1, 2, 3, 4, 5";
            Assert.IsTrue(Converter.ConvertTo<IList<string>>(array).Count() == 5);
            Assert.IsTrue(Converter.ConvertTo<IList<int>>(array).Count() == 5);
            Assert.IsTrue(Converter.ConvertTo<IList<float>>(array).Count() == 5);
            Assert.IsTrue(Converter.ConvertTo<IList<decimal>>(array).Count() == 5);
            Assert.IsTrue(Converter.ConvertTo<IList<uint>>(array).Count() == 5);
            Assert.IsTrue(Converter.ConvertTo<IList<short>>(array).Count() == 5);
        }

        [TestMethod]
        public void ConvertToList()
        {
            string array = "1, 2, 3, 4, 5";
            Assert.IsTrue(Converter.ConvertTo<List<string>>(array).Count() == 5);
            Assert.IsTrue(Converter.ConvertTo<List<int>>(array).Count() == 5);
            Assert.IsTrue(Converter.ConvertTo<List<float>>(array).Count() == 5);
            Assert.IsTrue(Converter.ConvertTo<List<decimal>>(array).Count() == 5);
            Assert.IsTrue(Converter.ConvertTo<List<uint>>(array).Count() == 5);
            Assert.IsTrue(Converter.ConvertTo<List<short>>(array).Count() == 5);
        }
    }
}
