using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MDCMS.Server.Models;

namespace MDCMS.Server.Data
{
    public interface IVideoRepository
    {
        Task<List<Video>> GetAllAsync();
        Task<List<Video>> SearchAsync(string keyword);
        Task<Video?> GetByIdAsync(string id);
        Task CreateAsync(Video video);
        Task UpdateAsync(Video video);
        Task DeleteAsync(string id);
    }

    public class VideoRepository : IVideoRepository
    {
        private readonly IMongoCollection<Video> _videos;

        public VideoRepository(IOptions<CosmosMongoOptions> cfg, IMongoClient client)
        {
            var db = client.GetDatabase(cfg.Value.Database);
            _videos = db.GetCollection<Video>("Videos");

            // Optional: unique index on title
            var idx = new CreateIndexModel<Video>(
                Builders<Video>.IndexKeys.Ascending(v => v.Title),
                new CreateIndexOptions { Unique = true }
            );
            _videos.Indexes.CreateOne(idx);
        }

        public async Task<List<Video>> GetAllAsync() =>
            await _videos.Find(_ => true).ToListAsync();

        public async Task<List<Video>> SearchAsync(string keyword) =>
            await _videos.Find(v => v.Title.ToLower().Contains(keyword.ToLower())).ToListAsync();

        public async Task<Video?> GetByIdAsync(string id) =>
            await _videos.Find(v => v.Id == id).FirstOrDefaultAsync();

        public Task CreateAsync(Video video) =>
            _videos.InsertOneAsync(video);

        public Task UpdateAsync(Video video) =>
            _videos.ReplaceOneAsync(v => v.Id == video.Id, video);

        public Task DeleteAsync(string id) =>
            _videos.DeleteOneAsync(v => v.Id == id);
    }
}
