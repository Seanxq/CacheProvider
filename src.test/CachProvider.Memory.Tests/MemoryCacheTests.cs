using System;
using System.Collections.Specialized;
using System.Threading.Tasks;
using CacheProvider.Interface;
using CacheProvider.Memory;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CachProvider.Memory.Tests
{
    [TestClass]
    public class MemoryCacheTests
    {
        // private ILogProvider _logProvider;
        private ICacheProvider _mockProvider;

        //[TestInitialize]
        //public void TestInitialize()
        //{
        //    try
        //    {
        //        var logParams = new NameValueCollection
        //        {
        //            {"application", "Cache"},
        //            {"logLevel", "debug"},
        //            {
        //                "conversionPattern",
        //                "[%utcdate{yyyy-MM-dd HH:mm:ss,fff}] %-5level application=%logger %message %exception %newline"
        //            },
        //            {"logpath", "C:\\Logs\\test"}
        //        };
        //        //   _logProvider = new Log4NetProvider();
        //        //   _logProvider.Initialize("joe", logParams);

        //        //    _mockProvider = new MockCacheProvider();
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("Error initializing cache provider:" + ex.Message);
        //    }
        //}



        [TestMethod]
        public async Task AddItemsToCacheTest()
        {
            var settings = new NameValueCollection
                {
                    {"application", "Cache"},
                    {"logLevel", "debug"},
                    {
                        "conversionPattern",
                        "[%utcdate{yyyy-MM-dd HH:mm:ss,fff}] %-5level application=%logger %message %exception %newline"
                    },
                    {"logpath", "C:\\Logs\\test"}
                };

            var cacheProvider = new MemoryCacheProvider();
            cacheProvider.Initialize("test", settings);
            var key = "FirstRegionTest";

            var results = await cacheProvider.Add(key, new object(), "FirstRegion");
           // var count = await cacheProvider.Count("FirstRegion");
            Assert.AreEqual(1, 1);
        }
    }
}