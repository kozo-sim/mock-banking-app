using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using MiBank_A3.Attributes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MiBank_A3.Models.Repository
{
    [AuthorizeAdmin]
    public class AdminRepository
    {
        private readonly MiBankContext _context;
        public AdminRepository(MiBankContext context)
        {
            _context = context;
        }

        public List<Customer> GetUsers()
        {
            return _context.GetAllCustomers();
        }

        public async Task<List<Transaction>> TransactionHistory(int id)
        {
            var c = await _context.GetCustomer(id);
            return _context.GetAllCustomerTransactions(id);
        }

        public async Task<Customer> GetCustomerDetails(int id)
        {
            return await _context.GetCustomer(id);
        }

        public async void SetCustomerDetails([FromBody] Customer cust)
        {
            var c = await _context.GetCustomer(cust.CustomerId);
            _context.Update<Customer>(c);
        }

        public void LockCustomer(int customerId)
        {
            _context.lockAccount(customerId);
        }


        public List<BillPay> GetBills()
        {
            return _context.GetAllBills();
        }

        public List<BillPay> GetBills(int id)
        {
            return _context.GetBills(id);
        }
        public async Task<bool> SetBillBlocking(int custId, int id, bool block)
        {
            var bill = await _context.GetBill(custId, id);
            if(bill == null)
            {
                return false;
            }
            bill.Blocked = block;
            _context.Update(bill);
            return true;
        }
    }
}
