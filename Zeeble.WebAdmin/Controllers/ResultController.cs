using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Zeeble.Shared.Entities;
using Zeeble.Shared.Helpers;
using Zeeble.Web.Admin.Models;
using FS = System.IO.File;

namespace Zeeble.Web.Admin.Controllers
{
    [Authorize(Roles = "admin,operator,teacher,foundation")]
    public class ResultController : AdminBaseController
    {
        private readonly WebApiDBContext _dbContext;
        private readonly IConfiguration _configuration;
        public ResultController(WebApiDBContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }

        public async Task<IActionResult> Index(int examId = 0)
        {
            var examList = await _dbContext.Exams
                .Where(m => m.TenantId == TenantId)
                .OrderByDescending(e => e.Id)
                .AsNoTracking()
                .ToListAsync();

            ViewData["ExamList"] = examList.Select(x => new KeyPairModel
            {
                Id = x.Id,
                Title = x.Name
            });

            var examRow = await _dbContext.Exams.FindAsync(examId);
            ViewData["ExamRow"] = examRow != null && examRow.ExamModeId == 0 ? examRow : null;
            ViewData["ExamId"] = examId.ToString();

            var batchList = await _dbContext.Batches
                    .Where(x => x.TenantId == TenantId).ToListAsync();

            ViewData["BatchList"] = batchList.Select(x => new KeyPairModel
            {
                Id = x.Id,
                Title = x.Title
            });

            if (examId > 0)
            {
                ViewData["Rows"] = await _dbContext
                        .Results
                        .Include(e => e.Student)
                        .Include(e => e.Student.Batch)
                        .Include(e => e.Exam)
                        .Where(w => w.ExamId == examId)
                        .OrderByDescending(x => x.Marks)
                        .AsNoTracking().ToListAsync();

                return View();
            }

            ViewData["Rows"] = await _dbContext
                        .Results
                        .Include(e => e.Student)
                        .Include(e => e.Student.Batch)
                        .Include(e => e.Exam)
                        .Where(w => w.Student.TenantId == TenantId)
                        .OrderByDescending(x => x.Id)
                        .AsNoTracking().ToListAsync();

            return View();
        }

        [HttpGet]        
        public async Task<IActionResult> Add(int examId)
        {
            var exam = await _dbContext.Exams.FindAsync(examId);
            var subjectList = await _dbContext.Subjects.Where(x => x.TenantId == TenantId).ToListAsync();

            ViewData["ExamRow"] = exam;
            ViewData["SubjectRows"] = subjectList;
            ViewData["SubjectJson"] = JsonConvert.SerializeObject(subjectList);

            return View();
        }

        [HttpPost]        
        public async Task<IActionResult> Add(IFormFile resultFile, [FromForm] IEnumerable<int> subjectIds, [FromForm] int examId, [FromForm] string subjectColumns)
        {
            var exam = await _dbContext.Exams.Include(i => i.ExamBatches).FirstOrDefaultAsync(x => x.Id == examId);
            var fileId = Guid.NewGuid().ToString();

            var directory = _configuration.GetSection("ExamStorage:ResultUpload").Value;
            var filePath = $"{directory}/{fileId}{Path.GetExtension(resultFile.FileName)}";
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await resultFile.CopyToAsync(stream);
            };
            var subjectList = await _dbContext.Subjects.Where(x => x.TenantId == TenantId).AsNoTracking().ToListAsync();

            var lines = FS.ReadAllLines(filePath);
            var header = lines.FirstOrDefault();
            var headerItems = header.Split(",");
            var studentList = new List<Student>();
            foreach (var batch in exam.ExamBatches)
            {
                studentList.AddRange(await _dbContext.Students.Where(m => m.BatchId == batch.BatchId)
                    .AsNoTracking().ToListAsync());
            }

            var selectedHeaders = new List<string>
                {
                    headerItems[0],
                    headerItems[1]
                };

            var columnInfo = subjectColumns.Split("#");
            var resultSubjecList = new List<ResultSubject>();
            int counter = 0;
            var result = new List<Result>();

            foreach (var line in lines)
            {
                if (counter == 0)
                {
                    counter++;
                    continue;
                }
                var lineItem = line.Split(',');
                var student = studentList.Find(p => p.RollNumber == Convert.ToInt32(lineItem[0]));
                if (student == null) 
                {
                    continue;
                }

                var resultItem = new Result();
                resultItem.StudentId = student.Id;
                resultItem.Student = new Student
                {
                    FullName = student.FullName,
                    RollNumber = student.RollNumber
                };

                resultItem.ResultFile = fileId;
                resultItem.CreatedOn = DateTime.Now;
                resultItem.ExamId = exam.Id;
                resultSubjecList.Clear();


                foreach (var column in columnInfo)
                {
                    var col = column.Split('@');
                   
                    var subject = subjectList.Find(x => x.Id == Convert.ToInt16(col[0].Trim()));
                    if (!selectedHeaders.Contains(subject.Code))
                    {
                        selectedHeaders.Add(subject.Code);
                    }

                    resultSubjecList.Add(new ResultSubject
                    {
                        Marks = Convert.ToInt32(lineItem[Convert.ToInt32(col[1].Trim())]),
                        StudentId = student.Id,
                        SubjectId = Convert.ToInt32(col[0].Trim())
                    });
                }

                resultItem.Marks = resultSubjecList.Sum(u => u.Marks);
                resultItem.ResultSubjects = resultSubjecList;
                result.Add(resultItem);
                resultSubjecList = new List<ResultSubject>();
            }

            selectedHeaders.Add("Marks");

            HttpContext.Session.SetString("_csvFileId", fileId);
            HttpContext.Session.SetString("_headers", JsonConvert.SerializeObject(selectedHeaders));
            HttpContext.Session.SetString("_resultItems", JsonConvert.SerializeObject(result));

            return RedirectToAction("confirm",new {id = examId});
        }

        public IActionResult Confirm(int id)
        {
            ViewData["Rows"] = JsonConvert.DeserializeObject<IEnumerable<Result>>(HttpContext.Session.GetString("_resultItems"));
            ViewData["Headers"] = JsonConvert.DeserializeObject<IEnumerable<string>>(HttpContext.Session.GetString("_headers"));
            ViewData["ExamId"] = id;

            return View();
        }

        public async Task<IActionResult> Upload(int examId)
        {
            var resultItems = JsonConvert.DeserializeObject<IEnumerable<Result>>(HttpContext.Session.GetString("_resultItems"));
            var fileId = HttpContext.Session.GetString("_csvFileId");
            var examRow = await _dbContext.Exams.FindAsync(examId);
            examRow.ResultExcel = fileId;

            foreach (var item  in resultItems)
            {
                var entity = _dbContext.Results.AddAsync(new Result
                {
                    CreatedOn = DateTime.Now,
                    ExamId = examId,
                    Marks = item.Marks,
                    ResultFile = fileId,
                    StatusId = 1,                    
                    StudentId = item.StudentId,
                    ResultSubjects = item.ResultSubjects                
                });
            }

            await _dbContext.SaveChangesAsync();

            return RedirectToAction("index","exam");
        }
    }
}
