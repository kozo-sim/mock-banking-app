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
            var res = await adminRepository.TransactionHistory(customerId);
            SetBad(res);
            return res;
        }

        //GET /api/user/1
        [HttpGet("user/{id}")]
        public async Task<Customer> GetCustomerDetails(int id)
        {
            var res = await adminRepository.GetCustomerDetails(id);
            SetBad(res);
            return res;
        }

        //POST /api/user/1
        [HttpPost("user/{id}")]
        public string SetCustomerDetails([FromBody] Customer cust)
        {
            var res = adminRepository.SetCustomerDetails(cust);
            return SetBad(res, $"updated customer {cust.CustomerId}");
        }

        //POST /api/lock/1
        [HttpPost("lock/{id}")]
        public string LockBillPay(int billPayId)
        {
            var res = adminRepository.LockCustomerAccount(billPayId);
            return SetBad(res, $"locked {billPayId}");
        }

        //GET /api/bills
        [HttpGet("bills")]
        public List<BillPay> GetBills()
        {
            var res = adminRepository.GetBills();
            if (SetBad(res))
            {
                return null;
            }
            return res;
        }

        //GET /api/bills/1
        [HttpGet("bills/{customerId}")]
        public List<BillPay> GetBills(int customerId)
        {
            var res = adminRepository.GetBills(customerId);
            if (SetBad(res))
            {
                return null;
            }
            return res;
        }

        //POST /api/bill/1/1/block
        [HttpPost("bill/{custid}/{billPayId}/block")]
        public async Task<string> BlockBillPay(int custid, int billPayId)
        {
            var found = await adminRepository.SetBillBlocking(custid, billPayId, true);
            return SetBad(found, $"blocked bill {billPayId}");
        }

        //POST /api/bill/1/1/unblock
        [HttpPost("bill/{custid}/{billPayId}/unblock")]
        public async Task<string> UnblockBillPay(int custid, int billPayId)
        {
            var found = await adminRepository.SetBillBlocking(custid, billPayId, false);
            return SetBad(found, $"unblocked bill {billPayId}");
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

        private bool SetBad(Object o)
        {
            if(o == null)
            {
                BadRequest();
                return true;
            }
            return false;
        }

        private string SetBad(Object o, string message)
        {
            if(o.GetType() == typeof(bool))
            {
                if((bool)o == false)
                {
                    BadRequest();
                    return "bad request";
                }
            }
            if (o == null)
            {
                BadRequest();
                return "bad request";
            }
            return message;
        }
    }
}
