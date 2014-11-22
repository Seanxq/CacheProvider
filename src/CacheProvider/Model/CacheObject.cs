using CacheProvider.Interface;

namespace CacheProvider.Model
{
    public class CacheObject: ICacheObject
    {
        public string Validator { get; set; }
        public byte[] Item { get; set; }
    }
}