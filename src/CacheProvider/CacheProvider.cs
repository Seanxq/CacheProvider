using System;
using System.Configuration.Provider;
using System.Threading.Tasks;
using CacheProvider.Interface;

namespace CacheProvider
{
    public abstract class CacheProvider: ProviderBase, ICacheProvider
    {
        /// <summary>
        ///     Get from cache.
        /// </summary>
        /// <param name="cacheKey">The cache key.</param>
        /// <param name="region"></param>
        /// <returns>
        ///     An object instance with the Cache Value corresponding to the entry if found, else null
        /// </returns>
        public abstract Task<object> Get(object cacheKey, string region);

        /// <summary>
        ///     Gets the specified cache key (Async).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cacheKey">The cache key.</param>
        /// <param name="region"></param>
        /// <returns>An Instance of T if the entry is found, else null.</returns>
        public abstract Task<T> Get<T>(object cacheKey, string region);

        /// <summary>
        /// Check if the item exist
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <param name="region"></param>
        /// <returns>true false</returns>
        public abstract Task<bool> Exist(object cacheKey, string region);

        /// <summary>
        ///     Add to cache.
        /// </summary>
        /// <param name="cacheKey">The cache key.</param>
        /// <param name="cacheObject">The cache object.</param>
        /// <param name="region"></param>
        /// <param name="expirationInMinutes"></param>
        /// <returns>True if successful else false.</returns>
        public abstract Task<bool> Add(object cacheKey, object cacheObject, string region, int expirationInMinutes = 15);

        /// <summary>
        ///     Add to cache.
        /// </summary>
        /// <param name="cacheKey">The cache key.</param>
        /// <param name="cacheObject">The cache object.</param>
        /// <param name="region"></param>
        /// <param name="allowSliddingTime">Updates the expiration x minutes from last write or reed</param>
        /// <param name="expirationInMinutes"></param>
        /// <returns>True if successful else false.</returns>
        public abstract Task<bool> Add(object cacheKey, object cacheObject, string region, bool allowSliddingTime, int expirationInMinutes = 15);

        /// <summary>
        ///     Add an item to the cache and will need to be removed manually
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <param name="cacheObject"></param>
        /// <param name="region"></param>
        /// <returns>true or false</returns>
        public abstract Task<bool> AddPermanent(object cacheKey, object cacheObject, string region);

        /// <summary>
        ///     Remove from cache.
        /// </summary>
        /// <param name="cacheKey">The cache key.</param>
        /// <param name="region"></param>
        /// <returns>True if successful else false.</returns>
        public abstract Task<bool> Remove(object cacheKey, string region);

        /// <summary>
        ///     Remove everything from cache.
        /// </summary>
        /// <returns>true or false</returns>
        public abstract Task<bool> RemoveAll();

        /// <summary>
        ///     Remove everything from cache.
        /// </summary>
        /// <returns>true or false</returns>
        public abstract Task<bool> RemoveAll(string region);

        /// <summary>
        ///     Remove everything that has expired from cache.
        /// </summary>
        /// <returns>true or false</returns>
        public abstract Task<bool> RemoveExpired(string region);

        /// <summary>
        ///     Gets the cache count by region
        /// </summary>
        /// <returns>count</returns>
        public abstract Task<long> Count(string region);
    }
}
