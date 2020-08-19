using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MiBank_A3.Models;
using MiBank_A3.ViewModels;
using MiBank_A3.Attributes;
using System.Security.Cryptography.X509Certificates;
using MiBank_A3.Views;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace MiBank_A3.Controllers
{
    [AuthorizeCustomer]
    public class HomeController : Controller
    {
        
        
        private readonly MiBankContext _context;

        public int CustomerId => HttpContext.Session.GetInt32(nameof(Customer.CustomerId)).Value;



        public HomeController(MiBankContext context)
        {
            _context = context;
        }

        //List accounts that each customer has
        //also provide links to deposit/withdraw/transfer functions
        public async Task<IActionResult> Accounts()
        {
            var cust = await _context.GetCustomerWithAccounts(CustomerId);
            
            return View(cust);
        }

        public async Task<IActionResult> ATM()
        {
            var vm = new ATMViewModel();
            vm.Customer = await _context.GetCustomerWithAccounts(CustomerId);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ATM(ATMViewModel vm)
        {
            vm.Account = await _context.GetAccountWithTransactions(CustomerId, vm.AccountId);

            if(vm.Account == null)
            {
                ModelState.AddModelError("AccountInvalid", "Account not found");
                goto ErrorCleanup;
            }

            switch (vm.TransactionType)
            {
                case TransactionType.Deposit:
                    var depositResult = vm.doDeposit();
                    switch (depositResult.transactionResult)
                    {
                        case TransactionResult.OK:
                            _context.BankTransactions.Add(depositResult.transaction);
                            await _context.SaveChangesAsync();
                            break;
                        case TransactionResult.FAIL_BELOW_ZERO:
                            ModelState.AddModelError(nameof(vm.Amount), "Amount must be above 0");
                            goto ErrorCleanup;
                        case TransactionResult.FAIL_EXTRA_DIGITS:
                            ModelState.AddModelError(nameof(vm.Amount), "More than 2 decimal places used.");
                            goto ErrorCleanup;
                        default:
                            break;
                    }
                    break;

                case TransactionType.Withdrawal:
                    var withdrawResult = vm.doWithdraw();
                    switch (withdrawResult.transactionResult)
                    {
                        case TransactionResult.OK:
                            _context.BankTransactions.Add(withdrawResult.transaction);
                            await _context.SaveChangesAsync();
                            if (withdrawResult.serviceCharge != null)
                            {
                                //attach previous transactionId to unused Comment field
                                //this value will be formatted and printed out correctly by the view
                                withdrawResult.serviceCharge.Comment += withdrawResult.transaction.TransactionId;
                                _context.BankTransactions.Add(withdrawResult.serviceCharge);
                                await _context.SaveChangesAsync();
                            }
                            break;
                        case TransactionResult.FAIL_BELOW_ZERO:
                            ModelState.AddModelError(nameof(vm.Amount), "Amount must be above 0");
                            goto ErrorCleanup;
                        case TransactionResult.FAIL_INSUFFICIENT_FUNDS:
                            ModelState.AddModelError(nameof(vm.Amount), "Not enough money to withdraw.");
                            goto ErrorCleanup;
                        case TransactionResult.FAIL_EXTRA_DIGITS:
                            ModelState.AddModelError(nameof(vm.Amount), "More than 2 decimal places used.");
                            goto ErrorCleanup;
                        default:
                            break;
                    }
                    break;


                case TransactionType.Transfer:
                    vm.TargetAccount = await _context.GetAccount(CustomerId, vm.TargetAccountId);
                    if(vm.TargetAccount == null)
                    {
                        ModelState.AddModelError(nameof(vm.TargetAccountId), "Destination account must be set for transfers");
                        goto ErrorCleanup;
                    }
                    if (vm.AccountId == vm.TargetAccountId)
                    {
                        ModelState.AddModelError(nameof(vm.TargetAccountId), "Source and destination account cannot be the same");
                        goto ErrorCleanup;
                    }
                    var transferResult = vm.doTransfer();
                    switch (transferResult.transactionResult)
                    {
                        case TransactionResult.OK:
                            await _context.BankTransactions.AddAsync(transferResult.transaction);
                            await _context.SaveChangesAsync();
                            if (transferResult.serviceCharge != null)
                            {
                                //include previous transactionId in comment
                                transferResult.serviceCharge.Comment += transferResult.transaction.TransactionId;
                                _context.BankTransactions.Add(transferResult.serviceCharge);
                                await _context.SaveChangesAsync();
                            }
                            break;
                        case TransactionResult.FAIL_BELOW_ZERO:
                            ModelState.AddModelError(nameof(vm.Amount), "Amount must be above 0.");
                            goto ErrorCleanup;
                        case TransactionResult.FAIL_INSUFFICIENT_FUNDS:
                            ModelState.AddModelError(nameof(vm.Amount), "Not enough money to withdraw.");
                            goto ErrorCleanup;
                        case TransactionResult.FAIL_EXTRA_DIGITS:
                            ModelState.AddModelError(nameof(vm.Amount), "More than 2 decimal places used.");
                            goto ErrorCleanup;
                        default:
                            break;
                    }
                    break;

                default:
                    ModelState.AddModelError(nameof(vm.TransactionType), "Invalid transaction type specified");
                    break;

            }

            TempData["successMessage"] = "Transaction successful.";
            return RedirectToAction(nameof(Accounts));


        ErrorCleanup:
            //set fields that weren't included in the POST so we can redirect safely
            vm.Customer = await _context.GetCustomerWithAccounts(CustomerId);
            return View(vm);
        }


        public async Task<IActionResult> Statement(int id, int? page)
        {
            var vm = new StatementViewModel();
            vm.Account = await _context.GetAccount(CustomerId, id);
            if(vm.Account == null)
            {
                ModelState.AddModelError("NoAccount", "Account not found");
                return View();
            }

            var allTransactions = _context.GetAllTransactions(id)
                .OrderByDescending(t => t.TransactionId)
                .ToList();

            var paginated = new Paginator<Transaction>(allTransactions, page ?? 1);
            //go to first page if there was an error
            if (paginated.partialList == null)
            {
                return RedirectToAction(nameof(Statement), new { id = id, page = 1});
            }
            vm.Transactions = paginated.partialList;
            vm.CurrentPage = paginated.currentPage;
            vm.NextPage = paginated.nextPage;
            vm.PrevPage = paginated.prevPage;


            return View(vm);
        }

        public async Task<IActionResult> Profile()
        {
            var vm = new ProfileViewModel();
            vm.Customer = await _context.GetCustomer(CustomerId);
            vm.Username = _context.GetUsername(CustomerId);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(ProfileViewModel vm)
        {
            if (vm.Action == profilePostAction.UPDATE_USER)
            {
                vm.Customer.CustomerId = CustomerId;
                _context.Update(vm.Customer);
                await _context.SaveChangesAsync();
                TempData["successMessage"] = $"Updated user info.";
                return View(vm);
            }else if (vm.Action == profilePostAction.UPDATE_LOGIN)
            {
                vm.Customer = await _context.GetCustomer(CustomerId);
                if(vm.NewPassword != vm.NewPasswordConfirm)
                {
                    ModelState.AddModelError(nameof(vm.NewPasswordConfirm), "New passwords must match each other");
                    goto ErrorCleanup;
                }
                if(await _context.UpdateLogin(CustomerId, vm.OldPassword, vm.Username, vm.NewPassword) == false)
                {
                    ModelState.AddModelError(nameof(vm.NewPassword), "Wrong password entered");
                    goto ErrorCleanup;
                }
                //show success message
                if(vm.NewPassword != null && vm.NewPassword != "")
                {
                    TempData["successMessage"] = $"Updated username and password.";
                }
                else
                {
                    TempData["successMessage"] = $"Updated username.";
                }
                return View(vm);
            }
            
            ModelState.AddModelError("", "Invalid option in POST");
        ErrorCleanup:
            return View(vm);
        }



    }
}
