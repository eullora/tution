using System.ComponentModel.DataAnnotations.Schema;

namespace Zeeble.Shared.Entities
{
    [Table("StreamingLog")]
    public class StreamingLog : BaseEntity
    {        
        public int StreamingId { get; set; }                
        public int StudentId { get; set; }
        public DateTime StartDateTime { get; set; }        
        public DateTime EndDateTime { get; set; }        


    }
}
