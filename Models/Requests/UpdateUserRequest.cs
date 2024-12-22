using System.ComponentModel.DataAnnotations;

namespace ConstructionManagementService.Models.Requests
{
    public class UpdateUserRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string PhoneNumber { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public string Password { get; set; }
        public string RoleName { get; set; }
    }
}
