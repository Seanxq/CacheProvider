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

        public Task<object> Get(object cacheKey, string region)
        {
            return null;
        }

        public Task<T> Get<T>(object cacheKey, string region)
        {
            return null;
        }

        public Task<bool> Exist(object cacheKey, string region)
        {
            return Task.FromResult(false);
        }

        public Task<bool> Add(object cacheKey, object cacheObject, string region, int expirationInMinutes = 15)
        {
            return Task.FromResult(true);
        }

        public Task<bool> Add(object cacheKey, object cacheObject, string region, bool allowSliddingTime, int expirationInMinutes = 15)
        {
            return Task.FromResult(true);
        }

        public Task<bool> AddPermanent(object cacheKey, object cacheObject, string region)
        {
            return Task.FromResult(true);
        }

        public Task<bool> Remove(object cacheKey, string region)
        {
            return Task.FromResult(true);
        }

        public Task<bool> RemoveAll()
        {
            return Task.FromResult(true);
        }

        public Task<bool> RemoveAll(string region)
        {
            return Task.FromResult(true);
        }

        public Task<bool> RemoveExpired(string region)
        {
            return Task.FromResult(true);
        }

        public Task<long> Count(string region)
        {
            return Task.FromResult(default(long));
        }
    }
}
