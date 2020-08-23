using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MiBank_A3.Controllers.API
{
    [Route("api")]
    [ApiController]
    [SkipStatusCodePages]
    public class AdminApiLoginController : ControllerBase
    {
        //NOTE;
        //this controller shares the same route as the main API controller
        //beware of conflicts


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
