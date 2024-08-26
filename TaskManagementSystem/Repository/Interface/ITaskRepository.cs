using TaskManagementSystem.Models;

namespace TaskManagementSystem.Repository.Interface
{
    public interface ITaskRepository
    {
        Task<List<ManagementTask>> GetTasksAsync(); 
        Task<ManagementTask> GetTaskByIdAsync(string id);

        Task CreateTaskAsync(ManagementTask task); 
        Task UpdateTaskAsync(string id, ManagementTask task);

        Task DeleteTaskAsync(string id);


    }
}
