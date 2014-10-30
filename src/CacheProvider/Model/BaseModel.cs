using System;

namespace CacheProvider.Model
{
    public class BaseModel
    {
        public BaseModel()
        {
            AllowSliddingTime = false;
        }
        public string CacheKey { get; set; }
        public DateTime Expires { get; set; }
        public byte[] CacheObject { get; set; }
        public bool AllowSliddingTime { get; set; }
    }
}