using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using CacheProvider.Model;

namespace CacheProvider.Memory
{
    public class MemoryCacheProvider : CacheProvider
    {
        private static ObjectCache cache = MemoryCache.Default;
        private CacheItemPolicy policy = null;
        private CacheEntryRemovedCallback callback = null;

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
        }



        public override async Task<object> Get(string cacheKey, string optionalKey)
        {
            if (!_isEnabled)
            {
                return null;
            }
            var key = optionalKey + cacheKey;
            var item = (BaseModel)cache.Get(key, optionalKey);

            return await MemoryStreamHelper.DeserializeObject(item.CacheObject);
        }

        public override Task<T> Get<T>(string cacheKey, string optionalKey)
        {
            throw new NotImplementedException();
        }

        public override async Task<bool> Add(string cacheKey, object cacheObject, string optionalKey, int expirationInMinutes = 15)
        {
            if (!_isEnabled)
            {
                return true;
            }

            var key = optionalKey + cacheKey;
            var expireCacheTime = expirationInMinutes == 15 ? _cacheExpirationTime : expirationInMinutes;

            var cacheItemPolicy = new CacheItemPolicy
            {
                AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(expireCacheTime)
            };

            var cacheData = await MemoryStreamHelper.SerializeObject(cacheObject);

            var expireTime = DateTime.UtcNow.AddMinutes(expireCacheTime);
            var item = new BaseModel
            {
                CacheKey = key,
                Expires = expireTime,
                CacheObject = cacheData
            };

           return await Task.Factory.StartNew(() => cache.Add(key,item, cacheItemPolicy, optionalKey));
        }

        public override Task<bool> Remove(string cacheKey, string optionalKey)
        {
            throw new NotImplementedException();
        }

        public override Task<bool> RemoveAll()
        {
            throw new NotImplementedException();
        }

        public override Task<bool> RemoveAll(string optionalKey)
        {
            throw new NotImplementedException();
        }

        public override Task<bool> RemoveExpired(string optionalKey)
        {
            throw new NotImplementedException();
        }

        
    }
}
