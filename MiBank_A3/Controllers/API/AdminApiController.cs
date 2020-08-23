using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using MiBank_A3.Models;
using MiBank_A3.Models.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MiBank_A3.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminApiController : ControllerBase
    {
        private AdminRepository adminRepository;
        public AdminApiController(MiBankContext context)
        {
            this.adminRepository = new AdminRepository(context);
        }

        //GET /api/users
        [HttpGet("users")]
        public List<Customer> GetUsers()
        {
            return adminRepository.GetUsers();
        }

        //GET /api/transactions/1
        [HttpGet("transactions/{customerId}")]
        public async Task<List<Transaction>> TransactionHistory(int customerId)
        {
            return await adminRepository.TransactionHistory(customerId);
        }

        //GET /api/user/1
        [HttpGet("user/{id}")]
        public async Task<Customer> GetCustomerDetails(int id)
        {
            return await adminRepository.GetCustomerDetails(id);
        }

        //POST /api/user/1
        [HttpPost("user/{id}")]
        public string SetCustomerDetails([FromBody] Customer cust)
        {
            adminRepository.SetCustomerDetails(cust);
            return $"updated customer {cust.CustomerId}";
        }

        //POST /api/lock/1
        [HttpPost("lock/{id}")]
        public string LockBillPay(int billPayId)
        {
            adminRepository.LockBillPay(billPayId);
            return $"locked {billPayId}";
        }

        //GET /api/bills
        [HttpGet("bills")]
        public List<BillPay> GetBills()
        {
            return adminRepository.GetBills();
        }

        //GET /api/bills/1
        [HttpGet("bills/{customerId}")]
        public List<BillPay> GetBills(int customerId)
        {
            return adminRepository.GetBills(customerId);
        }

        //POST /api/bill/1/1/block
        [HttpPost("bill/{custid}/{billPayId}/block")]
        public async Task<string> BlockBillPay(int custid, int billPayId)
        {
            var found = await adminRepository.SetBillBlocking(custid, billPayId, true);
            if (!found)
            {
                return "request parameters invalid";
            }
            return $"blocked bill {billPayId}";
        }

        //POST /api/bill/1/1/unblock
        [HttpPost("bill/{custid}/{billPayId}/unblock")]
        public async Task<string> UnblockBillPay(int custid, int billPayId)
        {
            var found = await adminRepository.SetBillBlocking(custid, billPayId, false);
            if (!found)
            {
                return "request parameters invalid";
            }
            return $"unblocked bill {billPayId}";
        }

        //POST /login
        [HttpPost("login")]
        public string Login(string username, string password)
        {
            if (username == "admin" && password == "admin")
            {
                HttpContext.Session.SetString("login", "true");
                return "logged in successfully";
            }
            return "login failed";
        }

        //POST /logout
        [HttpPost("logout")]
        public string Logout()
        {
            HttpContext.Session.SetString("login", "false");
            return "ok";
        }

    }
}
