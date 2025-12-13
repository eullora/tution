using Microsoft.EntityFrameworkCore;
using Zeeble.Shared.Entities;

namespace Zeeble.Shared.Helpers
{
    public class WebApiDBContext : DbContext
    {
        public WebApiDBContext(DbContextOptions<WebApiDBContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Bank> Banks { get; set; }        
        public DbSet<Student> Students { get; set; }        
        public DbSet<Standard> Standards { get; set; }
        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<Solution> Solutions { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<DocumentType> DocumentTypes { get; set; }
        public DbSet<DocumentBatch> DocumentBatches { get; set; }
        public DbSet<DocumentSubject> DocumentSubjects { get; set; }
        public DbSet<PayTerm> PayTerms { get; set; }
        public DbSet<Streaming> Streamings { get; set; }
        public DbSet<Subject> Subjects{ get; set; }        
        public DbSet<Transaction> Transactions{ get; set; }        
        public DbSet<Exam> Exams { get; set; }        
        public DbSet<ExamType> ExamTypes { get; set; }
        public DbSet<Instruction> Instructions { get; set; }
        public DbSet<Batch> Batches { get; set; }
        public DbSet<Submission> Submissions { get; set; }        
        public DbSet<Result> Results { get; set; }
        public DbSet<ResultSubject> ResultSubjects { get; set; }
        public DbSet<Enquiry> Enquiries{ get; set; }                
        public DbSet<Payment> Payments { get; set; }                
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }      
        public DbSet<Product> Products { get; set; }
        public DbSet<Stock> Stocks { get; set; }
        public DbSet<Attedance> Attedances { get; set; }
        public DbSet<BarCode> BarCodes { get; set; }
        public DbSet<ExamBatch> ExamBatches { get; set; }
        public DbSet<Fee> Fees{ get; set; }
        public DbSet<Section> Sections{ get; set; }
        public DbSet<TenantSection> TenantSections { get; set; }
        public DbSet<StreamingBatch> StreamingBatches { get; set; }
        public DbSet<DocumentUser> DocumentUsers { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<Chapter> Chapters { get; set; }        
        public DbSet<Installment> Installments   { get; set; }
        public DbSet<Calender> Calendars { get; set; }
        public DbSet<StreamingLog> StreamingLogs { get; set; }
        public DbSet<FlashCard> FlashCards { get; set; }
        public DbSet<Template> Templates { get; set; }
        public DbSet<Present> Presents { get; set; }

    }
}