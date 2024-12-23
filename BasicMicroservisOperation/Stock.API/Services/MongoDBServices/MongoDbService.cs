using MongoDB.Driver;

namespace Stock.API.Services.MongoDBServices
{
    public class MongoDbService
    {

        private readonly IMongoDatabase _mongoDatabase;
        public MongoDbService(IConfiguration configuration)
        {
            MongoClient client = new(configuration.GetConnectionString("MongoDB"));
            _mongoDatabase = client.GetDatabase("MicroservisStockAPIDB");
        }


        public IMongoCollection<T> GetCollection<T>() => _mongoDatabase.GetCollection<T>(typeof(T).Name.ToLowerInvariant());
    }
}
