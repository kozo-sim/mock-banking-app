using MiBank_A3.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiBank_A3.ViewModels
{
    public enum ATMPageTransactionTypes
    {
        Deposit = TransactionType.Deposit,
        Withdrawal = TransactionType.Withdrawal,
        Transfer = TransactionType.Transfer
    }
    public class ATMViewModel
    {
        public Customer Customer { get; set; }
        public List<Account> CustomerAccounts { get; set; }

        public decimal Amount { get; set; }

        public TransactionType TransactionType { get; set; }
        
        public int AccountId { get; set; }
        public Account Account { get; set; }
        public int? TargetAccountId { get; set; }
        public Account TargetAccount { get; set; }

        public string Comment { get; set; }





        //shorthand functions used by the controller that pass appropriate parameters
        //from the viewModel into the Model's methods
        public TransactionResultBag doDeposit()
        {
            return Account.doDeposit(Amount, Comment);
        }
        public TransactionResultBag doWithdraw()
        {
            return Account.doWithdraw(Amount, Comment);
        }
        public TransactionResultBag doTransfer()
        {
            return Account.doTransfer(Amount, TargetAccount, Comment);
        }
    }
}
