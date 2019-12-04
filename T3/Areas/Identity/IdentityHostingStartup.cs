using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using T3.Areas.Identity.Data;
using T3.Data;

[assembly: HostingStartup(typeof(T3.Areas.Identity.IdentityHostingStartup))]
namespace T3.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) =>
            {
    //            services.AddDefaultIdentity<AppUser>()
    //.AddEntityFrameworkStores<ApplicationDbContext>();
            });

        }
    }
}