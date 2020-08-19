using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MiBank_A3.Models
{


    //to keep the code simple,
    //values are stored on the database as the ascii code of their char value
    //e.g. DEPOSIT == 'D' == 68
    public enum TransactionType
    {
        Deposit = 'D',
        Withdrawal = 'W',
        Transfer = 'T',
        Service_Charge = 'S',
        BillPay = 'B'
    }

    public class Transaction
    {
        //properties

        [Display(Name = "Transaction Id")]
        public int TransactionId { get; set; }

        [Display(Name = "Account Id")]
        [Required]
        [ForeignKey("AccountId")]
        public int AccountId { get; set; }

        [Required]
        [Column(TypeName = "decimal(20,2)")]
        public decimal Amount { get; set; }

        [MaxLength(100)]
        public string Comment { get; set; }

        [Display(Name = "Transaction Time")]
        public DateTime TransactionTime { get; set; }

        [Display(Name = "Transaction Type")]
        [Required]
        public TransactionType TransactionType { get; set; }

        //navigation properties

        public Account Account { get; set; }
        public int? TransferTargetId { get; set; }
        public Account TransferTarget { get; set; }
    }
}
