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
using T3.Models.ViewModel;

namespace T3.Areas.Courses.Pages
{
    public class CourseCreateModel : PageModel
    {
        //private readonly T3.Data.ApplicationDbContext _context;
        private readonly AppUserManager<AppUser> _UserMananger;
        private readonly CourseManager _CourseManager;

        public CourseCreateModel(AppUserManager<AppUser> UserMgr, CourseManager CourseMgr)
        {
            //_context = context;
            _UserMananger = UserMgr;
            _CourseManager = CourseMgr;


        }

        public async Task<IActionResult> OnGetAsync()
        {
            var selectLists = (await _UserMananger.GetInstructorsAsync()).ToList();

            //Instructors = new MultiSelectList(selectLists, selectLists);
            //Instructors = new MultiSelectList(selectLists, null); //no Instructor to New Course
            Instructors = new MultiSelectList(selectLists, "Id", "UserName"); //no Instructor to New Course

            return Page();
        }

        [BindProperty]
        public VwCourse VwCourse { get; set; }

        //todo: the MultiSelectList is so uggly;syncfusion.com is beautyful but expensive
        //   anyway else?
        //   a box show items which are selected, there is a +/- button nearby, click the button
        //   a list of check box show , clicking one then box changed accordingly
        //   or customize a TagHelper
        public MultiSelectList Instructors { get; set; }
        //public MultiSelectList MultiInstructors { get; set; }

        //[BindProperty(SupportsGet =true)]
        //public string InstructorId { get; set; }

        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            //if (await TryUpdateModelAsync<Student>(
            //var instructorCourses = new HashSet<int>(instructor.CourseAssignments.Select(c => c.CourseID));

            //_context.VwCourse.Add(VwCourse);
            //await _context.SaveChangesAsync();
            //todo add the relative data such as courseAssigenment, enrollment
            //  明确的remove
            //  VwCourse 不太需要，字段的组合，可以直接在pagemodel中实现，需要采用“bindproperty”
            //  tenantId 有问题
            //  很多命名一致性问题，ID/Id,等等

            var course = new Course
            {
                Name = VwCourse.Name,
                IsCyclic = VwCourse.IsCyclic,
                StartTime = new DateTime(VwCourse.Date.Year, VwCourse.Date.Month, VwCourse.Date.Day,
                        VwCourse.StartTime.Hour, VwCourse.StartTime.Minute, 0),
                EndTime = new DateTime(VwCourse.Date.Year, VwCourse.Date.Month, VwCourse.Date.Day,
                        VwCourse.EndTime.Hour, VwCourse.EndTime.Minute, 0)
            };
            course.CourseAssignments = new List<CourseAssignment>();

            foreach (var instr in VwCourse.Instructors)
            {
                var cor = new CourseAssignment { AppUserId = instr ,CourseId= VwCourse.id};
                course.CourseAssignments.Add(cor);

            }


            await _CourseManager.AddAsync(course);
            //Instructors.SelectMany<>
            return RedirectToPage("./Index");
        }
    }
}
