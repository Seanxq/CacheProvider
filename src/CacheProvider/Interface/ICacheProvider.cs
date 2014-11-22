using System;
using System.Collections.Specialized;
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

        #region Get/Exist/Count
        /// <summary>
        ///     Get from cache.
        /// </summary>
        /// <param name="cacheKey">The cache key.</param>
        /// <param name="region">If region is supported by cache , it will seperate the lookups</param>
        /// <returns>
        ///     An object instance with the Cache Value corresponding to the entry if found, else null
        /// </returns>
        Task<object> Get(object cacheKey, string region);

        /// <summary>
        ///     Get from cache.
        /// </summary>
        /// <param name="cacheKey">The cache key.</param>
        /// <param name="region">If region is supported by cache , it will seperate the lookups</param>
        /// <param name="validationKey">A validation key can used to verify if the object is correct.  Used in Multi cache to help keep them in sync</param>
        /// <returns>
        ///     An object instance with the Cache Value corresponding to the entry if found, else null
        /// </returns>
        Task<object> Get(object cacheKey, string region, string validationKey);

        /// <summary>
        ///     Gets the specified cache key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cacheKey">The cache key.</param>
        /// <param name="region">If region is supported by cache , it will seperate the lookups</param>
        /// <returns>An Instance of T if the entry is found, else null.</returns>
        Task<T> Get<T>(object cacheKey, string region);
        /// <summary>
        ///     Get from cache.
        /// </summary>
        /// <param name="cacheKey">The cache key.</param>
        /// <param name="region">If region is supported by cache , it will seperate the lookups</param>
        /// <param name="validationKey">A validation key can used to verify if the object is correct.  Used in Multi cache to help keep them in sync</param>
        /// <returns>
        ///     An object instance with the Cache Value corresponding to the entry if found, else null
        /// </returns>
        Task<T> Get<T>(object cacheKey, string region, string validationKey);

        /// <summary>
        /// Check if the item exist
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <param name="region"></param>
        /// <returns>true false</returns>
        Task<bool> Exist(object cacheKey, string region);

        /// <summary>
        ///     Gets the cache count by region
        /// </summary>
        Task<long> Count(string region);
        #endregion

        #region Add

        /// <summary>
        ///     Add to cache.
        /// </summary>
        /// <param name="cacheKey">The cache key.</param>
        /// <param name="cacheObject">The cache object.</param>
        /// <param name="region">If region is supported by cache , it will seperate the lookups</param>
        /// <param name="options">Options that can be set for the cache</param>
        /// <returns>True if successful else false.</returns>
        Task<bool> Add(object cacheKey, object cacheObject, string region, ICacheOptions options);

        /// <summary>
        ///     Add an item to the cache and will need to be removed manually
        /// </summary>
        /// <param name="cacheKey">The cache key.</param>
        /// <param name="cacheObject">The cache object.</param>
        /// <param name="region">If region is supported by cache , it will seperate the lookups</param>
        /// <param name="options">Options that can be set for the cache</param>
        /// <returns>true or false</returns>
        Task<bool> AddPermanent(object cacheKey, object cacheObject, string region, ICacheOptions options);
        #endregion

        // todo remove after Dec
        #region Add obsolete
        /// <summary>
        ///     Add to cache.
        /// </summary>
        /// <param name="cacheKey">The cache key.</param>
        /// <param name="cacheObject">The cache object.</param>
        /// <param name="region"></param>
        /// <param name="expirationInMinutes"></param>
        /// <returns>True if successful else false.</returns>
        [Obsolete("will be removed after December, use the other Add with options")]
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
        [Obsolete("will be removed after December, use the other Add with options")]
        Task<bool> Add(object cacheKey, object cacheObject, string region, bool allowSliddingTime, int expirationInMinutes = 15);

        /// <summary>
        ///     Add an item to the cache and will need to be removed manually
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <param name="cacheObject"></param>
        /// <param name="region"></param>
        /// <returns>true or false</returns>
        [Obsolete("will be removed after December, use the other AddPermanent with options")]
        Task<bool> AddPermanent(object cacheKey, object cacheObject, string region);

        #endregion

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

        
    }
}