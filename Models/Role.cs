using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ConstructionManagementService.Models
{
    public class Role
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString(); // Use ObjectId for MongoDB

        [BsonElement("role_name")]
        public string RoleName { get; set; }

        [BsonElement("access_permissions")]
        public List<AccessPermission> AccessPermissions { get; set; }
    }

    public enum Permissions
    {
        NO_ACCESS,
        READ,
        DISPLAY,
        WRITE,
        ADMIN
    }

    public class AccessPermission
    {
        [BsonElement("item_id")]
        public string ItemId { get; set; } = ObjectId.GenerateNewId().ToString();

        [BsonElement("item_name")]
        public string ItemName { get; set; }

        [BsonElement("item_label")]
        public string ItemLabel { get; set; }

        [BsonElement("permission")]
        public Permissions Permission { get; set; }
    }
}
