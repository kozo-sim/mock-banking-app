using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace MiBank_A3.Attributes
{
    public class AuthorizeAdminAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (context.HttpContext.Session.GetString("login") != "true")
            {
                context.Result = new ContentResult()
                {
                    Content = "unauthorised",
                    StatusCode = (int)HttpStatusCode.Unauthorized
                };
            }
        }
    }
}
