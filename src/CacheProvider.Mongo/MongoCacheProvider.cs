using System;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
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

        public override void Initialize(string name, NameValueCollection config)
        {
            base.Initialize(name, config);

            var timeout = config["timeout"];
            if (string.IsNullOrEmpty(timeout))
            {
                throw new ConfigurationErrorsException("timeout must be set to the appropriate value");
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

        private MongoCollection InitializeMongoDatabase(string tenantApiKey)
        {
            var mongoDatabase = MongoUtilities.GetDatabaseFromUrl(new MongoUrl(_mongoConnectionString));
            var mongoCollection = mongoDatabase.GetCollection(tenantApiKey);
            mongoCollection.CreateIndex("CacheKey");
            return mongoCollection;
        }

        public override async Task<object> Get(string cacheKey, string tenantApiKey)
        {
            if (!_isEnabled)
            {
                return null;
            }

            var mongoCollection = InitializeMongoDatabase(tenantApiKey);

            var item = await Task.Factory.StartNew(() => mongoCollection.AsQueryable<CacheItem>()
                    .AsParallel()
                    .FirstOrDefault(x => x.CacheKey.Equals(cacheKey, StringComparison.CurrentCultureIgnoreCase) && x.Expires > DateTime.UtcNow));

            if (item == null)
            {
                return null;
            }

            var formatter = new BinaryFormatter();
            var ms = new MemoryStream(item.CacheObject);
            object cacheOject = formatter.Deserialize(ms);
            return cacheOject;
        }

        public override async Task<T> Get<T>(string cacheKey, string tenantApiKey)
        {
            if (!_isEnabled)
            {
                return default(T);
            }

            var mongoCollection = InitializeMongoDatabase(tenantApiKey);

            var item = await Task.Factory.StartNew(() => mongoCollection.AsQueryable<CacheItem>()
                    .AsParallel()
                    .FirstOrDefault(x => x.CacheKey.Equals(cacheKey, StringComparison.CurrentCultureIgnoreCase) && x.Expires > DateTime.UtcNow));

            if (item == null)
            {
                return default(T);
            }

            var formatter = new BinaryFormatter();
            var ms = new MemoryStream(item.CacheObject);
            return (T)formatter.Deserialize(ms);
        }

        /// <summary>
        /// Adds the specified cache key.
        /// </summary>
        /// <param name="cacheKey">The cache key.</param>
        /// <param name="cacheObject">The cache object.</param>
        /// <param name="tenantApiKey"></param>
        /// <returns>True if successful else false.</returns>
        public override async Task<bool> Add(string cacheKey, object cacheObject, string tenantApiKey)
        {
            if (!_isEnabled)
            {
                return true;
            }
            var mongoCollection = InitializeMongoDatabase(tenantApiKey);

            if (await Task.Factory.StartNew(() => mongoCollection.AsQueryable<CacheItem>()
                .AsParallel()
                .FirstOrDefault(x => x.CacheKey.Equals(cacheKey, StringComparison.CurrentCultureIgnoreCase))) !=
                null)
            {

                await Remove(cacheKey, tenantApiKey);
            }

            var formatter = new BinaryFormatter();
            var ms = new MemoryStream();
            formatter.Serialize(ms, cacheObject);

            var item = new CacheItem
            {
                CacheKey = cacheKey,
                Expires = DateTime.UtcNow.AddMinutes(_cacheExpirationTime),
                CacheObject = ms.ToArray()
            };

            Task.Factory.StartNew(() => RemoveExpired(tenantApiKey));
            var results = await Task.Factory.StartNew(() => mongoCollection.Save(item));
            return await VerifyReturnMessage(results);

        }

        public override async Task<bool> Remove(string cacheKey, string tenantApiKey)
        {
            if (!_isEnabled)
            {
                return true;
            }

            var mongoCollection = InitializeMongoDatabase(tenantApiKey);

            var query = Query.EQ("cacheKey", cacheKey);
            var results = await Task.Factory.StartNew(() => mongoCollection.Remove(query));
            return await VerifyReturnMessage(results);
        }

        public async override Task<bool> RemoveAll()
        {
            await
                Task.Factory.StartNew(
                    () => MongoUtilities.GetDatabaseFromUrl(new MongoUrl(_mongoConnectionString)).Drop());
            return true;
        }

        public async override Task<bool> RemoveAll(string tenantApiKey)
        {
            var mongoDatabase = MongoUtilities.GetDatabaseFromUrl(new MongoUrl(_mongoConnectionString));
            await Task.Factory.StartNew(() => mongoDatabase.DropCollection(tenantApiKey));
            return true;
        }

        public async override Task<bool> RemoveExpired(string tenantApiKey)
        {
            var mongoCollection = InitializeMongoDatabase(tenantApiKey);

            var item = await Task.Factory.StartNew(() => mongoCollection.AsQueryable<CacheItem>().AsParallel().Where(x => x.Expires < DateTime.UtcNow).Select(x => x.Id));

            var query2 = Query.In("_id", new BsonArray(item));
            mongoCollection.Remove(query2);

            if (item.Any())
            {
               // todo add logging
            }

            return true;
        }

        private static async Task<bool> VerifyReturnMessage(GetLastErrorResult writeConcernResult)
        {
            return await Task.Factory.StartNew(() =>
            {
                if (writeConcernResult == null)
                {
                    return false;
                }

                if (string.IsNullOrEmpty(writeConcernResult.LastErrorMessage))
                {
                    return true;
                }
                return false;
            });
        }

















    }







}