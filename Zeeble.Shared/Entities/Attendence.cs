using System.ComponentModel.DataAnnotations.Schema;

namespace Zeeble.Shared.Entities
{
    [Table("Attedance")]
    public  class Attedance : BaseEntity
    {
        public int StudentId { get; set; }
        public Student Student { get; set; }
        public DateTime CheckInTime { get; set; }
        public DateTime? CheckOutTime { get; set; }
        public int PresentId { get; set; }       
    }
}
