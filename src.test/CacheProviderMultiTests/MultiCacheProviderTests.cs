using System.Collections.Specialized;
using System.Threading.Tasks;
using CacheProvider.Memory;
using CacheProvider.Mongo;
using CacheProvider.Multi;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CacheProviderMultiTests
{
    [TestClass]
    public class MultiCacheProviderTests
    {
        private MultiCacheProvider _cacheProvider;
        private MongoCacheProvider _mongoCache;
        private MemoryCacheProvider _memoryCache;

        private readonly NameValueCollection _enabledSettings = new NameValueCollection
            {
                {"application", "CacheMulti"},
                {"timeout", "5"},
                {"host", "127.0.0.1"},
                {"port","27017"},
                {"env", "UnitTestMulti"},
                {"providers", "MemoryCacheProvider, MongoCacheProvider"}
            };

        private readonly NameValueCollection _disabledSettings = new NameValueCollection
            {
                {"application", "CacheMulti"},
                {"timeout", " 5"},
                {"host", "127.0.0.1"},
                {"port","27017"},
                {"env", "UnitTestMulti"},
                {"enable", "false"},
                {"providers", "MemoryCacheProvider, MongoCacheProvider"}
            };

        [TestInitialize]
        public void TestInitialize()
        {
            _cacheProvider = new MultiCacheProvider();
            _mongoCache = new MongoCacheProvider();
            _memoryCache = new MemoryCacheProvider();
            _mongoCache.Initialize("CacheMulti", _enabledSettings);
            _memoryCache.Initialize("CacheMulti", _enabledSettings);
            Task.FromResult(_mongoCache.RemoveAll());
        }

        [TestCleanup]
        public void TestCleanup()
        {
            Task.FromResult(_mongoCache.RemoveAll());
            Task.FromResult(_memoryCache.RemoveAll());
        }



        [TestMethod]
        public async Task InitializationTest()
        {
            _cacheProvider.Initialize("CacheMulti", _enabledSettings);
            var items = _cacheProvider.CacheProviders;
            Assert.AreEqual(items.Count, 2);

        }

        #region functionTest
        [TestMethod]
        public async Task GetItemThatDoesNotExistTest()
        {
            _cacheProvider.Initialize("CacheMulti", _enabledSettings);
            const string key = "TestKey";

            var results = await _cacheProvider.Get(key, "FirstRegion");
            Assert.IsNull(results);
        }

        [TestMethod]
        public async Task GetGenericItemThatDoesNotExistTest()
        {
            _cacheProvider.Initialize("CacheMulti", _enabledSettings);
            const string key = "TestKey";

            var results = await _cacheProvider.Get<object>(key, "FirstRegion");
            Assert.IsNull(results);
        }

        [TestMethod]
        public async Task AddItemsToCacheTest()
        {
            _cacheProvider.Initialize("CacheMulti", _enabledSettings);
            const string key = "TestKey";
            const string region = "FirstRegion";

            await _cacheProvider.Add(key, new object(), region);
            var count = await _cacheProvider.Count(region);
            Assert.AreEqual(1, count);
        }

        [TestMethod]
        public async Task AddAndGetTest()
        {
            _cacheProvider.Initialize("CacheMulti", _enabledSettings);
            const string key = "TestKey";
            const string region = "FirstRegion";
            const string cacheObject = "CacheMulti";
            await _cacheProvider.Add(key, cacheObject, region);
            var item = await _cacheProvider.Get(key, region);
            Assert.AreEqual(item, cacheObject);
        }

        [TestMethod]
        public async Task AddSliderTest()
        {
            _cacheProvider.Initialize("CacheMulti", _enabledSettings);
            const string key = "TestKey";
            const string region = "FirstRegion";
            const string cacheObject = "CacheMulti";
            Assert.IsTrue(await _cacheProvider.Add(key, cacheObject, region, true));
        }

        [TestMethod]
        public async Task AddPermTest()
        {
            _cacheProvider.Initialize("CacheMulti", _enabledSettings);
            const string key = "TestKey";
            const string region = "FirstRegion";
            const string cacheObject = "CacheMulti";
            Assert.IsTrue(await _cacheProvider.AddPermanent(key, cacheObject, region));
        }

        [TestMethod]
        public async Task AddAndGetCastTest()
        {
            _cacheProvider.Initialize("CacheMulti", _enabledSettings);
            const string key = "TestKey";
            const string region = "FirstRegion";
            const int cacheObject = 111;
            await _cacheProvider.Add(key, cacheObject, region);
            var item = await _cacheProvider.Get<int>(key, region);
            Assert.IsTrue(ReferenceEquals(item.GetType(), cacheObject.GetType()));
            Assert.AreEqual(item, cacheObject);
        }

        [TestMethod]
        public async Task AddAndExistTest()
        {
            _cacheProvider.Initialize("CacheMulti", _enabledSettings);
            const string key = "TestKey";
            const string region = "FirstRegion";
            const int cacheObject = 111;
            await _cacheProvider.Add(key, cacheObject, region);
            var item = await _cacheProvider.Exist(key, region);
            Assert.IsTrue(item);
        }

        [TestMethod]
        public async Task RemoveTest()
        {
            _cacheProvider.Initialize("CacheMulti", _enabledSettings);
            const string key = "TestKey";
            const string key1 = "TestKey1";
            const string region = "FirstRegion";
            const int cacheObject = 111;
            await _cacheProvider.Add(key, cacheObject, region);
            await _cacheProvider.Add(key1, cacheObject, region);
            await _cacheProvider.Remove(key, region);
            var item = await _cacheProvider.Exist(key, region);
            Assert.IsFalse(item);
        }
        #endregion

        #region Disable Test

        [TestMethod]
        public async Task DisableAddTest()
        {
            _cacheProvider.Initialize("CacheMulti", _disabledSettings);
            const string key = "TestKey";

            Assert.IsTrue(await _cacheProvider.Add(key, new object(), "FirstRegion"));
        }

        [TestMethod]
        public async Task DisableAddSliderTest()
        {
            _cacheProvider.Initialize("CacheMulti", _disabledSettings);
            const string key = "TestKey";

            Assert.IsTrue(await _cacheProvider.Add(key, new object(), "FirstRegion", true));
        }

        [TestMethod]
        public async Task DisableAddPermTest()
        {
            _cacheProvider.Initialize("CacheMulti", _disabledSettings);
            const string key = "TestKey";

            Assert.IsTrue(await _cacheProvider.AddPermanent(key, new object(), "FirstRegion"));
        }

        [TestMethod]
        public async Task DisableGetTest()
        {
            _cacheProvider.Initialize("CacheMulti", _disabledSettings);
            const string key = "TestKey";

            Assert.IsNull(await _cacheProvider.Get(key, "FirstRegion"));
        }

        [TestMethod]
        public async Task DisableGetCastTest()
        {
            _cacheProvider.Initialize("CacheMulti", _disabledSettings);
            const string key = "TestKey";

            Assert.IsNull(await _cacheProvider.Get<string>(key, "FirstRegion"));
        }

        [TestMethod]
        public async Task DisableRemoveTest()
        {
            _cacheProvider.Initialize("CacheMulti", _disabledSettings);
            const string key = "TestKey";

            Assert.IsTrue(await _cacheProvider.Remove(key, "FirstRegion"));
        }

        [TestMethod]
        public async Task DisableCountTest()
        {
            _cacheProvider.Initialize("CacheMulti", _disabledSettings);
            Assert.AreEqual(await _cacheProvider.Count("FirstRegion"), 0);
        }
        #endregion
    }
}