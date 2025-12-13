using System.ComponentModel.DataAnnotations;

namespace Zeeble.Shared.Entities
{
    public class BaseEntity
    {
        [Key]
        public int Id { get; set; }
    }
    public class BaseEntityWithTenant
    {
        [Key]
        public int Id { get; set; }
        public int TenantId { get; set; }
        public Tenant Tenant { get; set; }  
    }
}
