using System.Collections.Specialized;
using System.Text;
using System.Threading.Tasks;
using CacheProvider.Mongo;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;
using Moq;

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

        // timeout=10;host=vmmongocluster1-1;port=27017;env=FrontEndApi;enable=true"

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
           Task.FromResult( _cacheProvider.RemoveAll());
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

        //http://blog.wjshome.com/how-to-mock-mongocollection-with-moq

        [TestMethod]
        public async Task AddItemsToCacheTest()
        {
            const string key = "TestKey";
            const string region = "FirstRegion";
            

            var message = string.Empty;
            var server = new Mock<MongoServer>(_serverSettings);
            server.Setup(s => s.Settings).Returns(_serverSettings);
            server.Setup(s => s.IsDatabaseNameValid(It.IsAny<string>(), out message)).Returns(true);

            var database = new Mock<MongoDatabase>(server.Object, "test", _databaseSettings);
            database.Setup(db => db.Settings).Returns(_databaseSettings);
            database.Setup(db => db.IsCollectionNameValid(It.IsAny<string>(), out message)).Returns(true);

            var cache = new MongoCacheProvider(database.Object);
            cache.Initialize("test", _enabledSettings);

            var test = await cache.Add(key, new object(), region);
            Assert.IsTrue(test);






          //  _cacheProvider.Initialize("test", _enabledSettings);
            

            if (await _cacheProvider.Add(key, new object(), region))
            {
                var count = await _cacheProvider.Count(region);
                Assert.AreEqual(1, count);
            }
            else
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public async Task AddAndGetTest()
        {
            _cacheProvider.Initialize("test", _enabledSettings);
            const string key = "TestKey";
            const string region = "FirstRegion";
            const string cacheObject = "test";
            if (await _cacheProvider.Add(key, cacheObject, region))
            {
                var item = await _cacheProvider.Get(key, region);
                Assert.AreEqual(item, cacheObject);
            }
            else
            {
                Assert.Fail();
            }
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
            if (await _cacheProvider.Add(key, cacheObject, region))
            {
                var item = await _cacheProvider.Get<int>(key, region);
                Assert.IsTrue(ReferenceEquals(item.GetType(), cacheObject.GetType()));
                Assert.AreEqual(item, cacheObject);
            }
            else
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public async Task AddAndExistTest()
        {
            _cacheProvider.Initialize("test", _enabledSettings);
            const string key = "TestKey";
            const string region = "FirstRegion";
            const int cacheObject = 111;
            if (await _cacheProvider.Add(key, cacheObject, region))
            {
                var item = await _cacheProvider.Exist(key, region);
                Assert.IsTrue(item);
            }
            else
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public async Task RemoveTest()
        {
            _cacheProvider.Initialize("test", _enabledSettings);
            const string key = "TestKey";
            const string key1 = "TestKey1";
            const string region = "FirstRegion";
            const int cacheObject = 111;
            if (await _cacheProvider.Add(key, cacheObject, region) && await _cacheProvider.Add(key1, cacheObject, region))
            {
                await _cacheProvider.Remove(key, region);
                var item = await _cacheProvider.Exist(key, region);
                Assert.IsFalse(item);
            }
            else
            {
                Assert.Fail();
            }
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