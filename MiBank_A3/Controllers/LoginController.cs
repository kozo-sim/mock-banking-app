using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MiBank_A3.Models;
using MiBank_A3.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MiBank_A3.Controllers
{
    public class LoginController : Controller
    {
        private readonly MiBankContext _context;
        public LoginController(MiBankContext context)
        {
            _context = context;
        }

        // GET: LoginController
        public ActionResult Index()
        {
            ViewData["loginPage"] = true;
            return View();
        }

        // POST: LoginController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(LoginViewModel vm)
        {
            var customerId = await _context.Login(vm.Username, vm.Password);
            if (customerId == -1)
            {
                ModelState.AddModelError(nameof(vm.Password), "Username/password combination incorrect");
                //vm.Password = "";
                return View(vm);
            }

            HttpContext.Session.SetInt32(nameof(Customer.CustomerId), customerId);
            TempData["successMessage"] = "Logged in successfully.";
            return RedirectToAction("", "");
        }

        public ActionResult Logout()
        {
            TempData["successMessage"] = "Logged out successfully.";
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
    }
}
