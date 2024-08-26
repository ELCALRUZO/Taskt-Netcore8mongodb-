using System.Text.RegularExpressions;
using TaskManagementSystem.Models;

namespace TaskManagementSystem.Repository.Interface
{
    public interface IGroupRepository
    {
        Task<List<ManagementGroup>> GetGroupsAsync();
        Task<ManagementGroup> GetGroupByIdAsync(string id);
        Task CreateGroupAsync(ManagementGroup group);
        Task UpdateGroupAsync(string id, ManagementGroup group);
        Task DeleteGroupAsync(string id);


    }
}
