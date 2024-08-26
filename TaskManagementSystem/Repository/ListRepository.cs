using Microsoft.Extensions.Caching.Distributed;
using MongoDB.Driver;
using TaskManagementSystem.Models;
using TaskManagementSystem.Repository.Interface;

namespace TaskManagementSystem.Repository
{
    public class ListRepository : IListRepository
    {
        private readonly IMongoCollection<ManagementList> _lists;
        private readonly IDistributedCache _cache;
        private readonly string _cacheKeyPrefix = "List_";

        public ListRepository(IMongoDatabase database, IDistributedCache cache)
        {
            _lists = database.GetCollection<ManagementList>("Lists");
            _cache = cache;
        }

        public async Task<List<ManagementList>> GetListsAsync()
        {
            return await _lists.Find(list => true).ToListAsync();
        }

        public async Task<ManagementList> GetListByIdAsync(string id)
        {
            var cacheKey = $"{_cacheKeyPrefix}{id}";
            var cachedList = await _cache.GetStringAsync(cacheKey);

            if (cachedList != null)
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject<ManagementList>(cachedList);

            }

            var list = await _lists.Find(list => list.Id == id).FirstOrDefaultAsync();

            if (list != null)
            {
                var serializedList = Newtonsoft.Json.JsonConvert.SerializeObject(list);         
                await _cache.SetStringAsync(cacheKey, serializedList, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                });
            }

            return list;
        }

        public async Task CreateListAsync(ManagementList list)
        {
            await _lists.InsertOneAsync(list);
            await InvalidateCache(list.Id);
        }

        public async Task UpdateListAsync(string id, ManagementList list)
        {
            await _lists.ReplaceOneAsync(l => l.Id == id, list);
            await InvalidateCache(id);
        }

        public async Task DeleteListAsync(string id)
        {
            await _lists.DeleteOneAsync(l => l.Id == id);
            await InvalidateCache(id);
        }

        private async Task InvalidateCache(string id)
        {
            var cacheKey = $"{_cacheKeyPrefix}{id}";
            await _cache.RemoveAsync(cacheKey);
        }
    }



}
