using MiBank_A3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiBank_A3.ViewModels
{
    public class BillListViewModel
    {
        public List<BillPay> Bills { get; set; }
        public int prevPage { get; set; }
        public int currentPage { get; set; }
        public int nextPage { get; set; }
    }
}
