using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MDCMS.Server.Models;

namespace MDCMS.Server.Data
{
    public interface IKioskRepository
    {
        Task<List<Kiosk>> GetAllAsync();
        Task<Kiosk?> GetByCodeAsync(string kioskCode);
        Task<Kiosk?> GetByIdAsync(string id);
        Task CreateAsync(Kiosk kiosk);
        Task UpdateAsync(Kiosk kiosk);
        Task DeleteAsync(string id);
    }

    public class KioskRepository : IKioskRepository
    {
        private readonly IMongoCollection<Kiosk> _kiosks;

        public KioskRepository(IOptions<CosmosMongoOptions> cfg, IMongoClient client)
        {
            var db = client.GetDatabase(cfg.Value.Database);
            _kiosks = db.GetCollection<Kiosk>("Kiosks");

            // Optional: ensure kioskCode is unique
            var idx = new CreateIndexModel<Kiosk>(
                Builders<Kiosk>.IndexKeys.Ascending(k => k.KioskCode),
                new CreateIndexOptions { Unique = true }
            );
            _kiosks.Indexes.CreateOne(idx);
        }

        public async Task<List<Kiosk>> GetAllAsync() =>
            await _kiosks.Find(_ => true).ToListAsync();

        public async Task<Kiosk?> GetByCodeAsync(string kioskCode) =>
            await _kiosks.Find(k => k.KioskCode == kioskCode).FirstOrDefaultAsync();

        public async Task<Kiosk?> GetByIdAsync(string id) =>
            await _kiosks.Find(k => k.Id == id).FirstOrDefaultAsync();

        public Task CreateAsync(Kiosk kiosk) =>
            _kiosks.InsertOneAsync(kiosk);

        public Task UpdateAsync(Kiosk kiosk) =>
            _kiosks.ReplaceOneAsync(k => k.Id == kiosk.Id, kiosk);

        public Task DeleteAsync(string id) =>
            _kiosks.DeleteOneAsync(k => k.Id == id);
    }
}
