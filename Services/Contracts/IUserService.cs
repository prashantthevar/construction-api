using ConstructionManagementSaaS.Models.Requests;
using ConstructionManagementService.Models;
using ConstructionManagementService.Models.Requests;

namespace ConstructionManagementSaaS.Services
{
    public interface IUserService
    {
        Task<User> CreateUserAsync(CreateUserRequest request);
        Task<User> AuthenticateAsync(LoginRequest request);
        Task<bool> UpdateUserAsync(string userId, UpdateUserRequest updateRequest);
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User> GetUserByIdAsync(string userId);
    }
}
