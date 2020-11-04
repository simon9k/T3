using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using T3.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using T3.Areas.Identity.Data;
using T3.Models;

namespace T3
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));

            //why AddUserManager here? cause we need tell system that is <AppUser> not <IdentityUser>
            //why AddRoleMananger not here? cause system is ready for <IdentityRole>
            //AddRoles should call first before AddEntityFrameWorkStores that is reasonable
            services.AddDefaultIdentity<AppUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>().AddUserManager<AppUserManager<AppUser>>();

            services.AddScoped<ITenantResolver, TenantResolver>();
            //Do we need interfaces for dependency injection?
            //      Does it work? Yes. Should you do it? No.
            //      The only benefit you get over simply creating instances with new Storage() is service lifetime management (transient vs. scoped vs. singleton)
            //      That's useful, but only part of the power of using DI. 
            //      As @DavidG pointed out, the big reason why interfaces are so often paired with DI is because of testing. 
            //      Making your consumer classes depend on interfaces (abstractions) instead of other concrete classes makes them much easier to test. 
            services.AddScoped<CourseManager>();
            services.AddScoped<StaffManager>();

            services.AddControllersWithViews();
            services.AddRazorPages();
            services.AddAuthorization(options =>
            {
                options.AddPolicy("EmployeeOnly", policy => policy.RequireClaim("EmployeeNumber"));
            });

            //By default, ExcelDataReader throws a NotSupportedException "No data is available for encoding 1252." on .NET Core.
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            //Obsolete codes£º Inject TenantId
            //  abandon this because there is claim info in the httpContext after add Tenant claim to each appUser
            //  when their accout created.
            //app.UseTenantInjector();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
