using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using CacheProvider.Mongo.Model;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.Linq;

namespace CacheProvider.Mongo
{
    public class MongoCacheProvider : CacheProvider
    {
        private string _mongoConnectionString;
        private int _cacheExpirationTime;
        private bool _isEnabled;

        /// <summary>
        ///     Initialize from config
        /// </summary>
        /// <param name="name"></param>
        /// <param name="config">Config properties</param>
        public override void Initialize(string name, NameValueCollection config)
        {
            base.Initialize(name, config);

            var timeout = config["timeout"];
            if (string.IsNullOrEmpty(timeout))
            {
                timeout = "10";
            }

            _cacheExpirationTime = 60;

            if (!int.TryParse(timeout, out _cacheExpirationTime))
            {
                throw new ConfigurationErrorsException("invalid timeout value");
            }

            var host = config["host"];
            if (string.IsNullOrEmpty(host))
            {
                throw new ConfigurationErrorsException("host must be set to the appropriate value");
            }

            // port is optional, if not provided it will use the base mongo port number
            var port = config["port"];

            var baseDbName = config["env"];
            if (string.IsNullOrEmpty(baseDbName))
            {
                throw new ConfigurationErrorsException("env must be set to the appropriate value");
            }

            _isEnabled = true;
            bool.TryParse(config["enable"], out _isEnabled);

            _mongoConnectionString = host.ToLower().StartsWith("mongodb://") ?
                host : MongoUtilities.GetMongoDatabaseString(host, port, baseDbName);
        }

        /// <summary>
        ///     Get from cache.
        /// </summary>
        /// <param name="cacheKey">The cache key.</param>
        /// <param name="region"></param>
        /// <returns>
        ///     An object instance with the Cache Value corresponding to the entry if found, else null
        /// </returns>
        public override async Task<object> Get(object cacheKey, string region)
        {
            if (!_isEnabled)
            {
                return null;
            }

            var item = await GetItem(cacheKey, region);

            if (item == null)
            {
                return null;
            }

            return await MemoryStreamHelper.DeserializeObject(item.CacheObject);
        }

        /// <summary>
        ///     Gets the specified cache key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cacheKey">The cache key.</param>
        /// <param name="region"></param>
        /// <returns>An Instance of T if the entry is found, else null.</returns>
        public override async Task<T> Get<T>(object cacheKey, string region)
        {
            return (T) await Get(cacheKey, region);
        }

        /// <summary>
        /// Check if the item exist
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <param name="region"></param>
        /// <returns>true false</returns>
        public override async Task<bool> Exist(object cacheKey, string region)
        {
            return _isEnabled && await GetItem(cacheKey, region) != null;
        }

        /// <summary>
        /// Adds the specified cache key.
        /// </summary>
        /// <param name="cacheKey">The cache key.</param>
        /// <param name="cacheObject">The cache object.</param>
        /// <param name="region"></param>
        /// <param name="expirationInMinutes"></param>
        /// <returns>True if successful else false.</returns>
        public override async Task<bool> Add(object cacheKey, object cacheObject, string region, int expirationInMinutes = 15)
        {
            if (!_isEnabled)
            {
                return true;
            }

            var expireCacheTime = expirationInMinutes == 15 ? _cacheExpirationTime : expirationInMinutes;

            if (await GetItem(cacheKey, region) != null)
            {
                await Remove(cacheKey, region);
            }

            var expireTime = DateTime.UtcNow.AddMinutes(expireCacheTime);
            var item = new CacheItem
            {
                CacheKey = cacheKey.ToString(),
                Expires = expireTime,
                CacheObject = await MemoryStreamHelper.SerializeObject(cacheObject)
            };

            return await CreateUpdateItem(region, item);

        }

        /// <summary>
        /// Adds to the cache and sets a slidding time, each time and cache item is read it's expire time refreshes
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <param name="cacheObject"></param>
        /// <param name="region"></param>
        /// <param name="allowSliddingTime"></param>
        /// <param name="expirationInMinutes"></param>
        /// <returns></returns>
        public override async Task<bool> Add(object cacheKey, object cacheObject, string region, bool allowSliddingTime, int expirationInMinutes = 15)
        {
            if (!_isEnabled)
            {
                return true;
            }

            var expireCacheTime = expirationInMinutes == 15 ? _cacheExpirationTime : expirationInMinutes;
            var item = new CacheItem();
            var exist = await GetItem(cacheKey, region);

            if (exist == null)
            {
                item.CacheKey = cacheKey.ToString();
            }
            else
            {
                item = exist;
            }

            var expireTime = DateTime.UtcNow.AddMinutes(expireCacheTime);
            item.Expires = expireTime;

            item.CacheObject = await MemoryStreamHelper.SerializeObject(cacheObject);
            item.AllowSliddingTime = allowSliddingTime;

            return await CreateUpdateItem(region, item);
        }
        
