using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MiBank_A3.Attributes;
using MiBank_A3.Data;
using MiBank_A3.Models;
using MiBank_A3.ViewModels;
using MiBank_A3.Views;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;

namespace MiBank_A3.Controllers
{
    [AuthorizeCustomer]
    public class BillsController : Controller
    {
        private readonly MiBankContextWrapper _context;
        private int CustomerId => HttpContext.Session.GetInt32(nameof(Customer.CustomerId)).Value;

        public BillsController(MiBankContext context)
        {
            _context = new MiBankContextWrapper(context);
        }


        const int BILL_PAGINATION_SIZE = 4;


        public IActionResult Index(int? id)
        {
            var vm = new BillListViewModel();
            vm.Bills = _context.GetBills(CustomerId);
            if(vm.Bills.Count == 0)
            {
                return View(vm);
            }
            var paginated = new Paginator<BillPay>(vm.Bills, id ?? 1);
            //go to first page if there was an error
            if(paginated.partialList == null)
            {
                return RedirectToAction(nameof(Index), new { id = 1 });
            }
            vm.Bills = paginated.partialList;
            vm.currentPage = paginated.currentPage;
            vm.nextPage = paginated.nextPage;
            vm.prevPage = paginated.prevPage;
            return View(vm);
        }

        public async Task<IActionResult> Pay(int? id)
        {
            BillPayViewModel vm;
            vm = new BillPayViewModel();
            if (id != null)
            {
                vm.Bill = await _context.GetBill(CustomerId,id);
            }
            else
            {
                vm.Bill = new BillPay();
            }
            vm.Payees = _context.GetAllPayees();
            vm.Customer = await _context.GetCustomerWithAccounts(CustomerId);

            return View(vm);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Pay(BillPayViewModel vm)
        {
            bool update = vm.Bill.BillPayId == 0 ? false : true;
            vm.Bill.Account = await _context.GetAccount(CustomerId, vm.Bill.AccountId);
            vm.Bill.Payee = await _context.GetPayee(vm.Bill.PayeeId);

            switch (vm.Bill.SchedulePayment())
            {
                case BillPayResult.FAIL_DATE:
                    ModelState.AddModelError($"Bill.{nameof(BillPay.ScheduleDate)}", "Date cannot be in the past");
                    goto ErrorCleanup;
                case BillPayResult.FAIL_NEGATIVE:
                    ModelState.AddModelError($"Bill.{nameof(BillPay.Amount)}", "Amount must be above 0");
                    goto ErrorCleanup;
                case BillPayResult.FAIL_FORMAT:
                    goto ErrorCleanup;
                case BillPayResult.OK:
                    if(update)
                    {
                        var oldBill = await _context.GetBill(vm.Bill.BillPayId, CustomerId);
                        //check we don't edit someone else's bill
                        if (oldBill == null)
                        {
                            ModelState.AddModelError($"Bill.{nameof(BillPay.BillPayId)}", "Illegal BillId in request");
                            goto ErrorCleanup;
                        }
                        //prevent 'Blocked' attribute from being set by users
                        vm.Bill.Blocked = oldBill.Blocked;
                        _context.UpdateBill(CustomerId, vm.Bill);
                    }
                    else
                    {
                        vm.Bill.Blocked = false;
                        _context.CreateBill(CustomerId, vm.Bill);
                    }
                    _context.SaveChangesAsync();
                    break;
            }
            if (update)
            {
                TempData["successMessage"] = $"Updated bill #{vm.Bill.BillPayId}.";
            }
            else
            {
                TempData["successMessage"] = $"Created bill #{vm.Bill.BillPayId}.";
            }
            
            return RedirectToAction(nameof(Index));
        ErrorCleanup:
            vm.Payees = _context.GetAllPayees();
            vm.Customer = await _context.GetCustomerWithAccounts(CustomerId);
            return View(vm);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var bill = await _context.GetBill(CustomerId,id);
            return View(bill);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(BillPay bill)
        {
            _context.DeleteBill(CustomerId, bill.BillPayId);
            TempData["successMessage"] = $"Deleted bill #{bill.BillPayId}.";
            return RedirectToAction("Index");
        }
    }
}
