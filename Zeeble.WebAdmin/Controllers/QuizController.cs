using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Zeeble.Shared.Entities;
using Zeeble.Shared.Helpers;
using Zeeble.Web.Admin.Helpers;
using Zeeble.Web.Admin.Models;
using FS = System.IO.File;

namespace Zeeble.Web.Admin.Controllers
{
    [Authorize(Roles = "admin,operator")]
    public class QuizController : AdminBaseController
    {
        private readonly WebApiDBContext _dbContext;
        private readonly IConfiguration _configuration;

        public QuizController(WebApiDBContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var rows = await _dbContext.Banks
                .Include(x => x.Subject)
                .Include(x => x.Chapter)
                .Include(x => x.Standard)
                .Where(m => m.TenantId == TenantId)
                .OrderByDescending(x => x.Id).AsNoTracking().ToListAsync();

            ViewData["Rows"] = rows;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Questions(int id)
        {
            var row = await _dbContext.Banks.FindAsync(id);
            ViewData["QuizTitle"] = row.Title;

            var directory = _configuration.GetSection("QuestionStorage:JsonData").Value;
            var filePath = $"{directory}/files/{row.FileId}.json";

#if DEBUG
            filePath = $"D:\\bank\\files\\{row.FileId}.json";
#endif
            var data = await FS.ReadAllTextAsync(filePath);
            var questionModel = JsonConvert.DeserializeObject<IEnumerable<QuestionModel>>(data);

            ViewData["QuestionModel"] = questionModel;

            return View();
        }


        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var subjects = await _dbContext.Subjects.Where(x => x.TenantId == TenantId).ToListAsync();
            ViewData["Subjects"] = subjects.Select(x => new KeyPairModel
            {
                Id = x.Id,
                Title = x.Name,
            });
            var standards = await _dbContext.Standards.ToListAsync();
            ViewData["Standards"] = standards.Select(x => new KeyPairModel
            {
                Id = x.Id,
                Title = x.Name,
            });

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromForm] BankModel model, IFormFile questionFile)
        {
            var fileId = Guid.NewGuid().ToString();
            var directory = _configuration.GetSection("QuestionStorage:SourceFile").Value;
            var sourceFilePath = $"{directory}/documents/{fileId}.docx";

#if DEBUG
            sourceFilePath = $"D:\\bank\\documents\\{fileId}.docx";
#endif
            using (var stream = new FileStream(sourceFilePath, FileMode.Create))
            {
                await questionFile.CopyToAsync(stream);
            };

            var officeHelper = new OfficeDocumentHelper();
            var jsonModel = officeHelper.QuizQuestionsFromWord(sourceFilePath);

            var jsonFilePath = $"{directory}/files/{fileId}.json";

#if DEBUG
            jsonFilePath = $"D:\\bank\\files\\{fileId}.json";
#endif
            await FS.WriteAllTextAsync(jsonFilePath, JsonConvert.SerializeObject(jsonModel));

            await _dbContext.Banks.AddAsync(new Bank
            {
                FileId = fileId,
                SourceFileId = fileId,
                TenantId = TenantId,
                Title = model.Title,
                ChapterId = model.ChapterId,
                SubjectId = model.SubjectId,
                StandardId = model.StandardId
            });

            await _dbContext.SaveChangesAsync();

            return RedirectToAction("index");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var row = await _dbContext.Banks.FindAsync(id);
            _dbContext.Banks.Remove(row);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction("index");
        }

    }
}
