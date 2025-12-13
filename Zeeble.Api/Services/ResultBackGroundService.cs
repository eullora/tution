using FirebaseAdmin.Messaging;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Zeeble.Api.Models;
using Zeeble.Shared.Entities;
using Zeeble.Shared.Helpers;

namespace Zeeble.Api.Services
{
    public class ResultBackGroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private WebApiDBContext _dbContext;
        private IConfiguration _configuration;
        private int _tenantId = 0;
        public ResultBackGroundService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (true)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    _dbContext = scope.ServiceProvider.GetRequiredService<WebApiDBContext>();
                    _configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();                    

                    var rows = await _dbContext.Submissions.Where(w => w.StatusId == 1).Take(50).AsNoTracking().ToListAsync();

                    if (rows.Any())
                    {
                        foreach (var row in rows)
                        {
                            
                            var entityId = await Calculate(row);
                            if (entityId > 0)
                            {
                                var studentRow = await _dbContext.Students.FindAsync(row.StudentId);
                                var messaging = FirebaseMessaging.DefaultInstance;
                                var message = new Message()
                                {
                                    Notification = new Notification
                                    {
                                        Title = "Zeeble",
                                        Body = $"Your exam result is ready.",
                                    },
                                    Token = studentRow.DeviceToken
                                };

                                var msgResult = await messaging.SendAsync(message);
                                await Task.Delay(5000);
                            }
                        }
                    }
                }

#if DEBUG
                await Task.Delay(180000);

#else
                await Task.Delay(1800000);
#endif
            }
        }

        public async Task<int> Calculate(Submission row)
        {
            var scholorshipIds = new List<int>();
            var directory = _configuration.GetSection("ExamStorage:ExamFiles").Value;
            var exam = await _dbContext.Exams.FindAsync(row.ExamId);
            var subjectList = await _dbContext.Subjects.Where(x => x.TenantId == exam.TenantId).AsNoTracking().ToListAsync();

            var result = new List<ExamResultModel>();
            
            var examSubject = JsonConvert.DeserializeObject<IEnumerable<int>>(exam.SubjectData);

            var submissionPath = $"{directory}/submissions/{row.DataFile}.json";
            var paperFilePath = $"{directory}/jsonfiles/{exam.PaperFile}.json";
#if DEBUG
            submissionPath = $"{directory}\\submissions\\{row.DataFile}.json";
            paperFilePath = $"{directory}\\jsonfiles\\{exam.PaperFile}.json";
#endif

            var submitData = await File.ReadAllTextAsync(submissionPath);
            var paperData = await File.ReadAllTextAsync(paperFilePath);
            var submitModel = JsonConvert.DeserializeObject<IEnumerable<AnswerSheetModel>>(submitData);
            var paperModel = JsonConvert.DeserializeObject<IEnumerable<ExamPaperModel>>(paperData);
            int serialNumber = 1;

            foreach (var item in paperModel)
            {
                var questionModel = submitModel.Where(x => x.Id == item.Id).FirstOrDefault();
                var entry = new ExamResultModel();
                if (questionModel == null)
                {
                    entry.UserAnswer = "";
                    entry.Remark = "No Attempt";
                    result.Add(entry);
                    serialNumber++;
                    continue;
                }
                entry.SerialNumber = serialNumber;
                entry.Id = item.Id;
                entry.CorrectAnswer = item.CorrectAnswer;
                entry.ImageData = item.ImageData;
                entry.Subject = item.SubjectName;
                entry.UserAnswer = questionModel.UserAnswer;
                entry.Mark = 0;
                entry.Remark = "Wrong";
                if (questionModel.UserAnswer == item.CorrectAnswer)
                {
                    entry.Mark = 4;
                    entry.Remark = "Correct";
                }

                if (questionModel.UserAnswer == "S")
                {
                    entry.Remark = "Skip";
                }

                result.Add(entry);

                serialNumber++;
            }

            var resultFile = Guid.NewGuid().ToString();
            var obtainedMarks = result.Where(w => w.Mark > 0).Sum(x => x.Mark);

            var resultFilePath = $"{directory}/results/{resultFile}.json";
#if DEBUG
            resultFilePath = $"{directory}\\results\\{resultFile}.json";
#endif
            await File.WriteAllTextAsync(resultFilePath, JsonConvert.SerializeObject(result));

            var resultGroup = result
                .GroupBy(d => d.Subject)
                .Select(group => new SubjectGroup
                {
                    SubjectName = group.Key,
                    Marks = group.Sum(d => d.Mark)
                }).ToList();

            var subjectResultRows = new List<ResultSubject>();

            foreach (var subjectId in examSubject)
            {
                var subject = subjectList.Find(x => x.Id == subjectId);

                subjectResultRows.Add(new ResultSubject
                {
                    StudentId = row.StudentId,
                    SubjectId = subjectId,
                    Marks = result.FindAll(x => x.Subject.ToLower() == subject.Name.ToLower()).Sum(x => x.Mark)
                });
            }

            var entity = await _dbContext.Results.AddAsync(new Result
            {
                ResultFile = resultFile,
                CreatedOn = DateTime.Now,
                ExamId = row.ExamId,
                Marks = obtainedMarks,
                StudentId=row.StudentId,
                ResultSubjects = subjectResultRows,
            });

            row.StatusId = 2;

            await _dbContext.SaveChangesAsync();

            return entity.Entity.Id;
        }

    }
}
