using System.Collections.Specialized;
using System.Text;
using System.Threading.Tasks;
using CacheProvider.Mongo;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;

namespace CacheProvider.MongoTests
{
    [TestClass]
    public class MongoCacheTests
    {
        private MongoCacheProvider _cacheProvider;

        private readonly NameValueCollection _enabledSettings = new NameValueCollection
            {
                {"application", "Cache"},
                {"timeout", "5"},
                {"host", "127.0.0.1"},
                {"port","27017"},
                {"env", "UnitTest"}
            };

        private readonly NameValueCollection _disabledSettings = new NameValueCollection
            {
                {"application", "Cache"},
                {"timeout", " 5"},
                {"host", "127.0.0.1"},
                {"port","27017"},
                {"env", "UnitTest"},
                {"enable", "false"}
            };

        private readonly MongoServerSettings _serverSettings = new MongoServerSettings
        {
            GuidRepresentation = MongoDB.Bson.GuidRepresentation.Standard,
            ReadEncoding = new UTF8Encoding(),
            ReadPreference = new ReadPreference(),
            WriteConcern = new WriteConcern(),
            WriteEncoding = new UTF8Encoding()
        };

        private readonly MongoDatabaseSettings _databaseSettings = new MongoDatabaseSettings
        {
            GuidRepresentation = MongoDB.Bson.GuidRepresentation.Standard,
            ReadEncoding = new UTF8Encoding(),
            ReadPreference = new ReadPreference(),
            WriteConcern = new WriteConcern(),
            WriteEncoding = new UTF8Encoding()
        };


        [TestInitialize]
        public void TestInitialize()
        {
            _cacheProvider = new MongoCacheProvider();
            Task.FromResult(_cacheProvider.RemoveAll());
        }

        [TestCleanup]
        public void TestCleanup()
        {
            Task.FromResult(_cacheProvider.RemoveAll());
        }

        #region functionTest
        [TestMethod]
        public async Task GetItemThatDoesNotExistTest()
        {
            _cacheProvider.Initialize("test", _enabledSettings);
            const string key = "TestKey";

            var results = await _cacheProvider.Get(key, "FirstRegion");
            Assert.IsNull(results);
        }

        [TestMethod]
        public async Task GetGenericItemThatDoesNotExistTest()
        {
            _cacheProvider.Initialize("test", _enabledSettings);
            const string key = "TestKey";

            var results = await _cacheProvider.Get<object>(key, "FirstRegion");
            Assert.IsNull(results);
        }

        [TestMethod]
        public async Task AddItemsToCacheTest()
        {
            const string key = "TestKey";
            const string region = "FirstRegion";

            _cacheProvider.Initialize("test", _enabledSettings);
            Assert.IsTrue(await _cacheProvider.Add(key, new object(), region));
            var count = await _cacheProvider.Count(region);
            Assert.AreEqual(1, count);
        }

        [TestMethod]
        public async Task AddAndGetTest()
        {
            _cacheProvider.Initialize("test", _enabledSettings);
            const string key = "TestKey";
            const string region = "FirstRegion";
            const string cacheObject = "test";
            Assert.IsTrue(await _cacheProvider.Add(key, cacheObject, region));
            var item = await _cacheProvider.Get(key, region);
            Assert.AreEqual(item, cacheObject);
        }

        [TestMethod]
        public async Task AddSliderTest()
        {
            _cacheProvider.Initialize("test", _enabledSettings);
            const string key = "TestKey";
            const string region = "FirstRegion";
            const string cacheObject = "test";
            Assert.IsTrue(await _cacheProvider.Add(key, cacheObject, region, true));
        }

        [TestMethod]
        public async Task AddPermTest()
        {
            _cacheProvider.Initialize("test", _enabledSettings);
            const string key = "TestKey";
            const string region = "FirstRegion";
            const string cacheObject = "test";
            Assert.IsTrue(await _cacheProvider.AddPermanent(key, cacheObject, region));
        }

        [TestMethod]
        public async Task AddAndGetCastTest()
        {
            _cacheProvider.Initialize("test", _enabledSettings);
            const string key = "TestKey";
            const string region = "FirstRegion";
            const int cacheObject = 111;
            Assert.IsTrue(await _cacheProvider.Add(key, cacheObject, region));
            var item = await _cacheProvider.Get<int>(key, region);
            Assert.IsTrue(ReferenceEquals(item.GetType(), cacheObject.GetType()));
            Assert.AreEqual(item, cacheObject);
        }

        [TestMethod]
        public async Task AddAndExistTest()
        {
            _cacheProvider.Initialize("test", _enabledSettings);
            const string key = "TestKey";
            const string region = "FirstRegion";
            const int cacheObject = 111;
            Assert.IsTrue(await _cacheProvider.Add(key, cacheObject, region));
            var item = await _cacheProvider.Exist(key, region);
            Assert.IsTrue(item);
        }

        [TestMethod]
        public async Task RemoveTest()
        {
            _cacheProvider.Initialize("test", _enabledSettings);
            const string key = "TestKey";
            const string key1 = "TestKey1";
            const string region = "FirstRegion";
            const int cacheObject = 111;
            Assert.IsTrue(await _cacheProvider.Add(key, cacheObject, region));
            Assert.IsTrue(await _cacheProvider.Add(key1, cacheObject, region));
            Assert.IsTrue(await _cacheProvider.Remove(key, region));
            var item = await _cacheProvider.Exist(key, region);
            Assert.IsFalse(item);
        }
        #endregion

        #region Disable Test

        [TestMethod]
        public async Task DisableAddTest()
        {
            _cacheProvider.Initialize("test", _disabledSettings);
            const string key = "TestKey";

            Assert.IsTrue(await _cacheProvider.Add(key, new object(), "FirstRegion"));
        }

        [TestMethod]
        public async Task DisableAddSliderTest()
        {
            _cacheProvider.Initialize("test", _disabledSettings);
            const string key = "TestKey";

            Assert.IsTrue(await _cacheProvider.Add(key, new object(), "FirstRegion", true));
        }

        [TestMethod]
        public async Task DisableAddPermTest()
        {
            _cacheProvider.Initialize("test", _disabledSettings);
            const string key = "TestKey";

            Assert.IsTrue(await _cacheProvider.AddPermanent(key, new object(), "FirstRegion"));
        }

        [TestMethod]
        public async Task DisableGetTest()
        {
            _cacheProvider.Initialize("test", _disabledSettings);
            const string key = "TestKey";

            Assert.IsNull(await _cacheProvider.Get(key, "FirstRegion"));
        }

        [TestMethod]
        public async Task DisableGetCastTest()
        {
            _cacheProvider.Initialize("test", _disabledSettings);
            const string key = "TestKey";

            Assert.IsNull(await _cacheProvider.Get<string>(key, "FirstRegion"));
        }

        [TestMethod]
        public async Task DisableRemoveTest()
        {
            _cacheProvider.Initialize("test", _disabledSettings);
            const string key = "TestKey";

            Assert.IsTrue(await _cacheProvider.Remove(key, "FirstRegion"));
        }

        [TestMethod]
        public async Task DisableCountTest()
        {
            _cacheProvider.Initialize("test", _disabledSettings);
            Assert.AreEqual(await _cacheProvider.Count("FirstRegion"), 0);
        }
        #endregion
    }
}