using System;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace CacheProvider.Mongo
{
    public static class MongoUtilities
    {
        public static string GetMongoDatabaseString(string host, string port, string baseDbName)
        {
            return string.IsNullOrWhiteSpace(port) ?
                string.Format("mongodb://{0}/{2}", host, baseDbName) :
                string.Format("mongodb://{0}:{1}/{2}", host, port, baseDbName);
        }

        public static MongoCollection InitializeMongoDatabase(string region, string mongoConnectionString)
        {
            var mongoDatabase = GetDatabaseFromUrl(new MongoUrl(mongoConnectionString));
            var mongoCollection = mongoDatabase.GetCollection(region);
            mongoCollection.CreateIndex("CacheKey");
            return mongoCollection;
        }

        /// <summary>
        ///     Gets the database from URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="database"></param>
        /// <returns>MongoDatabase.</returns>
        public static MongoDatabase GetDatabaseFromUrl(MongoUrl url)
        {
            var client = new MongoClient(url);
            MongoServer server = client.GetServer();
            if (url.DatabaseName == null)
            {
                throw new Exception("No database name specified in connection string");
            }
            return server.GetDatabase(url.DatabaseName); // WriteConcern defaulted to Acknowledged
        }

        /// <summary>
        ///     Gets the database from connection string.
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <param name="baseDbName"></param>
        /// <returns>MongoDatabase.</returns>
        /// <exception cref="System.Exception">No database name specified in connection string</exception>
        public static MongoDatabase GetDatabaseFromSqlStyle(string host, string port, string baseDbName)
        {
            MongoUrl databaseString;
            if (string.IsNullOrWhiteSpace(port))
            {
                databaseString = new MongoUrl(string.Format("mongodb://{0}/{2}", host, baseDbName));
            }
            else
            {
                databaseString = new MongoUrl(string.Format("mongodb://{0}:{1}/{2}", host, port, baseDbName));
            }

            return GetDatabaseFromUrl(databaseString);
        }
        
        public static async Task<bool> VerifyReturnMessage(GetLastErrorResult writeConcernResult)
        {
            return await Task.Factory.StartNew(() =>
            {
                if (writeConcernResult == null)
                {
                    return false;
                }

                if (string.IsNullOrEmpty(writeConcernResult.LastErrorMessage))
                {
                    return true;
                }
                return false;
            });
        }
    }
}