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
        /// <param name="optionalKey"></param>
        /// <returns>
        ///     An object instance with the Cache Value corresponding to the entry if found, else null
        /// </returns>
        public abstract Task<object> Get(string cacheKey, string optionalKey);

        /// <summary>
        ///     Gets the specified cache key (Async).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cacheKey">The cache key.</param>
        /// <param name="optionalKey"></param>
        /// <returns>An Instance of T if the entry is found, else null.</returns>
        public abstract Task<T> Get<T>(string cacheKey, string optionalKey);

        /// <summary>
        ///     Add to cache.
        /// </summary>
        /// <param name="cacheKey">The cache key.</param>
        /// <param name="cacheObject">The cache object.</param>
        /// <param name="optionalKey"></param>
        /// <param name="expirationInMinutes"></param>
        /// <returns>True if successful else false.</returns>
        public abstract Task<bool> Add(string cacheKey, object cacheObject, string optionalKey, int expirationInMinutes = 15);

        /// <summary>
        ///     Remove from cache.
        /// </summary>
        /// <param name="cacheKey">The cache key.</param>
        /// <param name="optionalKey"></param>
        /// <returns>True if successful else false.</returns>
        public abstract Task<bool> Remove(string cacheKey, string optionalKey);

        /// <summary>
        ///     Remove everything from cache.
        /// </summary>
        public abstract Task<bool> RemoveAll();

        /// <summary>
        ///     Remove everything from cache.
        /// </summary>
        public abstract Task<bool> RemoveAll(string optionalKey);

        /// <summary>
        ///     Remove everything that has expired from cache.
        /// </summary>
        public abstract Task<bool> RemoveExpired(string optionalKey);
    }
}
