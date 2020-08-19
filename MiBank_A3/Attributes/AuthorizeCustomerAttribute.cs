using MiBank_A3.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiBank_A3.Attributes
{
    public class AuthorizeCustomerAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var customerId = context.HttpContext.Session.GetInt32(nameof(Customer.CustomerId));
            if (!customerId.HasValue)
            {
                context.Result = new RedirectToActionResult("Index", "Login", null);
            }
        }
    }
}
