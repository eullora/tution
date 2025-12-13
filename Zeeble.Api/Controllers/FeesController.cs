using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zeeble.Shared.Helpers;
using Zeeble.Shared.Models;

namespace Zeeble.Api.Controllers
{
	[ApiController]
	[Route("api/fees")]
	public class FeesController : ControllerBase
	{
        private readonly WebApiDBContext _dbContext;        
        public FeesController(WebApiDBContext dbContext)
        {
            _dbContext = dbContext;            
        }

        [Route("{studentId:int}")]
        public async Task<IActionResult> Get(int studentId)
        {            
            var student = await _dbContext.Students.Include(x => x.Batch)
                .Include(m => m.Batch.Fee)
                .Include(m => m.Installments)
                .Include("Installments.PayTerm")
                .Include(t => t.Payments)
                .Where(x => x.Id == studentId).FirstOrDefaultAsync();

            var payments = new List<InstallmentPayModel>();
            foreach (var install in student.Installments)
            {
                var payment = new InstallmentPayModel();
                payment.Title = install.PayTerm.Title;
                payment.DueDate = install.DueDate;
                payment.DueAmount= install.Amount;                
                var paid = student.Payments.Where(x => x.InstallmentId == install.Id).FirstOrDefault();
                //if (paid != null)
                //{
                //    payment.PaymentDate = paid.CreatedOn;
                //    payment.PaidAmount = paid.Amount;
                //    payment.IsPaid = true;
                //}

                if (paid != null)
                {
                    continue;
                }

                payments.Add(payment);
            }

            var nextPayment = payments.OrderBy(x => x.DueDate).FirstOrDefault();
            payments.Clear();
            payments.Add(nextPayment);  

            var result = new
            {
                student.FullName,
                BatchName = student.Batch.Title,
                student.Discount,
                TotalFee = student.Batch.Fee.Amount,                
                Payments = payments,
                PaidAmount = student.Payments.Sum(a => a.Amount),                
                Balance = student.Batch.Fee.Amount - student.Payments.Sum(a => a.Amount) - student.Discount
            };

            return Ok(result);

        }
    }
}
