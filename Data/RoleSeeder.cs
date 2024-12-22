using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using ConstructionManagementSaaS.Models;
using ConstructionManagementService.Models;

namespace ConstructionManagementSaaS.Data
{
    public class RoleSeeder
    {
        private readonly IMongoCollection<Role> _rolesCollection;
        private readonly IMongoCollection<Item> _itemsCollection;

        public RoleSeeder(IMongoClient mongoClient, IMongoDBSettings settings)
        {
            var database = mongoClient.GetDatabase(settings.DatabaseName);
            _rolesCollection = database.GetCollection<Role>("roles");
            _itemsCollection = database.GetCollection<Item>("items");
        }

        public async Task SeedRolesAsync()
        {
            // Check if roles already exist
            if (await _rolesCollection.CountDocumentsAsync(_ => true) > 0)
            {
                Console.WriteLine("Roles already seeded.");
                return;
            }

            // Predefined items
            var items = new List<Item>
            {
                new Item
                {
                    ItemName = "Dashboard",
                    ItemLabel = "View Dashboard",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Item
                {
                    ItemName = "User Management",
                    ItemLabel = "Manage Users",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Item
                {
                    ItemName = "Project Management",
                    ItemLabel = "Manage Projects",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Item
                {
                    ItemName = "Task Assignment",
                    ItemLabel = "Assign Tasks",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Item
                {
                    ItemName = "Technical Review",
                    ItemLabel = "Review Technical Details",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            };

            // Insert items into the database
            await _itemsCollection.InsertManyAsync(items);

            // Retrieve the generated item documents
            var itemList = await _itemsCollection.Find(_ => true).ToListAsync();
            var itemMap = new Dictionary<string, (string Id, string ItemName, string ItemLabel)>();
            foreach (var item in itemList)
            {
                itemMap[item.ItemName] = (item.Id, item.ItemName, item.ItemLabel);
            }

            // Predefined roles
            var roles = new List<Role>
            {
                new Role
                {
                    RoleName = "Admin",
                    AccessPermissions = new List<AccessPermission>
                    {
                        new AccessPermission
                        {
                            ItemId = itemMap["Dashboard"].Id.ToString(),
                            ItemName = itemMap["Dashboard"].ItemName,
                            ItemLabel = itemMap["Dashboard"].ItemLabel,
                            Permission = Permissions.ADMIN
                        },
                        new AccessPermission
                        {
                            ItemId = itemMap["User Management"].Id.ToString(),
                            ItemName = itemMap["User Management"].ItemName,
                            ItemLabel = itemMap["User Management"].ItemLabel,
                            Permission = Permissions.ADMIN
                        },
                        new AccessPermission
                        {
                            ItemId = itemMap["Project Management"].Id.ToString(),
                            ItemName = itemMap["Project Management"].ItemName,
                            ItemLabel = itemMap["Project Management"].ItemLabel,
                            Permission = Permissions.WRITE
                        },
                        new AccessPermission
                        {
                            ItemId = itemMap["Task Assignment"].Id.ToString(),
                            ItemName = itemMap["Task Assignment"].ItemName,
                            ItemLabel = itemMap["Task Assignment"].ItemLabel,
                            Permission = Permissions.DISPLAY
                        },
                        new AccessPermission
                        {
                            ItemId = itemMap["Technical Review"].Id.ToString(),
                            ItemName = itemMap["Technical Review"].ItemName,
                            ItemLabel = itemMap["Technical Review"].ItemLabel,
                            Permission = Permissions.READ
                        }
                    }
                },
                new Role
                {
                    RoleName = "Builder",
                    AccessPermissions = new List<AccessPermission>
                    {
                        new AccessPermission
                        {
                            ItemId = itemMap["Project Management"].Id.ToString(),
                            ItemName = itemMap["Project Management"].ItemName,
                            ItemLabel = itemMap["Project Management"].ItemLabel,
                            Permission = Permissions.WRITE
                        }
                    }
                },
                new Role
                {
                    RoleName = "Contractor",
                    AccessPermissions = new List<AccessPermission>
                    {
                        new AccessPermission
                        {
                            ItemId = itemMap["Task Assignment"].Id.ToString(),
                            ItemName = itemMap["Task Assignment"].ItemName,
                            ItemLabel = itemMap["Task Assignment"].ItemLabel,
                            Permission = Permissions.DISPLAY
                        }
                    }
                },
                new Role
                {
                    RoleName = "Engineer",
                    AccessPermissions = new List<AccessPermission>
                    {
                        new AccessPermission
                        {
                            ItemId = itemMap["Technical Review"].Id.ToString(),
                            ItemName = itemMap["Technical Review"].ItemName,
                            ItemLabel = itemMap["Technical Review"].ItemLabel,
                            Permission = Permissions.READ
                        }
                    }
                }
            };

            // Insert roles into the database
            await _rolesCollection.InsertManyAsync(roles);
        }
    }
}
