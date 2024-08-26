using TaskManagementSystem.Models;

namespace TaskManagementSystem.Repository.Interface
{
    public interface IListRepository
    {
        Task<List<ManagementList>> GetListsAsync();
        Task<ManagementList> GetListByIdAsync(string id);
        Task CreateListAsync(ManagementList list);
        Task UpdateListAsync(string id, ManagementList list);
        Task DeleteListAsync(string id);
    }
}
