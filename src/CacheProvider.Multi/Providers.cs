using System.Collections.Specialized;
using CacheProvider.Interface;

namespace CacheProvider.Multi
{
    public class Providers
    {
        public ICacheProvider CacheProviders { get; set; }
        public NameValueCollection ValueCollection { get; set; }

    }
}