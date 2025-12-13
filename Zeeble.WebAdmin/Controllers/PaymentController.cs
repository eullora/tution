using Barcoder.Qr;
using Barcoder.Renderer.Image;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zeeble.Shared.Entities;
using Zeeble.Shared.Helpers;
using Zeeble.Shared.Models;

namespace Zeeble.Web.Admin.Controllers
{
    [Authorize(Roles = "admin,cashier")]
    public class PaymentController : AdminBaseController
    {
        private readonly WebApiDBContext _dbContext;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public PaymentController(WebApiDBContext dbContext, IWebHostEnvironment hostingEnvironment)
        {
            _dbContext = dbContext;
            _hostingEnvironment = hostingEnvironment;
        }
        public async Task<IActionResult> Index()
        {
            ViewData["PayTermRows"] = await _dbContext.PayTerms.ToListAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index([FromForm] int RollNumber)
        {
            var student = await
                _dbContext.Students
                .Include(x => x.Batch)
                .Include(x => x.Batch.Fee)
                .Include(x => x.Payments)
                .Where(x => x.RollNumber == RollNumber && x.TenantId == TenantId)
                .FirstOrDefaultAsync();

            ViewData["PayTermRows"] = await _dbContext.PayTerms.ToListAsync();
            if (student == null)
            {
                ViewData["Message"] = $"Unable to find student with roll number {RollNumber}. Please enter correct roll number.";
                return View();
            }

            var payTerms = await _dbContext.PayTerms.ToListAsync();
            var installments = await _dbContext.Installments.Where(w => w.StudentId == student.Id).ToListAsync();

            var installmentList = new List<InstallmentPayModel>();
            foreach (var install in installments)
            {
                var payterm = payTerms.FirstOrDefault(p => p.Id == install.PayTermId);
                var payrow = student.Payments.FirstOrDefault(x => x.InstallmentId == install.Id);

                installmentList.Add(new InstallmentPayModel
                {
                    Id = install.Id,
                    DueDate = install.DueDate,
                    DueAmount = install.Amount,
                    Title = payterm.Title,
                    IsPaid = payrow == null ? false : true,
                    PaymentDate = payrow != null ? payrow.CreatedOn : null,
                    PaidAmount = payrow != null ? payrow.Amount : null,
                });
            }

            ViewData["Row"] = student;
            ViewData["Installment"] = installmentList;
            return View();
        }


        public async Task<IActionResult> Reciept(int id)
        {
            var row = await
              _dbContext.Students
              .Include(x => x.Batch)
              .Include(x => x.Batch.Fee)
              .Include(x => x.Payments)
              .Include("Payments.Installment")
              .Include("Payments.Installment.PayTerm")
              .Where(x => x.Id == id).AsNoTracking().FirstOrDefaultAsync();

            var random = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 9).ToUpper() + "-" + UserId;
            var path = Path.Combine(_hostingEnvironment.WebRootPath, "barcodes", $"{random}.png");
            GenerateBarCode(random, path);
            await _dbContext.BarCodes.AddAsync(new BarCode
            {
                BarCodeValue = random,
                CreatedOn = DateTime.Now,
                StudentId = row.Id,
            });

            ViewData["Row"] = row;
            ViewData["BarCode"] = $"/barcodes/{random}.png";

            return View();
        }

        public async Task<IActionResult> Paid(int id, int installmentId)
        {
            var row = await
              _dbContext.Students
              .Include(x => x.Batch)
              .Include(x => x.Batch.Fee)
              .Include(x => x.Payments)
              .Include("Payments.Installment")
              .Include("Payments.Installment.PayTerm")
              .Where(x => x.Id == id).AsNoTracking().FirstOrDefaultAsync();

            var random = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 9).ToUpper() + "-" + UserId;
            var path = Path.Combine(_hostingEnvironment.WebRootPath, "barcodes", $"{random}.png");
            GenerateBarCode(random, path);
            await _dbContext.BarCodes.AddAsync(new BarCode
            {
                BarCodeValue = random,
                CreatedOn = DateTime.Now,
                StudentId = row.Id,
            });

            var payment = row.Payments.Where(x => x.InstallmentId == installmentId).FirstOrDefault();

            ViewData["Row"] = row;
            ViewData["PaymentRow"] = payment;

            ViewData["BarCode"] = $"/barcodes/{random}.png";

            ViewData["UserName"] = UserFullName;

            return View();
        }
        private void GenerateBarCode(string rndm, string path)
        {
            var barcode = QrEncoder.Encode(rndm, ErrorCorrectionLevel.M, Encoding.Unicode);
            var renderer = new ImageRenderer(new ImageRendererOptions { ImageFormat = ImageFormat.Png });
            using (var stream = new FileStream($"{path}", FileMode.Create))
            {
                renderer.Render(barcode, stream);
            }
        }
    }
}
