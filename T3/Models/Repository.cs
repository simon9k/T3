using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using T3.Data;

namespace T3.Models
{

    public class Repository<TEnt> where TEnt : class
    {
        public Repository(ApplicationDbContext dbContext,ITenantResolver tenantResolver)
        {
            _db = dbContext;
            _tenantResolver = tenantResolver;

        }

        //Core does't need Unity to deal with DI problems!
        public ApplicationDbContext _db { get; set; }

        public ITenantResolver _tenantResolver { get; set; }
        public virtual async Task<List<TEnt>> GetAllAsync()
        {
            return await _db.Set<TEnt>().ToListAsync();
        }

        public IEnumerable<TEnt> GetAll()
        {
            return _db.Set<TEnt>().ToList();
        }

        //TEnt Get(TPk id);
        public virtual async Task<int> AddAsync(TEnt entity)
        {
            _db.Set<TEnt>().Add(entity);
            return await _db.SaveChangesAsync();
        }

        void Remove(TEnt entity)
        {

        }

    }


    public class CourseManager : Repository<Course>
    {
        public CourseManager(ApplicationDbContext dbContext,ITenantResolver tenantResolver)
            : base(dbContext,tenantResolver)
        {

        }

        public async Task<List<Course>> GetByDateAsync(DateTime StartDate)
        {
            //Courses = Courses.Where(c => c.StartTime >= StartDate && c.StartTime < EndDate).OrderBy(c => c.StartTime);
            //Need eager loading Instuctors?
            //    * Yes, if not the related data is null!

            var EndDate = StartDate.AddDays(7).AddMilliseconds(-1);
            return await _db.Set<Course>().Where(c => c.StartTime >= StartDate && c.StartTime < EndDate)
                .Include(c => c.CourseAssignments)
                .ThenInclude(ca => ca.AppUser)
                .OrderBy(c => c.StartTime).ToListAsync();
        }


        public override async Task<int> AddAsync(Course course)
        {
            //_db.Set<Course>().Add(entity);
            //固定课、临时课(一次性课时)，均保存课程设定，对于固定课，还需进行“重复”
            //course.StartTime = new DateTime(StartDate.Year, StartDate.Month, StartDate.Day, course.StartTime.Hour, course.StartTime.Minute, 0);
            //course.EndTime = new DateTime(StartDate.Year, StartDate.Month, StartDate.Day, course.EndTime.Hour, course.EndTime.Minute, 0);

            //EF6在执行完context.SaveChanges()之后，会自动将这个自增Id主键值从数据库中返回并赋给当前Id属性。
            //EF core 在repository方式实现后，是否也是如此？
            //   **So do too
            course.TanentId = _tenantResolver.GetTenantId();

            _db.Set<Course>().Add(course);
            var x = await _db.SaveChangesAsync();

            if (course.IsCyclic)
            {
                //固定课时，一次性安排到第二年底
                int j = course.StartTime.AddYears(2).AddDays(0 - course.StartTime.DayOfYear).Subtract(course.StartTime).Duration().Days / 7;

                for (int i = 1; i <= j; i++)
                {
                    var temp = new Course
                    {
                        OriginCourseId = course.CourseId,
                        TanentId = course.TanentId,
                        StartTime = course.StartTime.AddDays(7 * i),
                        EndTime = course.EndTime.AddDays(7 * i),
                        //StudentName = course.StudentName,
                        IsCyclic = course.IsCyclic
                    };
                    _db.Set<Course>().Add(temp);
                }
                _db.SaveChanges();
                return await _db.SaveChangesAsync();
            }

            return x;

        }
    }



}

//#region abstract Repository for all kind of Entity

//public interface IRepository<TEnt, in TPk> : IDisposable where TEnt : class
//{
//    //IEnumerable<TEnt> Get();
//    //TEnt Get(TPk id);
//    //void Add(TEnt entity);
//    //void Remove(TEnt entity);
//    IEnumerable<TEnt> GetAll();
//    Task<List<TEnt>> GetAllAsync();

//}
//#endregion

//public class CourseRepository : IRepository<Course, string>
//{

//    //Core does't need Unity to deal with DI problems!
//    public ApplicationDbContext _db { get; set; }
//    //private ApplicationDbContext _db = new ApplicationDbContext();

//    public async Task<List<Course>> GetAllAsync()
//    {
//        return await _db.Set<Course>().ToListAsync();
//        return await _db.Course.ToListAsync();

//    }

//    public IEnumerable<Course> GetAll()
//    {
//        return _db.Course.ToList();
//    }

//    #region IDisposable Members
//    public void Dispose()
//    {
//        //注意：public interface ICourseRepository:IDisposable，这是继承了IDisposable
//        //在实现此接口函数后，每次web request后，这个函数都会被调用，说明Unity实现了Dispose管理
//        // Dispose does nothing since we want Unity to manage the lifecycle of our Unit of Work
//    }
//    #endregion
//}
