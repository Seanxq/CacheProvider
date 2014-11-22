namespace CacheProvider.Interface
{
    public interface ICacheOptions
    {
        bool AllowSliddingTime { get; set; }  
        int ExpirationInMinutes { get; set; }
        string[] SkipProvider { get; set; }
    }
}