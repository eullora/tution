
using System.ComponentModel.DataAnnotations.Schema;

namespace Zeeble.Shared.Entities
{
    [Table("Document")]
    public class Document : BaseEntityWithTenant
    {
        public string Title { get; set; }
        public string Comments { get; set; }
        public string FileName { get; set; }
        public int DocumentTypeId { get; set; }
        public virtual DocumentType DocumentType { get; set; }                
        public DateTime CreatedOn { get; set; }
        public virtual ICollection<DocumentSubject> DocumentSubjects { get; set; }  
        public virtual ICollection<DocumentBatch> DocumentBatches { get; set; }  
    }

    [Table("DocumentType")]
    public class DocumentType : BaseEntity
    {
        public string Title { get; set; }
        public string Code { get; set; }

    }

}

