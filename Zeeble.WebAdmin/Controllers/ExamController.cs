using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Zeeble.Shared.Entities;
using Zeeble.Shared.Helpers;
using Zeeble.Web.Admin.Models;
using Zeeble.Web.Admin.Helpers;
using FS = System.IO.File;
using Microsoft.AspNetCore.Authorization;

namespace Zeeble.Web.Admin.Controllers
{
    [Authorize(Roles = "admin,operator,foundation")]
    public class ExamController : AdminBaseController
    {
        private readonly WebApiDBContext _dbContext;       
        private readonly IConfiguration _configuration;

        public ExamController(WebApiDBContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;            
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int mode = 0)
        {
            var rows = await _dbContext.Exams
                .Include(i => i.ExamType)
                .Include(i => i.ExamBatches)
                .Include("ExamBatches.Batch")              
                .Where(w => w.TenantId == TenantId && w.IsActive)
                .OrderByDescending(x => x.Id).AsNoTracking().ToListAsync();

            ViewData["Rows"] = rows;
            ViewData["Mode"] = mode;
            return View();
        }        

        [HttpGet]        
        public async Task<IActionResult> Add()
        {
            await SetViewState();

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Paper(int id)
        {
            var row = await _dbContext.Exams.FindAsync(id);
            ViewData["ExamTitle"] = row.Name;

            var directory = _configuration.GetSection("ExamStorage:ExamFiles").Value;
            var filePath = row.ExamModeId == 1 ?  $"{directory}/jsonfiles/{row.PaperFile}.json" : $"{directory}/wordfiles/{row.PaperFile}";

            if(row.ExamModeId == 0)
            {
                var extension = Path.GetExtension(row.PaperFile.ToLower());
                return File(await FS.ReadAllBytesAsync(filePath), extension ==".pdf" ?  "application/pdf" : "application/vnd.ms-word",row.Description);
            }

#if DEBUG
            filePath = $"D:\\Exams\\JsonFiles\\{row.PaperFile}.json";
#endif
            var data = await FS.ReadAllTextAsync(filePath);
            var paperModel = JsonConvert.DeserializeObject<IEnumerable<QuestionModel>>(data);
            ViewData["PaperModel"] = paperModel;

            return View();
        }

        [HttpPost]        
        public async Task<IActionResult> Add([FromForm] ExamModel model , IFormFile questionFile)
        {
            var fileId = Guid.NewGuid().ToString();
            var directory = _configuration.GetSection("ExamStorage:ExamFiles").Value;
            var extension = Path.GetExtension(questionFile.FileName);
            var srcPath = $"{directory}/wordfiles/{fileId}{extension}";

#if DEBUG
            srcPath = $"D:\\Exams\\WordFiles\\{fileId}{extension}";
#endif
            
            using (var stream = new FileStream(srcPath, FileMode.Create))
            {
                await questionFile.CopyToAsync(stream);
            };

            if(model.ExamModeId == 1) 
            {
                var jsonFile = $"{directory}/jsonfiles/{fileId}.json";
#if DEBUG
                jsonFile = $"D:\\Exams\\JsonFiles\\{fileId}.json";
#endif
                var officeHelper = new OfficeDocumentHelper();
                var jsonModel = officeHelper.QuestionsFromWord(srcPath);

                if(model.QuestionCount != jsonModel.Count())
                {
                    ViewData["Message"] = "Question count and questions in the template are not matching. Please check the template and try again";
                    FS.Delete(srcPath);
                    await SetViewState();
                    return View();
                }

                await FS.WriteAllTextAsync(jsonFile, JsonConvert.SerializeObject(jsonModel));
            }

            await _dbContext.Exams.AddAsync(new Exam
            {                
                CreatedOn = DateTime.Now,
                EndDate = model.EndDate,
                StartDate = model.StartDate,
                ExamTypeId = model.ExamTypeId,            
                Description = questionFile.FileName,
                IsActive =  true,
                Marks = model.Marks,
                Time = model.Time,                
                QuestionCount = model.QuestionCount,    
                Name = model.Name,
                PaperFile = model.ExamModeId == 1 ? fileId : $"{fileId}{extension}",
                TenantId = TenantId,
                SubjectData =  JsonConvert.SerializeObject(model.SubjectIds),
                ExamBatches = model.BatchIds.Select(x => new ExamBatch
                {
                    BatchId = x
                }).ToList(),
                
                ExamModeId = model.ExamModeId
            });

            await _dbContext.SaveChangesAsync();

            return RedirectToAction("index", new {mode = model.ExamModeId == 1 ? 1 : 0});
        }

        [HttpGet]
        public async Task<IActionResult> Result()
        {
            var results = await _dbContext
                    .Results
                    .Include(x => x.Exam)
                    .Include(m => m.Student)
                    .Where(w => w.Exam.TenantId == TenantId)
                    .AsNoTracking().ToListAsync();

            ViewData["ResultRows"] = results;
            return View();
        }

        [HttpGet]        
        public async Task<IActionResult> Delete(int id)
        {
            var row = await _dbContext.Exams.FindAsync(id);
            _dbContext.Exams.Remove(row);   

            var examBatches = await _dbContext.ExamBatches.Where(x => x.ExamId == id).ToListAsync();   
            _dbContext.ExamBatches.RemoveRange(examBatches);

            await _dbContext.SaveChangesAsync();            
            return RedirectToAction("index");
        }
        private async Task SetViewState()
        {
            var batches = await _dbContext.Batches.Where(x => x.IsActive).AsNoTracking().ToListAsync();
            var examTypes = await _dbContext.ExamTypes.AsNoTracking().ToListAsync();
            var subjects = await _dbContext.Subjects.AsNoTracking().ToListAsync();

            ViewData["Batches"] = batches;
            ViewData["ExamTypes"] = examTypes;
            ViewData["Subjects"] = subjects;
        }
    }
}
