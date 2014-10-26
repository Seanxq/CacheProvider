using System.Collections.Specialized;
using System.Threading.Tasks;

namespace CacheProvider.Interface
{
    interface ICacheProvider
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
        /// <param name="optionalKey"></param>
        /// <returns>
        ///     An object instance with the Cache Value corresponding to the entry if found, else null
        /// </returns>
        Task<object> Get(string cacheKey, string optionalKey);

        /// <summary>
        ///     Gets the specified cache key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cacheKey">The cache key.</param>
        /// <param name="optionalKey"></param>
        /// <returns>An Instance of T if the entry is found, else null.</returns>
        Task<T> Get<T>(string cacheKey, string optionalKey);

        /// <summary>
        ///     Add to cache.
        /// </summary>
        /// <param name="cacheKey">The cache key.</param>
        /// <param name="cacheObject">The cache object.</param>
        /// <param name="optionalKey"></param>
        /// <returns>True if successful else false.</returns>
        Task<bool> Add(string cacheKey, object cacheObject, string optionalKey);

        /// <summary>
        ///     Remove from cache.
        /// </summary>
        /// <param name="cacheKey">The cache key.</param>
        /// <param name="optionalKey"></param>
        /// <returns>True if successful else false.</returns>
        Task<bool> Remove(string cacheKey, string optionalKey);

        /// <summary>
        ///     Remove everything from cache.
        /// </summary>
        Task<bool> RemoveAll();

        /// <summary>
        ///     Remove everything from cache.
        /// </summary>
        Task<bool> RemoveAll(string optionalKey);

        /// <summary>
        ///     Remove everything that has expired from cache.
        /// </summary>
        Task<bool> RemoveExpired(string optionalKey);
    }
}
