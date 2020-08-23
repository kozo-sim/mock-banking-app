using System;
using System.Collections.Generic;
using System.Linq;
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

        [HttpGet("users")]
        public List<Customer> GetUsers()
        {
            return adminRepository.GetUsers();
        }

        [HttpGet("transactions/{id}")]
        public async Task<List<Transaction>> TransactionHistory(int id)
        {
            return await adminRepository.TransactionHistory(id);
        }

        [HttpGet("user/{id}")]
        public async Task<Customer> GetCustomerDetails(int id)
        {
            return await adminRepository.GetCustomerDetails(id);
        }

        [HttpPost("user/{id}")]
        public string SetCustomerDetails([FromBody] Customer cust)
        {
            adminRepository.SetCustomerDetails(cust);
            return $"updated customer {cust.CustomerId}";
        }

        [HttpPost("lock/{id}")]
        public string LockCustomer(int customerId)
        {
            adminRepository.LockCustomer(customerId);
            return $"locked {customerId}";
        }

        [HttpGet("bills")]
        public List<BillPay> GetBills()
        {
            return adminRepository.GetBills();
        }

        [HttpGet("bills/{id}")]
        public List<BillPay> GetBills(int id)
        {
            return adminRepository.GetBills(id);
        }

        [HttpPost("block/{custid}/{id}")]
        public async Task<string> BlockBillPay(int custid, int id)
        {
            var found = await adminRepository.SetBillBlocking(custid, id, true);
            if (!found)
            {
                return "request parameters invalid";
            }
            return $"blocked bill {id}";
        }

        [HttpPost("unblock/{custid}/{id}")]
        public async Task<string> UnblockBillPay(int custid, int id)
        {
            var found = await adminRepository.SetBillBlocking(custid, id, false);
            if (!found)
            {
                return "request parameters invalid";
            }
            return $"unblocked bill {id}";
        }

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

        [HttpPost("logout")]
        public string Logout()
        {
            HttpContext.Session.SetString("login", "false");
            return "ok";
        }
    }
}
