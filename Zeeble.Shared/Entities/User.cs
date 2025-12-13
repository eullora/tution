using System.ComponentModel.DataAnnotations.Schema;

namespace Zeeble.Shared.Entities
{
    [Table("User")]
    public class User : BaseEntity
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool IsActive { get; set; }        
        public int TenantId { get; set; }
        public Tenant Tenant { get; set; }
        public int RoleId { get; set; }
        public Role Role { get; set; }
    }
}
