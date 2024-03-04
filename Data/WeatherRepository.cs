using DOTNET_CRAWLER_AWS.Models;
using MongoDB.Driver;

namespace DOTNET_CRAWLER_AWS.Data
{
    public class WeatherRepository
    {
        private readonly IMongoCollection<WheaterModel> _weatherCollection;

        public WeatherRepository(MongoDbContext dbContext)
        {
            _weatherCollection = dbContext.Database.GetCollection<WheaterModel>("Weather");
        }

        public async Task<List<WheaterModel>> GetWheater()
        {
            return await _weatherCollection.Find(_ => true).ToListAsync();
        }

        public async Task Insert(WheaterModel wheater)
        {
            await _weatherCollection.InsertOneAsync(wheater);
        }
    }
}
