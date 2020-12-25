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
        private readonly StaffManager _StaffManager;


        public CourseCreateModel(AppUserManager<AppUser> UserMgr, CourseManager CourseMgr, StaffManager StaffMgr)
        {
            //_context = context;
            _UserMananger = UserMgr;
            _CourseManager = CourseMgr;
            _StaffManager = StaffMgr;



        }

        public async Task<IActionResult> OnGetAsync()
        {
            //var selectLists = (await _UserMananger.GetInstructorsAsync()).ToList();
            //var selectLists = (await _StaffManager.GetAllAsync()).ToList();
            //VwCourse.Instructors = new List<Instructor>();

            //foreach (var staff in selectLists)
            //{
            //    VwCourse.Instructors.Add(new Instructor { Id = staff.Id, Name = staff.Name, Checked = false });

            //}
            ////Instructors = new MultiSelectList(selectLists, selectLists);
            ////Instructors = new MultiSelectList(selectLists, null); //no Instructor to New Course
            //Instructors = new MultiSelectList(selectLists, "Id", "Name"); //no Instructor to New Course
            //VwCourse.InstructorsSelectList = new SelectList(selectLists, nameof(Staff.Id), nameof(Staff.Name));
            VwCourse.InstructorsSelectList = new SelectList((await _StaffManager.GetAllAsync()).ToList(), nameof(Staff.Id), nameof(Staff.Name));

            return Page();
        }

        [BindProperty]
        public VwCourse VwCourse { get; set; } = new VwCourse { };

        //public SelectList InstructorsSelectList { get; set; }
        //the MultiSelectList is so uggly;syncfusion.com is beautyful but expensive
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                //codes below is for keeping the value unchanged when validation failed
                //when submit failed because of validation failed & binding mechanism, the instructors[i].name turn to Null, why ?
                //now it's InstructorsSelectList turn to Null
                VwCourse.InstructorsSelectList = new SelectList((await _StaffManager.GetAllAsync()).ToList(), nameof(Staff.Id), nameof(Staff.Name));

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
            //todo should validation the multiple selected first, if no item selected
            //Checkboxes in a Razor Pages Form : https://www.learnrazorpages.com/razor-pages/forms/checkboxes
            //foreach (var instr in VwCourse.Instructors)
            //{
            //    if (instr.Checked)
            //    {
            //        var cor = new CourseAssignment { StaffId = instr.Id, CourseId = VwCourse.id };
            //        course.CourseAssignments.Add(cor);

            //    }

            //}

            foreach (var i in VwCourse.InstructorsIdList)
            {
                var cor = new CourseAssignment { StaffId = i, CourseId = VwCourse.id };
                course.CourseAssignments.Add(cor);

            }

            await _CourseManager.AddAsync(course);
            //Instructors.SelectMany<>
            return RedirectToPage("./Index");
        }
    }
}
