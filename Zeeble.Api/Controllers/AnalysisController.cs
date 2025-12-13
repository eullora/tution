using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zeeble.Shared.Entities;
using Zeeble.Shared.Helpers;

namespace Zeeble.Api.Controllers
{
    [Route("api/analyze")]
    public class AnalysisController : ControllerBase
    {
        private readonly WebApiDBContext _dbContext;
        private readonly IConfiguration _configuration;
        private List<Result> _results;
        public AnalysisController(WebApiDBContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }

        [HttpGet]
        [Route("{studentId:int}/exams")]
        public async Task<IActionResult> GetExams(int studentId)
        {
            var result = await _dbContext.Results
                .Include(i => i.Exam)
                .Include(i => i.ResultSubjects)
                .Include("ResultSubjects.Subject")
                .Where(x => x.StudentId == studentId)
                .Take(10)
                .AsNoTracking()
                .ToListAsync();

            var response = result.Select(m => new { m.Marks, 
                
                Title = $"{m.Exam.StartDate.Day} {m.Exam.StartDate:MMM}", Breakdown = m.ResultSubjects.Select(b => new
                {
                    b.Marks, SubjectName = b.Subject.Name
                }) });

            return Ok(response);
        }

        [HttpGet]
        [Route("{studentId:int}/exams/all")]
        public async Task<IActionResult> GetAllExams(int studentId)
        {
            var student = await _dbContext.Students.FindAsync(studentId);

            var exams = await _dbContext.Exams
                        .Include(m => m.ExamBatches)
                        .Where(e => e.ExamBatches.Any(eb => eb.BatchId == student.BatchId) && e.StartDate.Date < DateTime.Now.Date)
                        .AsNoTracking().ToListAsync();

            _results = await _dbContext.Results
                    .Include(i => i.ResultSubjects)
                    .Include("ResultSubjects.Subject")
                    .Where(x => x.StudentId == studentId)
                    .AsNoTracking().ToListAsync();

            var response = exams.Select(m => new { ExamName = m.Name,  Marks = GetMarks(m.Id), m.StartDate.Day, m.StartDate.Month, Remark = GetRemark(m.Id) , SubjectMarks = GetSubjectMarksLabel(m.Id) });

            return Ok(response);
        }

        private string GetRemark(int id)
        {
            var res = _results.Find(m => m.ExamId == id);
            if (res != null)
            {
                return "Present";
            }

            return "Absent";
        }
        private int GetMarks(int id)
        {
            var res = _results.Find(m => m.ExamId == id);
            if (res != null)
            {
                return res.Marks;
            }

            return 0;
        }  
        private string GetSubjectMarksLabel(int id)
        {
            var res = _results.Find(m => m.ExamId == id);
            if (res == null)
            {
                return "";
            }

            var labelList = new List<string>();
            foreach(var rs in res.ResultSubjects)
            {
                labelList.Add($"{rs.Subject.Code}:{rs.Marks}");
            }

            return string.Join(",", labelList);
            
        }
    }
}
