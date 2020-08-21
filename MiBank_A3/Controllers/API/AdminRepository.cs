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
    public class AdminRepository : ControllerBase
    {
        private readonly MiBankContext _context;
        public AdminRepository(MiBankContext context)
        {
            _context = context;
        }

        [HttpGet("transactions/{id}")]
        public async Task<List<Transaction>> TransactionHistory(int id)
        {
            if (!IsLoggedIn())
            {
                return null;
            }
            var c = await _context.GetCustomer(id);
            if(c == null)
            {
                HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return null;
            }
            return _context.GetAllCustomerTransactions(id);
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
