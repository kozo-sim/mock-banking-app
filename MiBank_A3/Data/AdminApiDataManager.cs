using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using MiBank_A3.Attributes;
using MiBank_A3.Data;
using MiBank_A3.Data.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MiBank_A3.Models.Repository
{
    public class AdminApiDataManager : IAdminApiRepository
    {
        private readonly MiBankContextWrapper _context;
        public AdminApiDataManager(MiBankContext context)
        {
            _context = new MiBankContextWrapper(context);
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

        public async Task<Customer> SetCustomerDetails(Customer cust)
        {
            var c = await _context.GetCustomer(cust.CustomerId);
            _context.UpdateCustomer(c);
            return c;
        }

        public bool LockCustomerAccount(int customerId)
        {
            return _context.LockCustomerAccount(customerId);
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
            _context.UpdateBill(custId, bill);
            return true;
        }
    }
}
