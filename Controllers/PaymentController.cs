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
        private readonly RefundService _refund;

        public PaymentController(BillService bill, PaymentService payment, RefundService refund)
        {
            _bill = bill;
            _refund = refund;
            _payment = payment;
        }

        // =========================================================
        // RECEIPT - Payment received from patient against a Bill
        // =========================================================

        [HttpGet]
        public IActionResult Pay(int id)
        {
            Bill bill = _bill.Get(id);

            if (bill == null)
                return NotFound();

            bill.Payments = _payment.GetPaymentsByBill(id);
            bill.TotalPaidAmount = _payment.GetTotalPaid(id);

            PaymentCreateVM vm = new PaymentCreateVM(bill);

            return View("Payment", vm);
        }


        // =========================================================
        // REFUND - Payment made to patient against a Refund
        // =========================================================

        [HttpGet]
        public IActionResult Refund(int id)
        {
            Refund refund = _refund.Get(id);

            if (refund == null)
                return NotFound();

            refund.Payments = _payment.GetPaymentsByRefund(id);
            refund.RefundedAmount = _payment.GetTotalRefunded(id);

            PaymentCreateVM vm = new PaymentCreateVM(refund);

            return View("Payment", vm);
        }

        // =========================================================
        // CREATE PAYMENT
        // Handles both Receipt and Refund
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(PaymentCreateVM vm)
        {
            if (!ModelState.IsValid)
                return View("Payment", vm);

            _payment.Create(vm.Payment);

            if (vm.Payment.PaymentType == PaymentType.Receipt)
            {
                return RedirectToAction("Pay", new { id = vm.Payment.BillId });
            }

            if (vm.Payment.PaymentType == PaymentType.Refund)
            {
                return RedirectToAction("Refund", new { id = vm.Payment.RefundId });
            }

            return RedirectToAction("Index");
        }

    }
}