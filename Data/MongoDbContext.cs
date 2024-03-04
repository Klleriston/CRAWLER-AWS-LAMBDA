]using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace DOTNET_CRAWLER_AWS.Data
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _db;

        public MongoDbContext(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("MongoDb");
            var databaseName = configuration.GetValue<string>("DatabaseSettings:DatabaseName");
            var client = new MongoClient(connectionString);
            _db = client.GetDatabase(databaseName);
        }
        public IMongoDatabase Database => _db;
        
    }
}
