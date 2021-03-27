using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MiBank_A3.Data;
using MiBank_A3.Models;
using MiBank_A3.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MiBank_A3.Controllers
{
    public class LoginController : Controller
    {
        private readonly MiBankContextWrapper _context;
        public LoginController(MiBankContext context)
        {
            _context = new MiBankContextWrapper(context);
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
            switch (customerId)
            {
                case (int)loginResult.MAX_ATTEMPTS:
                    ModelState.AddModelError(nameof(vm.Password), "Maximum login attempts exceeded");
                    return View(vm);
                case (int)loginResult.NO_PASSWORD:
                    ModelState.AddModelError(nameof(vm.Password), "No password entered");
                    return View(vm);
                case (int)loginResult.NO_USER:
                    ModelState.AddModelError(nameof(vm.Username), "User not found");
                    return View(vm);
                case (int)loginResult.WRONG_PASSWORD:
                    ModelState.AddModelError(nameof(vm.Password), "Username/password combination incorrect");
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
