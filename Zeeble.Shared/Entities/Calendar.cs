using System.ComponentModel.DataAnnotations.Schema;

namespace Zeeble.Shared.Entities
{
    [Table("Calender")]
    public  class Calender : BaseEntityWithTenant
    {
        public DateTime LectureDay { get; set; }
        public bool IsHoliday { get; set; }
    }
}
