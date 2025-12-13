using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Zeeble.Api.Models;
using Zeeble.Shared.Entities;
using Zeeble.Shared.Helpers;
using FS = System.IO.File;

namespace Zeeble.Api.Controllers
{
    [Route("api/exams")]
    public class ExamController : ControllerBase
    {
        private readonly WebApiDBContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly string _directory;
        private List<Result> _results;
        private List<Submission> _submissions;
        private List<Subject> _subjects;

        public ExamController(WebApiDBContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _directory = _configuration.GetSection("ExamStorage:ExamFiles").Value;
        }


        [HttpGet]
        [Route("offline/{tenantId:int}/{isempty:bool}")]
        public async Task<IActionResult> GetAllAsync(int tenantId, bool isempty)
        {
            var query = _dbContext.Exams
                    .Include(i => i.ExamBatches)
                    .Include("ExamBatches.Batch")

                    .Include(i => i.ExamType)
                    .Where(x => x.ExamModeId == 0 && x.IsActive && x.TenantId == tenantId);

            query = isempty ? 
                query.Where(x => string.IsNullOrEmpty(x.ResultExcel)) : 
                query = query.Where(x => !string.IsNullOrEmpty(x.ResultExcel));

            _subjects = _dbContext.Subjects.Where(x => x.TenantId == tenantId).AsNoTracking().ToList();
            var rows = await query.OrderByDescending(v => v.Id).AsNoTracking().ToListAsync();
            var response = rows.Select(x => new
            {
                x.Id,
                x.Name,
                x.Marks,
                x.QuestionCount,
                x.StartDate,
                x.EndDate,
                ExamType = x.ExamType.Name,
                Subjects = GetSubjects(JsonConvert.DeserializeObject<IEnumerable<int>>(x.SubjectData)),
                Batches = string.Join(",", x.ExamBatches.Select(b => b.Batch.Title)),
            });

            return Ok(response);
        }


        [HttpGet]
        [Route("{batchId:int}/student/{studentId:int}")]
        public async Task<IActionResult> GetAllAsync(int batchId, int studentId)
        {

            var exams = await _dbContext.Exams
                .Include(m => m.ExamBatches)
                .Include(m => m.ExamType)
                .Include(m => m.ExamType.Instruction)
                .Where(e => e.ExamBatches.Any(eb => eb.BatchId == batchId))
                .OrderByDescending(u => u.Id)
                .AsNoTracking().ToListAsync();

            _results = await _dbContext.Results
                .Include(m => m.ResultSubjects)
                .Include("ResultSubjects.Subject")
                .Where(x => x.StudentId == studentId).AsNoTracking().ToListAsync();

            _submissions = await _dbContext
                        .Submissions
                        .Where(x => x.StudentId == studentId)
                        .AsNoTracking().ToListAsync();

            var response = exams.Select(x => new
            {
                x.Id,
                x.Name,
                x.Marks,
                x.Time,
                x.StartDate,
                x.EndDate,
                x.CreatedOn,
                x.ExamTypeId,
                ExamType = x.ExamType.Name,
                x.ExamType.Code,
                Mode = x.ExamModeId == 0 ? "Offline" : "Online",
                Remark = x.StartDate.Date > DateTime.Now.Date ? "Upcoming" : GetRemark(x.Id),
                StatusId = x.ExamTypeId == 0 ? GetStatus(x.Id) : GetSubmissionStatus(x.Id),
                BreakDown = GetBreakDown(x.Id),
                IsCompleted = x.EndDate.Date < DateTime.Now.Date ? true : false,
                x.ExamType.InstructionId,
                x.ExamType.Instruction.Description
            });

            return Ok(response);
        }

        private int GetStatus(int examId)
        {
            var result = _results.Find(x => x.ExamId == examId);
            if (result == null)
            {
                return 0;
            }
            return result.StatusId;
        }

        private int GetSubmissionStatus(int examId)
        {
            var subms = _submissions.Find(x => x.ExamId == examId);
            if (subms == null)
            {
                return 0;
            }
            return subms.StatusId;
        }

        private string GetRemark(int examId)
        {
            var submission = _submissions.Find(x => x.ExamId == examId);
            var result = _results.Find(x => x.ExamId == examId);            

            if (result == null && submission == null)
            {
                return "Absent";
            }

            if(result == null && submission != null)
            {
                return "Submitted";
            }

            return "Present";
        }

