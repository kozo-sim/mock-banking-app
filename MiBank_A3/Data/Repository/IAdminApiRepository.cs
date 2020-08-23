using MiBank_A3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiBank_A3.Data.Repository
{
    public interface IAdminApiRepository
    {
        public List<Customer> GetUsers();
        public Task<List<Transaction>> TransactionHistory(int id);
        public Task<Customer> GetCustomerDetails(int id);
        public Task<Customer> SetCustomerDetails(Customer cust);
        public bool LockCustomerAccount(int customerId);
        public List<BillPay> GetBills();
        public List<BillPay> GetBills(int id);
        public Task<bool> SetBillBlocking(int custId, int id, bool block);
    }
}
