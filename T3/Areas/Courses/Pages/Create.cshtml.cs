using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using T3.Areas.Identity.Data;
using T3.Data;
using T3.Models;

namespace T3.Areas.Courses.Pages
{
    public class CreateModel : PageModel
    {
        //private readonly T3.Data.ApplicationDbContext _context;
        private CourseManager _CourseMgr;
        private AppUserManager<AppUser> userManager;


        public CreateModel(T3.Data.ApplicationDbContext context,
            AppUserManager<AppUser> UserManager,ITenantResolver tenantResolver)
        {
            //_context = context;
            _CourseMgr = new CourseManager(context,tenantResolver);
            userManager = UserManager;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Course Course { get; set; }

        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            Course.TanentId = userManager.TenantId;//todo not sure this is ready

            //固定课、临时课(一次性课时)，均保存课程设定，对于固定课，还需进行“重复”
            DateTime StartDate = DateTime.Now;//todo 按照实际业务完善

            Course.StartTime = new DateTime(StartDate.Year, StartDate.Month, StartDate.Day, Course.StartTime.Hour, Course.StartTime.Minute, 0);
            Course.EndTime = new DateTime(StartDate.Year, StartDate.Month, StartDate.Day, Course.EndTime.Hour, Course.EndTime.Minute, 0);

            //todo core TempData for what????
            //_context.Course.Add(Course);
            //await _context.SaveChangesAsync();
            await _CourseMgr.AddAsync(Course);

            return RedirectToPage("./Index");
        }
    }
}