        private string GetSubjects(IEnumerable<int> ids)
        {
            var list = _subjects.Where(subject => ids.Contains(subject.Id));
            return string.Join(",", list.Select(x => x.Name));
        }

        private IEnumerable<SubjectGroup> GetBreakDown(int examId)
        {
            var result = _results.Find(x => x.ExamId == examId);
            if (result == null)
            {
                return null;
            }

            return result.ResultSubjects.Select(m => new SubjectGroup
            {
                Marks = m.Marks,
                SubjectName = m.Subject.Name
            });
        }

        [HttpGet]
        [Route("{id:int}/instruction")]
        public async Task<IActionResult> GetInstruction(int id)
        {
            var row = await _dbContext.Instructions.FindAsync(id);

            return Ok(new { row.Description });
        }

        [HttpGet]
        [Route("{id:int}/paper")]
        public async Task<IActionResult> GetPaper(int id)
        {
            var row = await _dbContext.Exams.FindAsync(id);
            var paperFilePath = $"{_directory}/jsonfiles/{row.PaperFile}.json";
#if DEBUG
            paperFilePath = $"{_directory}\\jsonfiles\\{row.PaperFile}.json";
#endif
            var data = await FS.ReadAllTextAsync(paperFilePath);

            return Ok(data);
        }

        [HttpGet]
        [Route("{examId:int}/result/{studentId:int}")]
        public async Task<IActionResult> GetResult(int examId, int studentId)
        {
            var row = await _dbContext.Results
                        .Where(x => x.ExamId == examId && x.StudentId == studentId)
                        .FirstOrDefaultAsync();

            var resultFilePath = $"{_directory}/results/{row.ResultFile}.json";
#if DEBUG
            resultFilePath = $"{_directory}\\results\\{row.ResultFile}.json";
#endif
            var resultData = await FS.ReadAllTextAsync(resultFilePath);

            var response = new
            {
                row.Id,
                row.Marks,
                Data = resultData
            };

            return Ok(response);
        }

        [HttpGet]
        [Route("solution/{fileName}")]
        public async Task<IActionResult> GetSolution(string fileName)
        {
            var data = await FS.ReadAllTextAsync(fileName);
            return Ok(data);
        }

        [HttpPost]
        [Route("{id:int}/submit")]
        public async Task<IActionResult> Post(int id, [FromBody] ExamSubmitModel model)
        {
            var row = await _dbContext.Exams.FindAsync(id);
            if (row == null)
            {
                return BadRequest();
            }

            var fileId = Guid.NewGuid().ToString();

            var resultEntity = await _dbContext.Submissions.AddAsync(new Submission
            {
                ExamId = model.ExamId,
                StudentId = model.UserId,
                DataFile = fileId,
                StatusId = 1,
                CreatedOn = DateTime.Now
            });

            var submissionPath = $"{_directory}/submissions/{fileId}.json";
#if DEBUG
            submissionPath = $"{_directory}\\submissions\\{fileId}.json";
#endif
            await FS.WriteAllTextAsync(submissionPath, JsonConvert.SerializeObject(model.ExamPaper));
            await _dbContext.SaveChangesAsync();

            return Ok(new { resultEntity.Entity.Id });
        }

        [HttpPost]
        [Route("result/{examId:int}/rollnumber/{rollNumber}")]
        public async Task<IActionResult>Post(int examId, int rollNumber, [FromBody]  IEnumerable<ResultSubjectModel> subjectMarks)
        {
            var student = await _dbContext.Students.FirstOrDefaultAsync(m => m.RollNumber == rollNumber);
            var subjects = await _dbContext.Subjects.ToListAsync();
            var resultSubjects = new List<ResultSubject>();

            foreach (var itm in subjectMarks) 
            {
                var subject = subjects.Find(x => x.Code == itm.Code);
                resultSubjects.Add(new ResultSubject
                {
                    Marks = itm.Marks,
                    StudentId = student.Id,
                    SubjectId = subject.Id,
                });
            }

            var entity = _dbContext.Results.AddAsync(new Result
            {
                CreatedOn = DateTime.Now,
                ExamId = examId,
                Marks = subjectMarks.Sum(u => u.Marks),
                ResultFile = "NA",
                StatusId = 1,
                StudentId = student.Id,
                ResultSubjects = resultSubjects
            });

            return Ok();
        }

    }
}
