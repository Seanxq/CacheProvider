using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

namespace CacheProvider
{
    public static class MemoryStreamHelper
    {
        public static async Task<object> DeserializeObject(byte[] item)
        {
            return await Task.Factory.StartNew(() =>
            {

                var formatter = new BinaryFormatter();
                var ms = new MemoryStream(item);
                var cacheOject = formatter.Deserialize(ms);
                return cacheOject;
            });
        }

        public static async Task<byte[]> SerializeObject(object cacheObject)
        {
            return await Task.Factory.StartNew(() =>
            {
                var formatter = new BinaryFormatter();
                var ms = new MemoryStream();
                formatter.Serialize(ms, cacheObject);
                return ms.ToArray();
            });
        }
    }
}