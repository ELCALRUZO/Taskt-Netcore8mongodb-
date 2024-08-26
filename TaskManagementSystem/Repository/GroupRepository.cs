using Microsoft.Extensions.Caching.Distributed;
using MongoDB.Bson.IO;
using MongoDB.Driver;
using System.Text.RegularExpressions;
using TaskManagementSystem.Models;
using TaskManagementSystem.Repository.Interface;
using Newtonsoft.Json;

namespace TaskManagementSystem.Repository
{
    public class GroupRepository : IGroupRepository
    {
        private readonly IMongoCollection<ManagementGroup> _groups;
        private readonly IDistributedCache _cache;
        private readonly string _cacheKeyPrefix = "Group_";

        public GroupRepository(IMongoDatabase database, IDistributedCache cache)
        {
            _groups = database.GetCollection<ManagementGroup>("Groups");
            _cache = cache;
        }

        public async Task<List<ManagementGroup>> GetGroupsAsync()
        {
            // Implement caching logic here
            return await _groups.Find(group => true).ToListAsync();
        }

        public async Task<ManagementGroup> GetGroupByIdAsync(string id)
        {
            var cacheKey = $"{_cacheKeyPrefix}{id}";
            var cachedGroup = await _cache.GetStringAsync(cacheKey);

            if (cachedGroup != null)
            {
               return Newtonsoft.Json.JsonConvert.DeserializeObject<ManagementGroup>(cachedGroup);    
                //   JsonConvert.DeserializeObject<ManagementGroup>(cachedGroup);
            }

            var group = await _groups.Find(group => group.Id == id).FirstOrDefaultAsync();

            if (group != null)
            {
                var serializedGroup =  Newtonsoft.Json.JsonConvert.SerializeObject(group);
                await _cache.SetStringAsync(cacheKey, serializedGroup, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                });
            }

            return group;
        }

        public async Task CreateGroupAsync(ManagementGroup group)
        {
            await _groups.InsertOneAsync(group);
            await InvalidateCache(group.Id);
        }

        public async Task UpdateGroupAsync(string id, ManagementGroup group)
        {
            await _groups.ReplaceOneAsync(g => g.Id == id, group);
            await InvalidateCache(id);
        }

        public async Task DeleteGroupAsync(string id)
        {
            await _groups.DeleteOneAsync(g => g.Id == id);
            await InvalidateCache(id);
        }

        private async Task InvalidateCache(string id)
        {
            var cacheKey = $"{_cacheKeyPrefix}{id}";
            await _cache.RemoveAsync(cacheKey);
        }
    }




}
