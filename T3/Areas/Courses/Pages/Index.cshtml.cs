using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using T3.Data;
using T3.Models;

namespace T3.Areas.Courses.Pages
{
    public class IndexModel : PageModel
    {
        //private readonly T3.Data.ApplicationDbContext _context;
        private CourseManager _CourseMgr;

        public IndexModel(T3.Data.ApplicationDbContext context,ITenantResolver tenantResolver)
        {
            //_context = context;
            _CourseMgr = new CourseManager(context,tenantResolver);

        }

        public IList<Course> Course { get; set; }
        [ViewData]
        public DateTime StartDate { get; set; }
        [ViewData]
        public DateTime EndDate { get; set; }

        public async Task OnGetAsync(DateTime? dtStartDate/*, DateTime EndDate*/)
        {
            //Course = await _context.Course.ToListAsync();
            //一个时间区间的courses（周）：如果dtStartDate空，这当前周
            //var courses = from c in db.Courses
            //              select c;



            if (!dtStartDate.HasValue) //null
            {
                StartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            }
            else
                StartDate = new DateTime(dtStartDate.Value.Year, dtStartDate.Value.Month, dtStartDate.Value.Day);

            EndDate = StartDate.AddDays(7).AddMilliseconds(-1);

            //Courses = Courses.Where(c => c.StartTime >= StartDate && c.StartTime < EndDate).OrderBy(c => c.StartTime);

            //public async Task<List<Course>> GetByDateAsync(DateTime StartDate)
            Course = await _CourseMgr.GetByDateAsync(StartDate);
        }
    }
}
