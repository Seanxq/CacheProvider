using System;
using CacheProvider.Interface;

namespace CacheProvider.Model
{
    public class CacheModel : ICacheModel
    {
        public string CacheKey { get; set; }
        public DateTime Expires { get; set; }
        public ICacheObject CacheObject { get; set; }
        public ICacheOptions CacheOptions { get; set; }
    }
}