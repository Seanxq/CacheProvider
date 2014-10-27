using System;
using System.Collections.Specialized;
using System.Threading.Tasks;
using CacheProvider.Interface;

namespace CacheProvider
{
    public class NullCacheProvider: ICacheProvider
    {
        public void Initialize(string name, NameValueCollection config)
        {
            Console.Write("Null Cache");
        }

        public Task<object> Get(string cacheKey, string optionalKey)
        {
            return null;
        }

        public Task<T> Get<T>(string cacheKey, string optionalKey)
        {
            return null;
        }

        public Task<bool> Add(string cacheKey, object cacheObject, string optionalKey, int expirationInMinutes = 15)
        {
            return Task.FromResult(true);
        }

        public Task<bool> Remove(string cacheKey, string optionalKey)
        {
            return Task.FromResult(true);
        }

        public Task<bool> RemoveAll()
        {
            return Task.FromResult(true);
        }

        public Task<bool> RemoveAll(string optionalKey)
        {
            return Task.FromResult(true);
        }

        public Task<bool> RemoveExpired(string optionalKey)
        {
            return Task.FromResult(true);
        }
    }
}
