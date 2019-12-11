using System;
using System.Collections;
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

            //TenantId+NickName should be unique,basically we can use NickName to find the
            // client specification.
            modelBuilder.Entity<Client>().HasIndex(c => new { c.TenantId, c.NickName })
                .IsUnique();

            //Set composite primary key
            modelBuilder.Entity<CourseAssignment>()
                .HasKey(c => new { c.CourseId, c.AppUserId });

            //Set CourseId,ClientId unique
            modelBuilder.Entity<Enrollment>()
                .HasKey(e => new { e.CourseId, e.ClientId });
            //.HasIndex(e => new { e.CourseId, e.ClientId })
            //.IsUnique();


            base.OnModelCreating(modelBuilder);
        }
        public DbSet<Tenant> Tenants { get; set; }


    }

    public static class DbInitializer
    {
        public static async Task<IdentityResult> Initialize(ApplicationDbContext context, AppUserManager<AppUser> appUserManager,
            RoleManager<IdentityRole> roleManager, Guid rootTenantId)
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
            IdentityResult x = null;

            context.Tenants.RemoveRange(context.Tenants.IgnoreQueryFilters());//clear all data
            await context.SaveChangesAsync();

            //role no TenantId, so need delete directly
            //some exception ejept just like concurrence problem
            //foreach (IdentityRole r in roleManager.Roles)
            //{
            //    x = await roleManager.DeleteAsync(r);
            //}


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
                new AppUser{UserName="simon9k1.1@outlook.com",Email="simon9k1.1@outlook.com",EmailConfirmed=true,TenantId=tenants[1].TenantId  },
                new AppUser{UserName="simon9k1.2@outlook.com",Email="simon9k1.2@outlook.com",EmailConfirmed=true,TenantId=tenants[1].TenantId  },
                new AppUser{UserName="simon9k1.3@outlook.com",Email="simon9k1.3@outlook.com",EmailConfirmed=true,TenantId=tenants[1].TenantId  },
                new AppUser{UserName="simon9k1.4@outlook.com",Email="simon9k1.4@outlook.com",EmailConfirmed=true,TenantId=tenants[1].TenantId  },
                new AppUser{UserName="simon9k5@outlook.com",Email="simon9k5@outlook.com",EmailConfirmed=true,TenantId=tenants[2].TenantId  },
                new AppUser{UserName="simon9k6@outlook.com",Email="simon9k6@outlook.com",EmailConfirmed=true,TenantId=tenants[2].TenantId  },
                new AppUser{UserName="simon9k7@outlook.com",Email="simon9k7@outlook.com",EmailConfirmed=true,TenantId=tenants[2].TenantId  },
                new AppUser{UserName="simon9k8@outlook.com",Email="simon9k8@outlook.com",EmailConfirmed=true,TenantId=tenants[3].TenantId  },
                new AppUser{UserName="simon9k9@outlook.com",Email="simon9k9@outlook.com",EmailConfirmed=true,TenantId=tenants[3].TenantId  }

            };

            IDictionary<string, List<string>> userRoles = new Dictionary<string, List<string>>();
            userRoles.Add("simon9k@outlook.com", new List<string> { "SuperAdmin" });
            userRoles.Add("simon9k1.1@outlook.com", new List<string> { "Admin", "Manager", "Client" });
            userRoles.Add("simon9k1.2@outlook.com", new List<string> { "Instructor" });
            userRoles.Add("simon9k1.3@outlook.com", new List<string> { "Instructor", "Client" });
            userRoles.Add("simon9k1.4@outlook.com", new List<string> { "Client" });
            userRoles.Add("simon9k1.5@outlook.com", new List<string> { "Assitant" });
            userRoles.Add("simon9k1.6@outlook.com", new List<string> { "Client" });

            var appRole = new IdentityRole[]
            {
                new IdentityRole("SuperAdmin"),
                new IdentityRole("Admin"),
                new IdentityRole("Client"), //Parents
                new IdentityRole("Instructor"),
                new IdentityRole("Manager"),
                new IdentityRole("Assistant"),
                new IdentityRole("Guest")

            };

            //initial Tenants
            foreach (Tenant tenant in tenants)
            {
                context.Add(tenant);

            }
            await context.SaveChangesAsync();



            //initial roles
            foreach (IdentityRole role in appRole)
            {
                //var b = await roleManager.RoleExistsAsync(role.Name);
                if(!await roleManager.RoleExistsAsync(role.Name))
                {
                    x = await roleManager.CreateAsync(role);

                }

            }

            //intital AppUsers
            foreach (AppUser user in appUsers)
            {
                appUserManager.TenantId = user.TenantId;//初始化这里需要强制设定
                x = await appUserManager.CreateAsync(user, "1234.abcD");//Claims created too

                //fetch Roles with user
                //var roles = userRoles[user.UserName];
                List<string> roles = null;
                if (userRoles.TryGetValue(user.UserName, out roles))
                //if (roles != null)
                    foreach (string r in roles)
                        x = await appUserManager.AddToRoleAsync(user, r);

            }
            //todo RoleManager.Create , add role, instructor/client/manager/parent
            //todo Client/Course/Enrollment/CourseAssignment



            return x;
        }
    }
}

