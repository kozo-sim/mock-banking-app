using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MiBank_A3.Models;
using Microsoft.AspNetCore.Mvc;

namespace MiBank_A3.Controllers
{
    public class ErrorController : Controller
    {
        [HttpGet("/error")]
        public IActionResult Error(int? httpCode)
        {
            //correctly set status code for browser/client
            if (httpCode.HasValue)
            {
                HttpContext.Response.StatusCode = httpCode.Value;
            }
            else
            {
                //give a generic server error
                httpCode = 500;
            }
            return View(new ErrorViewModel(httpCode));
        }
    }
}
