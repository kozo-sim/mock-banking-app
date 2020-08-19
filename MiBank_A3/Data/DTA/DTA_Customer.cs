using System.Collections.Generic;

namespace MiBank_A3.Models
{
    public partial class SeedData
    {
        public class DTA_Customer
        {
            public int CustomerID { get; set; }
            public string Name { get; set; }
            public string Address { get; set; }
            public string City { get; set; }
            public string PostCode { get; set; }
            public List<DTA_Account> Accounts = new List<DTA_Account>();
        }




    }




}
