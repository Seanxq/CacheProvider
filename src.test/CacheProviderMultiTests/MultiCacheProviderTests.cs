using System;
using System.Collections.Specialized;
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




        [TestMethod]
        public void TestMethod1()
        {
        }
    }
}
