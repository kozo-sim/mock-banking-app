using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiBank_A3.Models
{
    public class BillPayResultBag
    {
        public BillPayResultBag(ScheduledBillPayResult r, Transaction t = null, Transaction s = null)
        {
            transactionResult = r;
            transaction = t;
            serviceCharge = s;
        }
        public ScheduledBillPayResult transactionResult { get; }
        public Transaction transaction { get; }
        public Transaction serviceCharge { get; }
    }
}
