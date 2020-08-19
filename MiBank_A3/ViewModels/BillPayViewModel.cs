using MiBank_A3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiBank_A3.ViewModels
{
    public class BillPayViewModel
    {
        public BillPay Bill { get; set; }
        public Customer Customer { get; set; }
        public List<Payee> Payees { get; set; }

    }
}
