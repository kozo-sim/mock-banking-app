using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MiBank_A3.Models
{

    public enum BillingPeriod
    {
        Minutes = 'M',
        Quarterly = 'Q',
        Annually = 'A',
        Once_Off = 'S'
    }

    public enum BillPayResult
    {
        OK,
        FAIL_NEGATIVE,
        FAIL_FORMAT,
        FAIL_DATE,
    }

    public enum ScheduledBillPayResult
    {
        OK_PAID,
        OK_NOT_PAID,
        FAIL_NOT_ENOUGH,
    }

    public class BillPay
    {
        //properties

        [Display(Name = "BillPay Id")]
        public int BillPayId { get; set; }

        [Display(Name = "Source Account")]
        [Required]
        public int AccountId { get; set; }

        [Display(Name = "Payee")]
        [Required]
        public int PayeeId { get; set; }

        [Column(TypeName = "decimal(20,2)")]
        [Required]
        public decimal Amount { get; set; }

        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]
        [Required]
        public DateTime ScheduleDate { get; set; }

        [Display(Name = "Billing Period")]
        [Required]
        public BillingPeriod Period { get; set; }

        [Display(Name = "Last Charged On")]
        public DateTime ModifyDate { get; set; }


        //navigation properites

        public Account Account { get; set; }
        public Payee Payee { get; set; }


        //methods

        public BillPayResult SchedulePayment()
        {
            //New payments can't start in the past
            //if (BillPayId == 0 && ScheduleDate < DateTime.Now)
            //{
            //    return BillPayResult.FAIL_DATE;
            //}

            if (Amount <= 0)
            {
                return BillPayResult.FAIL_NEGATIVE;
            }

            return BillPayResult.OK;
        }



        

        //called by the system once a minute when a payment should be attempted
        public BillPayResultBag doScheduledPayment()
        {
            ModifyDate = roundDown(ModifyDate);
            ScheduleDate = roundDown(ScheduleDate);
            var now = roundDown(DateTime.Now);
            if (ScheduleDate > now)
            {
                return new BillPayResultBag(ScheduledBillPayResult.OK_NOT_PAID);
            }

            DateTime nextDue = ScheduleDate.AddDays(0);

            switch (Period)
            {
                case BillingPeriod.Minutes:
                    nextDue = now;
                    break;
                case BillingPeriod.Annually:
                    while (nextDue < now)
                    {
                        nextDue = nextDue.AddYears(1);
                    }
                    break;
                case BillingPeriod.Quarterly:
                    while (nextDue < now)
                    {
                        nextDue = nextDue.AddMonths(3);
                    }
                    break;
                case BillingPeriod.Once_Off:
                    break;
            }

            if(nextDue != now)
            {
                return new BillPayResultBag(ScheduledBillPayResult.OK_NOT_PAID);
            }

            Transaction transaction;
            //attempt payment
            if (Account.Balance - Amount < Account.minimumBalance())
            {
                transaction = new Transaction
                {
                    TransactionType = TransactionType.BillPay,
                    AccountId = AccountId,
                    Amount = 0,
                    TransactionTime = DateTime.Now,
                    //hijack unused comment field to store amount to display in the error message
                    Comment = Amount.ToString() 
                };
                return new BillPayResultBag(
                    ScheduledBillPayResult.FAIL_NOT_ENOUGH, 
                    transaction
                    );
            }

            //commit payment
            transaction = new Transaction
            {
                TransactionType = TransactionType.BillPay,
                AccountId = AccountId,
                Amount = Amount,
                TransactionTime = DateTime.Now
            };


            ModifyDate = DateTime.Now;
            Account.Balance -= Amount;
            return new BillPayResultBag(
                ScheduledBillPayResult.OK_PAID,
                transaction
                );
        }

        readonly TimeSpan _minute = new TimeSpan(hours: 0, minutes: 1, seconds: 0);
        private DateTime roundDown(DateTime date)
        {
            return date.AddTicks((date.Ticks % _minute.Ticks) * -1);
        }


    }
}
