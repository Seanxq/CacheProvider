using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Threading.Tasks;
using CacheProvider.Interface;

namespace CacheProvider.Multi
{
    public class MultiCacheProvider: CacheProvider
    {
        private bool _isEnabled;
        private List<ICacheProvider> _cacheProviders; 

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

            var providers = config["providers"];
            if (providers == null)
            {
                throw new ConfigurationErrorsException("Missing providers list.. example memory,mongo");
            }

            foreach (var c in providers.Split(','))
            {
                var item = (ICacheProvider) System.Reflection.Assembly.GetExecutingAssembly().CreateInstance(c);

                _cacheProviders.Add(item);
            }


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
