using ConstructionManagementSaaS.Models.Requests;
using ConstructionManagementService.Models;
using ConstructionManagementService.Models.Requests;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ConstructionManagementSaaS.Services
{
    public class UserService : IUserService
    {
        private readonly IMongoCollection<User> _userCollection;
        private readonly IMongoCollection<Role> _roleCollection;

        public UserService(IMongoClient mongoClient, IMongoDBSettings settings)
        {
            var database = mongoClient.GetDatabase("test");
            _userCollection = database.GetCollection<User>("users");
            _roleCollection = database.GetCollection<Role>("roles");
        }

        public async Task<User> CreateUserAsync(CreateUserRequest request)
        {
            var role = await _roleCollection.Find(r => r.RoleName == request.RoleName).FirstOrDefaultAsync();

            if (role == null)
            {
                throw new Exception($"Role '{request.RoleName}' not found.");
            }

            var user = new User
            {
                RoleId = role.Id,
                FirstName = request.FirstName,
                LastName = request.LastName,
                UserName = request.UserName,
                PhoneNumber = request.PhoneNumber,
                Email = request.Email,
                Password = request.Password,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _userCollection.InsertOneAsync(user);

            return new User
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.UserName,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email,
                RoleId = role.Id,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };
        }

        public async Task<User> AuthenticateAsync(LoginRequest request)
        {
            // Attempt to find the user by email
            var user = await _userCollection.Find(u => u.UserName == request.UserName).FirstOrDefaultAsync();

            if (user == null || user.Password != request.Password)
            {
                return null; // Authentication failed
            }

            return user; // Authentication successful
        }

        public async Task<bool> UpdateUserAsync(string userId, UpdateUserRequest updateRequest)
        {
            var updateDefinition = new List<UpdateDefinition<User>>();

            if (!string.IsNullOrEmpty(updateRequest.FirstName))
                updateDefinition.Add(Builders<User>.Update.Set(u => u.FirstName, updateRequest.FirstName));

            if (!string.IsNullOrEmpty(updateRequest.LastName))
                updateDefinition.Add(Builders<User>.Update.Set(u => u.LastName, updateRequest.LastName));

            if (!string.IsNullOrEmpty(updateRequest.UserName))
                updateDefinition.Add(Builders<User>.Update.Set(u => u.UserName, updateRequest.UserName));

            if (!string.IsNullOrEmpty(updateRequest.PhoneNumber))
                updateDefinition.Add(Builders<User>.Update.Set(u => u.PhoneNumber, updateRequest.PhoneNumber));

            if (!string.IsNullOrEmpty(updateRequest.Email))
                updateDefinition.Add(Builders<User>.Update.Set(u => u.Email, updateRequest.Email));

            if (!string.IsNullOrEmpty(updateRequest.Password))
                updateDefinition.Add(Builders<User>.Update.Set(u => u.Password, updateRequest.Password));

            if (!string.IsNullOrEmpty(updateRequest.RoleName))
            {
                var role = await _roleCollection.Find(r => r.RoleName == updateRequest.RoleName).FirstOrDefaultAsync();
                if (role == null)
                    throw new ArgumentException($"Role with name '{updateRequest.RoleName}' does not exist.");

                updateDefinition.Add(Builders<User>.Update.Set(u => u.RoleId, role.Id));
            }

            if (!updateDefinition.Any())
                return false;

            var update = Builders<User>.Update.Combine(updateDefinition);

            var result = await _userCollection.UpdateOneAsync(
                u => u.Id == userId,
                update
            );

            return result.MatchedCount > 0 && result.ModifiedCount > 0;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            var users = await _userCollection.Find(Builders<User>.Filter.Empty).ToListAsync();

            var userResponses = users.Select(user =>
            {
                var role = _roleCollection.Find(r => r.Id == user.RoleId).FirstOrDefault();

                return new User
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    UserName = user.UserName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    RoleId = role?.Id,
                    CreatedAt = user.CreatedAt,
                    UpdatedAt = user.UpdatedAt
                };
            });

            return userResponses;
        }

        public async Task<User> GetUserByIdAsync(string userId)
        {
            if (!ObjectId.TryParse(userId, out var objectId))
                throw new ArgumentException("Invalid user ID format.");

            var user = await _userCollection.Find(u => u.Id == userId).FirstOrDefaultAsync();

            if (user == null)
                return null;

            var role = await _roleCollection.Find(r => r.Id == user.RoleId).FirstOrDefaultAsync();

            return new User
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                RoleId = role?.Id,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };
        }

    }
}
