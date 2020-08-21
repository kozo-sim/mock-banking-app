﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MiBank_A3.Models.Repository
{
    [Route("api")]
    [ApiController]
    [SkipStatusCodePages]
    public class AdminRepository : ControllerBase
    {
        private readonly MiBankContext _context;
        public AdminRepository(MiBankContext context)
        {
            _context = context;
        }

        [HttpGet("users")]
        public List<Customer> GetUsers()
        {
            return _context.GetAllCustomers();
        }

        [HttpGet("transactions/{id}")]
        public async Task<List<Transaction>> TransactionHistory(int id)
        {
            if (!IsLoggedIn())
            {
                return null;
            }
            var c = await _context.GetCustomer(id);
            if (c == null)
            {
                HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return null;
            }
            return _context.GetAllCustomerTransactions(id);
        }

        [HttpGet("user/{id}")]
        public async Task<Customer> GetCustomerDetails(int id)
        {
            if (!IsLoggedIn())
            {
                return null;
            }
            return await _context.GetCustomer(id);
        }


        [HttpPost("user/{id}")]
        public async Task<string> SetCustomerDetails([FromBody] Customer cust)
        {
            if (!IsLoggedIn())
            {
                return "not logged in";
            }

            var c = await _context.GetCustomer(cust.CustomerId);
            _context.Update<Customer>(c);
            return $"updated customer {cust.CustomerId}";
        }

        [HttpPost("lock/{id}")]
        public string LockCustomer(int customerId)
        {
            _context.lockAccount(customerId);
            return $"locked {customerId}";
        }


        [HttpGet("bills")]
        public List<BillPay> getBills()
        {
            return _context.GetAllBills();
        }

        [HttpGet("bills/{id}")]
        public List<BillPay> getBills(int id)
        {
            return _context.GetBills(id);
        }


        [HttpPost("block/{custid}/{id}")]
        public async Task<string> BlockBillPay(int custid,int id)
        {
            var bill = await _context.GetBill(custid, id);
            if(bill == null)
            {
                return "request parameters invalid";
            }
            bill.Blocked = true;
            _context.Update(bill);
            return $"blocked bill {id}";
        }

        [HttpPost("unblock/{custid}/{id}")]
        public async Task<string> UnblockBillPay(int custid, int id)
        {
            var bill = await _context.GetBill(custid, id);
            if (bill == null)
            {
                return "request parameters invalid";
            }
            bill.Blocked = false;
            _context.Update(bill);
            return $"unblocked bill {id}";
        }



        [HttpPost("login")]
        public string Login(string username, string password)
        {
            //stub
            if(username == "admin" && password == "admin")
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



        private bool IsLoggedIn()
        {
            if (HttpContext.Session.GetString("login") == "true")
            {
                return true;
            }
            HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            return false;
        }
    }
}
