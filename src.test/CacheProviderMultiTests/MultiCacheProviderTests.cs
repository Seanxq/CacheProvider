using System;
using System.Collections.Specialized;
using System.Threading.Tasks;
using CacheProvider.Mongo;
using CacheProvider.Multi;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CacheProviderMultiTests
{
    [TestClass]
    public class MultiCacheProviderTests
    {
        private MultiCacheProvider _cacheProvider;

        private readonly NameValueCollection _enabledSettings = new NameValueCollection
            {
                {"application", "Cache"},
                {"timeout", "5"},
                {"host", "127.0.0.1"},
                {"port","27017"},
                {"env", "UnitTest"},
                {"providers", "MemoryCacheProvider, MongoCacheProvider"}
            };

        private readonly NameValueCollection _disabledSettings = new NameValueCollection
            {
                {"application", "Cache"},
                {"timeout", " 5"},
                {"host", "127.0.0.1"},
                {"port","27017"},
                {"env", "UnitTest"},
                {"enable", "false"},
                {"providers", "MemoryCacheProvider, MongoCacheProvider"}
            };

        [TestInitialize]
        public void TestInitialize()
        {
            _cacheProvider = new MultiCacheProvider();
        }


        [TestMethod]
        public async Task InitializationTest()
        {
            _cacheProvider.Initialize("test", _enabledSettings);
            var items = _cacheProvider.CacheProviders;
            Assert.AreEqual(items.Count, 2);

        }
    }
}
