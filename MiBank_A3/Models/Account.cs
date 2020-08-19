using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace MiBank_A3.Models
{
    public class Account
    {
        public enum Type { Savings = 1, Checking = 2 }


        //properties
        [Display(Name = "Customer Id")]
        public int CustomerId { get; set; }

        [Display(Name = "Account Id")]
        [ForeignKey("CustomerId")]
        [Required]
        public int AccountId { get; set; }

        [Display(Name = "Account Type")]
        [Required]
        public Type AccountType { get; set; }
        
        [Column(TypeName = "decimal(20,2)")]
        [DataType(DataType.Currency)]
        [Required]
        public decimal Balance { get; set; }

        //navigation properties
        public List<BillPay> Bills { get; set; }
        public Customer Customer { get; set; }
        public List<Transaction> Transactions { get; set; }

        public List<Transaction> ReceivedTransactions { get; set; }


        private const decimal WITHDRAW_CHARGE = 0.10M;
        private const decimal TRANSFER_CHARGE = 0.20M;

        //methods
        public TransactionResultBag doDeposit(decimal amount, string comment)
        {
            var basicValidation = this.basicValidation(amount);
            if (basicValidation != TransactionResult.OK)
            {
                return new TransactionResultBag(basicValidation);
            }

            Balance += amount;

            var t = new Transaction
            {
                TransactionType = TransactionType.Deposit,
                AccountId = this.AccountId,
                Amount = amount,
                TransactionTime = DateTime.Now,
                Comment = comment
            };

            return new TransactionResultBag(TransactionResult.OK, t);
        }

        public TransactionResultBag doWithdraw(decimal amount, string comment)
        {
            var basicValidation = this.basicValidation(amount);
            if (basicValidation != TransactionResult.OK)
            {
                return new TransactionResultBag(basicValidation);
            }
            if (Balance - amount < minimumBalance())
            {
                return new TransactionResultBag(TransactionResult.FAIL_INSUFFICIENT_FUNDS);
            }

            var t = new Transaction
            {
                TransactionType = TransactionType.Withdrawal,
                AccountId = this.AccountId,
                Amount = amount,
                TransactionTime = DateTime.Now,
                Comment = comment
            };

            

            Transaction serviceCharge = null;
            if (!FreeTransaction())
            {
                if (Balance - amount - WITHDRAW_CHARGE < minimumBalance())
                {
                    return new TransactionResultBag(TransactionResult.FAIL_INSUFFICIENT_FUNDS);
                }
                serviceCharge = new Transaction
                {
                    TransactionType = TransactionType.Service_Charge,
                    AccountId = AccountId,
                    Amount = WITHDRAW_CHARGE,
                    TransactionTime = DateTime.Now
                };
                Balance -= WITHDRAW_CHARGE;
            }

            Balance -= amount;
            return new TransactionResultBag(TransactionResult.OK, t, serviceCharge);
        }

        public TransactionResultBag doTransfer(decimal amount, Account destination, string comment)
        {
            var basicValidation = this.basicValidation(amount);
            if (basicValidation != TransactionResult.OK)
            {
                return new TransactionResultBag(basicValidation);
            }
            if (Balance - amount < minimumBalance())
            {
                return new TransactionResultBag(TransactionResult.FAIL_INSUFFICIENT_FUNDS);
            }

            var t = new Transaction
            {
                TransactionType = TransactionType.Transfer,
                AccountId = this.AccountId,
                Amount = amount,
                TransactionTime = DateTime.Now,
                TransferTargetId = destination.AccountId,
                Comment = comment
            };

            Transaction serviceCharge = null;
            if (!FreeTransaction())
            {
                if(Balance - amount - TRANSFER_CHARGE < minimumBalance())
                {
                    return new TransactionResultBag(TransactionResult.FAIL_INSUFFICIENT_FUNDS);
                }
                serviceCharge = new Transaction
                {
                    TransactionType = TransactionType.Service_Charge,
                    AccountId = AccountId,
                    Amount = TRANSFER_CHARGE,
                    TransactionTime = DateTime.Now,
                    Comment = "For transaction #"
                };
                Balance -= TRANSFER_CHARGE;
            }

            Balance -= amount;
            destination.Balance += amount;

            return new TransactionResultBag(TransactionResult.OK, t, serviceCharge);
        }

        private TransactionResult basicValidation(decimal amount)
        {
            if (amount <= 0)
            {
                return TransactionResult.FAIL_BELOW_ZERO;
            }
            var parts = amount.ToString().Split(".");
            if (parts.Length == 2)
            {
                if(parts[1].Length > 2)
                {
                    return TransactionResult.FAIL_EXTRA_DIGITS;
                }
            }
            return TransactionResult.OK;
        }


        private const int MAX_TRANSACTIONS = 4;
        public bool FreeTransaction()
        {
            int paidTransactions = Transactions.Where(
                t => t.TransactionType == TransactionType.Withdrawal
                || t.TransactionType == TransactionType.Transfer)
                .ToList().Count;
            if (paidTransactions <= MAX_TRANSACTIONS)
            {
                return false;
            }
            return true;
        }

        public decimal minimumBalance()
        {
            switch (AccountType)
            {
                case Type.Savings:
                    return 0;
                case Type.Checking:
                    return 50;
            }
            throw new ArgumentException("Switch not populated correctly");
        }

    }






    public enum TransactionResult
    {
        OK,
        FAIL_INSUFFICIENT_FUNDS,
        FAIL_BELOW_ZERO,
        FAIL_EXTRA_DIGITS
    }
}
