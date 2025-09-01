using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MDCMS.Server.Models;

namespace MDCMS.Server.Data
{
    public interface IStoreRepository
    {
        Task<List<Store>> GetAllAsync();
        Task<Store?> GetByIdAsync(string id);
        Task<Store?> GetByStoreIdAsync(int storeId);
        Task<List<Store>> SearchAsync(string keyword);
        Task CreateAsync(Store store);
        Task UpdateAsync(Store store);
        Task DeleteAsync(string id);
    }

    public class StoreRepository : IStoreRepository
    {
        private readonly IMongoCollection<Store> _stores;

        public StoreRepository(IOptions<CosmosMongoOptions> cfg, IMongoClient client)
        {
            var db = client.GetDatabase(cfg.Value.Database);
            _stores = db.GetCollection<Store>("Stores");

            // Indexes
            var idx1 = new CreateIndexModel<Store>(
                Builders<Store>.IndexKeys.Ascending(s => s.StoreId),
                new CreateIndexOptions { Unique = true });

            _stores.Indexes.CreateOne(idx1);
        }

        public Task<List<Store>> GetAllAsync() =>
            _stores.Find(_ => true).ToListAsync();

        public Task<Store?> GetByIdAsync(string id) =>
            _stores.Find(s => s.Id == id).FirstOrDefaultAsync();

        public Task<Store?> GetByStoreIdAsync(int storeId) =>
            _stores.Find(s => s.StoreId == storeId).FirstOrDefaultAsync();

        public Task<List<Store>> SearchAsync(string keyword) =>
            _stores.Find(s =>
                s.StoreName.ToLower().Contains(keyword.ToLower()) ||
                s.StoreType.ToLower().Contains(keyword.ToLower()) ||
                s.StoreTags.Any(t => t.ToLower().Contains(keyword.ToLower()))
            ).ToListAsync();

        public Task CreateAsync(Store store) => _stores.InsertOneAsync(store);

        public Task UpdateAsync(Store store) =>
            _stores.ReplaceOneAsync(s => s.Id == store.Id, store);

        public Task DeleteAsync(string id) =>
            _stores.DeleteOneAsync(s => s.Id == id);
    }
}
