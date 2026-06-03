using Microsoft.AspNetCore.Mvc;
using DAL;
using BLL;
using Models;
using ViewModels;
using Helpers;

namespace Controllers
{
    public class PaymentController : Controller
    {
        private readonly BillService _bill;
        private readonly PaymentService _payment;

        public PaymentController(BillService bill, PaymentService payment)
        {
            _bill = bill;
            _payment = payment;
        }

        [HttpGet]
        public IActionResult Pay(int id)
        {
            Bill bill = _bill.Get(id);

            bill.Payments = _payment.GetPaymentsByBill(id);

            bill.TotalPaidAmount = _payment.GetTotalPaid(id);

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
            _payment.Create(vm.Payment);

            return RedirectToAction("Pay", new { billId = vm.Payment.BillId });
        }
    }
}