        /// <summary>
        /// Adds something to the cache that will only be removed by manual deletion
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <param name="cacheObject"></param>
        /// <param name="region"></param>
        /// <returns></returns>
        public override async Task<bool> AddPermanent(object cacheKey, object cacheObject, string region)
        {
            if (!_isEnabled)
            {
                return true;
            }

            if (await GetItem(cacheKey, region) != null)
            {
                await Remove(cacheKey, region);
            }

            var expireTime = DateTime.UtcNow.AddYears(100);
            var item = new CacheItem
            {
                CacheKey = cacheKey.ToString(),
                Expires = expireTime,
                CacheObject = await MemoryStreamHelper.SerializeObject(cacheObject)
            };

            return await CreateUpdateItem(region, item);
        }

        /// <summary>
        /// removes an item from cache region
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <param name="region"></param>
        /// <returns></returns>
        public override async Task<bool> Remove(object cacheKey, string region)
        {
            if (!_isEnabled)
            {
                return true;
            }

            var mongoCollection = MongoUtilities.InitializeMongoDatabase(region, _mongoConnectionString);

            var query = Query.EQ("cacheKey", cacheKey.ToString());
            var results = await Task.Factory.StartNew(() => mongoCollection.Remove(query));
            return await MongoUtilities.VerifyReturnMessage(results);
        }

        /// <summary>
        /// removes all items from cache from all regions
        /// </summary>
        /// <returns></returns>
        public async override Task<bool> RemoveAll()
        {
            if (!_isEnabled)
            {
                return true;
            }

            await
                Task.Factory.StartNew(
                    () => MongoUtilities.GetDatabaseFromUrl(new MongoUrl(_mongoConnectionString)).Drop());
            return true;
        }

        /// <summary>
        /// removes all items from a cache region
        /// </summary>
        /// <param name="region"></param>
        /// <returns></returns>
        public async override Task<bool> RemoveAll(string region)
        {
            if (!_isEnabled)
            {
                return true;
            }

            var mongoDatabase = MongoUtilities.GetDatabaseFromUrl(new MongoUrl(_mongoConnectionString));
            await Task.Factory.StartNew(() => mongoDatabase.DropCollection(region));
            return true;
        }

        /// <summary>
        /// remove expired items by region
        /// </summary>
        /// <param name="region"></param>
        /// <returns></returns>
        public async override Task<bool> RemoveExpired(string region)
        {
            if (!_isEnabled)
            {
                return true;
            }

            var mongoCollection = MongoUtilities.InitializeMongoDatabase(region, _mongoConnectionString);

            var item = await Task.Factory.StartNew(() => mongoCollection.AsQueryable<CacheItem>().AsParallel().Where(x => x.Expires < DateTime.UtcNow).Select(x => x.Id));

            var query2 = Query.In("_id", new BsonArray(item));
            mongoCollection.Remove(query2);

            if (item.Any())
            {
                // todo add logging
            }

            return true;
        }

        /// <summary>
        /// get the count of items in a cache region
        /// </summary>
        /// <param name="region"></param>
        /// <returns></returns>
        public override async Task<long> Count(string region)
        {
            if (!_isEnabled)
            {
                return 0;
            }
            var mongoCollection = MongoUtilities.InitializeMongoDatabase(region, _mongoConnectionString);
            return await Task.Factory.StartNew(() => mongoCollection.Count());
        }

        #region Helpers
        private async Task<bool> CreateUpdateItem(string region, CacheItem item)
        {
            var mongoCollection = MongoUtilities.InitializeMongoDatabase(region, _mongoConnectionString);

            Task.Factory.StartNew(() => RemoveExpired(region));
            var results = await Task.Factory.StartNew(() => mongoCollection.Save(item));
            return await MongoUtilities.VerifyReturnMessage(results);
        }

        private async Task<CacheItem> GetItem(object cacheKey, string region)
        {
            var mongoCollection = MongoUtilities.InitializeMongoDatabase(region, _mongoConnectionString);

            var item = await Task.Factory.StartNew(() => mongoCollection.AsQueryable<CacheItem>()
                    .AsParallel()
                    .FirstOrDefault(x => x.CacheKey.Equals(cacheKey.ToString(), StringComparison.CurrentCultureIgnoreCase) && x.Expires > DateTime.UtcNow));

            if (!item.AllowSliddingTime) return item;

            item.Expires = DateTime.UtcNow.AddMinutes(_cacheExpirationTime);
            await CreateUpdateItem(region, item);

            return item;
        }
        #endregion
    }
}