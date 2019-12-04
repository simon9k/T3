using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using T3.Areas.Identity.Data;
using T3.Models;

namespace T3.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ITenantResolver _tenantResolver;
        
        
        //following codes are obsolete
        //private bool _bEnableFilter { get; set; } = false;//false means filter works, vice verse
        //public void DisableFilter() => _bEnableFilter = true;
        //public void EnableFilter() => _bEnableFilter = false;
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ITenantResolver tenantResolver)
            : base(options)
        {
            _tenantResolver = tenantResolver;
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //foreach (var type in GetEntityTypes())
            //{

            //    var method = SetGlobalQueryMethod.MakeGenericMethod(type);
            //    method.Invoke(this, new object[] { modelBuilder });
            //}

            //this works! that's set the global filter to a LINQ which is a function with a boolean return.
            //  so we can control the LINQ function such as deal with diffrent TenantId to switch Tenant 
            //  without change the filter itself.!!! 
            // great job!


            //***how to turn off the query filter?!!
            //query filter set when dbConext created, toggle it when login/register etc otherwise it will failed because
            //  of query filter works( this is done by a smart way which turn off the filter when TenantID == Guid.Empty!

            //modelBuilder.Entity<AppUser>().HasQueryFilter(b => (EF.Property<Guid>(b, "TenantId") == _tenantResolver.GetTenantId()) || _bEnableFilter);
            //modelBuilder.Entity<Tenant>().HasQueryFilter(b => (EF.Property<Guid>(b, "TenantId") == _tenantResolver.GetTenantId()) || _bEnableFilter);
            modelBuilder.Entity<AppUser>().HasQueryFilter(b => (EF.Property<Guid>(b, "TenantId") == _tenantResolver.GetTenantId()) || (_tenantResolver.GetTenantId() == Guid.Empty));
            modelBuilder.Entity<Tenant>().HasQueryFilter(b => (EF.Property<Guid>(b, "TenantId") == _tenantResolver.GetTenantId()) || (_tenantResolver.GetTenantId() == Guid.Empty));
            //modelBuilder.Entity<IdentityUser>().HasQueryFilter(b => true);
            //modelBuilder.Entity<IdentityUser>().HasQueryFilter(
            //{
            //      { 
            //          EF.Property<string>(b, "UserName") == _tenantResolver.CurrentTenant()
            //      }
            //);

            base.OnModelCreating(modelBuilder);
        }
        public DbSet<Tenant> Tenants { get; set; }


    }

    public static class DbInitializer
    {
        public static async Task<IdentityResult> Initialize(ApplicationDbContext context, AppUserManager<AppUser> appUserManager,
            Guid rootTenantId)
        {


            context.Database.EnsureCreated();

            // Look for any data.
            //var anyData = await context.Tenants.FindAsync(Guid.Empty);
            ////bAnyData = context.Tenants.FindAsync(Guid.Empty);
            //if (anyData != null)
            //{
            //    // DB has been seeded, remove all
            //    //Remove Global filter firstly otherwise couldn't delete all data
            //    //Removing Tenants can automately remove AppUser cause of cascading delete
            //    context.Tenants.RemoveRange(context.Tenants.IgnoreQueryFilters());
            //    await context.SaveChangesAsync();
            //}
            context.Tenants.RemoveRange(context.Tenants.IgnoreQueryFilters());
            await context.SaveChangesAsync();

            var tenants = new Tenant[]
            {
                new Tenant{TenantId=rootTenantId,Name="root", EnrollmentDate=DateTime.Parse("2019-11-20"), Location="北京",Desctiption="根节点" },
                new Tenant{TenantId=Guid.NewGuid(),Name="西安天使", EnrollmentDate=DateTime.Parse("2019-12-01"), Location="西安",Desctiption="科学幼教" },
                new Tenant{TenantId=Guid.NewGuid(),Name="成都大世界", EnrollmentDate=DateTime.Parse("2019-12-01"), Location="成都",Desctiption="花样滑冰" },
                new Tenant{TenantId=Guid.NewGuid(),Name="深圳帝王", EnrollmentDate=DateTime.Parse("2019-12-09"), Location="深圳",Desctiption="英语外教" }
            };
            var appUsers = new AppUser[]
            {
                new AppUser{UserName="simon9k@outlook.com",Email="simon9k@outlook.com",EmailConfirmed=true,TenantId=tenants[0].TenantId },
                new AppUser{UserName="simon9k1@outlook.com",Email="simon9k1@outlook.com",EmailConfirmed=true,TenantId=tenants[1].TenantId  },
                new AppUser{UserName="simon9k2@outlook.com",Email="simon9k2@outlook.com",EmailConfirmed=true,TenantId=tenants[1].TenantId  },
                new AppUser{UserName="simon9k3@outlook.com",Email="simon9k3@outlook.com",EmailConfirmed=true,TenantId=tenants[2].TenantId  },
                new AppUser{UserName="simon9k4@outlook.com",Email="simon9k4@outlook.com",EmailConfirmed=true,TenantId=tenants[3].TenantId  }

            };

            foreach (Tenant tenant in tenants)
            {
                context.Add(tenant);

            }
            await context.SaveChangesAsync();

            IdentityResult x = null;
            foreach (AppUser user in appUsers)
            {
                appUserManager.TenantId = user.TenantId;//初始化这里需要强制设定
                x = await appUserManager.CreateAsync(user, "1234.abcD");//Claims created too
            }

            return x;
        }
    }
}

