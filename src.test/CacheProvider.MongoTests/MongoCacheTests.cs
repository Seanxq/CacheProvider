using System;
using System.Collections.Specialized;
using CacheProvider.Mongo;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CacheProvider.MongoTests
{
    [TestClass]
    public class MongoCacheTests
    {
        [TestMethod]
        public void AddItemsToCacheTest()
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
           
            var cacheProvider = new MongoCacheProvider();
            cacheProvider.Initialize("test", settings);
            var key = "FirstRegionTest";

            var results = cacheProvider.Add(key, new object(), "FirstRegion");
            var count = cacheProvider.Count("FirstRegion");
            Assert.AreEqual(count, 1);
    
        }
    }
}
