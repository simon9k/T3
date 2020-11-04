using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting.Internal;
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
            modelBuilder.Entity<Staff>().HasQueryFilter(b => (EF.Property<Guid>(b, "TenantId") == _tenantResolver.GetTenantId()) || (_tenantResolver.GetTenantId() == Guid.Empty));
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

            //Set CourseId,Id unique
            modelBuilder.Entity<Enrollment>()
                .HasKey(e => new { e.CourseId, e.StudentId });
            //.HasIndex(e => new { e.CourseId, e.Id })
            //.IsUnique();

            //Set composite primary key
            modelBuilder.Entity<GuardianRelation>()
                .HasKey(g => new { g.AppUserId, g.Id });
    
            //.HasKey(g => new { g.Id, g.AppUserId });  
            //.HasNoKey()
            //.HasIndex(g => new { g.AppUserId, g.Id });
            //.HasKey(g => new { g.AppUserId, g.Id });  
            //Introducing FOREIGN KEY constraint 'FK_GuardianRelation_Student_Id' 
            //on table 'GuardianRelation' may cause cycles or multiple cascade paths. Specify ON DELETE NO ACTION or ON UPDAT
            // todo make sure:  **don't know why, so just set Index, not sure about the cascade delete is OK

            base.OnModelCreating(modelBuilder);
        }
        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<T3.Models.Course> Courses { get; set; }
        public DbSet<T3.Models.Student> Students { get; set; }
        public DbSet<T3.Models.Staff> Staffs { get; set; }
        public DbSet<T3.Areas.Templates.Data.Template> Templates { get; set; }

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


        //获取[Display(Name="")]指定的中文名，以便seeding时自动匹配列名
        //未来可以考虑动态实现该属性的指定，以便灵活配置UI，但未知是否影响效率
        //public static string GetDisplayName(Type type, String ProperityName)
        //{
        //    //异常情况未处理，如该属性没有定义等：即没有[Display(Name = "")]语句
        //    return ((DisplayAttribute)Attribute.GetCustomAttribute(type.GetProperty(ProperityName), typeof(DisplayAttribute))).Name;
        //}
        //public static void Seed(S3KDContext context)
        //{
        //    //采用excel数据，作为初始化演示数据。还可以作为生产系统的数据初始化的基础。
        //    //增加一个需seed的类，操作步骤如下：
        //    //*****注意*****
        //    //**  1. 类的相关属性要加attribute：在类定义中要添加[Display(Name="????")] attribute，且与excel中对应列的列名（即首行）一致
        //    //**  1. 需要seeding赋值的类属性的名称要指定：需要赋值的相关属性在下List<string>中，不得错误
        //    //**  3. 遍历各个sheet部分代码，增加新的sheet的处理代码
        //    //**            switch (p.TableName)
        //    //             {
        //    //                    case "宝贝":   //seeding Pupil，给出指定需要赋值的属性
        //    //                    EntityProperities = new List<string> { "Name", "NickName", "EnrollmentDate" };
        //    //                    type = typeof(Pupil);
        //    //                    break;
        //    //**  3. 在更新context代码部分，加入新需要seeding的类相关代码；
        //    //**      case "NewClass":
        //    //**                    listEntity.ForEach(s => context.Classes.Add((NewClass)s));
        //    //**  4. 新建excel表对应的sheet：sheet名称，在下面代码中加入，以便启动后续操作
        //    //**  5. 输入合法数据于sheet中：确保excel单元格数据正确性，不得无法转换为对应合理数据，比如时间字段出现非法字符等
        //    //**  6. 确保不得违反数据库constraint限定
        //    //**当前可转换的数据类型可能需要增加


        //    //通过第三方包读取excel数据
        //    var sPath = HostingEnvironment.MapPath("~/App_Data/");
        //    using (var stream = File.Open(sPath + "seed.xlsx", FileMode.Open, FileAccess.Read))
        //    {

        //        // Auto-detect format, supports:
        //        //  - Binary Excel files (2.0-2003 format; *.xls)
        //        //  - OpenXml Excel files (2007 format; *.xlsx)
        //        using (var reader = ExcelReaderFactory.CreateReader(stream))
        //        {

        //            //ExelReader的第二种读取方式，即读入到DataSet中 //2. Use the AsDataSet extension method
        //            //获取excel数据至result中
        //            var result = reader.AsDataSet(
        //                new ExcelDataSetConfiguration()
        //                {
        //                    // Gets or sets a callback to obtain configuration options for a DataTable. 
        //                    ConfigureDataTable = (tableReader) => new ExcelDataTableConfiguration()
        //                    {
        //                        // Gets or sets a value indicating whether to use a row from the 
        //                        // data as column names.
        //                        //第一行为标题列
        //                        UseHeaderRow = true,
        //                    }
        //                });

        //            Type type = null;

        //            List<string> EntityProperities = null;


        //            //遍历每一张sheet，每个sheet对应一个entity类（如Pupil、PupilLog），即将初始化的数据
        //            foreach (DataTable p in result.Tables)
        //            {
        //                //TableName是excel的sheet的名称，根据不同名称对应不同的实体entity
        //                switch (p.TableName)
        //                {
        //                    case "宝贝":   //seeding Pupil，给出指定需要赋值的属性
        //                        EntityProperities = new List<string> { "Name", "NickName", "Sex", "EnrollmentDate", "ClassID" };
        //                        type = typeof(Pupil);
        //                        break;
        //                    case "宝贝日志": //seeding PupilLog，给出指定需要赋值的属性
        //                        EntityProperities = new List<string> { "PupilID", "Log", "Date", "Star" };
        //                        type = typeof(PupilLog);
        //                        break;
        //                    case "班级": //seeding Class，给出指定需要赋值的属性
        //                        EntityProperities = new List<string> { "ClassNum", "Name", "Grade", "Describle" };
        //                        type = typeof(Class);
        //                        break;
        //                    case "通知": //seeding Class，给出指定需要赋值的属性
        //                        EntityProperities = new List<string> { "Type", "Title", "Message" };
        //                        type = typeof(Notice);
        //                        break;
        //                    //*******************需要新的类seeding在此添加类似上述的语句（未来考虑脱离硬编码）
        //                    default:
        //                        break;
        //                }

        //                var listEntity = new List<Object>();
        //                object Entity = null;   // Activator.CreateInstance(System.Type.GetType("S3.KD.Models.PupilLog"));

        //                //获取每一行数据（对应每一条记录）
        //                foreach (DataRow row in p.Rows)
        //                {
        //                    Entity = Activator.CreateInstance(type);
        //                    listEntity.Add(Entity);

        //                    //原始的添加方式
        //                    //listEntity.Last<Pupil>().NickName = row["小名"].ToString();


        //                    foreach (var i in type.GetProperties())
        //                    {
        //                        //if (EntityProperities.FindIndex(delegate (string properity) { return properity == i.Name; }) > -1)

        //                        //确定属性是否在需要seed的范围内
        //                        if (EntityProperities.Contains(i.Name))
        //                        {
        //                            //根据属性的不同类型，采用不同赋值方式
        //                            switch (i.PropertyType.Name)
        //                            {
        //                                //用excel读出的数据特定列（即：列名字与实体类的[Dispaley(Name=]属性相符的列）的数值赋值
        //                                case "String":
        //                                    i.SetValue(Entity, row[GetDisplayName(type, i.Name)].ToString());
        //                                    break;

        //                                case "DateTime":
        //                                    i.SetValue(Entity, DateTime.Parse(row[GetDisplayName(type, i.Name)].ToString()));
        //                                    break;
        //                                case "Int32":
        //                                case "Nullable`1":
        //                                    i.SetValue(Entity, int.Parse(row[GetDisplayName(type, i.Name)].ToString()));
        //                                    break;
        //                                case "Boolean":
        //                                    i.SetValue(Entity, bool.Parse(row[GetDisplayName(type, i.Name)].ToString()));
        //                                    break;

        //                                default:

        //                                    break;
        //                            }

        //                        }

        //                    }

        //                }

        //                //todo:这里的处理还没能跳出硬编码类型，应该可以context活动名字叫"Pupils"的Entry，然后再增加数据记录
        //                //     .Add使用的强类型，也需要解决；这样就可以完全自定义而非硬编码seeding了。
        //                //     EF的DBContext应该使用的DataSet，研究这些可以解决上述问题。
        //                switch (type.Name)
        //                {
        //                    case "Pupil":
        //                        listEntity.ForEach(s => context.Pupils.Add((Pupil)s));
        //                        break;
        //                    case "PupilLog":
        //                        listEntity.ForEach(s => context.PupilLogs.Add((PupilLog)s));
        //                        break;
        //                    case "Class":
        //                        listEntity.ForEach(s => context.Classes.Add((Class)s));
        //                        break;
        //                    case "Notice":
        //                        listEntity.ForEach(s => context.Notices.Add((Notice)s));
        //                        break;
        //                    //*******************需要新的类seeding在此添加类似上述的语句（未来考虑脱离硬编码）
        //                    default:
        //                        break;
        //                }

        //                context.SaveChanges();

        //            }
        //        }
        //    }


        //    //原始直接的加入记录方式
        //    //var PupilLogs = new List<PupilLog>
        //    //{
        //    //      new PupilLog{PupilID=5,Log="Literature",Date=DateTime.Parse("2017-09-02"),Star=false}
        //    //};


        //}
    }
}

