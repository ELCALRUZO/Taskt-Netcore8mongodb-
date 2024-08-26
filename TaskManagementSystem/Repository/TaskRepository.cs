using Microsoft.Extensions.Caching.Distributed;
using MongoDB.Driver;
using TaskManagementSystem.Models;
using TaskManagementSystem.Repository.Interface;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace TaskManagementSystem.Repository
{
    public class TaskRepository : ITaskRepository
    {
        private readonly IMongoCollection<ManagementTask> _tasks;
        private readonly IDistributedCache _cache;
        private readonly string _cacheKeyPrefix = "Task_";

        public TaskRepository(IMongoDatabase database, IDistributedCache cache)
        {
            _tasks = database.GetCollection<ManagementTask>("ManagementTask");
            _cache = cache;
        }

        public async Task<List<ManagementTask>> GetTasksAsync()
        {
            return await _tasks.Find(task => true).ToListAsync();
        }

        public async Task<ManagementTask> GetTaskByIdAsync(string id)
        {
            var cacheKey = $"{_cacheKeyPrefix}{id}";
            var cachedTask = await _cache.GetStringAsync(cacheKey);

            if (cachedTask != null)
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject<ManagementTask>(cachedTask);

                // JsonConvert.DeserializeObject<ManagementTask>(cachedTask);
            }

            var task = await _tasks.Find(task => task.Id.ToString() == id).FirstOrDefaultAsync();

            if (task != null)
            {
                var serializedTask = Newtonsoft.Json.JsonConvert.SerializeObject(task);
                //JsonConvert.SerializeObject(task);

                await _cache.SetStringAsync(cacheKey, serializedTask, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                });
            }

            return task;
        }

        public async Task CreateTaskAsync(ManagementTask task)
        {
            await _tasks.InsertOneAsync(task);
            await InvalidateCache(task.Id.ToString());
        }

        public async Task UpdateTaskAsync(string id, ManagementTask task)
        {
            await _tasks.ReplaceOneAsync(t => t.Id.ToString() == id, task);
            await InvalidateCache(id);
        }

        public async Task DeleteTaskAsync(string id)
        {
            await _tasks.DeleteOneAsync(t => t.Id.ToString() == id);
            await InvalidateCache(id);
        }

        private async Task InvalidateCache(string id)
        {
            var cacheKey = $"{_cacheKeyPrefix}{id}";
            await _cache.RemoveAsync(cacheKey);
        }
    }



}
