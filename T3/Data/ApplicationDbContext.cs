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
using T3.Models.ViewModel;

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
            // Student specification.
            //modelBuilder.Entity<Student>().HasIndex(c => new { c.TenantId, c.NickName })
            //    .IsUnique();

            //Set composite primary key
            modelBuilder.Entity<CourseAssignment>()
                .HasKey(c => new { c.CourseId, c.AppUserId });

            //Set CourseId,StudentId unique
            modelBuilder.Entity<Enrollment>()
                .HasKey(e => new { e.CourseId, e.StudentId });
            //.HasIndex(e => new { e.CourseId, e.StudentId })
            //.IsUnique();

            //Set composite primary key
            modelBuilder.Entity<GuardianRelation>()
                .HasKey(g => new { g.AppUserId, g.StudentId });
    
            //.HasKey(g => new { g.StudentId, g.AppUserId });  
            //.HasNoKey()
            //.HasIndex(g => new { g.AppUserId, g.StudentId });
            //.HasKey(g => new { g.AppUserId, g.StudentId });  
            //Introducing FOREIGN KEY constraint 'FK_GuardianRelation_Student_StudentId' 
            //on table 'GuardianRelation' may cause cycles or multiple cascade paths. Specify ON DELETE NO ACTION or ON UPDAT
            // todo make sure:  **don't know why, so just set Index, not sure about the cascade delete is OK

            base.OnModelCreating(modelBuilder);
        }
        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<T3.Models.Course> Courses { get; set; }
        public DbSet<T3.Models.Student> Students { get; set; }

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
            userRoles.Add("simon9k1.1@outlook.com", new List<string> { "Admin", "Manager", "Student" });
            userRoles.Add("simon9k1.2@outlook.com", new List<string> { "Instructor" });
            userRoles.Add("simon9k1.3@outlook.com", new List<string> { "Instructor", "Student" });
            userRoles.Add("simon9k1.4@outlook.com", new List<string> { "Student" });
            userRoles.Add("simon9k1.5@outlook.com", new List<string> { "Assitant" });
            userRoles.Add("simon9k1.6@outlook.com", new List<string> { "Student" });

            var appRole = new IdentityRole[]
            {
                new IdentityRole("SuperAdmin"),
                new IdentityRole("Admin"),
                new IdentityRole("Student"), //Parents
                new IdentityRole("Instructor"),
                new IdentityRole("Manager"),
                new IdentityRole("Assistant"),
                new IdentityRole("Guest")

            };

            var students = new Student[]
            {
                //new Student{TenantId =tenants[1].TenantId, Name="大宝",NickName="贝贝",BOD=DateTime.Parse("2007-01-01") },
                //new Student{TenantId =tenants[1].TenantId, Name="大宝1",NickName="贝贝1",BOD=DateTime.Parse("2008-01-01") },
                //new Student{TenantId =tenants[1].TenantId, Name="大宝2",NickName="贝贝2",BOD=DateTime.Parse("2009-01-01") },
                //new Student{TenantId =tenants[1].TenantId, Name="大宝3",NickName="贝贝3",BOD=DateTime.Parse("2017-01-01") },
                //new Student{TenantId =tenants[1].TenantId, Name="大宝4",NickName="贝贝4",BOD=DateTime.Parse("2017-01-01") },
                //new Student{TenantId =tenants[1].TenantId, Name="大宝5",NickName="贝贝5",BOD=DateTime.Parse("2013-01-01") }
            };

            //var courses = new List<Course>
            //{
            //    new Course{
            //        ID=1,OriginID=1,
            //        StartTime =DateTime.Parse("2018-3-8 9:00"), EndTime=DateTime.Parse("2018-3-8 9:45"),
            //        StudentName ="Dora" ,IsCyclic=true
            //    },
            //    new Course{
            //        ID=2,OriginID=2,
            //        StartTime =DateTime.Parse("2018-3-8 10:00"), EndTime=DateTime.Parse("2018-3-8 10:45"),
            //        StudentName ="Tommy" ,IsCyclic=true
            //    },
            //    new Course{
            //        ID=3,OriginID=3,
            //        StartTime =DateTime.Parse("2018-3-8 11:00"), EndTime=DateTime.Parse("2018-3-8 11:45"),
            //        StudentName ="逗比" ,IsCyclic=false
            //    },
            //    new Course{
            //        ID=4,OriginID=4,
            //        StartTime =DateTime.Parse("2018-3-8 12:00"), EndTime=DateTime.Parse("2018-3-8 12:45"),
            //        StudentName ="Blade" ,IsCyclic=true
            //    },
            //    new Course{
            //        ID=5,OriginID=5,
            //        StartTime =DateTime.Parse("2018-3-8 21:00"), EndTime=DateTime.Parse("2018-3-8 21:45"),
            //        StudentName ="小虎"  ,IsCyclic=true
            //    },
            //    new Course{
            //        ID=6,OriginID=4,
            //        StartTime =DateTime.Parse("2018-3-13 12:00"), EndTime=DateTime.Parse("2018-3-13 12:45"),
            //        StudentName ="Blade" ,IsCyclic=true
            //    },
            //    new Course{
            //        ID=7,OriginID=6,
            //        StartTime =DateTime.Parse("2018-3-20 12:00"), EndTime=DateTime.Parse("2018-3-20 12:45"),
            //        StudentName ="Blade" ,IsCyclic=true
            //    }

            //};
            //courses.ForEach(s => context.Courses.Add(s));
            //context.SaveChanges();

            var courses = new Course[]
            {
                new Course{Name = "", StartTime = DateTime.Parse("2007-01-01"),EndTime =DateTime.Parse("2007-01-01")}

            };

            //initial Tenants
            foreach (Tenant tenant in tenants)
            {
                context.Add(tenant);

            }

            await context.SaveChangesAsync();

            foreach (Student student in students)
            {
                context.Add(student);

            }
            await context.SaveChangesAsync();

            //initial roles
            foreach (IdentityRole role in appRole)
            {
                //var b = await roleManager.RoleExistsAsync(role.Name);
                if (!await roleManager.RoleExistsAsync(role.Name))
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
            //todo RoleManager.Create , add role, instructor/Student/manager/parent
            //todo Student/Course/Enrollment/CourseAssignment



            return x;
        }
    }
}

