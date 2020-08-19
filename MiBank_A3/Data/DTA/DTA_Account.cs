using System.Collections.Generic;

namespace MiBank_A3.Models
{
    public partial class SeedData
    {
        public class DTA_Account
        {
            public int AccountNumber { get; set; }
            public char AccountType { get; set; }
            public decimal Balance { get; set; }
            public List<DTA_Transaction> Transactions = new List<DTA_Transaction>();

        }




    }




}
