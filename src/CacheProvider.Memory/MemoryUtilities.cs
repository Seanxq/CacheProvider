namespace CacheProvider.Memory
{
    public static class MemoryUtilities
    {
        private const string KeySeparator = "_";
        private const string DefaultRegion = "region";
        /// <summary>
        /// Parse domain from combinedKey.
        /// This method is exposed publicly because it can be useful in callback methods.
        /// The key property of the callback argument will in our case be the combinedKey.
        /// To be interpreted, it needs to be split into domain and key with these parse methods.
        /// </summary>
        public static string ParseDomain(string combinedKey)
        {
            return combinedKey.Substring(0, combinedKey.IndexOf(KeySeparator, System.StringComparison.Ordinal));
        }

        /// <summary>
        /// Parse key from combinedKey.
        /// This method is exposed publicly because it can be useful in callback methods.
        /// The key property of the callback argument will in our case be the combinedKey.
        /// To be interpreted, it needs to be split into domain and key with these parse methods.
        /// </summary>
        public static string ParseKey(string combinedKey)
        {
            return combinedKey.Substring(combinedKey.IndexOf(KeySeparator, System.StringComparison.Ordinal) + KeySeparator.Length);
        }

        /// <summary>
        /// Create a combined key from given values.
        /// The combined key is used when storing and retrieving from the inner MemoryCache instance.
        /// </summary>
        /// <param name="key">Key within specified domain</param>
        /// <param name="region">region to place the cache into</param>
        private static string CombinedKey(object key, string region)
        {
            return string.Format("{0}{1}{2}", string.IsNullOrEmpty(region) ? DefaultRegion : region, KeySeparator, key);
        }
    }
}