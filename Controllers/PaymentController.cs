using Microsoft.AspNetCore.Mvc;
using DAL;
using BLL;
using Models;
using ViewModels;

namespace Controllers
{
    public class PaymentController : Controller
    {
        private readonly BillService _bill;
        private readonly PaymentService _payment;
        private readonly SessionHelper _session;


        public PaymentController(IConfiguration config, SessionHelper session)
        {
            _bill = new BillService(config);
            _payment = new PaymentService(config);
            _session = session;
        }

        [HttpGet]
        public IActionResult Pay(int id)
        {
            Bill bill = _bill.Get(id);

            bill.Payments = _payment.GetPaymentsByBill(id);

            bill.TotalPaidAmount = _payment.GetTotalPaid(id);

            bill.BalanceAmount = bill.GrossAmount - bill.TotalPaidAmount;

            PaymentCreateVM vm = new PaymentCreateVM
            {
                Bill = bill,
                Payment = new Payment
                {
                    BillId = id,
                    Amount = bill.BalanceAmount ?? 0
                }
            };

            return View("Payment", vm);
        }

        [HttpPost]
        public IActionResult Pay(PaymentCreateVM vm)
        {
            _payment.Add(vm.Payment);

            return RedirectToAction("Pay", new { billId = vm.Payment.BillId });
        }
    }
}