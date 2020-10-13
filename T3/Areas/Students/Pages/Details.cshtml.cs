using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using T3.Data;
using T3.Models;

namespace T3.Areas.Students.Pages
{
    public class DetailsModel : PageModel
    {
        private readonly T3.Data.ApplicationDbContext _context;

        public DetailsModel(T3.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public Student Student { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Student = await _context.Students
                .FirstOrDefaultAsync(m => m.StudentId == id);
                //.Include(s => s.Tenant).FirstOrDefaultAsync(m => m.StudentId == id);

            if (Student == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
