using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using CacheProvider.Interface;
using CacheProvider.Memory;
using CacheProvider.Mongo;

namespace CacheProvider.Multi
{
    public class MultiCacheProvider : CacheProvider
    {
        private bool _isEnabled;
        public readonly List<Providers> CacheProviders;

        public MultiCacheProvider()
        {
            CacheProviders = new List<Providers>();
        }

        /// <summary>
        ///     Initialize from config
        /// </summary>
        /// <param name="name"></param>
        /// <param name="config">Config properties</param>
        public override void Initialize(string name, NameValueCollection config)
        {
            base.Initialize(name, config);

            _isEnabled = true;
            var enabled = config["enable"];
            if (enabled == null)
            {
                _isEnabled = true;
            }
            else
            {
                bool.TryParse(config["enable"], out _isEnabled);
            }

            var timeout = config["timeout"];
            if (string.IsNullOrEmpty(timeout))
            {
                timeout = "20";
            }

            var cacheExpirationTime = 60;

            if (!int.TryParse(timeout, out cacheExpirationTime))
            {
                throw new ConfigurationErrorsException("invalid timeout value");
            }

            var providers = config["providers"];
            if (providers == null)
            {
                throw new ConfigurationErrorsException("Missing providers list.. example memory,mongo");
            }

            var providerOptions = providers.Split(',');
            foreach (var provider in providerOptions.Select(c => c.Trim()).Where(provider => !string.IsNullOrWhiteSpace(provider)))
            {
                switch (provider.ToLower())
                {
                    case "memorycacheprovider":
                        var memoryProvider = new Providers
                        {
                            CacheProviders = new MemoryCacheProvider(),
                            ValueCollection = config
                        };

                        memoryProvider.ValueCollection["timeout"] = (cacheExpirationTime / providerOptions.Count()).ToString();

                        CacheProviders.Add(memoryProvider);
                        break;

                    case "mongocacheprovider":
                        var mongoProvider = new Providers
                        {
                            CacheProviders = new MemoryCacheProvider(),
                            ValueCollection = config
                        };

                        var mongoTimeSlice = providerOptions.Count();
                        if (mongoTimeSlice < 1)
                        {
                            mongoTimeSlice = 1;
                        }

                        mongoProvider.ValueCollection["timeout"] = (cacheExpirationTime / mongoTimeSlice).ToString();

                        CacheProviders.Add(mongoProvider);
                        break;
                }
            }

            //if (c.Equals("MultiCacheProvider", StringComparison.CurrentCultureIgnoreCase)) continue;
            //var test = System.Reflection.Assembly.GetExecutingAssembly().CreateInstance(c);
            //ICacheProvider item = System.Reflection.Assembly.GetExecutingAssembly().CreateInstance(c);

            //_cacheProviders.Add(item);
        }


        /*
         public interface IPreparer
    {
        void Run(Dictionary<string, object> data);
    }

    public interface IPreparerFactory : IFactory<IPreparer>
    {
        IPreparer GetInstance(string key);
    }

    public class UnityPreparerFactory : UnityFactory<IPreparer>, IPreparerFactory
    {
        public UnityPreparerFactory() : base(ServiceLayerConstants.DEFAULT_CONTAINER_NAME) { }

        IPreparer IPreparerFactory.GetInstance(string key)
        {
            return base.GetInstance(key.ToLower());
        }
    }
          
         */


        public override Task<object> Get(object cacheKey, string region)
        {
            throw new NotImplementedException();
        }

        public override Task<T> Get<T>(object cacheKey, string region)
        {
            throw new NotImplementedException();
        }

        public override Task<bool> Exist(object cacheKey, string region)
        {
            throw new NotImplementedException();
        }

        public override Task<bool> Add(object cacheKey, object cacheObject, string region, int expirationInMinutes = 15)
        {
            throw new NotImplementedException();
        }

        public override Task<bool> Add(object cacheKey, object cacheObject, string region, bool allowSliddingTime, int expirationInMinutes = 15)
        {
            throw new NotImplementedException();
        }

        public override Task<bool> AddPermanent(object cacheKey, object cacheObject, string region)
        {
            throw new NotImplementedException();
        }

        public override Task<bool> Remove(object cacheKey, string region)
        {
            throw new NotImplementedException();
        }

        public override Task<bool> RemoveAll()
        {
            throw new NotImplementedException();
        }

        public override Task<bool> RemoveAll(string region)
        {
            throw new NotImplementedException();
        }

        public override Task<bool> RemoveExpired(string region)
        {
            throw new NotImplementedException();
        }

        public override Task<long> Count(string region)
        {
            throw new NotImplementedException();
        }
    }
}
