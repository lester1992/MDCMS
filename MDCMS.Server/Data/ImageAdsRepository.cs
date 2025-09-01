using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MDCMS.Server.Models;
namespace MDCMS.Server.Data
{
    public interface IImageAdsRepository
    {
        Task<List<ImageAds>> GetAllAsync();
        Task<List<ImageAds>> SearchAsync(string keyword);
        Task<ImageAds?> GetByIdAsync(string id);
        Task CreateAsync(ImageAds ad);
        Task UpdateAsync(ImageAds ad);
        Task DeleteAsync(string id);
    }

    public class ImageAdsRepository : IImageAdsRepository
    {
        private readonly IMongoCollection<ImageAds> _ads;

        public ImageAdsRepository(IOptions<CosmosMongoOptions> cfg, IMongoClient client)
        {
            var db = client.GetDatabase(cfg.Value.Database);
            _ads = db.GetCollection<ImageAds>("ImageAds");

            // Optional: unique title index
            var idx = new CreateIndexModel<ImageAds>(
                Builders<ImageAds>.IndexKeys.Ascending(a => a.Title),
                new CreateIndexOptions { Unique = true }
            );
            _ads.Indexes.CreateOne(idx);
        }

        public async Task<List<ImageAds>> GetAllAsync() =>
            await _ads.Find(_ => true).SortBy(a => a.Sequence).ToListAsync();

        public async Task<List<ImageAds>> SearchAsync(string keyword) =>
            await _ads.Find(a => a.Title.ToLower().Contains(keyword.ToLower())).SortBy(a => a.Sequence).ToListAsync();

        public async Task<ImageAds?> GetByIdAsync(string id) =>
            await _ads.Find(a => a.Id == id).FirstOrDefaultAsync();

        public Task CreateAsync(ImageAds ad) =>
            _ads.InsertOneAsync(ad);

        public Task UpdateAsync(ImageAds ad) =>
            _ads.ReplaceOneAsync(a => a.Id == ad.Id, ad);

        public Task DeleteAsync(string id) =>
            _ads.DeleteOneAsync(a => a.Id == id);
    }
}
