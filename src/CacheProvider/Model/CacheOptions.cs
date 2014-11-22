using CacheProvider.Interface;

namespace CacheProvider.Model
{
    public class CacheOptions:ICacheOptions
    {
        public CacheOptions()
        {
            AllowSliddingTime = false;
            ExpirationInMinutes = 15;
            SkipProvider = new[] {string.Empty};
        }
        public bool AllowSliddingTime { get; set; }
        public int ExpirationInMinutes { get; set; }
        public string[] SkipProvider { get; set; }
    }
}