using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MDCMS.Server.Models;
namespace MDCMS.Server.Data
{
    public interface IUserRepository
    {
        Task<User?> GetByUsernameAsync(string username);
        Task<List<User>> GetAllAsync(); // <-- new method
        Task CreateAsync(User user);
        Task UpdateAsync(User user);
    }

    public class UserRepository : IUserRepository
    {
        private readonly IMongoCollection<User> _users;

        public UserRepository(IOptions<CosmosMongoOptions> cfg, IMongoClient client)
        {
            var db = client.GetDatabase(cfg.Value.Database);
            _users = db.GetCollection<User>("Users");

            // optional: ensure index on username & email
            var idx1 = new CreateIndexModel<User>(Builders<User>.IndexKeys.Ascending(u => u.Username),
                new CreateIndexOptions { Unique = true });
            var idx2 = new CreateIndexModel<User>(Builders<User>.IndexKeys.Ascending(u => u.Email),
                new CreateIndexOptions { Unique = true });
            _users.Indexes.CreateMany(new[] { idx1, idx2 });
        }

        public Task<User?> GetByUsernameAsync(string username) =>
            _users.Find(u => u.Username == username).FirstOrDefaultAsync();

        public Task<List<User>> GetAllAsync() =>
            _users.Find(_ => true).ToListAsync(); // <-- return all users

        public Task CreateAsync(User user) => _users.InsertOneAsync(user);

        public Task UpdateAsync(User user) =>
            _users.ReplaceOneAsync(u => u.Id == user.Id, user);
    }
}
