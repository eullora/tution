using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zeeble.Shared.Entities;
using Zeeble.Shared.Helpers;
using Zeeble.Web.Admin.Models;

namespace Zeeble.Web.Admin.Controllers
{
    [Authorize(Roles = "admin,operator,teacher,foundation")]
    public class DocumentController : AdminBaseController
    {
        private readonly WebApiDBContext _dbContext;
        private readonly IConfiguration _configuration;

        public DocumentController(WebApiDBContext dbContext,  
            IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var rows = await _dbContext.Documents
                .Include(i => i.DocumentType)
                .Include(i => i.DocumentBatches)
                .Include("DocumentBatches.Batch")                
                .Include(i => i.DocumentSubjects)
                .Include("DocumentSubjects.Subject")
                .Where(x => x.TenantId == TenantId)
                .AsNoTracking()
                .OrderByDescending(x => x.Id).ToListAsync();

            ViewData["Rows"] = rows;                
            return View();
        }
        
        [HttpGet]        
        public async Task<IActionResult> Add()
        {            
            var batches = await _dbContext.Batches.AsNoTracking().ToListAsync();    
            var docTypes = await _dbContext.DocumentTypes.AsNoTracking().ToListAsync();
            var subjects = await _dbContext.Subjects.AsNoTracking().ToListAsync();
            
            ViewData["Batches"] = batches;
            ViewData["DocumentTypeRows"] = docTypes;
            ViewData["Subjects"] = subjects;

            return View();
        }
      

        [HttpPost]        
        public async Task<IActionResult> Add([FromForm] DocumentRequestModel model , IFormFile documentFile)
        {
            var fileId = Guid.NewGuid().ToString();
            var directory = _configuration.GetSection("DocumentStorage:Files").Value;
            var sourceFilePath = $"{directory}/{fileId}.pdf";

#if DEBUG
            sourceFilePath = $"D:\\Exams\\WordFiles\\{fileId}.pdf";
#endif
            using (var stream = new FileStream(sourceFilePath, FileMode.Create))
            {
                await documentFile.CopyToAsync(stream);
            };          
           
            var entity = await _dbContext.Documents.AddAsync(new Document
            {
                DocumentTypeId = model.DocumentTypeId,
                Title = model.Title,
                Comments = model.Comments,  
                CreatedOn = DateTime.Now,
                FileName = fileId,                
                TenantId = TenantId,
                DocumentSubjects = model.SubjectIds.Select(x => new DocumentSubject
                {                    
                    SubjectId = x
                }).ToList(),
                DocumentBatches = model.BatchIds .Select(x => new DocumentBatch
                {
                    BatchId = x
                }).ToList(),
            });

            await _dbContext.SaveChangesAsync();

            return RedirectToAction("index");
        }

        [HttpGet]        
        public async Task<IActionResult> Delete(int id)
        {
            var row = await _dbContext.Documents.FindAsync(id);
            _dbContext.Documents.Remove(row);   
            await _dbContext.SaveChangesAsync();            
            return RedirectToAction("index");
        }      
    }
}
