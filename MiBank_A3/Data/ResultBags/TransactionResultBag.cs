using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiBank_A3.Models
{
    //used by Account to return a transaction result along with 
    //transaction records to be stored in the database
    public class TransactionResultBag
    {
        public TransactionResultBag(TransactionResult r, Transaction t = null, Transaction s = null)
        {
            transactionResult = r;
            transaction = t;
            serviceCharge = s;
        }
        public TransactionResult transactionResult { get; }
        public Transaction transaction { get; }
        public Transaction serviceCharge { get; }
    }
}
