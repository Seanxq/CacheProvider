using System;

namespace CacheProvider.Interface
{
    public interface ICacheOptions
    {
        bool AllowSliddingTime { get; set; }  
        int ExpirationInMinutes { get; set; }
        string Validator { get; set; }
    }
}