using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace T3
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    // this is obsolet
    public class TenantInjector
    {
        private readonly RequestDelegate _next;

        public TenantInjector(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            //var tenant = string.Empty;
            //ClaimsPrincipal principal = httpContext.User as ClaimsPrincipal;
            //if (null != principal)
            //{
            //    foreach (Claim claim in principal.Claims)
            //    {
            //        tenant += claim.Value;

            //        //Response.Write("CLAIM TYPE: " + claim.Type + "; CLAIM VALUE: " + claim.Value + "</br>");
            //    }
            //}
            //if (principal.Claims.Count() != 0)
            //    httpContext.Items.Add("CURRENT_TENANT", httpContext.User?.Claims.First().ToString());
            //else
            //    httpContext.Items.Add("CURRENT_TENANT", httpContext.Request.Host.Host);


            //if (httpContext.Request.Host.Host.Contains("."))
            //{
            //    tenant = httpContext.Request.Host.Host.Split('.')[0].ToLowerInvariant();
            //    tenant = httpContext.User?.Claims.First().ToString(); 
            //    httpContext.Items.Add("CURRENT_TENANT", tenant);


            //}
            //else
            //{
            //    //httpContext.Items.Add("CURRENT_TENANT", httpContext.Request.Host.Host);
            //    httpContext.Items.Add("CURRENT_TENANT", httpContext.User?.Claims.First().ToString());
            //}
            await _next(httpContext);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class TenantInjectorExtensions
    {
        public static IApplicationBuilder UseTenantInjector(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<TenantInjector>();
        }
    }
}
