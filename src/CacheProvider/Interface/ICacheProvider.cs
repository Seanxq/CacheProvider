﻿using System.Collections.Specialized;
using System.Threading.Tasks;

namespace CacheProvider.Interface
{
    public interface ICacheProvider
    {
        /// <summary>
        ///     Initialize from config
        /// </summary>
        /// <param name="name"></param>
        /// <param name="config">Config properties</param>
        void Initialize(string name, NameValueCollection config);

        /// <summary>
        ///     Get from cache.
        /// </summary>
        /// <param name="cacheKey">The cache key.</param>
        /// <param name="region"></param>
        /// <returns>
        ///     An object instance with the Cache Value corresponding to the entry if found, else null
        /// </returns>
        Task<object> Get(object cacheKey, string region);

        /// <summary>
        ///     Gets the specified cache key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cacheKey">The cache key.</param>
        /// <param name="region"></param>
        /// <returns>An Instance of T if the entry is found, else null.</returns>
        Task<T> Get<T>(object cacheKey, string region);

        /// <summary>
        /// Check if the item exist
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <param name="region"></param>
        /// <returns>true false</returns>
        Task<bool> Exist(object cacheKey, string region);

        /// <summary>
        ///     Add to cache.
        /// </summary>
        /// <param name="cacheKey">The cache key.</param>
        /// <param name="cacheObject">The cache object.</param>
        /// <param name="region"></param>
        /// <param name="expirationInMinutes"></param>
        /// <returns>True if successful else false.</returns>
        Task<bool> Add(object cacheKey, object cacheObject, string region, int expirationInMinutes = 15);

        /// <summary>
        ///     Add to cache.
        /// </summary>
        /// <param name="cacheKey">The cache key.</param>
        /// <param name="cacheObject">The cache object.</param>
        /// <param name="region"></param>
        /// <param name="allowSliddingTime">Updates the expiration x minutes from last write or reed</param>
        /// <param name="expirationInMinutes"></param>
        /// <returns>True if successful else false.</returns>
        Task<bool> Add(object cacheKey, object cacheObject, string region, bool allowSliddingTime, int expirationInMinutes = 15);

        /// <summary>
        ///     Add an item to the cache and will need to be removed manually
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <param name="cacheObject"></param>
        /// <param name="region"></param>
        /// <returns>true or false</returns>
        Task<bool> AddPermanent(object cacheKey, object cacheObject, string region);

        /// <summary>
        ///     Remove from cache.
        /// </summary>
        /// <param name="cacheKey">The cache key.</param>
        /// <param name="region"></param>
        /// <returns>True if successful else false.</returns>
        Task<bool> Remove(object cacheKey, string region);

        /// <summary>
        ///     Remove everything from cache.
        /// </summary>
        Task<bool> RemoveAll();

        /// <summary>
        ///     Remove everything from cache.
        /// </summary>
        Task<bool> RemoveAll(string region);

        /// <summary>
        ///     Remove everything that has expired from cache.
        /// </summary>
        Task<bool> RemoveExpired(string region);

        /// <summary>
        ///     Gets the cache count by region
        /// </summary>
        Task<long> Count(string region);
    }
}