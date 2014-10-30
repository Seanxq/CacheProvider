using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Runtime.Caching;
using System.Threading.Tasks;
using CacheProvider.Model;

namespace CacheProvider.Memory
{
    public class MemoryCacheProvider : CacheProvider
    {
        private static MemoryCache _cache;
        private readonly CacheItemPolicy policy;
        

        private int _cacheExpirationTime;
        private bool _isEnabled;
        static readonly object _sync = new object();

        public MemoryCacheProvider()
            : this(MemoryCache.Default)
        {
        }

        private MemoryCacheProvider(MemoryCache cache, int? slidingExpirationSeconds = 1800)
        {
            _cache = cache;
            policy = new CacheItemPolicy();
            var expirationSeconds = slidingExpirationSeconds.HasValue ? slidingExpirationSeconds.Value : 1800;
            policy.SlidingExpiration = TimeSpan.FromSeconds(expirationSeconds);
        }

        public override void Initialize(string name, NameValueCollection config)
        {
            base.Initialize(name, config);

            var timeout = config["timeout"];
            if (string.IsNullOrEmpty(timeout))
            {
                timeout = "10";
            }

            _isEnabled = true;
            var enabled = config["enable"];
            if (enabled == null)
            {
                _isEnabled = true;
            }
            else
            {
                bool.TryParse(config["enable"], out _isEnabled);  
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
            var item = (BaseModel)_cache.Get(key, optionalKey);

            return await MemoryStreamHelper.DeserializeObject(item.CacheObject);
        }

        public override async Task<T> Get<T>(string cacheKey, string optionalKey)
        {
            if (!_isEnabled)
            {
                return default(T);
            }

            var key = optionalKey + cacheKey;
            var item = (BaseModel)_cache.Get(key, optionalKey);
            return (T) await MemoryStreamHelper.DeserializeObject(item.CacheObject);
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

            bool results;
            lock (_sync)
            {
                results = _cache.Add(key, item, cacheItemPolicy, optionalKey);
            }

           
            return results;
        }

        public override async Task<bool> Remove(string cacheKey, string optionalKey)
        {
            var key = optionalKey + cacheKey;
            await Task.Factory.StartNew(() => _cache.Remove(key, optionalKey));
            return true;
        }

        public override async Task<bool> RemoveAll()
        {
            var cacheKeys = _cache.Select(kvp => kvp.Key).ToList();
            foreach (var ck in cacheKeys)
            {
                
            }
            throw new NotImplementedException();
        }

        public override async Task<bool> RemoveAll(string optionalKey)
        {
            var test = MemoryCache.Default.Select(kvp => kvp.Key).ToList();
            var cacheKeys = _cache.Select(kvp => kvp.Key).ToList();
            foreach (var ck in cacheKeys)
            {
                await Remove(ck, optionalKey);
            }
            return true;
        }

        public override Task<bool> RemoveExpired(string optionalKey)
        {
            throw new NotImplementedException();
        }

        public override async Task<long> Count(string optionalKey)
        {
           return await Task.Factory.StartNew(() => _cache.GetCount(optionalKey));
        }





    }
}
