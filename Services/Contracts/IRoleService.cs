using ConstructionManagementService.Models;

namespace ConstructionManagementService.Services.Contracts
{
    public interface IRoleService
    {
        Task<IEnumerable<Role>> GetAllRolesAsync();
        Task<Role> GetRoleByIdAsync(string id);
    }
}
