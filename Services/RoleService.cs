using ConstructionManagementService.Models;
using ConstructionManagementService.Services.Contracts;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ConstructionManagementService.Services
{
    
    public class RoleService : IRoleService
    {
        private readonly IMongoCollection<Role> _rolesCollection;
        private readonly IMongoCollection<Item> _itemsCollection;

        public RoleService(IMongoDatabase database)
        {
            _rolesCollection = database.GetCollection<Role>("roles");
            _itemsCollection = database.GetCollection<Item>("items");
        }

        public async Task<IEnumerable<Role>> GetAllRolesAsync()
        {
            var roles = await _rolesCollection.Find(_ => true).ToListAsync();
            return roles.Select(r => new Role
            {
                Id = r.Id.ToString(),
                RoleName = r.RoleName,
                AccessPermissions = r.AccessPermissions
            });
        }

        public async Task<Role> GetRoleByIdAsync(string id)
        {
            if (!ObjectId.TryParse(id, out var objectId))
                throw new ArgumentException("Invalid role ID format.");

            var role = await _rolesCollection.Find(r => r.Id == objectId.ToString()).FirstOrDefaultAsync();
            if (role == null) return null;

            return new Role
            {
                Id = role.Id.ToString(),
                RoleName = role.RoleName,
                AccessPermissions = role.AccessPermissions
            };
        }

        
    }

}
