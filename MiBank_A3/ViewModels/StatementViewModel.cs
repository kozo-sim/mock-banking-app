using MiBank_A3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiBank_A3.ViewModels
{
    public class StatementViewModel
    {
        public List<Transaction> Transactions { get; set; }
        public List<Transaction> ReceivedTransactions { get; set; }
        public Account Account { get; set; }
        public int PrevPage { get; set; }
        public int CurrentPage { get; set; }
        public int NextPage { get; set; }
    }
}
